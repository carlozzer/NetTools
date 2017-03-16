using System;

namespace NetParser
{
    public class MarkupParser
    {
        StatesMachine machine;

        public MarkupParser() {
            
            machine = new StatesMachine();

            machine.Add( "INIT", "<" , "OPEN_TAG" );
            machine.Add( "OPEN_TAG", "!" , "DECLARATION" );
        }

        public void Parse( string markup ) { 

            if ( !string.IsNullOrWhiteSpace( markup ) ) { 

                int pointer = 0;
                while ( pointer < markup.Length ) { 

                    int ret = machine.Parse( markup[pointer] );
                    pointer -= ret;

                    pointer++;
                }

            }
        }
    }
}
