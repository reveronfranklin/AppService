using System;
using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using Microsoft.IdentityModel.Tokens;

namespace AppService.Core.Services
{
    public class PcProcesosService: IPcProcesosService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public PcProcesosService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<DTOs.Shared.ResultDto<List<PcProceso>>> GetAll()
        {
            DTOs.Shared.ResultDto<List<PcProceso>> result = new DTOs.Shared.ResultDto<List<PcProceso>>(null);
            var procesos =await _unitOfWork.PcProcesoRepository.GetAll();
            result.Data = procesos;
            result.Message = "";
            result.IsValid = true;
            return result;
        }

   
        public async Task<DTOs.Shared.ResultDto<PcProceso>> EjecutarProceso(EjecutarProcesoDto dto)
        {

            DTOs.Shared.ResultDto<PcProceso> result = new DTOs.Shared.ResultDto<PcProceso>(null);
            try
            {

                var proceso = await _unitOfWork.PcProcesoRepository.GetById(dto.Id);
                if (proceso == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "proceso no existe";
                    return result;
                }

                proceso.Iniciado = false;
                proceso.Accion = dto.Accion;

           
                _unitOfWork.PcProcesoRepository.Update(proceso);
                await _unitOfWork.SaveChangesAsync();
        
                
                var resultDto = proceso;
                result.Data = resultDto;
                result.IsValid = true;
                result.Message = "";

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.IsValid = false;
                result.Message = ex.Message;
            }



            return result;
        }

      


    }
}
