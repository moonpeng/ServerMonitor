using App.Metrics;
using App.Metrics.Builder;
using Moon.Reporting.AliTSDB;
using Moon.Reporting.AliTSDB.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace App.Metrics
{
    public static class MetricsAliTSDBReporterBuilder
    {
        public static IMetricsBuilder ToAliTSDB(
       this IMetricsReportingBuilder metricReporterProviderBuilder,
       MetricsReportingAliTSDBOptions options)
        {
            if (metricReporterProviderBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));
            }

            var httpClient = CreateClient(options.AliTSDB, options.HttpPolicy);
            var reporter = new AliTSDBMetricsReporter(options, httpClient);

            return metricReporterProviderBuilder.Using(reporter);
        }

        public static IMetricsBuilder ToAliTSDB(
           this IMetricsReportingBuilder metricReporterProviderBuilder,
           Action<MetricsReportingAliTSDBOptions> setupAction)
        {
            if (metricReporterProviderBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));
            }

            var options = new MetricsReportingAliTSDBOptions();

            setupAction?.Invoke(options);

            var httpClient = CreateClient(options.AliTSDB, options.HttpPolicy);
            var reporter = new AliTSDBMetricsReporter(options, httpClient);

            return metricReporterProviderBuilder.Using(reporter);
        }

        internal static ILineProtocolClient CreateClient(
          AliTSDBOptions influxDbOptions,
          HttpPolicy httpPolicy,
          HttpMessageHandler httpMessageHandler = null)
        {
            var httpClient = httpMessageHandler == null ? new HttpClient() : new HttpClient(httpMessageHandler);

            httpClient.BaseAddress = new Uri(influxDbOptions.Endpoint);


            if (string.IsNullOrWhiteSpace(influxDbOptions.UserName) || string.IsNullOrWhiteSpace(influxDbOptions.Password))
            {
                return new DefaultLineProtocolClient(
                    influxDbOptions,
                    httpClient);
            }

            var byteArray = Encoding.ASCII.GetBytes($"{influxDbOptions.UserName}:{influxDbOptions.Password}");
            //httpClient.BaseAddress = influxDbOptions.BaseUri;
            //httpClient.Timeout = httpPolicy.Timeout;
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return new DefaultLineProtocolClient(
                influxDbOptions,
                httpClient);
        }
    }
}
