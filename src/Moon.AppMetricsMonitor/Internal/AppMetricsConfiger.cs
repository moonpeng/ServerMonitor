using App.Metrics;
using App.Metrics.Health;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using App.Metrics.Reporting.InfluxDB;

namespace Moon.AppMetricsMonitor.Internal
{
    public class AppMetricsConfiger
    {
        private static IHealthRoot Health { get; set; }

        private static IMetricsRoot Metrics { get; set; }

        private static IConfiguration Configuration { get; set; }

        private static void Init(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;

            //构建Metrics对象
            Metrics = BuildMetricsRoot();

            Health = BuildHealthRoot(AppMetricsHealth.CreateDefaultBuilder());
        }

        public static void AddService(IServiceCollection services, IConfiguration configuration)
        {
            //初始化
            Init(services, configuration);

            services.AddMetrics(Metrics);
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsReportingHostedService();

            services.AddHealth(Health);
        }

        public static void UseMiddleware(IApplicationBuilder app)
        {
            app.UseMetricsAllMiddleware();
            //app.UseMetricsApdexTrackingMiddleware();
            //app.UseMetricsErrorTrackingMiddleware();
            //app.UseMetricsRequestTrackingMiddleware();
            //app.UseMetricsActiveRequestMiddleware();
        }

        private static IMetricsRoot BuildMetricsRoot()
        {
            var builder = AppMetrics.CreateDefaultBuilder();

            //基本配置
            builder.Configuration.Configure(MetricsConfiger);
            //注册InfluxDB接收数据
            builder.Report.ToInfluxDb(MetricsToInfluxDB);

            return builder.Build();
        }

        private static void MetricsConfiger(MetricsOptions options)
        {
            var configer = Configuration.GetSection("MetricsOptions");

            options.AddAppTag(configer.GetValue<string>("DefaultContextLabel"));
            //options.AddEnvTag("Developer");
            //options.AddServerTag("LocationServer");
        }

        private static void MetricsToInfluxDB(MetricsReportingInfluxDbOptions options)
        {
            options.InfluxDb.BaseUri = new Uri("http://47.99.65.174:8086/");
            options.InfluxDb.Database = "AppMetricsData";
            options.InfluxDb.UserName = "user";
            options.InfluxDb.CreateDataBaseIfNotExists = true;
            options.InfluxDb.Password = "123456";

            //刷新时间
            options.FlushInterval = TimeSpan.FromSeconds(20);

            //options.Filter = new MetricsFilter().WhereType(MetricType.Timer);

            //options.InfluxDb.RetensionPolicy = "rp";
            options.InfluxDb.CreateDataBaseIfNotExists = true;
            options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
            options.HttpPolicy.FailuresBeforeBackoff = 5;
            options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
            //options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
        }

        private static IHealthRoot BuildHealthRoot(IHealthBuilder builder)
        {
            //配置Health配置信息
            builder.Configuration.Configure(HealthConfiger);

            builder.Report.ToMetrics(Metrics);

            builder.HealthChecks.AddProcessPrivateMemorySizeCheck("Private Memory Size", 200);
            builder.HealthChecks.AddProcessVirtualMemorySizeCheck("Virtual Memory Size", 200);
            builder.HealthChecks.AddProcessPhysicalMemoryCheck("Working Set", 200);

            return builder.Build();
        }

        private static void HealthConfiger(HealthOptions options)
        {
            options.Enabled = true;
        }
    }
}
