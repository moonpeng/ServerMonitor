using App.Metrics.Formatters;
using Moon.Reporting.AliTSDB.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moon.Reporting.AliTSDB
{
    public class MetricsReportingAliTSDBOptions
    {
        public MetricsReportingAliTSDBOptions()
        {
            FlushInterval = TimeSpan.FromSeconds(10);
            HttpPolicy = new HttpPolicy
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            AliTSDB = new AliTSDBOptions();
        }

        public IMetricsOutputFormatter MetricsOutputFormatter { get; set; }

        public HttpPolicy HttpPolicy { get; set; }

        public AliTSDBOptions AliTSDB { get; set; }

        public TimeSpan FlushInterval { get; set; }
    }
}
