using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    public class TableUtils
    {
        private static readonly LogMessageRepository _logMessageRepository;

        static TableUtils()
        {
            _logMessageRepository = new LogMessageRepository();
        }

        public static string GetTableName(bool isHandleLog)
        {
            String tableName = "tbl_Interface_ProcessLog";
            if (isHandleLog)
            {
                tableName = "tbl_Interface_HandleLog";
            }
            tableName = string.Format("{0}{1}", tableName, GetLogTableSuffix());
            return tableName;
        }

        private static string GetLogTableSuffix()
        {
            string cacheKey = "logcenter.TableSuffix";
            string cacheValue = MemoryCache.Default.Get(cacheKey) as string;

            if (string.IsNullOrEmpty(cacheValue))
            {
                DateTime logDbServerTime = _logMessageRepository.GetDbServerTime();
                int leftTime = 60;
                cacheValue = logDbServerTime.ToString("_yyyyMM");

                DateTime monEnd = new DateTime(logDbServerTime.Year, logDbServerTime.Month, 1, 0, 0, 0);
                monEnd = monEnd.AddMonths(1);

                if ((monEnd - logDbServerTime).TotalSeconds < leftTime)
                {
                    return cacheValue;
                }
                MemoryCache.Default.Set(cacheKey, cacheValue, monEnd.AddSeconds(-1 * leftTime));
            }

            return cacheValue;
        }
    }
}
