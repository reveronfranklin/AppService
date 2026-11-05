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
    public class PagosManualesService: IPagosManualesService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public PagosManualesService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<ResultDto<List<Wsmy685>>> GetPaginate(PagosManualesFilter filter)
        {
            return await _unitOfWork.PcPagosManualesRepository.GetPaginate(filter);
        }

   
    public async Task<ResultDto<Wsmy685>> Update(PagosManualesUpdateDto dto)
        {

            ResultDto<Wsmy685> result = new ResultDto<Wsmy685>(null);
            try
            {

                var pago = await _unitOfWork.PcPagosManualesRepository.GetById(dto.IdPago);
                if (pago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Pago no existe";
                    return result;
                }
                
                List<string> tipos = new List<string> { "PN", "PM" };
                var findTipo = tipos.Where(x => x == dto.Transaccion).First();
                if (findTipo == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Transaccion Invalida";
                    return result;
                }

                if (dto.Monto == 0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Monto Invalido";
                    return result;
                }
                if (dto.Observaciones.IsNullOrEmpty())
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Observaciones Invalida";
                    return result;
                }
                if (dto.Observaciones.Length>500)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Observaciones Invalida(Longitud Max 500)";
                    return result;
                }
                
                var ventas = await _unitOfWork.AppVentasRepository.GetByOrdenProducto(dto.Orden,dto.Producto);
                if (ventas==null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden - Producto No existe";
                    return result;
                }

                if (pago.FlagPagado == true)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "No puede Modificar una Transaccion Pagada";
                    return result;
                }

                pago.Orden = dto.Orden;
                pago.Producto = dto.Producto;
                pago.Monto= dto.Monto;
                pago.MontoGte= dto.MontoGte;
                pago.MontoGteProducto= dto.MontoGteProducto;
                pago.Observaciones = dto.Observaciones;
                pago.Transaccion = dto.Transaccion;
                pago.OrdenString = dto.Orden.ToString();
                pago.SearchText=$"{dto.Orden.ToString()} {pago.Producto}";
                _unitOfWork.PcPagosManualesRepository.Update(pago);
                await _unitOfWork.SaveChangesAsync();
        
                
                var resultDto = pago;
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

        public async Task<ResultDto<Wsmy685>> Create(PagosManualesUpdateDto dto)
        {

            ResultDto<Wsmy685> result = new ResultDto<Wsmy685>(null);
            try
            {
                var pago = await _unitOfWork.PcPagosManualesRepository.GetById(dto.IdPago);
                if (pago != null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Pago ya  existe";
                    return result;
                }
                if (dto.Monto == 0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Monto Invalido";
                    return result;
                }

                if (dto.Observaciones.IsNullOrEmpty())
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Observaciones Invalida";
                    return result;
                }
                if (dto.Observaciones.Length>500)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Observaciones Invalida(Longitud Max 500)";
                    return result;
                }


                
                var ventas = await _unitOfWork.AppVentasRepository.GetByOrdenProducto(dto.Orden,dto.Producto);
                if (ventas==null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Orden - Producto No existe";
                    return result;
                }
                
                List<string> tipos = new List<string> { "PN", "PM" };
                var findTipo = tipos.Where(x => x == dto.Transaccion).First();
                if (findTipo == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Transaccion Invalida";
                    return result;
                }
                
                Wsmy685 entity = new Wsmy685();
                entity.Orden = dto.Orden;
                entity.Producto= dto.Producto;
                entity.Monto = dto.Monto;
                entity.MontoGte = dto.MontoGte;
                entity.MontoGteProducto = dto.MontoGteProducto;
                entity.Transaccion = dto.Transaccion;
                entity.Observaciones = dto.Observaciones;
                entity.FlagPagado = false;
                entity.FichaGteProducto = 0;
                entity.Rmonto = 0;
                entity.FechaActualiza=DateTime.Now;
                entity.UsuarioActualiza = dto.Usuario;
                entity.FechaPagado = null;
                entity.OrdenString = dto.Orden.ToString();
                entity.SearchText=$"{dto.Orden.ToString()} {entity.Producto}";
                
                await _unitOfWork.PcPagosManualesRepository.Add(entity);
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
 
        public async Task<ResultDto<PagosManualesDeleteDto>> Delete(PagosManualesDeleteDto dto)
        {

            ResultDto<PagosManualesDeleteDto> result = new ResultDto<PagosManualesDeleteDto>(null);
            try
            {

                var pago = await _unitOfWork.PcPagosManualesRepository.GetById(dto.IdPago);
                if (pago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Pago no existe";
                    return result;
                }
                if (pago.FlagPagado == true)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "No puede Eliminar una Transaccion Pagada";
                    return result;
                }

                await _unitOfWork.PcPagosManualesRepository.Delete(dto.IdPago);
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
