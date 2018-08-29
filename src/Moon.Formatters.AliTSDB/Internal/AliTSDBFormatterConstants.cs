using System;
using System.Collections.Generic;
using System.Text;

namespace Moon.Formatters.AliTSDB.Internal
{
    public static class AliTSDBFormatterConstants
    {
        public class LineProtocol
        {
            public static readonly Func<string, string, string> MetricNameFormatter =
                (metricContext, metricName) => string.IsNullOrWhiteSpace(metricContext)
                    ? $"{metricName}".Replace(' ', '_').ToLowerInvariant()
                    : $"{metricContext}__{metricName}".Replace(' ', '_').ToLowerInvariant();
        }
    }
}
