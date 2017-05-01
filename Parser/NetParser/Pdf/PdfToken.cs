using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Pdf
{
    public class PdfToken
    {
        public string lexeme { get; set; }
        public string type { get; set; }

        public PdfToken() { 
            this.lexeme = string.Empty;
            this.type = "UNKNOWN";
        }
    }
}
