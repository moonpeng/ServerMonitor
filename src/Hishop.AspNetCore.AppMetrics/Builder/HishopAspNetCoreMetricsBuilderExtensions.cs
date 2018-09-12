using Hishop.AspNetCore.AppMetrics.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 监控扩展程序
    /// </summary>
    public static class HishopAspNetCoreMetricsBuilderExtensions
    {
        /// <summary>
        /// 注册监控中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHishopMetricsMiddleware(this IApplicationBuilder app)
        {
            // 活跃请求  
            if (HishopMetricsConfig.MonitorActiveRequest) { app.UseMetricsActiveRequestMiddleware(); }

            // 错误轨迹  
            if (HishopMetricsConfig.MonitorErrorTracking) { app.UseMetricsErrorTrackingMiddleware(); }

            // Post、Put Size轨迹  
            if (HishopMetricsConfig.MonitorPostAndPutSizeTracking) { app.UseMetricsPostAndPutSizeTrackingMiddleware(); }

            // 请求轨迹  
            if (HishopMetricsConfig.MonitorRequestTracking) { app.UseMetricsRequestTrackingMiddleware(); }

            // OAuth2轨迹  
            if (HishopMetricsConfig.MonitorOAuth2Tracking) { app.UseMetricsOAuth2TrackingMiddleware(); }

            // 用户满意度轨迹 
            if (HishopMetricsConfig.MonitorApdexTracking) { app.UseMetricsApdexTrackingMiddleware(); }

            return app;
        }
    }
}
