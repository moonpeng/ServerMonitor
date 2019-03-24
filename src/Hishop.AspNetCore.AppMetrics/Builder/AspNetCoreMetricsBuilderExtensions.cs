using Moon.AspNetCore.AppMetrics.Internal;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 监控扩展程序
    /// </summary>
    public static class AspNetCoreMetricsBuilderExtensions
    {
        /// <summary>
        /// 注册监控中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMoonMetricsMiddleware(this IApplicationBuilder app)
        {
            // 活跃请求  
            if (MetricsConfig.MonitorActiveRequest) { app.UseMetricsActiveRequestMiddleware(); }

            // 错误轨迹  
            if (MetricsConfig.MonitorErrorTracking) { app.UseMetricsErrorTrackingMiddleware(); }

            // Post、Put Size轨迹  
            if (MetricsConfig.MonitorPostAndPutSizeTracking) { app.UseMetricsPostAndPutSizeTrackingMiddleware(); }

            // 请求轨迹  
            if (MetricsConfig.MonitorRequestTracking) { app.UseMetricsRequestTrackingMiddleware(); }

            // OAuth2轨迹  
            if (MetricsConfig.MonitorOAuth2Tracking) { app.UseMetricsOAuth2TrackingMiddleware(); }

            // 用户满意度轨迹 
            if (MetricsConfig.MonitorApdexTracking) { app.UseMetricsApdexTrackingMiddleware(); }

            return app;
        }
    }
}
