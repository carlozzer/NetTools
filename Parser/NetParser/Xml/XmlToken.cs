using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser
{
    public class XmlToken
    {
        public string lexeme { get; set; }
        public string type { get; set; }

        public XmlToken() { 
            this.lexeme = string.Empty;
            this.type = "UNKNOWN";
        }
    }
}
