
using System;
using System.Configuration;
using System.Threading.Tasks;
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
            ZkClient.SubscribeStateChange(new ZkStateListener());
            ZkDataListener = new ZkDataListener();
        }

        #region CRUD

        public async Task<bool> CreateAsync(string key, string value)
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

            return await Task.FromResult(true);
        }

        public async Task<string> GetAsync(string key)
        {
            var path = PathUtils.KeyToPath(_group, key);

            ZkClient.SubscribeDataChange(path, ZkDataListener);

            return await ZkClient.GetData(path);
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

            return await Task.FromResult(true);
        }
        #endregion
    }
}