using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AppService.Core.Utility;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppService.Core.Services
{
    public class OdooClient : IOdooClient
    {

        private readonly HttpClient _client;
        private readonly HttpClient _httpClientEmail;
        private readonly IUnitOfWork unitOfWork;

        public OdooClient(HttpClient httpClient,HttpClient httpClientEmail)
        {

            /*OdooUrlBaseProd
            OdooUrlBaseDev*/


            var ambiente = Ambiente.ValorAmbiente();

            //Prod
            //httpClient.BaseAddress = new Uri("https://mooreveit-moore-qa1-5818605.dev.odoo.com/jsonrpc/");
         
            if (ambiente == 1)
            {
                httpClient.BaseAddress = new Uri(" https://mooreveit-moore.odoo.com/jsonrpc/");
            }
            else
            {
                httpClient.BaseAddress = new Uri("https://mooreveit-moore-qa-7272646.dev.odoo.com/jsonrpc/");
            }
            httpClientEmail.BaseAddress = new Uri("http://localhost:3002/api/email/send-email");

            //var url="https://mooreveit-moore-qa1-5818605.dev.odoo.com/jsonrpc/"
            //httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            //Prod
            //httpClient.DefaultRequestHeaders.Add("sap-client", "400");

            //DEV
            //httpClient.DefaultRequestHeaders.Add("sap-client", "250");
            _client = httpClient;
            _httpClientEmail = httpClientEmail;
        }

        
        public async Task<Metadata> PostEmail(StringContent data)
        {
            Metadata metadata = new Metadata();

            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Accept", "application/json");


                var result = await _httpClientEmail.PostAsync(_httpClientEmail.BaseAddress, data);
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


        public async Task<Metadata> Post(StringContent data)
        {
            Metadata metadata = new Metadata();

            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
                //_client.DefaultRequestHeaders.Add("x-csrf-token", token);
                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-csrf-token", token);



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
