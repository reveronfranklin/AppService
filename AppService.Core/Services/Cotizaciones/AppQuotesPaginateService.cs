
using AppService.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Shared;
using AppService.Core.Entities;
using AppService.Core.Interfaces.Cotizaciones;


namespace AppService.Core.Services
{
    public class AppQuotesPaginateService: IAppQuotesPaginateService
    {

        private readonly IUnitOfWork _unitOfWork;
     

        public AppQuotesPaginateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          
        }

        public async Task<DTOs.Shared.ResultDto<List<AppQuotesPaginate>>> GetPaginate(AppQuotesPaginateFilter filter)
        {
            ResultDto<List<AppQuotesPaginate>> resultDto = new ResultDto<List<AppQuotesPaginate>>(null);
            
            if (string.IsNullOrEmpty(filter.UsuarioConsulta) )
            {
                resultDto.Data = null;
                resultDto.IsValid = false;
                resultDto.Message = "Usuario Invalido";
                return resultDto;
            }
            
            return await _unitOfWork.AppQuotesPaginateRepository.GetPaginate(filter);
        }

   


    }
}
