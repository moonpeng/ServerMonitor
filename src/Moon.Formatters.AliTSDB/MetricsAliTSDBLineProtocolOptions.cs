using Moon.Formatters.AliTSDB.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moon.Formatters.AliTSDB
{
    public class MetricsAliTSDBLineProtocolOptions
    {
        public MetricsAliTSDBLineProtocolOptions()
        {
            MetricNameFormatter = AliTSDBFormatterConstants.LineProtocol.MetricNameFormatter;
        }

        public Func<string, string, string> MetricNameFormatter { get; set; }
    }
}
