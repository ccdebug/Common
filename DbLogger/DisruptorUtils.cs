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
        private const int RingBufferSize = 1024 * 1024;
        private const int ConsumerNum = 4;     //默认4个消费者
        private static readonly Disruptor<LogMessageEvent> Disruptor;
        private static readonly LogMessageEventTranslator Tanslator;

        static DisruptorUtils()
        {
            Disruptor = new Disruptor<LogMessageEvent>(() => new LogMessageEvent(), RingBufferSize,
                TaskScheduler.Current, ProducerType.Single, new YieldingWaitStrategy());

            //初始化4个消费者
            var consumers = new List<LogMessageWorkHandler>();
            for (var i = 1; i < ConsumerNum; i++)
			{
			    consumers.Add(new LogMessageWorkHandler());
			}

            //每个事件被一个消费者消费
            // ReSharper disable once CoVariantArrayConversion
            Disruptor.HandleEventsWithWorkerPool(consumers.ToArray());

            Tanslator = new LogMessageEventTranslator();   //初始化生产者

            Disruptor.Start();

        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="logMessage"></param>
        public static void Publish(LogMessage logMessage)
        {
            Disruptor.RingBuffer.PublishEvent(Tanslator, logMessage);

            MetricsInfluxDb.Metrics.Mark(MetricKeys.LogmessagePublish);
        }

        public void Dispose()
        {
            Disruptor.Shutdown();
        }
    }
}
