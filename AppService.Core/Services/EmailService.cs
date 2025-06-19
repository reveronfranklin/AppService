using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.EntitiesMaestros;
using Newtonsoft.Json;

namespace AppService.Core.Services
{
    public class EmaiService: IEmaiService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOdooClient _emailClient;

        public EmaiService(IUnitOfWork unitOfWork,IOdooClient emailClient)
        {
            _unitOfWork = unitOfWork;
            _emailClient = emailClient;
        }

        public async Task<ApiResponse<List<Email>>> GetAll()
        {
            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };
            List<Email> resultDto = new List<Email>();
            ApiResponse<List<Email>> response = new ApiResponse<List<Email>>(resultDto);

            resultDto = await _unitOfWork.EmailRepository.GetAll();
         
           
            response.Meta = metadata;
            response.Data = resultDto;
            return response;

           
        }
        
        
        
        public async Task Send()
        {

            await _unitOfWork.EmailRepository.LimpiaEmailInvalidos();
            var emails = await _unitOfWork.EmailRepository.GetAllByIdFile("hvenezue.id");

            if (emails.Any())
            {

                var top = emails.Take(10).ToList();
                foreach (var item in top)
                {
                    
                    SendEmailDto emailDto = new SendEmailDto();
                    emailDto.from = "helpdesk.venezuela@moore.com.ve";
                    emailDto.subjectEmail = item.Subject;
                    emailDto.sendTo = item.Email1;
                    if (!string.IsNullOrEmpty(item.Copia))
                    {
                        emailDto.sendTo = $"{item.Email1},{item.Copia}";
                    }
                    emailDto.bodyText = item.Texto;
                    if (string.IsNullOrEmpty(item.Texto))
                    {
                        emailDto.bodyText = item.Subject;
                    }
                
                    emailDto.template="welcome";
                    
                   /* ParamsTemplate paramsTemplate = new ParamsTemplate();
                    paramsTemplate.name = item.Email1;
                    emailDto.Params=paramsTemplate;*/
                    
                    var json1 = JsonConvert.SerializeObject(emailDto);
                    StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");

                    var result = await _emailClient.PostEmail(data);

                    if (result.IsValid)
                    {
                        //TODO - Borrar el registro de la tabla Elail y pararla a el historico
                       await  _unitOfWork.EmailRepository.Delete(item.Id);
                    }
                    else
                    {
                        var resultError = result.Message;
                    }
                    
                }
            }
         

           
        }
   



    }
}
