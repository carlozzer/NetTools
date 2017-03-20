using System;

namespace NetParser
{
    public class MarkupParser
    {
        Automata machine;

        public MarkupParser() {
            
            machine = new Automata();

            machine.Add( Automata.START, "<" , MarkupStates.OPEN_TAG );
            machine.Add( MarkupStates.OPEN_TAG, "!" , MarkupStates.DECLARATION );
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
