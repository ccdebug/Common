namespace ZkClient.Net
{
    public class ZkOptions
    {
        /// <summary>
        /// zookeeper服务器，192.168.2.103:2181,192.168.2.104:2181,192.168.2.105:2181
        /// </summary>
        public string ZkServers { get; set; }

        /// <summary>
        /// 客户端与zk服务器的session超时时间，默认10s，单位毫秒
        /// </summary>
        public int SessionTimeout { get; set; }
        /// <summary>
        /// 连接zk服务器的超时时间，默认15秒，单位毫秒
        /// </summary>

        public int ConnectionTimeout { get; set; }
        /// <summary>
        /// 执行操作的超时时间，默认10s，单位毫秒
        /// </summary>

        public int OprationRetryTimeout { get; set; }

        public ZkOptions()
        {
            SessionTimeout = 1000 * 10;
            ConnectionTimeout = 1000 * 15;
            OprationRetryTimeout = 1000 * 10;
        }
    }
}