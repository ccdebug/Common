using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor.Dsl;
using Disruptor;

namespace DbLogger
{
    public class DisruptorUtils : IDisposable
    {
        private const int RING_BUFFER_SIZE = 1024 * 1024;
        private const int CONSUMER_NUM = 4;     //默认4个消费者
        private static readonly Disruptor<LogMessageEvent> _disruptor;
        private static readonly LogMessageEventTranslator _tanslator;

        static DisruptorUtils()
        {
            _disruptor = new Disruptor<LogMessageEvent>(() => new LogMessageEvent(), RING_BUFFER_SIZE,
                TaskScheduler.Current, ProducerType.Single, new YieldingWaitStrategy());

            //初始化4个消费者
            var consumers = new List<LogMessageWorkHandler>();
            for (int i = 1; i < CONSUMER_NUM; i++)
			{
			    consumers.Add(new LogMessageWorkHandler());
			}

            //每个事件被一个消费者消费
            _disruptor.HandleEventsWithWorkerPool(consumers.ToArray());

            _tanslator = new LogMessageEventTranslator();   //初始化生产者

            _disruptor.Start();

        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="logMessage"></param>
        public static void Publish(LogMessage logMessage)
        {
            _disruptor.RingBuffer.PublishEvent(_tanslator, logMessage);

            MetricsInfluxDb.Metrics.Mark(MetricKeys.LOGMESSAGE_PUBLISH);
        }

        public void Dispose()
        {
            _disruptor.Shutdown();
        }
    }
}
