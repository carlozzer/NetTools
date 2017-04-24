using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetParser
{
    public class XmlParser
    {
        #region XML TREE

        XmlDocument _xml;
        XmlNode _current; 

        void EnsureDocument () { 

            if ( _xml == null ) {
                _xml = new XmlDocument();
                _current = _xml.DocumentElement;
            }
        }

        void AddElement ( string element ,  ) { 

            this.EnsureDocument();
            XmlElement new_element = _xml.CreateElement( element );
            
            _current = _current == null ? 
                _xml.AppendChild( new_element ) :
                _current.AppendChild( new_element );

        }

        void BackToParent () { 
            if ( _current != null ) {
                _current = _current.ParentNode;
            }
        }

        public void TraceTree() { 

            Trace.WriteLine( _xml.OuterXml );

        }

        #endregion

        #region Attributes

        NameValueCollection all_attributes;

        void ResetAttributes() { 
            all_attributes = new NameValueCollection();
        }

        void EnsureAttributes() { 
            if ( all_attributes == null ) ResetAttributes();
        }

        void AccumulateAttribute ( string[] attr ) { 

            EnsureAttributes();
            all_attributes.Add( attr[0] , attr[1] );
        }
        
        #endregion


        public void Parse( List<XmlToken> tokens ) { 

            // flags
            bool is_opening = false;
            bool is_closing = false;
            bool is_attr    = false;
            bool is_value   = false;

            string current_element = "";
            string[] current_attr = new string[2];

            if ( tokens != null && tokens.Count > 0 ) { 

                foreach ( XmlToken token in tokens ) { 

                    if ( token.type == "<" ) { 
                         
                        is_closing = false;
                        is_attr    = false;
                        is_value   = false;
                        is_opening = true;

                    }

                    if ( token.type == "</" || token.type == "/>" ) {
                        
                        is_closing = true;
                        is_attr    = false;
                        is_value   = false;
                        is_opening = false;
                    }

                    if ( token.type == "=" ) {
                        is_value = true;
                    }

                    if ( token.type == "ALPHA" ) { 

                        if ( is_value ) { 

                            current_attr[1] = token.lexeme;
                            is_value = false;

                        } else if ( is_attr ) {

                            current_attr[0] = token.lexeme;

                        } else {

                            if ( is_opening ) { 
                                current_element = token.lexeme;
                                is_attr = true;
                            }
                        }

                    }

                    if ( token.type == ">" || token.type == "/>" ) {

                        

                        if ( is_opening ) { 
                            AddElement( current_element );
                        }

                        if ( is_closing ) { 
                            BackToParent();
                        }

                        is_attr = false;
                        // reset attributes

                    }

                    

                }

            }

        } 

        
    }
}
