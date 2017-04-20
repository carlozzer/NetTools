using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetParser
{
    public class XmlLexer
    {
        const string PATTERN_ALPHANUMERIC = @"\w";
        const string PATTERN_NON_ALPHANUMERIC = @"\W";

        const string STATE_INIT = "INIT";

        string current_state;
        int read_offset;

        XmlToken current_token;
        List<XmlToken> tokens;
        
        List<XmlStateTransition> transition_table;

        void AddStateTransition ( 

            string state_in , 
            string pattern , 
            string state_out ,
            bool acceptance=false,
            int back=0

        ) { 

            XmlStateTransition transition = new XmlStateTransition(
                state_in,pattern,state_out,acceptance,back
            );

            this.transition_table.Add ( transition );

        }

        XmlStateTransition FindTransition( char c )
        {
            XmlStateTransition ret = null;

            bool found = false;
            int current = 0;

            while ( transition_table.Count > current && !found ) { 

                if ( transition_table[current].state_in == this.current_state ) { 

                    if ( Regex.IsMatch( c.ToString() , transition_table[current].pattern ) ) {

                        ret = transition_table[current];
                        found = true;

                    }
                }
                current++;
            }

            return ret;
        }

        void Accumulate( char c ) { 
            this.current_token.lexeme = string.Concat( this.current_token.lexeme , c );
        }

        string TransitionFunction( char c ) { 

            string ret = STATE_INIT;

            XmlStateTransition transition = FindTransition( c );
            if ( transition != null ) { 

                if ( transition.back == 0 ) {
                    Accumulate ( c );
                }

                this.current_state = transition.state_out;
                this.read_offset -= transition.back;

                if ( transition.acceptance ) { 

                    this.current_token.type = transition.back > 0 ? transition.state_out : transition.state_in;
                    this.tokens.Add ( this.current_token );
                    this.current_token = new XmlToken();

                } 
            }

            return ret;
        }

        public XmlLexer() {
            
            this.current_state = STATE_INIT;
            this.read_offset = 0;
            this.current_token = new XmlToken();
            this.tokens = new List<XmlToken>();

            this.transition_table = new List<XmlStateTransition>();
            
            this.AddStateTransition( STATE_INIT , "'" , "STRING" );
            this.AddStateTransition( "STRING" , "[^']" , "STRING" );
            this.AddStateTransition( "STRING" , "'" , STATE_INIT , acceptance:true );

            this.AddStateTransition( STATE_INIT , "\"" , "STRINGD" );
            this.AddStateTransition( "STRINGD" , "[^\"]" , "STRINGD" );
            this.AddStateTransition( "STRINGD" , "\"" , STATE_INIT , acceptance:true );

            this.AddStateTransition( STATE_INIT , "<" , "<" );
            
            this.AddStateTransition( "<" , "[?]", "<?");
            this.AddStateTransition( "<?" , "[xX]", "<?x");
            this.AddStateTransition( "<?x" , "[mM]", "START_PROLOG");
            this.AddStateTransition( "START_PROLOG" , "[lL]", STATE_INIT , acceptance:true );

            this.AddStateTransition( "<" , "!", "DECLARATION");
            this.AddStateTransition( "DECLARATION" , "-" , "<!-");
            this.AddStateTransition( "<!-", "-" , "COMMENT" , acceptance:true );
            this.AddStateTransition( "DECLARATION" , "[^!]" , STATE_INIT , acceptance:true , back:1 );
            // DOCTYPE DTD
            // </
            this.AddStateTransition( "<" , "[^<]" , STATE_INIT , acceptance:true, back:1 );



            this.AddStateTransition( STATE_INIT , PATTERN_ALPHANUMERIC , "ALPHA" );
            this.AddStateTransition( "ALPHA" , PATTERN_ALPHANUMERIC , "ALPHA" );
            this.AddStateTransition( "ALPHA" , PATTERN_NON_ALPHANUMERIC , STATE_INIT , true, 1 );
            this.AddStateTransition( STATE_INIT , "=" , "=" );
            this.AddStateTransition( "=" , "[^=]" , STATE_INIT , acceptance:true , back:1 );

            // CLOSING
            this.AddStateTransition( STATE_INIT , "[?]", "END_PROLOG");
            this.AddStateTransition( "END_PROLOG" , ">", STATE_INIT , acceptance:true );

            this.AddStateTransition( STATE_INIT , ">" , ">" );
            this.AddStateTransition( ">" , "[^>]" , STATE_INIT , acceptance:true, back:1 );
            
        }

        public List<XmlToken> GetTokens() { 
            return this.tokens;
        }

        public void Read ( string input ) { 

            while ( input.Length > this.read_offset ) { 

                this.TransitionFunction( input[ read_offset ] );
                this.read_offset++;

            }

        }
    }
}
