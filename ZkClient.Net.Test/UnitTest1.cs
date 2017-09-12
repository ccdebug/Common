using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.zookeeper;
using System.Linq;
using System.Threading;

namespace ZkClient.Net.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ZkClient client = new ZkClient("114.215.169.82:12181", 1000 * 5, 1000 * 5);
            client.SubscribeStateChange(new ZkStateListener());
            client.SubscribeChildChange("/data", new ZkChildListener()).GetAwaiter().GetResult();
            client.SubscribeDataChange("/data", new ZkDataListener()).GetAwaiter().GetResult();
            client.Create("/data", "1111", CreateMode.PERSISTENT).GetAwaiter().GetResult();
            for (var i = 0; i < 20; i++)
            {
                client.Create($"/data/node{i}", i.ToString(), CreateMode.PERSISTENT).GetAwaiter().GetResult();
                var sleeping = new Random().Next(4 * 1000, 10 * 1000);
                Debug.WriteLine(sleeping);
                Thread.Sleep(sleeping);
            }
            //var result = client.GetData("/data").GetAwaiter().GetResult();
            Thread.Sleep(1000 * 600);

        }

        class ZkStateListener : IZkStateListener
        {
            public void HandleStateChanged(Watcher.Event.KeeperState state)
            {
                
            }

            public void HandleNewSession()
            {
                Debug.WriteLine("超时重建连接");
            }

            public void HandleSessionEstablishmentError(Exception error)
            {
            }
        }

        class ZkDataListener: IZkDataListener
        {
            public void HandleDataChange(string dataPath, string data)
            {
            }

            public void HandleDataDeleted(string dataPath)
            {
            }
        }

        class ZkChildListener : IZkChildListener
        {
            public void HandleChildChange(string parentPath, List<string> currentChildren)
            {
                Debug.WriteLine(parentPath);
                Debug.WriteLine(string.Join(",", currentChildren.ToArray()));
                Debug.WriteLine("--------------------------------------");
            }
        }
    }
}
