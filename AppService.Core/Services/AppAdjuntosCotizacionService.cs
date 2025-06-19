using AppService.Core.CustomEntities;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Responses;

namespace AppService.Core.Services
{
    public class AppAdjuntosCotizacionService :IAppAdjuntosCotizacionService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly PaginationOptions _paginationOptions;

        public AppAdjuntosCotizacionService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

   
        public async Task<ApiResponse<List<AppAdjuntosCotizacion>>> GetAdjuntosCotizacion(AppAdjuntosCotizacionFilter filters)
        {
          

            Metadata metadata = new Metadata
            {
                IsValid = false,
                Message = "",
                TotalCount = 0
            };
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;
            List<AppAdjuntosCotizacion> result = new List<AppAdjuntosCotizacion>();
            
            result = await _unitOfWork.AppAdjuntosCotizacionRepository.GetByCotizacion(filters.Cotizacion,filters.Renglon);
            if (result.Count == 0)
            {
                metadata.IsValid = false;
                metadata.TotalCount = 0;
                metadata.Message = "No Data";
            }
            else
            {
                metadata.IsValid = true;
                metadata.TotalCount = result.Count;
            }
            var response = new ApiResponse<List<AppAdjuntosCotizacion>>(result)
            {
                Meta = metadata
            };
          
            
            
            return response;


        }
    


    }
}
