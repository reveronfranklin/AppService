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

        public EmailClient(HttpClient httpClient)
        {

            
            httpClient.BaseAddress = new Uri("http://localhost:3002/api/email/send-email");
          
            
            _client = httpClient;

        }



        public async Task<Metadata> Post(StringContent data)
        {
            Metadata metadata = new Metadata();

            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Accept", "application/json");


                var result = await _client.PostAsync(_client.BaseAddress, data);
                string resultContent = await result.Content.ReadAsStringAsync();

                metadata.IsValid = result.IsSuccessStatusCode;
                metadata.Message = resultContent;
                return metadata;
            }
            catch (Exception ex)
            {

                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
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
