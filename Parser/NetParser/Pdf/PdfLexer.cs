using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetParser.Pdf
{
    public class PdfLexer
    {
        const string PATTERN_HEXA = @"[0-9A-F]";
        const string PATTERN_SEPARATOR = "[\\[\\]\\\\\\{\\}\\(\\)\\/<>\x00\x09\x10\x12\x20\r\n]";
        const string PATTERN_NO_SEPARATOR = "[^\\[\\]\\\\\\{\\}\\(\\)\\/<>\x00\x09\x10\x12\x20\r\n]";
        const string PATTERN_NUMBER = "[0-9.+-]";
        const string PATTERN_NO_NUMBER = "[^0-9.+-]";

        const string STATE_INIT = "INIT";
        const string STATE_KEYWORD = "KEYWORD";

        string current_state;
        int read_offset;
        bool mode_stream;

        PdfToken current_token;
        PdfToken stream_token;
        List<PdfToken> tokens;
        
        List<PdfStateTransition> transition_table;

        void AddStateTransition ( 

            string  state_in , 
            string  pattern , 
            string  state_out ,
            bool    acceptance = false,
            int     back       = 0

        ) { 

            PdfStateTransition transition = new PdfStateTransition() { 

                state_in    = state_in,
                pattern     = pattern,
                state_out   = state_out,
                acceptance  = acceptance,
                back        = back

            };
                
            this.transition_table.Add ( transition );
        }

        PdfStateTransition FindTransition( char c )
        {
            PdfStateTransition ret = null;

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

        void AccumulateStream( char c ) { 
            this.stream_token.lexeme = string.Concat( this.stream_token.lexeme , c );
        }

        string TransitionFunction( char c ) { 

            string ret = STATE_INIT;

            if ( mode_stream ) { 
                AccumulateStream( c );
            }

            PdfStateTransition transition = FindTransition( c );
            if ( transition != null ) { 

                if ( transition.back == 0 ) {
                    Accumulate ( c );
                    this.current_token.type = transition.state_out;
                }

                this.current_state = transition.state_out;
                this.read_offset -= transition.back;

                if ( transition.acceptance ) { 

                    if ( this.current_token.lexeme == "stream" ) { 
                        mode_stream = true;
                        stream_token = new PdfToken();
                        stream_token.type = "STREAM";
                    }

                    if ( this.current_token.lexeme == "endstream" ) { 
                        mode_stream = false;
                        this.tokens.Add ( this.stream_token );
                    }

                    this.tokens.Add ( this.current_token );
                    this.current_token = new PdfToken();
                    this.current_state = STATE_INIT;

                    
                } 
            }

            return ret;
        }

        public PdfLexer() {
            
            this.current_state = STATE_INIT;
            this.read_offset = 0;
            this.current_token = new PdfToken();
            this.tokens = new List<PdfToken>();

            this.transition_table = new List<PdfStateTransition>();
            
            this.AddStateTransition( STATE_INIT , "%" , "COMMENT" );
            this.AddStateTransition( "COMMENT" , "[^\r]" , "COMMENT" );
            this.AddStateTransition( "COMMENT" , "\r" , "COMMENT" , acceptance:true , back:1 );

            this.AddStateTransition( STATE_INIT , PATTERN_NUMBER , "NUMBER" );
            this.AddStateTransition( "NUMBER" , PATTERN_NUMBER, "NUMBER" );
            this.AddStateTransition( "NUMBER" , PATTERN_NO_NUMBER , "NUMBER" , acceptance:true , back:1 );

            // TODO, probablement elsigno + y menos solo podrá estar en primre lugar al ser números

            this.AddStateTransition( STATE_INIT , "<" , "<" );
            this.AddStateTransition( "<" , "<" , "<<" , acceptance:true );
            
            this.AddStateTransition( "<" , PATTERN_HEXA , "HEXA" );
            this.AddStateTransition( "HEXA" , PATTERN_HEXA , "HEXA" );
            this.AddStateTransition( "HEXA" , ">" , "HEXA" , acceptance:true );


            this.AddStateTransition( STATE_INIT , ">" , ">" );
            this.AddStateTransition( ">" , ">" , ">>" , acceptance:true );

            this.AddStateTransition( STATE_INIT , "\\(" , "STRING" );
            this.AddStateTransition( "STRING" , "[^\\)]" , "STRING" );
            this.AddStateTransition( "STRING" , "\\)", "STRING" , acceptance:true );


            this.AddStateTransition( STATE_INIT , @"\[" , "[" , acceptance:true );
            this.AddStateTransition( STATE_INIT , @"\]" , "]" , acceptance:true );

            this.AddStateTransition( STATE_INIT , "/" , "NAME" );
            this.AddStateTransition( "NAME" , PATTERN_NO_SEPARATOR , "NAME" );
            this.AddStateTransition( "NAME" , PATTERN_SEPARATOR , "NAME" , acceptance:true , back:1 );

            this.AddStateTransition( STATE_INIT , "t" , "t" );
            this.AddStateTransition( "t" , "r" , "tr" );
            this.AddStateTransition( "tr", "u" , "tru" );
            this.AddStateTransition( "tru" , "e" , "BOOLEAN" , acceptance:true );

            this.AddStateTransition( STATE_INIT , "f" , "f" );
            this.AddStateTransition( "f" , "a" , "fa" );
            this.AddStateTransition( "fa", "l" , "fal" );
            this.AddStateTransition( "fal" , "s", "fals" );
            this.AddStateTransition( "fals" , "e" , "BOOLEAN" , acceptance:true );

            this.AddStateTransition( STATE_INIT , "o" , "o" );
            this.AddStateTransition( "o" , "b" , "ob" );
            this.AddStateTransition( "ob" , "j" , STATE_KEYWORD , acceptance:true );

            this.AddStateTransition( STATE_INIT , "e" , "e" );
            this.AddStateTransition( "e" , "n" , "en" );
            this.AddStateTransition( "en" , "d" , "end" );
            this.AddStateTransition( "end" , "o" , "endo" );
            this.AddStateTransition( "endo" , "b" , "endob" );
            this.AddStateTransition( "endob" , "j" , STATE_KEYWORD , acceptance:true );

            this.AddStateTransition( STATE_INIT  , "s", "s" );
            this.AddStateTransition( "s"  , "t", "st" );
            this.AddStateTransition( "st"  , "r", "str" );
            this.AddStateTransition( "str"  , "e", "stre" );
            this.AddStateTransition( "stre"  , "a", "strea" );
            this.AddStateTransition( "strea"  , "m", STATE_KEYWORD , acceptance:true );

            this.AddStateTransition( "end" , "s" , "ends" );
            this.AddStateTransition( "ends" , "t" , "endst" );
            this.AddStateTransition( "endst" , "r" , "endstr" );
            this.AddStateTransition( "endstr" , "e" , "endstre" );
            this.AddStateTransition( "endstre" , "a" , "endstrea" );
            this.AddStateTransition( "endstrea" , "m" , STATE_KEYWORD , acceptance:true );
            
        }

        public List<PdfToken> GetTokens() { 
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
