using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser
{
    public class XmlStateTransition
    {
        public string   pattern     { get; set; }
        public string   state_in    { get; set; }
        public string   state_out   { get; set; }
        public bool     acceptance  { get; set; }
        public int      back        { get; set; }


        public XmlStateTransition() { }

        public XmlStateTransition ( 
            string state_in, 
            string pattern , 
            string state_out , 
            bool acceptance=false, 
            int back=0  
        ) { 

            this.state_in   = state_in;
            this.pattern    = pattern;
            this.state_out  = state_out;
            this.acceptance = acceptance;
            this.back       = back;
        }

    }
}
