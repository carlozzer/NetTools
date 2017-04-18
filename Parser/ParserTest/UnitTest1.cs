using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using NetParser;

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
            string markup = wc.DownloadString("https://www.w3schools.com/xml/note.xml");

            // ACT
            // MarkupParser parser = new MarkupParser();
            // parser.Parse( markup );

            XmlLexer lexer = new XmlLexer();
            lexer.Read( markup );


            // ASSERT
            Assert.AreEqual(1,1);

        }
    }
}
