using System;
using log4net;
using org.apache.zookeeper;
using ZkClient.Net;

namespace ConfigCenter.Core
{
    internal class ZkStateListener : IZkStateListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ZkStateListener));

        public void HandleStateChanged(Watcher.Event.KeeperState state)
        {
            Logger.Info($"状态变更：{state.ToString()}");
        }

        public void HandleNewSession()
        {
            Logger.Info("超时重连");
        }

        public void HandleSessionEstablishmentError(Exception error)
        {
            Logger.Info("超时重连失败");
        }
    }
}