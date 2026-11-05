using Microsoft.Extensions.Configuration;
using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AppService.Core.Utility;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppService.Core.Services
{
    public class EmailClient : IEmailClient
    {

        private readonly HttpClient _client;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration _emailConfiguration;

        public EmailClient(HttpClient httpClient, IConfiguration configuration)
        {

            
            _emailConfiguration = configuration;
            httpClient.BaseAddress = new Uri(configuration["EmailService:Url"] ?? "http://localhost:3002/api/email/send-email");
          
            
            _client = httpClient;

        }



        public async Task<Metadata> Post(StringContent data)
        {
            Metadata metadata = new Metadata();

            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Accept", "application/json");


                using var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress) { Content = data };
                var key = _emailConfiguration["EmailService:ServiceKey"];
                if (!string.IsNullOrWhiteSpace(key)) request.Headers.Add("X-Email-Service-Key", key);
                request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
                var result = await _client.SendAsync(request);
                string resultContent = await result.Content.ReadAsStringAsync();

                metadata.IsValid = result.IsSuccessStatusCode;
                metadata.Message = resultContent;
                return metadata;
            }
            catch (Exception ex)
            {

                metadata.IsValid = false;
                metadata.Message = ex.InnerException?.Message ?? ex.Message;
                return metadata;
            }

            // return await _client.GetStringAsync("/");
        }




        public async Task<string> GetData()
        {

            var result = await _client.GetAsync("/");
            return await _client.GetStringAsync("/");
        }

    }
}
