using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using NetParser.Pdf;
using System.Diagnostics;

namespace ParserTest
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void TestMethod1()
        {
            string content = File.ReadAllText( @"C:\Users\Carlos.merino\Downloads\pdf-sample.pdf" );

            PdfLexer lexer = new PdfLexer();
            lexer.Read( content );

            int index = 0;
            foreach ( PdfToken token in lexer.GetTokens() ) { 
              
                Trace.WriteLine( string.Format("({2})  [{0},{1}]", token.lexeme, token.type , index ) );
                index++;
            }

            bool stopped = true;

        }
    }
}
