using System;
using System.Collections.Generic;
using System.Text;

namespace NetParser
{
    public class StateTransition
    {
        public string Previous { get; set; }
        public string Next  { get; set; }
        public string Pattern { get; set; }
        public int    Return { get; set; }

        public Action CustomWork;

        public string TokenType { get; set; }
    }
}
