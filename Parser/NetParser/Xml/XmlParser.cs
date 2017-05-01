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
    public enum XmlParserFlags {
        
        idle,
        is_opening,
        is_closing,
        expect_attr,  
        expect_equal, 
        expect_value 

    }

    public class XmlParser
    {
        #region XML TREE

        XmlDocument _xml;
        XmlNode _current; 

        public XmlDocument Tree { get { 
            return _xml;
        } }

        void EnsureDocument () { 

            if ( _xml == null ) {
                _xml = new XmlDocument();
                _current = _xml.DocumentElement;
            }
        }

        void AddElement ( string element , List<string[]> attributes ) { 

            this.EnsureDocument();
            XmlElement new_element = _xml.CreateElement( element );
            
            if ( attributes != null && attributes.Count > 0 ) { 

                foreach ( string[] attr in attributes ) { 

                    new_element.SetAttribute( attr[0] , attr[1] );

                }

            }

            _current = _current == null ? 
                _xml.AppendChild( new_element ) :
                _current.AppendChild( new_element );

        }

        void AddInnerText ( string text ) { 

            _current.InnerText += text;

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

        string[] current_attr = new string[2];
        List<string[]> all_attributes;

        void ResetAttributes() { 
            all_attributes = new List<string[]>();
        }

        void ResetCurrentAttribute() { 
            current_attr = new string[2];
        }

        void EnsureAttributes() { 
            if ( all_attributes == null ) ResetAttributes();
        }

        void AccumulateAttribute ( string[] attr ) { 

            EnsureAttributes();
            all_attributes.Add( attr );
        }

        #endregion

        public void Parse( List<XmlToken> tokens ) { 

            string current_element = "";
            bool inserted = false;
            XmlParserFlags flag = XmlParserFlags.idle;

            if ( tokens != null && tokens.Count > 0 ) { 

                foreach ( XmlToken token in tokens ) { 

                    if ( token.type == "<" ) { 
                         
                        flag = XmlParserFlags.is_opening;
                        inserted = false;
      
                    }

                    if ( token.type == "</" || token.type == "/>" ) {
      
                        flag = XmlParserFlags.is_closing;
                 
                    }

                    if ( token.type == "=" ) {
                        
                        flag = XmlParserFlags.expect_value;
                    }

                    if ( token.type == "STRING" || token.type == "STRINGD" ) { 

                        if ( flag == XmlParserFlags.expect_value ) {
                            
                            current_attr[1] = token.lexeme.Replace("\"",string.Empty).Replace("'",string.Empty);
                            AccumulateAttribute( current_attr );
                            flag = XmlParserFlags.expect_attr;

                        }

                    } 

                    if ( token.type == "ALPHA" ) { 

                        if ( flag == XmlParserFlags.idle ) {
                            
                            AddInnerText( token.lexeme );

                        }

                        if ( flag == XmlParserFlags.expect_equal ) {

                            AccumulateAttribute( current_attr );
                            flag = XmlParserFlags.expect_attr;

                        }

                        if ( flag == XmlParserFlags.expect_value ) { 

                            current_attr[1] = token.lexeme;
                            AccumulateAttribute( current_attr );
                            flag = XmlParserFlags.expect_attr;

                        } 
                        
                        if ( flag == XmlParserFlags.expect_attr ) {

                            current_attr[0] = token.lexeme;
                            flag = XmlParserFlags.expect_equal;

                        } 

                        if ( flag == XmlParserFlags.is_opening ) { 

                            current_element = token.lexeme;
                            flag = XmlParserFlags.expect_attr;
                            
                        }

                    }

                    if ( token.type == ">" || token.type == "/>" ) {

                        if ( flag == XmlParserFlags.expect_attr ) { 
                            AddElement( current_element , this.all_attributes );
                            inserted = true;
                            ResetAttributes();
                            flag = XmlParserFlags.idle;
                        }

                        if ( flag == XmlParserFlags.is_closing ) { 
                            if ( !inserted ) { 
                                AddElement ( current_element , this.all_attributes );
                                inserted = true;
                                ResetAttributes();
                                flag = XmlParserFlags.idle;
                            }
                            
                            BackToParent();
                                
                        }

                        

                    }

                    

                }

            }

        } 

        
    }
}
