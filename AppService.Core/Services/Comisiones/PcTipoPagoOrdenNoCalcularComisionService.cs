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
using AppService.Core.Entities;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using Microsoft.IdentityModel.Tokens;

namespace AppService.Core.Services
{
    public class PcTipoPagoOrdenNoCalcularComisionService:IPcTipoPagoOrdenNoCalcularComisionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public PcTipoPagoOrdenNoCalcularComisionService(IUnitOfWork unitOfWork, IMapper mapper,
            IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<DTOs.Shared.ResultDto<List<PCTipoPagoOrdenNoCalcularComision>>> GetPaginate(
            PagosManualesFilter filter)
        {
            return await _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.GetPaginate(filter);
        }


        public async Task<ResultDto<PCTipoPagoOrdenNoCalcularComision>> Update(PcTipoPagoOrdenNoCalcularComisionUpdateDto dto)
        {
            ResultDto<PCTipoPagoOrdenNoCalcularComision> result = new ResultDto<PCTipoPagoOrdenNoCalcularComision>(null);
            try
            {
                var ordenSinComision = await _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.GetById(dto.Id);
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

                var tipoPago = await _unitOfWork.PcTipoPagoRepository.GetById(dto.TipoPagoId);
                if (tipoPago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Tipo de Pago  No existe";
                    return result;
                }

                ordenSinComision.Orden = dto.Orden;
                ordenSinComision.Cliente =ventas.Cliente;
                ordenSinComision.DescripcionTipoPago = tipoPago.Descripcion;
                ordenSinComision.UsuarioActualizacion = dto.UsuarioConectado.ToString();
                ordenSinComision.FechaActualizacion = DateTime.Now;
                ordenSinComision.SearchText = dto.Orden.ToString() + ventas.Cliente;
                

                _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.Update(ordenSinComision);
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

        public async Task<ResultDto<PCTipoPagoOrdenNoCalcularComision>> Create(PcTipoPagoOrdenNoCalcularComisionUpdateDto dto)
        {
            ResultDto<PCTipoPagoOrdenNoCalcularComision> result = new ResultDto<PCTipoPagoOrdenNoCalcularComision>(null);
            try
            {
                var pago = await _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.GetByTipoPagoOrden(dto.TipoPagoId,dto.Orden);
                if (pago != null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden y tipo de pago ya  existe";
                    return result;
                }

                var ventas = await _unitOfWork.AppVentasRepository.GetByOrden(dto.Orden);
                if (ventas == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden No existe";
                    return result;
                }
                var tipoPago = await _unitOfWork.PcTipoPagoRepository.GetById(dto.TipoPagoId);
                if (tipoPago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Tipo de Pago  No existe";
                    return result;
                }


                PCTipoPagoOrdenNoCalcularComision entity = new PCTipoPagoOrdenNoCalcularComision();
                entity.Orden = dto.Orden;
                entity.Cliente = ventas.Cliente;    
                entity.DescripcionTipoPago = tipoPago.Descripcion;
                entity.SearchText = dto.Orden.ToString() + ventas.Cliente;
                entity.FechaCreacion = DateTime.Now;
                entity.UsuarioCreacion =dto.UsuarioConectado.ToString();


                await _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.Add(entity);
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

        public async Task<ResultDto<PcTipoPagoOrdenNoCalcularComisionUpdateDto>> Delete(
            PcTipoPagoOrdenNoCalcularComisionUpdateDto dto)
        {
            ResultDto<PcTipoPagoOrdenNoCalcularComisionUpdateDto> result =
                new ResultDto<PcTipoPagoOrdenNoCalcularComisionUpdateDto>(null);
            try
            {
                var pago = await _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.GetById(dto.Id);
                if (pago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden - Tipo Pago - no existe";
                    return result;
                }

                await _unitOfWork.PcTipoPagoOrdenNoCalcularComisionRepository.Delete(dto.Id);
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