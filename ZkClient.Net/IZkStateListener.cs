using System;
using System.Threading.Tasks;
using org.apache.zookeeper;

namespace ZkClient.Net
{
    public interface IZkStateListener
    {
        /// <summary>
        /// 当zk连接状态发生变化时触发
        /// </summary>
        /// <param name="state"></param>
        void HandleStateChanged(Watcher.Event.KeeperState state);

        /// <summary>
        /// 当Session过期，并且重新创建一个新的session成功后触发
        /// </summary>
        void HandleNewSession();

        /// <summary>
        /// 当Session过期，并且重新创建一个新的sessions失败后触发
        /// </summary>
        /// <param name="error"></param>
        void HandleSessionEstablishmentError(Exception error);
    }
}