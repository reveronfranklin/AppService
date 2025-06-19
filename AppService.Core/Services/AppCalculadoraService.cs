
using System;
using AppService.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.Dtos;
using AppService.Core.DTOs.AppCalculadora;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Shared;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Utility;

namespace AppService.Core.Services
{
    public class AppCalculadoraService: IAppCalculadoraService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppProductsService _appProductsService;

        public AppCalculadoraService(IUnitOfWork unitOfWork,IAppProductsService appProductsService)
        {
            _unitOfWork = unitOfWork;
            _appProductsService = appProductsService;
        }

        public async  Task<ResultDto<List<AppCalculadoraResponseDto>>> GetByVendedor(AppCalculadoraFilterDto filter)
        {
            
            ResultDto<List<AppCalculadoraResponseDto>> result = new ResultDto<List<AppCalculadoraResponseDto>>(null);

            
            List<AppCalculadoraResponseDto> resultList = new List<AppCalculadoraResponseDto>();
            var calculos=  await _unitOfWork.AppCalculadoraRepository.GetByVendedor(filter);


            foreach (var calculo in calculos)
            {
                AppCalculadoraResponseDto resultItem = new AppCalculadoraResponseDto();
                resultItem.Id = calculo.Id;
                resultItem.Fecha=$"{calculo.CreatedAt.Day}/{calculo.CreatedAt.Month}/{calculo.CreatedAt.Year}";
                resultItem.IdVendedor = calculo.IdVendedor;
                resultItem.IdMunicipio = calculo.IdMunicipio;
                resultItem.DescripcionMunicipio = calculo.DescripcionMunicipio;
                resultItem.IdCondPago = calculo.IdCondPago;
                resultItem.DescripcionCondPago = calculo.DescripcionCondPago;
                resultItem.IdMtrTipoMonedaPago = calculo.IdMtrTipoMonedaPago;
                resultItem.DescripcionTipoMonedaPago = calculo.DescripcionTipoMonedaPago;
                resultItem.IdSubcategoria = calculo.IdSubcategoria;
                resultItem.DescripcionSubcategoria = calculo.DescripcionSubcategoria;
                resultItem.IdProducto = calculo.IdProducto;
                resultItem.DescripcionProducto = calculo.DescripcionProducto;
                resultItem.Cantidad = calculo.Cantidad;
                resultItem.IdUnidad = calculo.IdUnidad;
                resultItem.DescripcionUnidad = calculo.DescripcionUnidad;
                resultItem.Cantidad_X_Unidad = calculo.Cantidad_X_Unidad;
                resultItem.CantidadConvertida = calculo.CantidadConvertida;
                resultItem.MedidaBasica = calculo.MedidaBasica;
                resultItem.MedidaOpuesta = calculo.MedidaOpuesta;
                resultItem.PrecioMinimo = calculo.PrecioMinimo;
                resultItem.PrecioMaximo = calculo.PrecioMaximo;
                resultItem.Precio = calculo.Precio;
                resultItem.Total = calculo.Total;
                resultItem.CalculoId = calculo.CalculoId;
                resultItem.IdCliente= calculo.IdCliente;
                resultItem.RazonSocial= calculo.RazonSocial;
                resultItem.IdDireccionEntregar=calculo.IdDireccionEntregar;
                resultItem.SearchText = calculo.SearchText;
                resultItem.PorcFlete=calculo.PorcFlete;
                var producto = await _appProductsService.GetByIdProducto(resultItem.IdProducto);
                if (producto != null)
                {
                    resultItem.AppProduct = producto;
                }
                var unidad = producto.Conversiones.Where(x=>x.Id==resultItem.IdUnidad).FirstOrDefault();
                
                resultItem.AppProductConversion=unidad;
                resultList.Add(resultItem);
            }
            
            result.Data = resultList;
            result.IsValid = true;
            result.Message = "";
            result.CantidadRegistros=calculos.Count;
            result.TotalPage = 1;
            result.Page = 1;
                
            return result;
         
        }
      
