using App.Metrics;
using App.Metrics.Formatters;
using App.Metrics.Internal;
using App.Metrics.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moon.Formatters.AliTSDB
{
    public class MetricsAliTSDBLineProtocolOutputFormatter : IMetricsOutputFormatter
    {
        private readonly MetricsAliTSDBLineProtocolOptions _options;

        public MetricsAliTSDBLineProtocolOutputFormatter()
        {
            _options = new MetricsAliTSDBLineProtocolOptions();
        }

        /// <inheritdoc/>
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue("text", "vnd.appmetrics.metrics.influxdb", "v1", "plain");

        public Task WriteAsync(Stream output, MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var serilizerJson = JsonSerializer.Create();

            //using (var streamWriter = new StreamWriter(output))
            //{
            //    // TODO: #251 should apply metric field names
            //    using (var textWriter = new JsonTextWriter(streamWriter))
            //    {
            //        serilizerJson.Serialize(textWriter, metricsData);
            //    }
            //}

            var serializer = new MetricSnapshotSerializer();

            using (var streamWriter = new StreamWriter(output))
            {
                using (var textWriter = new MetricSnapshotAliTSDBLineProtocolWriter(streamWriter, _options.MetricNameFormatter))
                {
                    serializer.Serialize(textWriter, metricsData);
                }
            }

#if !NETSTANDARD1_6
            return AppMetricsTaskHelper.CompletedTask();
#else
            return Task.CompletedTask;
#endif
        }
    }
}
