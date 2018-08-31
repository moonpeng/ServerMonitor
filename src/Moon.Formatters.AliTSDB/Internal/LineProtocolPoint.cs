using App.Metrics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Moon.Formatters.AliTSDB.Internal
{
    internal class LineProtocolPoint
    {
        public LineProtocolPoint(
            string measurement,
            IReadOnlyDictionary<string, object> fields,
            MetricTags tags,
            DateTime? utcTimestamp = null)
        {
            if (string.IsNullOrEmpty(measurement))
            {
                throw new ArgumentException("A measurement name must be specified");
            }

            if (fields == null || fields.Count == 0)
            {
                throw new ArgumentException("At least one field must be specified");
            }

            if (fields.Any(f => string.IsNullOrEmpty(f.Key)))
            {
                throw new ArgumentException("Fields must have non-empty names");
            }

            if (utcTimestamp != null && utcTimestamp.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Timestamps must be specified as UTC");
            }

            Measurement = measurement;
            Fields = fields;
            Tags = tags;
            UtcTimestamp = utcTimestamp;
        }

        public IReadOnlyDictionary<string, object> Fields { get; }

        public string Measurement { get; }

        public MetricTags Tags { get; }

        public DateTime? UtcTimestamp { get; }

        public void Write(JArray array)
        {
            var json = new JObject();

            json.Add("metric", new JValue(Measurement));
            json.Add("timestamp", new JValue(LineProtocolSyntax.FormatTimestamp(DateTime.UtcNow)));

            var tags = new JObject();

            if (Tags.Count > 0)
            {
                for (var i = 0; i < Tags.Count; i++)
                {
                    tags.Add(Tags.Keys[i], new JValue(LineProtocolSyntax.EscapeName(Tags.Values[i])));
                }
            }

            foreach (var f in Fields)
            {
                if (f.Key == "p75" || f.Key == "p95" || f.Key == "p98" || f.Key == "p99" || f.Key == "value")
                {
                    var tempjson = (JObject)json.DeepClone();
                    var temptags = (JObject)tags.DeepClone();

                    temptags.Add("sample", f.Key);
                    tempjson.Add("tags", temptags);
                    tempjson.Add("value", LineProtocolSyntax.FormatValue(f.Value));

                    array.Add(tempjson);
                }
            }
        }
    }
}