        public async  Task<ResultDto<bool>> Create(AppCalculadoraUpdateDto dto)
        {
            
            ResultDto<bool> result = new ResultDto<bool>(false);

            try
            {
                /* var condicionPago =  await _unitOfWork.MtrCondicionPagoRepository.GetById((short)dto.IdCondPago);
                if (condicionPago == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Condicion de pago invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }*/

                if (dto.IdCliente != "000000")
                {
                    var cliente =  _unitOfWork.MtrClienteRepository.GetById(dto.IdCliente);
                    if (cliente != null)
                    {
                        dto.RazonSocial = cliente.Nombre.Trim();
                    }
                }
               
                

                //dto.DescripcionCondPago = condicionPago.Descripcion;
                dto.DescripcionCondPago = "";
                /*var moneda = await _unitOfWork.MtrTipoMonedaRepository.GetById((short)dto.IdMtrTipoMonedaPago);
                if (moneda == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Moneda invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }*/

                dto.DescripcionTipoMonedaPago = "";//moneda.Descripcion;
                var subCategoria = await _unitOfWork.AppSubCategoryRepository.GetById(dto.IdSubcategoria);
                if (subCategoria == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Subcategoria invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }

                dto.DescripcionSubcategoria = subCategoria.Description;
                
                var producto = await _unitOfWork.AppProductsRepository.GetById(dto.IdProducto);
                if (producto == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Producto invalido";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                dto.DescripcionProducto = producto.Description1;
                
                var unidad = await _unitOfWork.AppProductConversionRepository.GetById(dto.IdUnidad);
                if (unidad == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Unidad invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }

                dto.DescripcionUnidad = "";//;unidad.AppUnitsIdAlternativaNavigation.Description1;
                
                var municipios = await _unitOfWork.Winy243Repository.GetById(dto.IdMunicipio);
                if (municipios == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Municipio invalido";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }

                dto.DescripcionMunicipio = municipios.DescMunicipio;
               
                dto.PorcFlete=(decimal)municipios.PorcFlete;
                var tasasResult=  await _unitOfWork.AppCalculadoraRepository.Create(dto);
                if (tasasResult == "")
                {
                    result.Data = true;
                    result.IsValid = true;
                    result.Message = "";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                else
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = tasasResult;
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Data = false;
                result.IsValid = false;
                result.Message = e.Message;
                result.CantidadRegistros = 1;
                result.TotalPage = 1;
                result.Page = 1;
                return result;
            }
    
           
           
                
            return result;
         
        }


        
        
        public async  Task<ResultDto<bool>> Update(AppCalculadoraUpdateDto dto)
        {
            
            ResultDto<bool> result = new ResultDto<bool>(false);

            try
            {
                
                   var condicionPago =  await _unitOfWork.MtrCondicionPagoRepository.GetById((short)dto.IdCondPago);
                if (condicionPago == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Condicion de pago invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }

               /* var calculo = await _unitOfWork.AppCalculadoraRepository.GetById(dto.Id);
                if (calculo == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Calculo No Existe";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                dto.CreatedAt=calculo.CreatedAt;
                if (dto.IdCliente != "000000")
                {
                    var cliente =  _unitOfWork.MtrClienteRepository.GetById(dto.IdCliente);
                    if (cliente != null)
                    {
                        dto.RazonSocial = cliente.Nombre.Trim();
                    }
                }*/
               
                

                //dto.DescripcionCondPago = condicionPago.Descripcion;
                dto.DescripcionCondPago = "";
                /*var moneda = await _unitOfWork.MtrTipoMonedaRepository.GetById((short)dto.IdMtrTipoMonedaPago);
                if (moneda == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Moneda invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }*/

                dto.DescripcionTipoMonedaPago = "";//moneda.Descripcion;
                var subCategoria = await _unitOfWork.AppSubCategoryRepository.GetById(dto.IdSubcategoria);
                if (subCategoria == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Subcategoria invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }

                dto.DescripcionSubcategoria = subCategoria.Description;
                
                var producto = await _unitOfWork.AppProductsRepository.GetById(dto.IdProducto);
                if (producto == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Producto invalido";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                dto.DescripcionProducto = producto.Description1;
                
                var unidad = await _unitOfWork.AppProductConversionRepository.GetById(dto.IdUnidad);
                if (unidad == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Unidad invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }

                dto.DescripcionUnidad = "";//;unidad.AppUnitsIdAlternativaNavigation.Description1;
                
                var municipios = await _unitOfWork.Winy243Repository.GetById(dto.IdMunicipio);
                if (municipios == null)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Municipio invalido";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }

                dto.DescripcionMunicipio = municipios.DescMunicipio;
               
                dto.PorcFlete=(decimal)municipios.PorcFlete;
                var calculadorResult=  await _unitOfWork.AppCalculadoraRepository.Update(dto);
                if (calculadorResult == "")
                {
                    result.Data = true;
                    result.IsValid = true;
                    result.Message = "";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
                else
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = calculadorResult;
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
            }
            catch (Exception e)
            {
                result.Data = false;
                result.IsValid = false;
                result.Message = e.Message;
                result.CantidadRegistros = 1;
                result.TotalPage = 1;
                result.Page = 1;
            }
    
           
           
                
            return result;
         
        }


         public async  Task<ResultDto<bool>> Delete(AppCalculadoraDeleteDto dto)
        {
            
            ResultDto<bool> result = new ResultDto<bool>(false);

            try
            {
               
             
            
             
                var tasasResult=  await _unitOfWork.AppCalculadoraRepository.Delete(dto.Id);
                if (tasasResult == "")
                {
                    result.Data = true;
                    result.IsValid = true;
                    result.Message = "";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
                else
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = tasasResult;
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
            }
            catch (Exception e)
            {
                result.Data = false;
                result.IsValid = false;
                result.Message = e.Message;
                result.CantidadRegistros = 1;
                result.TotalPage = 1;
                result.Page = 1;
            }
    
           
           
                
            return result;
         
        }




    }
}
