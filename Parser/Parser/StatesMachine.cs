using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public class StatesMachine
    {
        List<StateTransition> Transitions;

        void EnsureTransitions() {
            this.Transitions = this.Transitions ?? new List<StateTransition>();
        }

        public void Add ( StateTransition transition ) {

            this.EnsureTransitions();

            if ( transition != null ) {
                this.Transitions.Add ( transition );
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
    }   
}
