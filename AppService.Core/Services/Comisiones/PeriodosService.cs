using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.Interfaces.Comisiones;

namespace AppService.Core.Services
{
    public class PeriodosService: IPeriodosService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public PeriodosService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<ApiResponse<List<PeriodosDto>>> GetAll()
        {

       
            List<PeriodosDto> resultDto = new List<PeriodosDto>();

            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };
            ApiResponse<List<PeriodosDto>> response = new ApiResponse<List<PeriodosDto>>(resultDto);


            var periodos = await _unitOfWork.Wsmy686Repository.GetAll();
            if (periodos!=null)
            {
                List<PeriodosDto> appUnitsDto = _mapper.Map<List<PeriodosDto>>(periodos);

                response.Data = appUnitsDto.OrderByDescending(x=>x.Id).ToList();
                response.Meta = metadata;
                return response;
            }
            else
            {
                response.Data = null;
                response.Meta = metadata;
                return response;
            }

           
        }

   


    }
}
