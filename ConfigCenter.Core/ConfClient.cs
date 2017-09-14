using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace ConfigCenter.Core
{
    /// <summary>
    /// 配置系统统一客户端，配置的值会首先从本地缓存中获取，如果本地缓存没有找到，则去zk服务中心去查找，并将取回的值存入本地缓存
    /// 使用时需要在客户端配置zk服务器地址，以及分组名称（建议为项目名称）
    /// 配置项：
    /// config.zkservers: 192.168.2.103:2181,192.168.2.104:2181,192.168.2.105:2181
    /// config.group: bookingsoa
    /// zk服务器存储的path为：/config/bookingsoa/key
    /// </summary>
    public class ConfClient
    {
        private static readonly MemoryCache Cache;
        private const int DefaultExpiryMils = 1000 * 60 * 20;
        private static readonly ConfZkClient ConfZkClient;

        static ConfClient()
        {
            Cache = MemoryCache.Default;
            ConfZkClient = new ConfZkClient(PathUtils.Group);
        }

        public static string Get(string key, string defaultValue = "")
        {
            var value = Cache.Get(key);
            if (value != null) return value.ToString();
            value = Task.Run(async () => await ConfZkClient.GetAsync(key)).Result;
            if (value != null)
            {
                Set(key, value.ToString(), DateTime.Now.AddMilliseconds(DefaultExpiryMils));
            }
            return (value ?? defaultValue)?.ToString();
        }

        public static async Task<string> GetAsync(string key, string defaultValue = "")
        {
            var value = Cache.Get(key);
            if (value != null) return value.ToString();
            value = await ConfZkClient.GetAsync(key);
            if (value != null)
            {
                Set(key, value.ToString(), DateTime.Now.AddMilliseconds(DefaultExpiryMils));
            }
            return (value ?? defaultValue)?.ToString();
        }

        public static bool Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;
            Cache.Set(key, value, new CacheItemPolicy());
            return true;
        }

        public static bool Set(string key, string value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key) || value == null || expiry < DateTime.Now)
                return false;
            var cacheItem = new CacheItem(key, value);
            var cacheItemPolicy = new CacheItemPolicy {AbsoluteExpiration = expiry};
            Cache.Set(cacheItem, cacheItemPolicy);
            return true;
        }

        public static bool Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return false;
            return Cache.Add(key, value, new CacheItemPolicy());
        }

        public static bool Add(string key, string value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key) || value == null || expiry < DateTime.Now)
                return false;
            var cacheItem = new CacheItem(key, value);
            var cacheItemPolicy = new CacheItemPolicy {AbsoluteExpiration = expiry};
            return Cache.Add(cacheItem, cacheItemPolicy);
        }

        public static bool Delete(string key)
        {
            return Cache.Remove(key) != null;
        }

        public static bool KeyExists(string key)
        {
            return Cache.Get(key) != null;
        }
    }
}