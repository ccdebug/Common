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
        private static ILog _logger = LogManager.GetLogger(typeof(LogMessageWorkHandler));

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
                    MetricsInfluxDb.Metrics.Mark(MetricKeys.LOGMESSAGE_STORE_SUCCESS);
                }
                else
                {
                    MetricsInfluxDb.Metrics.Mark(MetricKeys.LOGMESSAGE_STORE_ERROR);
                }
            }
            catch (Exception ex)
            {
                //记录日志，发送到metrics
                MetricsInfluxDb.Metrics.Mark(MetricKeys.LOGMESSAGE_STORE_ERROR);
                _logger.Error(ex.ToString());
            }
        }
    }
}
