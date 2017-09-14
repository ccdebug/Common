using System;
using log4net;
using ZkClient.Net;

namespace ConfigCenter.Core
{
    internal class ZkDataListener : IZkDataListener
    {
        private const int DefaultExpiryMils = 1000 * 60 * 10;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ZkDataListener));

        public void HandleDataChange(string dataPath, string data)
        {
            Logger.Info($"更新配置：{dataPath}, data: {data}");
            ConfClient.Set(PathUtils.PathToKey2(dataPath), data, DateTime.Now.AddMilliseconds(DefaultExpiryMils));
        }

        public void HandleDataDeleted(string dataPath)
        {
            Logger.Info($"删除配置：{dataPath}");
            ConfClient.Delete(PathUtils.PathToKey2(dataPath));
        }
    }
}