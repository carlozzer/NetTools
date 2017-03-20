using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser
{
    public class MarkupStates
    {
        public static State OPEN_TAG { get { 
                return new State("OPEN_TAG");
        } }

        public static State DECLARATION { get { 
                return new State("DECLARATION");
        } }

        public static State DOCTYPE { get {  
                return new State("DOCTYPE");
        } }
    }
}
