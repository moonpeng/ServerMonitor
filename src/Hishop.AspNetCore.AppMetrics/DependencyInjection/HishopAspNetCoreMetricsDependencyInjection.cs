using App.Metrics;
using App.Metrics.Formatters.InfluxDB;
using Hishop.AspNetCore.AppMetrics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 监控
    /// </summary>
    public static class HishopAspNetCoreMetricsDependencyInjection
    {
        /// <summary>
        /// 注册监控
        /// </summary>
        /// <param name="services"></param>
        /// <param name="metricsOption"></param>
        /// <returns></returns>
        public static IServiceCollection AddHishopMetrics(this IServiceCollection services, HishopMetricsOptions metricsOption)
        {
            var builder = AppMetrics.CreateDefaultBuilder();

            //基本配置
            builder.Configuration.Configure(options =>
            {
                // APP 标签名称
                if (metricsOption.AppTag != "") { options.AddAppTag(metricsOption.AppTag); }
                // 运行环境标签名称
                if (metricsOption.EnvTag != "") { options.AddEnvTag(metricsOption.EnvTag); }
                // 承载服务器标签名称
                if (metricsOption.ServerTag != "") { options.AddServerTag(metricsOption.ServerTag); }
            });

            //注册InfluxDB接收数据
            builder.Report.ToInfluxDb(options =>
            {
                options.InfluxDb.BaseUri = new Uri(metricsOption.BaseUri);
                options.InfluxDb.Database = metricsOption.Database;
                options.InfluxDb.UserName = metricsOption.UserName;
                options.InfluxDb.Password = metricsOption.Password;
                options.InfluxDb.CreateDataBaseIfNotExists = metricsOption.CreateDataBaseIfNotExists;

                //刷新时间
                options.FlushInterval = metricsOption.FlushInterval;

                options.HttpPolicy.BackoffPeriod = metricsOption.BackoffPeriod;
                options.HttpPolicy.FailuresBeforeBackoff = metricsOption.FailuresBeforeBackoff;
                options.HttpPolicy.Timeout = metricsOption.Timeout;
                options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
            });

            services.AddMetrics(builder.Build());
            services.AddMetricsTrackingMiddleware();

            services.AddMetricsReportScheduler();

            return services;
        }
    }
}
