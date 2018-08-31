using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moon.Reporting.AliTSDB.Client
{
    public class DefaultLineProtocolClient : ILineProtocolClient
    {
        private readonly HttpClient _httpClient;
        private readonly AliTSDBOptions _aliTSDBOptions;

        public DefaultLineProtocolClient(AliTSDBOptions aliTSDBOptions, HttpClient httpClient)
        {
            _aliTSDBOptions = aliTSDBOptions ?? throw new ArgumentNullException(nameof(aliTSDBOptions));

            _httpClient = httpClient;
        }

        public async Task<LineProtocolWriteResult> WriteAsync(string payload, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_aliTSDBOptions.Endpoint, content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"Failed to write to AliTSDB - StatusCode: {response.StatusCode} Reason: {await response.Content.ReadAsStringAsync()}";

                    return new LineProtocolWriteResult(false, errorMessage);
                }

                return new LineProtocolWriteResult(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
