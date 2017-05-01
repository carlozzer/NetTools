using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser
{
    public class State
    {
        #region MEMBERS

        public string Name { get; set; }
        public bool Acceptance { get; set; }
        public int Return { get; set; }

        #endregion

        #region CONSTRUCTOR

        public State() { }

        public 
        State( string name , bool acceptance=false, int ret=0) { 

            this.Name = name;
            this.Acceptance = acceptance;
            this.Return = ret;
        }

        #endregion

        #region OPERATOR ==

        public static bool operator ==(State a, State b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

                // Return true if the fields match:
                return a.Name == b.Name && a.Return == b.Return && a.Acceptance == b.Acceptance;
        }

        public static bool operator !=(State a, State b)
        {
            return !(a == b);
        }

        #endregion

    }
}
