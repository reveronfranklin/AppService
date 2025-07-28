using System;
using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.DTOs.Comisiones.PcOrdenesSinCalculoComision;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using Microsoft.IdentityModel.Tokens;

namespace AppService.Core.Services
{
    public class PcOrdenesSinCalculoComisionServices:IPcOrdenesSinCalculoComisionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public PcOrdenesSinCalculoComisionServices(IUnitOfWork unitOfWork, IMapper mapper,
            IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>> GetPaginate(
            PagosManualesFilter filter)
        {
            return await _unitOfWork.PcOrdenesSinCalculoComisionRepository.GetPaginate(filter);
        }


        public async Task<ResultDto<PcOrdenesSinCalculoComision>> Update(PcOrdenesSinCalculoComisionUpdateDto dto)
        {
            ResultDto<PcOrdenesSinCalculoComision> result = new ResultDto<PcOrdenesSinCalculoComision>(null);
            try
            {
                var ordenSinComision = await _unitOfWork.PcOrdenesSinCalculoComisionRepository.GetById(dto.Id);
                if (ordenSinComision == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "No existe";
                    return result;
                }


                var ventas = await _unitOfWork.AppVentasRepository.GetByOrden(dto.Orden);
                if (ventas == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden  No existe";
                    return result;
                }


                ordenSinComision.Orden = dto.Orden;

                _unitOfWork.PcOrdenesSinCalculoComisionRepository.Update(ordenSinComision);
                await _unitOfWork.SaveChangesAsync();


                var resultDto = ordenSinComision;
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

        public async Task<ResultDto<PcOrdenesSinCalculoComision>> Create(PcOrdenesSinCalculoComisionUpdateDto dto)
        {
            ResultDto<PcOrdenesSinCalculoComision> result = new ResultDto<PcOrdenesSinCalculoComision>(null);
            try
            {
                var pago = await _unitOfWork.PcOrdenesSinCalculoComisionRepository.GetByOrden(dto.Orden);
                if (pago != null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden ya  existe";
                    return result;
                }

                var ventas = await _unitOfWork.AppVentasRepository.GetByOrden(dto.Orden);
                if (ventas == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden - Producto No existe";
                    return result;
                }


                PcOrdenesSinCalculoComision entity = new PcOrdenesSinCalculoComision();
                entity.Orden = dto.Orden;


                await _unitOfWork.PcOrdenesSinCalculoComisionRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync();
                result.Data = null;
                result.IsValid = true;
                result.Message = "";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.IsValid = false;
                result.Message = ex.Message;
            }


            return result;
        }

        public async Task<ResultDto<PcOrdenesSinCalculoComisionUpdateDto>> Delete(
            PcOrdenesSinCalculoComisionUpdateDto dto)
        {
            ResultDto<PcOrdenesSinCalculoComisionUpdateDto> result =
                new ResultDto<PcOrdenesSinCalculoComisionUpdateDto>(null);
            try
            {
                var pago = await _unitOfWork.PcOrdenesSinCalculoComisionRepository.GetById(dto.Id);
                if (pago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden no existe";
                    return result;
                }

                await _unitOfWork.PcOrdenesSinCalculoComisionRepository.Delete(dto.Id);
                await _unitOfWork.SaveChangesAsync();
                result.Data = dto;
                result.IsValid = false;
                result.Message = "";
            }
            catch (Exception ex)
            {
                result.Data = dto;
                result.IsValid = false;
                result.Message = ex.Message;
            }


            return result;
        }
    }
}