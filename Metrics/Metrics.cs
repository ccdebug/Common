using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metrics.InfluxDB;
using Metrics;
using System.Configuration;
using Metrics.InfluxDB.Adapters;

namespace MetricsInfluxDb
{
    public class Metrics
    {
        private static readonly MetricTags DefaultTags = new MetricTags(string.Format("AppId={0}", AppSettings.AppId)
            , string.Format("ServerIP={0}", IpHelper.GetLocalIp()));

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(Metrics));

        static Metrics()
        {
            try
            {
                Metric.Config.WithReporting(report => report.WithInfluxDbHttp(
                        new Uri(AppSettings.InfluxDbUri),
                        TimeSpan.FromSeconds(10),
                        null,
                        c => c.WithFormatter(new CustomerInfluxdbFormatter())
                        ));
            }
            catch (Exception ex)
            {
                Logger.Error("初始化Metrics失败，" + ex.ToString());
            }
        }

        #region Gauges

        public static void Gauge(string name, double value)
        {
            Gauge(name, value, DefaultTags);
        }

        public static void Gauge(string name, double value, MetricTags tags)
        {
            Gauge(name, value, Unit.Requests, ConcatMetricTags(tags));
        }

        public static void Gauge(string name, double value, Unit unit, MetricTags tags)
        {
            Metric.Gauge(name, () => value, unit, ConcatMetricTags(tags));
        }

        #endregion

        #region Counter

       // Command Counter
       //      Count = 2550 Commands
       //Total Items = 5
       //     Item 0 = 20.90%   533 Commands [BillCustomer]
       //     Item 1 = 19.22%   490 Commands [MakeInvoice]
       //     Item 2 = 19.41%   495 Commands [MarkAsPreffered]
       //     Item 3 = 20.98%   535 Commands [SendEmail]
       //     Item 4 = 19.49%   497 Commands [ShipProduct]
        public static Counter Counter(string name, Unit unit, MetricTags tags)
        {
            return Metric.Counter(name, unit, tags);
        }

        public static void Increment(string name)
        {
            var counter = Counter(name, Unit.Requests, DefaultTags);
            counter.Increment();
        }

        public static void Increment(string name, Unit unit)
        {
            var counter = Counter(name, unit, DefaultTags);
            counter.Increment();
        }

        public static void Increment(string name, Unit unit, MetricTags tags)
        {
            var counter = Counter(name, unit, ConcatMetricTags(tags));
            counter.Increment();
        }
        #endregion

        #region Meter
        public static void Mark(string name)
        {
            Mark(name, Unit.Requests, TimeUnit.Seconds, DefaultTags);
        }

        public static void Mark(string name, Unit unit)
        {
            Mark(name, unit, TimeUnit.Seconds, DefaultTags);
        }

        public static void Mark(string name, MetricTags tags)
        {
            Mark(name, Unit.Requests, TimeUnit.Seconds, ConcatMetricTags(tags));
        }

        public static void Mark(string name, Unit unit, MetricTags tags)
        {
            Mark(name, unit, TimeUnit.Seconds, ConcatMetricTags(tags));
        }

        public static void Mark(string name, Unit unit, TimeUnit rateUnit, MetricTags tags)
        {
            var meter = Meter(name, unit, rateUnit, ConcatMetricTags(tags));
            meter.Mark();
        }

