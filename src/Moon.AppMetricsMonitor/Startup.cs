using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Filtering;
using App.Metrics.Formatters.InfluxDB;
using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moon.Formatters.AliTSDB;

namespace Moon.AppMetricsMonitor
{
    public class Startup
    {
        public Startup(IConfiguration Configuration)
        {
            configuration = Configuration;
        }

        IConfiguration configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //var metricsFilter = new MetricsFilter().WhereType(MetricType.Meter, MetricType.Gauge, MetricType.Histogram);

            var metrics = AppMetrics.CreateDefaultBuilder()
                //配置服务信息
                .Configuration.Configure(x =>
                {
                    //x.AddAppTag("Moon APP");
                    //x.AddEnvTag("Developer");
                    //x.AddServerTag("LocationServer");
                })
                //输出到控制台
                //.Report.ToConsole(x =>
                //{
                //    x.FlushInterval = TimeSpan.FromSeconds(30);
                //    //x.Filter = metricsFilter;

                //    x.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                //})
               .Report.ToAliTSDB(x =>
               {
                   x.AliTSDB.AliTSDB_IP = "ts-uf6ey6yyv05v19473.hitsdb.tsdb.aliyuncs.com";
                   x.AliTSDB.AliTSDB_Port = 8242;

                   x.FlushInterval= TimeSpan.FromSeconds(50);

                   x.MetricsOutputFormatter = new MetricsAliTSDBLineProtocolOutputFormatter();
               })
                //输出到 InfluxDB 
                //.Report.ToInfluxDb(x =>
                //{
                //    x.InfluxDb.BaseUri = new Uri("http://47.99.65.174:8086/");
                //    x.InfluxDb.Database = "AppMetricsData";
                //    x.InfluxDb.UserName = "user";
                //    x.InfluxDb.CreateDataBaseIfNotExists = true;
                //    x.InfluxDb.Password = "123456";

                //    //刷新时间
                //    x.FlushInterval = TimeSpan.FromSeconds(20);

                //    //x.Filter = new MetricsFilter().WhereType(MetricType.Timer);

                //    //x.InfluxDb.RetensionPolicy = "rp";
                //    x.InfluxDb.CreateDataBaseIfNotExists = true;
                //    x.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                //    x.HttpPolicy.FailuresBeforeBackoff = 5;
                //    x.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                //    //x.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                //})
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware();

            services.AddMetricsReportScheduler();

            services.AddMvc();//.AddMetrics();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricsAllMiddleware();
            //app.UseMetricsApdexTrackingMiddleware();
            //app.UseMetricsErrorTrackingMiddleware();
            //app.UseMetricsRequestTrackingMiddleware();
            //app.UseMetricsActiveRequestMiddleware();

            app.UseMvcWithDefaultRoute();
        }
    }
}
