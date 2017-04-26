using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetParser.Caml
{
    public class CamlQuery
    {
        #region XML

        XmlDocument _where;
        XmlDocument _orderby;

        XmlNode _current;

        void InitXml () { 
            _where = new XmlDocument();
        }

        void InsertAtTop ( XmlDocument tree , string elem , bool update_current=false ) { 

            XmlElement new_element = tree.CreateElement( elem );

            XmlNode old_root = tree.ImportNode( tree.DocumentElement , true );
            tree.RemoveChild( tree.DocumentElement );

            new_element.AppendChild( old_root );

            if ( update_current ) {
                _current = tree.AppendChild( new_element );
            } else { 
                tree.AppendChild( new_element );
            }
            
        } 

        internal void AddOperator ( string oper ) {
            
            InsertAtTop( _where, oper , update_current:true );

        } 

        internal void AddOperation( string operation , string field, string type, string val ) { 

            XmlElement new_element = _where.CreateElement( operation );
            _current = _current == null ? 
                _where.AppendChild( new_element ) :
                _current.AppendChild( new_element );

            XmlElement fieldref = _where.CreateElement( "FieldRef" );
            fieldref.SetAttribute( "Name" , field );

            XmlElement value = _where.CreateElement( "Value" );
            value.SetAttribute( "Type" , type );
            value.InnerText = val;

            _current.AppendChild ( fieldref );
            _current.AppendChild ( value );

        }

        internal void Order( string field, bool asc ) { 

            _orderby = new XmlDocument();

            XmlElement new_element = _orderby.CreateElement( "OrderBy" );
            XmlNode pointer = _orderby.AppendChild( new_element );

            XmlElement fieldref = _orderby.CreateElement( "FieldRef" );
            fieldref.SetAttribute( "Name" , field );
            fieldref.SetAttribute( "Ascending" , asc.ToString().ToUpper() );

            pointer.AppendChild ( fieldref );
            
        }

        #endregion

        public CamlQuery() { 

            InitXml();

        }

        public override string ToString()
        {
            XmlDocument ret = new XmlDocument();
            ret.LoadXml( _where.OuterXml );

            InsertAtTop( ret, "Where" );
            InsertAtTop( ret, "Query" );

            if ( _orderby != null ) { 

                ret.DocumentElement.AppendChild( ret.ImportNode( _orderby.DocumentElement, deep:true ) );    

            }

            return ret.OuterXml;
        }

        public CamlQueryWhere Where ( string field , string type="" ) { 

            return new CamlQueryWhere( this , field, type );
        }

        public void OrderBy ( string field , bool ascending=true ) { 

            this.Order( field , ascending );
        }

    }
}
