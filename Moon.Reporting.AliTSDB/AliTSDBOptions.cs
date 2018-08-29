using System;
using System.Collections.Generic;
using System.Text;

namespace Moon.Reporting.AliTSDB
{
    /// <summary>
    /// 提供阿里TSDB时序数据库的全局配置
    /// </summary>
    public class AliTSDBOptions
    {
        public AliTSDBOptions()
        {
            CreateDataBaseIfNotExists = true;

            SYNC_TIMEOUT_MS = 60 * 1000;
        }

        /// <summary>
        ///     Gets formatted endpoint for writes to AliTSDB
        /// </summary>
        /// <value>
        ///     The AliTSDB endpoint for writes.
        /// </value>
        public string Endpoint
        {
            get
            {
                //if (string.IsNullOrWhiteSpace(Database)) { return null; }

                var endpoint = $"http://{AliTSDB_IP}:{AliTSDB_Port}/api/put?sync_timeout={SYNC_TIMEOUT_MS}";

                return endpoint;
            }
        }

        /// <summary>
        /// 实例地址
        /// </summary>
        public string AliTSDB_IP { get; set; }

        /// <summary>
        /// 实例端口
        /// </summary>
        public int AliTSDB_Port { get; set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        public int SYNC_TIMEOUT_MS { get; set; }

        /// <summary>
        ///     Gets or sets the AliTSDB database name used to report metrics.
        /// </summary>
        /// <value>
        ///     The AliTSDB database name where metrics are flushed.
        /// </value>
        public string Database { get; set; }

        /// <summary>
        ///     Gets or sets the AliTSDB database username.
        /// </summary>
        /// <value>
        ///     The AliTSDB database username.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the AliTSDB database password.
        /// </summary>
        /// <value>
        ///     The AliTSDB database password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating(指示) whether or not to attempt(尝试) to create the specified database if it does not exist
        /// </summary>
        public bool CreateDataBaseIfNotExists { get; set; }
    }
}
