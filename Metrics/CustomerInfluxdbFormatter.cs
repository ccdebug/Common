using Metrics.InfluxDB.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsInfluxDb
{
    public class CustomerInfluxdbFormatter : InfluxdbFormatter
    {
        public static class Default
        {
            /// <summary>
            /// The default context name formatter which formats the context stack and context name into a custom context name string.
            /// </summary>
            public static ContextFormatterDelegate ContextNameFormatter { get; private set; }

            /// <summary>
            /// The default metric name formatter which formats the context name and metric into a string used as the table to insert records into.
            /// </summary>
            public static MetricFormatterDelegate MetricNameFormatter { get; private set; }

            /// <summary>
            /// The default tag key formatter which formats a tag key into a string used as the column name in the InfluxDB table.
            /// </summary>
            public static TagKeyFormatterDelegate TagKeyFormatter { get; private set; }

            /// <summary>
            /// The default field key formatter which formats a field key into a string used as the column name in the InfluxDB table.
            /// </summary>
            public static FieldKeyFormatterDelegate FieldKeyFormatter { get; private set; }

            /// <summary>
            /// The default character used to replace space characters in identifier names. This value is an underscore.
            /// </summary>
            public static String ReplaceSpaceChar { get; set; }

            /// <summary>
            /// The default value for whether to convert identifier names to lowercase. This value is true.
            /// </summary>
            public static Boolean LowercaseNames { get; private set; }

            static Default()
            {
                ContextNameFormatter = (contextStack, contextName) => String.Join(".", contextStack.Concat(new[] { contextName }).Where(c => !String.IsNullOrWhiteSpace(c)));
                MetricNameFormatter = (context, name, unit, tags) => name;
                TagKeyFormatter = key => key;
                FieldKeyFormatter = key => key;
                ReplaceSpaceChar = "_";
                LowercaseNames = true;
            }
        }

        public CustomerInfluxdbFormatter()
            : base()
        {
            ContextNameFormatter = Default.ContextNameFormatter;
            MetricNameFormatter = Default.MetricNameFormatter;
            TagKeyFormatter = Default.TagKeyFormatter;
            FieldKeyFormatter = Default.FieldKeyFormatter;
            ReplaceSpaceChar = Default.ReplaceSpaceChar;
            LowercaseNames = Default.LowercaseNames;
        }
    }
}
