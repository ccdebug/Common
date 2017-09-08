using Disruptor;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    public class LogMessageWorkHandler : IWorkHandler<LogMessageEvent>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogMessageWorkHandler));

        private static LogMessageRepository _repository;

        public LogMessageWorkHandler()
        {
            _repository = new LogMessageRepository();
        }

        public void OnEvent(LogMessageEvent @event)
        {
            try
            {
                var result = _repository.Insert(@event.LogMessage);
                if (result > 0)
                {
                    MetricsInfluxDb.Metrics.Mark(MetricKeys.LogmessageStoreSuccess);
                }
                else
                {
                    MetricsInfluxDb.Metrics.Mark(MetricKeys.LogmessageStoreError);
                }
            }
            catch (Exception ex)
            {
                //记录日志，发送到metrics
                MetricsInfluxDb.Metrics.Mark(MetricKeys.LogmessageStoreError);
                Logger.Error(ex.ToString());
            }
        }
    }
}
