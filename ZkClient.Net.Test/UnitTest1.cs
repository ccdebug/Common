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
        private ZkClient _zkClient = null;

        [TestInitialize]
        public void Init()
        {
            var zkOptions = new ZkOptions
            {
                ZkServers = "114.215.169.82:12181",
                SessionTimeout = 1000 * 5,
                ConnectionTimeout = 1000 * 5,
                OprationRetryTimeout = 1000 * 5
            };
            _zkClient = new ZkClient(zkOptions);
            _zkClient.SubscribeStateChange(new ZkStateListener());
            _zkClient.SubscribeChildChange("/data", new ZkChildListener());
            _zkClient.SubscribeDataChange("/data", new ZkDataListener());
        }

        [TestMethod]
        public void TestMethod1()
        {
            
            for (var i = 0; i < 20; i++)
            {
                _zkClient.CreatePersistent($"/data/node{i}", i.ToString()).GetAwaiter().GetResult();
                var sleeping = new Random().Next(4 * 1000, 10 * 1000);
                Debug.WriteLine(sleeping);
                Thread.Sleep(sleeping);
            }
            //var result = client.GetData("/data").GetAwaiter().GetResult();
            Thread.Sleep(1000 * 600);

        }

        [TestMethod]
        public void TestCreate()
        {
            Task.Run(async () => await _zkClient.CreatePersistent("/data/node01/node001/node0001", true));
            Thread.Sleep(1000 * 60);
        }

        [TestMethod]
        public void TesteDelete()
        {
            Task.Run(async () => await _zkClient.DeleteRecursive("/data"));
            Thread.Sleep(1000 * 60);
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
