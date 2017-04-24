using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NetParser
{
    public class XmlParser
    {
        XmlDocument _xml;
        XmlNode _current; 

        void EnsureDocument () { 

            if ( _xml == null ) {
                _xml = new XmlDocument();
                _current = _xml.DocumentElement;
            }
        }

        void AddChild ( XmlToken token ) { 

            this.EnsureDocument();
            XmlElement new_element = _xml.CreateElement( token.lexeme );
            _current.AppendChild( new_element );

        }

        void BackToParent () { 
            _current = _current.ParentNode;
        }

        public void TraceTree() { 

            Trace.WriteLine( _xml.OuterXml );

        }

        public void Parse( List<XmlToken> tokens ) { 

            // flags
            bool is_opening = false;
            bool is_closing = false;

            string current_element = "";

            if ( tokens != null && tokens.Count > 0 ) { 

                foreach ( XmlToken token in tokens ) { 

                    if ( token.type == "<" ) { 
                         
                        is_closing = false;
                        is_opening = true;

                    }

                    if ( token.type == "</" || token.type == "/>" ) {
                        
                        is_closing = true;
                        is_opening = false;
                    }

                    if ( token.type == "ALPHA" ) { 

                        if ( is_opening ) { 
                            current_element = token.lexeme;
                        }

                    }

                    if ( token.type == ">" || token.type == "/>" ) {

                        if ( is_opening ) { 
                            AddChild( token );
                        }

                        if ( is_closing ) { 
                            BackToParent();
                        }

                    }

                }

            }

        } 

        
    }
}
