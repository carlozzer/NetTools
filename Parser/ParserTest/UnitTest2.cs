using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetParser.Caml;
using System.Diagnostics;

namespace ParserTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            CamlQuery query = new CamlQuery();

            query.Where( "Title" ).Contains( "Madrid" ).And( "Title" ).Contains( "España" );
            query.OrderBy( "Fecha" );

            Trace.WriteLine( query.ToString());
            Assert.AreEqual(1,1);
        }
    }
}
