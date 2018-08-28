using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Filtering;
using App.Metrics.Formatters.InfluxDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(x =>
                {
                    x.AddAppTag("Moon APP"); x.AddEnvTag("Developer");

                   
                })
                .Report.ToInfluxDb(x =>
                {
                    x.InfluxDb.BaseUri = new Uri("http://47.99.65.174:8086/");
                    x.InfluxDb.Database = "AppMetricsData";
                    x.InfluxDb.UserName = "user";
                    x.InfluxDb.CreateDataBaseIfNotExists = true;
                    x.InfluxDb.Password = "123456";
             
                    //刷新时间
                    x.FlushInterval = TimeSpan.FromSeconds(20);

                    x.Filter = new MetricsFilter().WhereType(MetricType.Timer);

                    x.InfluxDb.RetensionPolicy = "rp";
                    x.InfluxDb.CreateDataBaseIfNotExists = true;
                    x.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                    x.HttpPolicy.FailuresBeforeBackoff = 5;
                    x.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                    x.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                })
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware(configuration);

            services.AddMetricsReportScheduler();

            services.AddMvc().AddMetrics();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricsApdexTrackingMiddleware();
            app.UseMetricsErrorTrackingMiddleware();
            app.UseMetricsRequestTrackingMiddleware();
            app.UseMetricsActiveRequestMiddleware();

            app.UseMvcWithDefaultRoute();
        }
    }
}
