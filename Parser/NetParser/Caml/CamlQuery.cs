using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Caml
{
    public class CamlQuery
    {



        public CamlBuilderWhere Where ( string field , string type="" ) { 

            CamlBuilderWhere ret = new CamlBuilderWhere();

            return ret;

        }

        public CamlBuilderWhere And ( string field , string type="" ) { 

            CamlBuilderWhere ret = new CamlBuilderWhere();

            return ret;

        }
        public CamlBuilderWhere Or ( string field , string type="" ) { 

            CamlBuilderWhere ret = new CamlBuilderWhere();

            return ret;
        }
    }
}
