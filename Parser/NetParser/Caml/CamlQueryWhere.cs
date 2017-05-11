using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TC.Framework.SharePoint.Caml
{
    public class CamlQueryWhere
    {
        CamlQuery _parent;
        string _field;
        string _type;

        public CamlQueryWhere ( CamlQuery parent , string field, string type ) { 

            _parent = parent;
            _field = field;
            _type  = type;

        }

        #region TYPES

        void EnsureType( string default_type ) {
            
            if ( string.IsNullOrEmpty(_type) ) { 
                _type = default_type;
            }
        }

        #endregion

        #region DATES

        string DateAdapter( DateTime dt ) { 

            return string.Format("{0}Z" , dt.ToString("s") );
        }

        #endregion

        #region OPERATIONS

        CamlQueryOperator AddOperation ( string operation , string default_type , string val ) { 

            EnsureType( default_type );
            _parent.AddOperation( operation , _field, _type , val );
            return new CamlQueryOperator( _parent );
        }

        #endregion

        #region OPERATIONS

        public CamlQueryOperator Eq ( string text ) { 

            return AddOperation( "Eq" , "Text" , text );

        }

        public CamlQueryOperator Eq ( int number ) { 

            return AddOperation( "Eq" , "Number" , number.ToString() );
            
        }

        public CamlQueryOperator Eq ( DateTime dt ) { 

            return AddOperation( "Eq" , "DateTime" , DateAdapter(dt) );
            
        }
        
        public CamlQueryOperator Geq ( string text ) { 

            return AddOperation( "Geq" , "Text" , text );

        }

        public CamlQueryOperator Geq ( int number ) { 

            return AddOperation( "Geq" , "Number" , number.ToString() );
            
        }

        public CamlQueryOperator Geq ( DateTime dt ) { 

            return AddOperation( "Geq" , "DateTime" , DateAdapter(dt) );
            
        }

        public CamlQueryOperator Leq ( string text ) { 

            return AddOperation( "Leq" , "Text" , text );

        }

        public CamlQueryOperator Leq ( int number ) { 

            return AddOperation( "Leq" , "Number" , number.ToString() );
            
        }

        public CamlQueryOperator Leq ( DateTime dt ) { 

            return AddOperation( "Leq" , "DateTime" , DateAdapter(dt) );
            
        }

        public CamlQueryOperator Gt ( string text ) { 

            return AddOperation( "Gt" , "Text" , text );

        }

        public CamlQueryOperator Gt ( int number ) { 

            return AddOperation( "Gt" , "Number" , number.ToString() );
            
        }

        public CamlQueryOperator Gt ( DateTime dt ) { 

            return AddOperation( "Gt" , "DateTime" , DateAdapter(dt) );
            
        }

        public CamlQueryOperator Lt ( string text ) { 

            return AddOperation( "Lt" , "Text" , text );

        }

        public CamlQueryOperator Lt ( int number ) { 

            return AddOperation( "Lt" , "Number" , number.ToString() );
            
        }

        public CamlQueryOperator Lt ( DateTime dt ) { 

            return AddOperation( "Lt" , "DateTime" , DateAdapter(dt) );
            
        }

        public CamlQueryOperator Neq ( string text ) { 

            return AddOperation( "Neq" , "Text" , text );

        }

        public CamlQueryOperator Neq ( int number ) { 

            return AddOperation( "Neq" , "Number" , number.ToString() );
            
        }

        public CamlQueryOperator Neq ( DateTime dt ) { 

            return AddOperation( "Neq" , "DateTime" , DateAdapter(dt) );
            
        }

        // TODO In

        // TODO Includes

        // TODO NotIncludes

        // TODO IsNull

        // TODO IsNotNull

        #endregion

        #region CONTAINS

        public CamlQueryOperator Contains ( string text ) { 

            if ( string.IsNullOrEmpty(_type) ) { 
                _type = "Text";
            }

            _parent.AddOperation( "Contains" , _field, _type , text );

            return new CamlQueryOperator( _parent );
        }

        #endregion

    }
}
