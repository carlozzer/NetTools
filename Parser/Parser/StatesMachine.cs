using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Parser
{
    public class StatesMachine
    {
        #region CONSTRUCTOR

        public StatesMachine() { 
            current_state = "INIT";
        }

        #endregion

        #region TABLE and other locals

        List<StateTransition> TransitionsTable;
        string current_state;
        string current_token;

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
            string  before, 
            string  pattern, 
            string  after, 
            int     ret         =0, 
            Action  CustomAction=null 
         ) { 
            StateTransition transition = new StateTransition() { 
                Before = before,
                Pattern = pattern,
                After = after,
                Return = ret,
                CustomWork = CustomAction
            };

            Add( transition );
        }

        #endregion

        #region REACT

        public int Parse( char c ) {
            
            int ret = 0;

            foreach ( StateTransition transition in this.TransitionsTable ) { 

                if ( transition.Before.ToLower() == current_state ) { 

                    bool pattern_match = Regex.IsMatch( c.ToString() , transition.Pattern );

                    if ( pattern_match ) { 

                        this.current_token += c.ToString();

                        bool state_change = current_state != transition.After;
                        if ( state_change ) {
                            
                            // hacer algo con el token

                            ret = transition.Return;
                            current_state = transition.Before;

                        }

                        if ( transition.CustomWork != null ) { 

                            transition.CustomWork();

                        }

                    }

                }

            }

            return ret;

        }

        #endregion
    }   
}
