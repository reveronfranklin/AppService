using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AppService.Api
{
    internal static class QuotationLocalEnvironment
    {
        internal static void Validate(IHostEnvironment environment, IConfiguration configuration)
        {
            if (!configuration.GetValue<bool>("QuotationLocal:Enabled")) return;
            if (!environment.IsDevelopment() || configuration.GetValue<bool>("LocalReplica:Enabled"))
                throw new InvalidOperationException("QuotationLocal requiere un host funcional Development separado del importador.");

            foreach (var key in new[] { "rrdConecction", "mooreveConecction", "clientesConecction", "maestrosConecction", "plantaConecction", "contratosStockConecction" })
                if (string.IsNullOrWhiteSpace(configuration.GetConnectionString(key)))
                    throw new InvalidOperationException($"QuotationLocal requiere ConnectionStrings:{key} local.");

            foreach (var connection in configuration.GetSection("ConnectionStrings").GetChildren())
            {
                if (string.IsNullOrWhiteSpace(connection.Value)) continue;
                var host = new SqlConnectionStringBuilder(connection.Value).DataSource.Split(',')[0];
                if (!host.Equals("localhost", StringComparison.OrdinalIgnoreCase) && host != "127.0.0.1" && host != "[::1]" && host != "::1")
                    throw new InvalidOperationException($"QuotationLocal rechaza la conexión no local: {connection.Key}.");
            }
        }
    }

    internal sealed class QuotationLocalHttpHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri;
            if (uri == null || !uri.IsAbsoluteUri || (uri.Scheme != "http" && uri.Scheme != "https") ||
                (uri.Host != "localhost" && uri.Host != "127.0.0.1" && uri.Host != "[::1]" && uri.Host != "::1"))
                throw new InvalidOperationException("QuotationLocal bloquea envíos HTTP a integraciones externas.");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
