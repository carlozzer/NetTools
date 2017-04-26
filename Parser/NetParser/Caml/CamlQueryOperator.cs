using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Caml
{
    public class CamlQueryOperator
    {
        CamlQuery _parent;

        public CamlQueryOperator( CamlQuery parent ) { 

            _parent = parent;

        }

        public CamlQueryWhere And ( string field , string type="" ) { 

            _parent.AddOperator( "And" );
            return new CamlQueryWhere( _parent , field , type );
        }

        public CamlQueryWhere Or ( string field , string type="" ) { 

            _parent.AddOperator( "Or" );
            return new CamlQueryWhere( _parent , field , type );
        }
    }
}
