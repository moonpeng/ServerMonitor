using App.Metrics;
using App.Metrics.Filters;
using App.Metrics.Formatters;
using App.Metrics.Reporting;
using Moon.Formatters.AliTSDB;
using Moon.Reporting.AliTSDB.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moon.Reporting.AliTSDB
{
    public class AliTSDBMetricsReporter : IReportMetrics
    {
        private readonly ILineProtocolClient _lineProtocolClient;
        private readonly IMetricsOutputFormatter _defaultMetricsOutputFormatter = new MetricsAliTSDBLineProtocolOutputFormatter();

        public AliTSDBMetricsReporter(
            MetricsReportingAliTSDBOptions options,
            ILineProtocolClient lineProtocolClient)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.FlushInterval < TimeSpan.Zero)
            {
                throw new InvalidOperationException($"{nameof(MetricsReportingAliTSDBOptions.FlushInterval)} must not be less than zero");
            }

            _lineProtocolClient = lineProtocolClient ?? throw new ArgumentNullException(nameof(lineProtocolClient));

            Formatter = options.MetricsOutputFormatter ?? _defaultMetricsOutputFormatter;

            FlushInterval = options.FlushInterval > TimeSpan.Zero
                ? options.FlushInterval
                : AppMetricsConstants.Reporting.DefaultFlushInterval;
        }

        public IFilterMetrics Filter { get; set; }
        public TimeSpan FlushInterval { get; set; }
        public IMetricsOutputFormatter Formatter { get; set; }

        public async Task<bool> FlushAsync(MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            LineProtocolWriteResult result;

            using (var stream = new MemoryStream())
            {
                await Formatter.WriteAsync(stream, metricsData, cancellationToken);

                result = await _lineProtocolClient.WriteAsync(Encoding.UTF8.GetString(stream.ToArray()), cancellationToken);
            }

            if (result.Success)
            {
                return true;
            }

            return false;
        }
    }
}
