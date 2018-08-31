using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moon.Reporting.AliTSDB.Client
{
    public interface ILineProtocolClient
    {
        Task<LineProtocolWriteResult> WriteAsync(
            string payload,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
