using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void APISample()
        {
            var result = new EtrApiSample.EtrApiSample();
            TextWriterTraceListener myListener = new TextWriterTraceListener("TextWriterOutput.html", "myListener");
            myListener.WriteLine(result.EtrApiTest());
            myListener.Flush();
            Assert.IsFalse(false);
        }
    }
}
