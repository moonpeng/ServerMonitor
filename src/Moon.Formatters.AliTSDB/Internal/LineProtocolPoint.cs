using App.Metrics;
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

        public void Write(TextWriter textWriter, bool writeTimestamp = true)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException(nameof(textWriter));
            }

            textWriter.Write(LineProtocolSyntax.EscapeName(Measurement));

            if (Tags.Count > 0)
            {
                for (var i = 0; i < Tags.Count; i++)
                {
                    textWriter.Write(',');
                    textWriter.Write(LineProtocolSyntax.EscapeName(Tags.Keys[i]));
                    textWriter.Write('=');
                    textWriter.Write(LineProtocolSyntax.EscapeName(Tags.Values[i]));
                }
            }

            var fieldDelim = ' ';

            foreach (var f in Fields)
            {
                textWriter.Write(fieldDelim);
                fieldDelim = ',';
                textWriter.Write(LineProtocolSyntax.EscapeName(f.Key));
                textWriter.Write('=');
                textWriter.Write(LineProtocolSyntax.FormatValue(f.Value));
            }

            if (!writeTimestamp)
            {
                return;
            }

            textWriter.Write(' ');

            if (UtcTimestamp == null)
            {
                textWriter.Write(LineProtocolSyntax.FormatTimestamp(DateTime.UtcNow));
                return;
            }

            textWriter.Write(LineProtocolSyntax.FormatTimestamp(UtcTimestamp.Value));
        }
    }
}
