using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.DTOs.Ventas;
using AppService.Core.Entities;
using AppService.Core.Interfaces.Ventas;

namespace AppService.Core.Services
{
    public class AppVentasService: IAppVentasService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public AppVentasService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<DTOs.Shared.ResultDto<List<AppVentas>>> GetPaginate(AppVentasFilter filter)
        {
            return await _unitOfWork.AppVentasRepository.GetPaginate(filter);
        }

   


    }
}