        //    Errors
        //         Count = 450 Errors
        //    Mean Value = 35.68 Errors/s
        // 1 Minute Rate = 25.44 Errors/s
        // 5 Minute Rate = 24.30 Errors/s
        //15 Minute Rate = 24.10 Errors/s
        //   Total Items = 5
        //        Item 0 = 19.56%    88 Errors [BillCustomer]
        //         Count = 88 Errors
        //    Mean Value = 6.98 Errors/s
        // 1 Minute Rate = 6.05 Errors/s
        // 5 Minute Rate = 6.01 Errors/s
        //15 Minute Rate = 6.00 Errors/s
        //        Item 1 = 18.67%    84 Errors [MakeInvoice]
        //         Count = 84 Errors
        //    Mean Value = 6.66 Errors/s
        // 1 Minute Rate = 4.23 Errors/s
        // 5 Minute Rate = 3.89 Errors/s
        //15 Minute Rate = 3.83 Errors/s
        //        Item 2 = 20.22%    91 Errors [MarkAsPreffered]
        //         Count = 91 Errors
        //    Mean Value = 7.22 Errors/s
        // 1 Minute Rate = 5.38 Errors/s
        // 5 Minute Rate = 5.24 Errors/s
        //15 Minute Rate = 5.21 Errors/s
        //        Item 3 = 19.78%    89 Errors [SendEmail]
        //         Count = 89 Errors
        //    Mean Value = 7.06 Errors/s
        // 1 Minute Rate = 4.92 Errors/s
        // 5 Minute Rate = 4.67 Errors/s
        //15 Minute Rate = 4.62 Errors/s
        //        Item 4 = 21.78%    98 Errors [ShipProduct]
        //         Count = 98 Errors
        //    Mean Value = 7.77 Errors/s
        // 1 Minute Rate = 4.86 Errors/s
        // 5 Minute Rate = 4.50 Errors/s
        //15 Minute Rate = 4.43 Errors/s
        public static Meter Meter(string name, Unit unit, TimeUnit rateUnit, MetricTags tags)
        {
            return Metric.Meter(name, unit, rateUnit, tags);
        } 
        #endregion

        #region Histograms

       //     Results
       //          Count = 90 Items
       //           Last = 46.00 Items
       //Last User Value = document-3
       //            Min = 2.00 Items
       // Min User Value = document-7
       //            Max = 98.00 Items
       // Max User Value = document-4
       //           Mean = 51.52 Items
       //         StdDev = 30.55 Items
       //         Median = 50.00 Items
       //           75% <= 80.00 Items
       //           95% <= 97.00 Items
       //           98% <= 98.00 Items
       //           99% <= 98.00 Items
       //         99.9% <= 98.00 Items
        public static Histogram Histogram(string name, Unit unit, SamplingType samplingType, MetricTags tags)
        {
            return Metric.Histogram(name, unit, samplingType, ConcatMetricTags(tags));
        }

        public static Histogram Histogram(string name, Unit unit)
        {
            return Metric.Histogram(name, unit, SamplingType.Default, DefaultTags);
        }

        public static Histogram Histogram(string name)
        {
            return Metric.Histogram(name, Unit.Requests, SamplingType.Default, DefaultTags);
        }

        public static void Update(string name, long value, string userValue = null)
        {
            var histogram = Histogram(name);
            histogram.Update(value, userValue);
        }

        #endregion

        #region Timer

        public static Timer Timer(string name)
        {
            return Metric.Timer(name, Unit.Requests, tags: DefaultTags);
        }

        public static Timer Timer(string name, Unit unit)
        {
            return Metric.Timer(name, unit, tags: DefaultTags);
        }

       //     Requests
       //          Count = 14 Requests
       //     Mean Value = 1.86 Requests/s
       //  1 Minute Rate = 1.80 Requests/s
       //  5 Minute Rate = 1.80 Requests/s
       // 15 Minute Rate = 1.80 Requests/s
       //          Count = 14 Requests
       //           Last = 869.03 ms
       //Last User Value = document-1
       //            Min = 59.90 ms
       // Min User Value = document-6
       //            Max = 869.03 ms
       // Max User Value = document-1
       //           Mean = 531.81 ms
       //         StdDev = 212.98 ms
       //         Median = 594.83 ms
       //           75% <= 670.18 ms
       //           95% <= 869.03 ms
       //           98% <= 869.03 ms
       //           99% <= 869.03 ms
       //         99.9% <= 869.03 ms
        public static Timer Timer(string name, Unit unit, MetricTags tags)
        {
            return Metric.Timer(name, unit, tags: ConcatMetricTags(tags));
        }

        #endregion

        private static MetricTags ConcatMetricTags(MetricTags tags)
        {
            if (tags.Tags == null || tags.Tags.Length <= 0) return DefaultTags;
            var concatTags = DefaultTags.Tags.Concat(tags.Tags);
            return new MetricTags(concatTags.ToArray());
        }
    }
}
