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
        const string ALPHANUMERIC = @"\w";
        const string NON_ALPHANUMERIC = @"\W";

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

            foreach ( XmlStateTransition transition in transition_table ) { 

                if ( transition.state_in == this.current_state ) { 

                    if ( Regex.IsMatch( c.ToString() , transition.pattern ) ) {

                        ret = transition;

                    }
                }
            }

            return ret;
        }

        string TransitionFunction( char c ) { 

            string ret = "START";

            XmlStateTransition transition = FindTransition( c );
            if ( transition != null ) { 

                if ( transition.acceptance ) { 
                    this.tokens.Add ( this.current_token );
                    this.current_token = new XmlToken();
                } else { 
                    this.current_token.lexeme = string.Concat( this.current_token.lexeme , c );
                }

                this.current_state = transition.state_out;
                this.read_offset -= transition.back;

            }

            return ret;
        }

        public XmlLexer() {
            
            this.current_state = "START";
            this.read_offset = 0;
            this.current_token = new XmlToken();
            this.tokens = new List<XmlToken>();

            this.transition_table = new List<XmlStateTransition>();
            
            this.AddStateTransition( "START" , "<" , "<" );
            this.AddStateTransition( "<" , "[^<]" , "START" , acceptance:true, back:1 );

            this.AddStateTransition( "START" , ALPHANUMERIC , "ALPHA" );
            this.AddStateTransition( "ALPHA" , NON_ALPHANUMERIC , "START" , true, 1 );


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
