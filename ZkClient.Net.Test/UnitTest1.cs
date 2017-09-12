using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.zookeeper;
using System.Linq;

namespace ZkClient.Net.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           ZkClient client = new ZkClient("114.215.169.82:12181", 1000 * 10, 1000 * 5);
           client.SubscribeStateChange(new ZkStateListener());

        }

        class ZkStateListener : IZkStateListener
        {
            public void HandleStateChanged(Watcher.Event.KeeperState state)
            {
                
            }

            public void HandleNewSession()
            {
                
            }

            public void HandleSessionEstablishmentError(Exception error)
            {
                throw new NotImplementedException();
            }
        }
    }
}
