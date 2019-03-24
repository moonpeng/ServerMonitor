using System;

namespace Moon.AspNetCore.AppMetrics
{
    /// <summary>
    /// 监控配置信息
    /// </summary>
    public class MetricsOptions
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MetricsOptions()
        {
            CreateDataBaseIfNotExists = true;

            FlushInterval = TimeSpan.FromSeconds(20);

            Timeout = TimeSpan.FromSeconds(10);
            BackoffPeriod = TimeSpan.FromSeconds(30);
            FailuresBeforeBackoff = 5;
        }

        /// <summary>
        /// APP名称
        /// </summary>
        public string AppTag { get; set; }

        /// <summary>
        /// 当前环境
        /// </summary>
        public string EnvTag { get; set; }

        /// <summary>
        /// 当前服务器
        /// </summary>
        public string ServerTag { get; set; }

        /// <summary>
        /// 时序数据库地址 
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// 时序数据库名称
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 时序数据库用户名 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 时序数据库密码 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否自动创建数据库
        /// </summary>
        public bool CreateDataBaseIfNotExists { get; set; }

        /// <summary>
        /// 刷新时间
        /// </summary>
        public TimeSpan FlushInterval { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan BackoffPeriod { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FailuresBeforeBackoff { get; set; }
    }
}
