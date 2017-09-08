using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsInfluxDb
{
    class AppSettings
    {
        #region AppSetting 节点

        private static int _appId = 0;
        /// <summary>
        /// 应用程序编号
        /// </summary>
        public static int AppId
        {
            get
            {
                if (_appId <= 0)
                {
                    string val = GetAppValue("AppId");
                    if (!int.TryParse(val, out _appId)) _appId = 100201;
                }
                return _appId;
            }
        }

        private static string _influxDbUri;
        /// <summary>
        /// Uri格式：http://114.215.169.82:8086/write?db=mydb&u=admin&p=admin
        /// </summary>
        public static string InfluxDbUri
        {
            get
            {
                if (string.IsNullOrEmpty(_influxDbUri))
                {
                    _influxDbUri = GetAppValue("influxDbUri");
                }
                return _influxDbUri;
            }
        }

        #endregion

        #region 辅助函数

        /// <summary>
        /// 获取配置项的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAppValue(string name)
        {
            NameValueCollection config = ConfigurationManager.AppSettings;
            return config[name];
        }

        #endregion
    }
}
