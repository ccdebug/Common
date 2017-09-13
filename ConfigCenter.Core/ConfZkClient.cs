
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using org.apache.zookeeper;
using ZkClient.Net;

namespace ConfigCenter.Core
{
    public class ConfZkClient
    {
        private static readonly ZkClient.Net.ZkClient ZkClient;
        private static readonly ZkDataListener ZkDataListener;
        private readonly string _group;

        public ConfZkClient(string group)
        {
            _group = group;
        }

        static ConfZkClient()
        {
            var zkServers = ConfigurationManager.AppSettings["config.zkservers"];
            ZkClient = new ZkClient.Net.ZkClient(new ZkOptions() {ZkServers = zkServers});
            ZkDataListener = new ZkDataListener();
        }

        #region CRUD

        public async Task<bool> AddAsync(string key, string value)
        {
            var path = PathUtils.KeyToPath(_group, key);
            try
            {
                await ZkClient.CreatePersistent(path);
                await ZkClient.SetData(path, value);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }

            ZkClient.SubscribeDataChange(path, ZkDataListener);

            return await Task.FromResult(true);
        }

        public async Task<string> GetDataAsync(string key)
        {
            return await ZkClient.GetData(PathUtils.KeyToPath(_group, key));
        }

        public async Task<bool> UpdateAsync(string key, string value)
        {
            try
            {
                await ZkClient.SetData(PathUtils.KeyToPath(_group, key), value);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var path = PathUtils.KeyToPath(_group, key);
            try
            {
                await ZkClient.DeleteRecursive(path);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }

            ZkClient.UnSubscribeDataChange(path, ZkDataListener);

            return await Task.FromResult(true);
        }
        #endregion
    }

    #region Listener

    internal class ZkDataListener : IZkDataListener
    {
        private const int DefaultExpiryMils = 1000 * 60 * 10;

        public void HandleDataChange(string dataPath, string data)
        {
            ConfClient.Set(PathUtils.PathToKey2(dataPath), data, DateTime.Now.AddMilliseconds(DefaultExpiryMils));
        }

        public void HandleDataDeleted(string dataPath)
        {
            ConfClient.Delete(PathUtils.PathToKey2(dataPath));
        }
    }

    internal class ZkStateListener : IZkStateListener
    {
        public void HandleStateChanged(Watcher.Event.KeeperState state)
        {
            
        }

        public void HandleNewSession()
        {
            
        }

        public void HandleSessionEstablishmentError(Exception error)
        {

        }
    }

    internal class ZkChildListener : IZkChildListener
    {
        public void HandleChildChange(string parentPath, List<string> currentChildren)
        {
            
        }
    }

    #endregion
}