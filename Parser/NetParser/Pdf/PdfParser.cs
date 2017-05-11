using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Pdf
{
    public enum PdfParserFlags { 
        idle,
        expect_gen,
        expect_obj,
        is_obj,
        is_dic_key,
        is_dic_value,
        is_stream
    }

    public class PdfParser
    {
        List<PdfObject> objects = new List<PdfObject>();

        DicNode dic_tree;
        DicNode current_node;
        DicEntry current_entry;

        public void Parse( List<PdfToken> tokens ) { 

            PdfObject current_obj = new PdfObject();

            PdfParserFlags flag = PdfParserFlags.idle;


            if ( tokens != null && tokens.Count > 0 ) { 

                foreach ( PdfToken token in tokens ) { 

                    if ( token.type == "NUMBER" ) { 
                        
                        if ( flag == PdfParserFlags.expect_gen ) {

                            current_obj.generation_number = token.lexeme;
                            flag = PdfParserFlags.expect_obj;
                            
                        }

                        if ( flag == PdfParserFlags.idle ) {
                            current_obj.object_number = token.lexeme;
                            flag = PdfParserFlags.expect_gen;
                        }
                    }

                    if ( token.type == "KEYWORD" ) { 

                        if ( token.lexeme == "obj" ) { 

                            flag = PdfParserFlags.is_obj;
                        }

                    }

                    if ( token.type == "<<" ) { 

                        current_entry = new DicEntry();

                        if ( dic_tree != null ) { 

                            dic_tree = new DicNode( new DicEntry() { Name="DICTIONARY",Value=null });
                            dic_tree.AddChild( current_entry );

                        } else { 

                            dic_tree.

                        }

                        flag = PdfParserFlags.is_dic_key;
                            
                    }

                    if ( token.type == "NAME" ) { 

                        if ( flag == PdfParserFlags.is_dic_key ) { 

                            current_entry

                        }

                        if ( flag == PdfParserFlags.is_dic_key ) { 

                            current_entry.Name = token.lexeme;
                            flag = PdfParserFlags.is_dic_value;

                        }

                    }


                }

            }

        } 
    }
}
