using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetParser
{
    public class StateTransition
    {
        public State  Previous      { get; set; }
        public State  Next          { get; set; }
        public string Pattern       { get; set; }

        public Action CustomWork;


    }
}
