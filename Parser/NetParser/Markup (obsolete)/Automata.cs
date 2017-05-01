using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetParser
{
    public class Automata
    {
        #region CONSTRUCTOR

        public Automata() { 
            current_state = START;
        }

        #endregion

        #region UNIVERSAL STATES

        public static State START { get { 
                return new State("START");
        } }

        public static State END { get { 
                return new State("END");
        } }

        #endregion

        #region TABLE and other locals

        List<StateTransition> TransitionsTable;
        State current_state;
        Token current_token;

        #endregion

        #region TRANSITIONS

        void EnsureTransitions() {
            this.TransitionsTable = this.TransitionsTable ?? new List<StateTransition>();
        }

        public void Add ( StateTransition transition ) {

            this.EnsureTransitions();

            if ( transition != null ) {
                this.TransitionsTable.Add ( transition );
            }


        }

        public void Add ( 
            State  before, 
            string  pattern, 
            State  after, 
            Action  CustomAction=null 
         ) { 
            StateTransition transition = new StateTransition() { 
                Previous = before,
                Pattern = pattern,
                Next = after,
                CustomWork = CustomAction
            };

            Add( transition );
        }

        #endregion

        #region TOKEN

        void TokenEnsure () { 

            if ( this.current_token == null ) { 
                this.current_token = new Token();
            }
        }

        void TokenAccumulate ( char c ) {

            TokenEnsure();
            
            if ( string.IsNullOrWhiteSpace( this.current_token.Lexeme ) ) { 
                this.current_token.Lexeme = string.Empty;
            } 

            this.current_token.Lexeme = string.Concat( this.current_token.Lexeme , c.ToString() );
        }

        #endregion

        #region REACT TO STATES

        bool TransitionMatches( char c , StateTransition transition ) { 

            bool ret = false;

            if ( current_state == transition.Previous ) { 
                ret = Regex.IsMatch( c.ToString() , transition.Pattern );
            }
                
            return ret;
        }

        bool IsStateChanging ( StateTransition transition ) { 

            bool ret = false;

            ret = current_state != transition.Next;

            return ret;
        }


        int StateChange( StateTransition transition ) { 

            int ret = 0;

            if ( transition.Next.Acceptance ) { 
                // send token to analizar sintáctico
            }
            ret = transition.Next.Return;
            current_state = transition.Next;

            return ret;
        }

        public int Parse( char c ) {
            
            int ret = 0;

            foreach ( StateTransition transition in this.TransitionsTable ) { 

                if ( TransitionMatches( c , transition ) ) {     

                    TokenAccumulate( c );

                    if ( IsStateChanging( transition ) ) {
                        ret = this.StateChange( transition );
                    }

                    if ( transition.CustomWork != null ) { 
                        transition.CustomWork();
                    }

                }

            }

            return ret;

        }

        #endregion
    }   
}
