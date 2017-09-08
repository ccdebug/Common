using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace DbLogger.Test
{
    [TestClass]
    public class DbLoggerTest
    {
        [TestMethod]
        public void TestPublish()
        {
            for (int i = 0; i < 100000; i++)
            {
                var message = new LogMessage() 
                { 
                    Ikey = Guid.NewGuid().ToString(),
                    Username = "test",
                    LogType = "测试",
                    LogTime = DateTime.Now,
                    ServerIp = "192.168.1.1",
                    ClientIp = "192.168.1.2",
                    OrderNo = Guid.NewGuid().ToString("n"),
                    Module = "module",
                    Content = @"转换Agent.GetLowPriceFlightRequest成功，{""SCity"":""PVG"",""ECity"":""CTU"",""FlightDate"":""2016-11-20T00:00:00"",""msgid"":null,""msgkey"":null,""msgtype"":null,""RequestKey"":null,""AppId"":100403,""Username"":null}",
                    Keyword = ""
                };
                Logger.Handle(message);
            }

            Thread.Sleep(1000 * 6000);
        }
    }
}
