using System;
using System.Configuration;

namespace ConfigCenter.Core
{
    public class PathUtils
    {
        private const string ConfigDataRootPath = "config";
        private static string _group;

        public static string Group
        {
            get { return _group ?? (_group = ConfigurationManager.AppSettings["config.group"]); }
        }

        public static string KeyToPath(string key)
        {
            return $"/{ConfigDataRootPath}/{Group}/{key}";
        }

        public static string KeyToPath(string group, string key)
        {
            return $"/{ConfigDataRootPath}/{group}/{key}";
        }

        public static string PathToKey2(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            var pathArr = path.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (pathArr.Length > 2)
            {
                return pathArr[2];
            }
            return path;
        }

        public static string PathToKey(string path)
        {
            return PathToKey(Group, path);
        }

        public static string PathToKey(string group, string path)
        {
            var prefix = $"/{ConfigDataRootPath}/{group}";
            if (string.IsNullOrEmpty(path) || path.Length < prefix.Length || !path.StartsWith(prefix))
            {
                return null;
            }
            return path.Substring(prefix.Length + 1, path.Length);
        }
    }
}