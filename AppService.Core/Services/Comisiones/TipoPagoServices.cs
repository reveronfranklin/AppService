
using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Shared;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using PcTipoPagoResponseDto = AppService.Core.DTOs.Comisiones.PcTipoPagoResponseDto;


namespace AppService.Core.Services
{
    public class TipoPagoService: ITipoPagoService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public TipoPagoService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<ResultDto<List<PCTipoPago>>> GetAll( )
        {
            
            ResultDto<List<PCTipoPago>> result = new ResultDto<List<PCTipoPago>>(null);
            
            result.Data= await _unitOfWork.PcTipoPagoRepository.GetAll();

            result.Message = "";
            result.IsValid = true;
            return result;
        }

        public async Task<ResultDto<List<PcTipoPagoResponseDto>>> GetAllManuales( )
        {
            
            ResultDto<List<PcTipoPagoResponseDto>> result = new ResultDto<List<PcTipoPagoResponseDto>>(null);
            List<PcTipoPagoResponseDto> data = new List<PcTipoPagoResponseDto>();
            var  tipos = await _unitOfWork.PcTipoPagoRepository.GetAllManuales();
            if (tipos.Any())
            {
                foreach (var item in tipos)
                {
                    PcTipoPagoResponseDto resultItem = new PcTipoPagoResponseDto();
                    resultItem.Id = item.TipoPago;
                    resultItem.Descripcion = item.Descripcion;
                    data.Add(resultItem);
                }
                
                
                
            }

            result.Data = data;
            result.Message = "";
            result.IsValid = true;
            return result;
        }
   
    

    }
}
