using MetricsInfluxDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetricsTest
{
    [TestClass]
    public class MetricsUtilsTest
    {
        [TestMethod]
        public void TestMark()
        {
            MetricsInfluxDb.Metrics.Mark("flightrequest");
            Thread.Sleep(1000 * 60);
        }
    }
}
