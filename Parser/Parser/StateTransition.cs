using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public class StateTransition
    {
        public string Before { get; set; }
        public string After  { get; set; }
        public int    Return { get; set; }

        public Action CustomWork;
    }
}
