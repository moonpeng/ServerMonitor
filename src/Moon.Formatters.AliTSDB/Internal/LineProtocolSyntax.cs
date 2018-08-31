using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Moon.Formatters.AliTSDB.Internal
{
    internal class LineProtocolSyntax
    {
        private static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private static readonly Dictionary<Type, Func<object, JValue>> Formatters = new Dictionary<Type, Func<object, JValue>>
                                                                                    {
                                                                                        { typeof(sbyte), FormatInteger },
                                                                                        { typeof(byte), FormatInteger },
                                                                                        { typeof(short), FormatInteger },
                                                                                        { typeof(ushort), FormatInteger },
                                                                                        { typeof(int), FormatInteger },
                                                                                        { typeof(uint), FormatInteger },
                                                                                        { typeof(long), FormatInteger },
                                                                                        { typeof(ulong), FormatInteger },
                                                                                        { typeof(float), FormatFloat },
                                                                                        { typeof(double), FormatFloat },
                                                                                        { typeof(decimal), FormatFloat },
                                                                                        { typeof(bool), FormatBoolean },
                                                                                        { typeof(TimeSpan), FormatTimespan }
                                                                                    };

        public static string EscapeName(string nameOrKey)
        {
            if (nameOrKey == null)
            {
                throw new ArgumentNullException(nameof(nameOrKey));
            }

            return nameOrKey
                //.Replace("=", "\\=")
                .Replace(" ", "_")
                //.Replace(",", "\\,")
                ;
        }

        public static string FormatTimestamp(DateTime time)
        {
            var t = time.Ticks - TimeZone.CurrentTimeZone.ToLocalTime(Origin).Ticks;

            return (t / 10000).ToString();
        }

        public static JValue FormatValue(object value)
        {
            var v = value ?? string.Empty;
            Func<object, JValue> format;

            return Formatters.TryGetValue(v.GetType(), out format)
                ? format(v)
                : FormatString(v.ToString());
        }

        private static JValue FormatBoolean(object b) { return new JValue((bool)b); }

        private static JValue FormatFloat(object f)
        {
            return new JValue(Convert.ToDecimal(f));
        }

        private static JValue FormatInteger(object i)
        {
            return new JValue(Convert.ToInt64(i));
        }

        private static JValue FormatString(string s) { return new JValue("\"" + s.Replace("\"", "\\\"") + "\""); }

        private static JValue FormatTimespan(object ts) { return new JValue(((TimeSpan)ts).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)); }
    }
}
