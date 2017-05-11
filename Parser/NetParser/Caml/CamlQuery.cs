using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TC.Framework.Lib.Parser.Xml;

namespace TC.Framework.SharePoint.Caml
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

        XmlDocument RemoveWhere( XmlDocument xml ) {

            XmlDocument ret = null;

            if ( xml!= null ) {

                if ( xml.DocumentElement.Name.ToLower() == "where" ) {
                    ret = new XmlDocument();
                    ret.AppendChild( ret.ImportNode( xml.DocumentElement.FirstChild , deep:true ) );
                } else {

                    ret = xml;

                }
            }

            return ret;
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

        XmlNode EnsureOrderBy(  ) { 

            XmlNode ret = null;

            if ( _orderby == null ) {
                
                _orderby = new XmlDocument();
                XmlElement new_element = _orderby.CreateElement( "OrderBy" );
                ret = _orderby.AppendChild( new_element );

            } else {

                ret = _orderby.DocumentElement;

            }

            return ret;
        }

        internal void AddOrder( string field, bool asc ) { 

            XmlNode order = EnsureOrderBy();

            XmlElement fieldref = _orderby.CreateElement( "FieldRef" );
            fieldref.SetAttribute( "Name" , field );
            fieldref.SetAttribute( "Ascending" , asc.ToString().ToUpper() );

            order.AppendChild ( fieldref );
        }

        #endregion

        #region CONSTRUCTOR
        
        public CamlQuery() { 

            InitXml();

        }

        #endregion

        #region LOAD XML

        public void LoadXml ( string xml ) {

            XmlParser parser = new XmlParser();
            parser.Parse( xml );
            _where = parser.Tree;

        }

        #endregion

        #region OUTPUT

        public override string ToString()
        {
            XmlDocument ret = new XmlDocument();

            if ( _where.DocumentElement != null ) {

                ret.LoadXml( _where.OuterXml );

                InsertAtTop( ret, "Where" );
                InsertAtTop( ret, "Query" );

            }

            if ( _orderby != null ) { 
                if ( ret.DocumentElement != null) {
                    ret.DocumentElement.AppendChild( ret.ImportNode( _orderby.DocumentElement, deep:true ) );    
                } else {
                    ret.AppendChild( ret.ImportNode( _orderby.DocumentElement, deep:true ) );
                }
            }

            return ret.DocumentElement != null ? ret.OuterXml : string.Empty;
        }

        #endregion

        #region WHERE
        
        public CamlQueryWhere Where ( string field , string type="" ) { 

            return new CamlQueryWhere( this , field, type );
        }

        #endregion

        #region APPEND QUERIES

        void Append( string op, string xml ) {

            XmlParser parser = new XmlParser();
            parser.Parse( xml );
            XmlDocument new_branch = RemoveWhere( parser.Tree );

            AddOperator( op );
            _current.AppendChild( _where.ImportNode( new_branch.DocumentElement , deep:true ));

        }

        public void And( string xml ) {

            Append( "And" , xml );

        }

        public void Or( string xml ) {

            Append( "Or" , xml );

        }

        #endregion

        #region ORDER BY

        public void OrderBy ( string field , bool ascending=true ) { 

            this.AddOrder( field , ascending );
        }

        public void AppendOrderBy ( string field , bool ascending=true ) { 

            this.AddOrder( field , ascending );
        }

        #endregion

    }
}
