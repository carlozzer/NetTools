using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Pdf
{
    public class PdfStateTransition
    {
        public string   pattern     { get; set; }
        public string   state_in    { get; set; }
        public string   state_out   { get; set; }
        public bool     acceptance  { get; set; }
        public int      back        { get; set; }

    }
}
