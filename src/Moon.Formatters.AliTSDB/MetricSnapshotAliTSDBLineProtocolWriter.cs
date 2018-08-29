using App.Metrics;
using App.Metrics.Serialization;
using Moon.Formatters.AliTSDB.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Moon.Formatters.AliTSDB
{
    public class MetricSnapshotAliTSDBLineProtocolWriter : IMetricSnapshotWriter
    {
        private readonly TextWriter _textWriter;
        private readonly Func<string, string, string> _metricNameFormatter;
        private readonly LineProtocolPoints _points;
        
        public MetricSnapshotAliTSDBLineProtocolWriter(
            TextWriter textWriter,
            Func<string, string, string> metricNameFormatter = null,
            GeneratedMetricNameMapping dataKeys = null)
        {
            _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            _points = new LineProtocolPoints();
            if (metricNameFormatter == null)
            {
                _metricNameFormatter = (metricContext, metricName) => string.IsNullOrWhiteSpace(metricContext)
                    ? metricName
                    : $"[{metricContext}] {metricName}";
            }
            else
            {
                _metricNameFormatter = metricNameFormatter;
            }

            MetricNameMapping = dataKeys ?? new GeneratedMetricNameMapping();
        }

        public GeneratedMetricNameMapping MetricNameMapping { get; }

        public void Write(string context, string name, object value, MetricTags tags, DateTime timestamp)
        {
            var measurement = _metricNameFormatter(context, name);

            _points.Add(new LineProtocolPoint(measurement, new Dictionary<string, object> { { "value", value } }, tags, timestamp));
        }

        public void Write(string context, string name, string field, object value, MetricTags tags, DateTime timestamp)
        {
            var measurement = _metricNameFormatter(context, name);

            _points.Add(new LineProtocolPoint(measurement, new Dictionary<string, object> { { field, value } }, tags, timestamp));
        }

        /// <inheritdoc />
        public void Write(string context, string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags, DateTime timestamp)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data }).ToDictionary(pair => pair.column, pair => pair.data);

            var measurement = _metricNameFormatter(context, name);

            _points.Add(new LineProtocolPoint(measurement, fields, tags, timestamp));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _points.Write(_textWriter);

#if !NETSTANDARD1_6
                _textWriter?.Close();
#endif
                _textWriter?.Dispose();
            }
        }

     
    }
}
