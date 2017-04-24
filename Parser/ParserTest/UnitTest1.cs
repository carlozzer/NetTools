using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using NetParser;
using System.Diagnostics;

namespace ParserTest
{
    [TestClass]
    public class MarkupParserTest
    {
        [TestMethod]
        public void TestParse()
        {
            // ARRANGE
            WebClient wc = new WebClient();
            //string markup = wc.DownloadString("http://www.wikipedia.org");
            //string markup = wc.DownloadString("https://www.w3schools.com/xml/note.xml");

            string markup = @" <Query>
 <Where>
  <And>
   <Gt>
    <FieldRef Name='Quantity' />
    <Value Type='Number'>0</Value>
   </Gt>
   <Leq>
    <FieldRef Name='Price' />
    <Value Type='Number'>2000</Value>
   </Leq>
  </And>
 </Where>
</Query>";

            // ACT
            // MarkupParser parser = new MarkupParser();
            // parser.Parse( markup );

            XmlLexer lexer = new XmlLexer();
            lexer.Read( markup );

            //// OUTPUT
            //foreach ( XmlToken tok in lexer.GetTokens()) {
            //    Trace.WriteLine( string.Format(" ['{0}',{1}]", tok.lexeme, tok.type) );
            //}

            XmlParser parser = new XmlParser();
            parser.Parse( lexer.GetTokens() );
            parser.TraceTree();

            // ASSERT
            Assert.AreEqual(1,1);

        }
    }
}
