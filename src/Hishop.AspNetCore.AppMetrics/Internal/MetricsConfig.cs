namespace Moon.AspNetCore.AppMetrics.Internal
{
    /// <summary>
    /// 内部配置信息
    /// </summary>
    internal static class MetricsConfig
    {
        /// <summary>
        /// 活动的请求
        /// </summary>
        public static bool MonitorActiveRequest { get; set; } = false;

        /// <summary>
        /// 错误的轨迹
        /// </summary>
        public static bool MonitorErrorTracking { get; set; } = false;

        /// <summary>
        /// POST、PUT Size轨迹
        /// </summary>
        public static bool MonitorPostAndPutSizeTracking { get; set; } = false;

        /// <summary>
        /// 请求信息轨迹
        /// </summary>
        public static bool MonitorRequestTracking { get; set; } = false;

        /// <summary>
        /// OAuth2 轨迹
        /// </summary>
        public static bool MonitorOAuth2Tracking { get; set; } = false;

        /// <summary>
        /// 用户满意度轨迹
        /// </summary>
        public static bool MonitorApdexTracking { get; set; } = false;
    }
}
