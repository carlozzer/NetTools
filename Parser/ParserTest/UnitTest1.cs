using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Parser;

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
            string markup = wc.DownloadString("http://www.wikipedia.org");

            // ACT
            MarkupParser parser = new MarkupParser();
            parser.Parse( markup );

            // ASSERT
            Assert.AreEqual(1,1);

        }
    }
}
