using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Pdf
{
    public class PdfObject
    {
        public string object_number { get; set; }
        public string generation_number { get; set; }
        IDictionary<string,object> dic { get; set; }
        byte[] stream { get; set; }
    }
}
