using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs.Aprobaciones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;

namespace AppService.Core.Services;

public class VCotizacionesPorAprobarService:IVCotizacionesPorAprobarService
{
    private readonly IUnitOfWork _unitOfWork;
    public VCotizacionesPorAprobarService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public string MapProducto(List<V_CotizacionesPorAprobar> entities,string cotizacion)
    {
        string result = "";
        
        NumberFormatInfo formato = new CultureInfo("es-AR").NumberFormat;

        formato.CurrencyGroupSeparator = ".";
        formato.NumberDecimalSeparator = ",";
        formato.NumberDecimalDigits = 2;
        
        foreach (var item in entities.Where(x=>x.Cotizacion==cotizacion).ToList())
        {
            result = $"{result} Producto: {item.CodigoProducto} USD:{item.TotalPropuestaUsd.ToString("N", formato)} ";
        }


        return result;
    }
    
    public string MapProductoAll(List<CotizacionesViewData> entities,string cotizacion)
    {
        string result = "";
        
        NumberFormatInfo formato = new CultureInfo("es-AR").NumberFormat;

        formato.CurrencyGroupSeparator = ".";
        formato.NumberDecimalSeparator = ",";
        formato.NumberDecimalDigits = 2;
        
        foreach (var item in entities.Where(x=>x.Cotizacion==cotizacion).ToList())
        {
            result = $"{result} Producto: {item.CodigoProducto} USD:{item.TotalPropuestaUsd}";
        }


        return result;
    }
    public async Task<List<V_CotizacionesPorAprobarDto>> Map(List<V_CotizacionesPorAprobar> entities)
    {
     
        NumberFormatInfo formato = new CultureInfo("es-AR").NumberFormat;

        formato.CurrencyGroupSeparator = ".";
        formato.NumberDecimalSeparator = ",";
        formato.NumberDecimalDigits = 2;
      
       
        
        List<V_CotizacionesPorAprobarDto> result = new List<V_CotizacionesPorAprobarDto>();
        foreach (var item in entities)
        {
            V_CotizacionesPorAprobarDto resultItem = new V_CotizacionesPorAprobarDto();

            resultItem.Cotizacion = item.Cotizacion;
            resultItem.Renglon = item.Renglon;
            resultItem.Propuesta = item.Propuesta;
            resultItem.IdCliente = item.IdCliente;
            resultItem.RazonSocial = item.RazonSocial;
            resultItem.Rif = item.Rif;
            resultItem.Producto = item.Producto;
            if (item.TipoProducto == "S")
            {
                resultItem.Producto =  MapProducto(entities,item.Cotizacion);
            }
            resultItem.CodigoProducto = item.CodigoProducto;
            resultItem.Vendedor = item.Vendedor;
            resultItem.Fecha = item.Fecha;
            resultItem.Oficina = item.Oficina;
            resultItem.NombreOficina = item.NombreOficina;
            resultItem.SearchText = item.SearchText;
            resultItem.TotalPropuestaUsd = item.TotalPropuestaUsd;
            resultItem.TotalPropuestaUsdString =item.TotalPropuestaUsd.ToString("N", formato);
            resultItem.FechaString = item.FechaString;
            resultItem.ObsSolicitudPrecio = item.ObsSolicitudPrecio;
            resultItem.IdSolicitudPrecio = item.IdSolicitudPrecio;
            resultItem.TasaExcepcion = item.TasaExcepcion;
            resultItem.FechaPago = item.FechaPago;
            resultItem.FechaPagoString = item.FechaPagoString;
            resultItem.ImprimirFacturaEnUSD = item.ImprimirFacturaEnUSD;
            resultItem.EstatusPlanta = item.EstatusPlanta;
            resultItem.Recibo = item.Recibo;
            resultItem.AppSubCategoryId = item.AppSubCategoryId;
            resultItem.Fiscal = item.Fiscal;
            resultItem.TieneRifAdjunto = item.TieneRifAdjunto;
            resultItem.Orden = item.Orden;
            resultItem.CotizacionCorta =  (int)item.CotizacionCorta;
            resultItem.SolicitudDeCredito = item.SolicitudDeCredito;
            resultItem.ObservacionesCreditoExcepcion = item.ObservacionesCreditoExcepcion;

            resultItem.Aprobado = false;
            resultItem.Rechazado = false;
            resultItem.SolicitudCerrada = false;
                
            resultItem.FechaCompromiso = null;
            
            var excepcion = await _unitOfWork.CreSolicitudExcepcionRepository.GetById(item.SolicitudDeCredito);
            if (excepcion != null)
            {
                resultItem.Aprobado = (bool)excepcion.Aprobado;
                resultItem.Rechazado = (bool)excepcion.Rechazado;
                resultItem.SolicitudCerrada = (bool)excepcion.SolicitudCerrada;
                
                resultItem.FechaCompromiso = excepcion.FechaCompromiso;
            }

            var existe = result.Where(x =>x.Cotizacion==item.Cotizacion && x.CodigoProducto == item.CodigoProducto)
                .Any();
            if (!existe)
            {
                result.Add(resultItem);
            }
           
        }
        return result;
       
       
       
    }
       public async Task<List<V_CotizacionesPorAprobarDto>> MapAll(List<CotizacionesViewData> entities)
    {
     
        NumberFormatInfo formato = new CultureInfo("es-AR").NumberFormat;

        formato.CurrencyGroupSeparator = ".";
        formato.NumberDecimalSeparator = ",";
        formato.NumberDecimalDigits = 2;
      
       
        
        List<V_CotizacionesPorAprobarDto> result = new List<V_CotizacionesPorAprobarDto>();
        foreach (var item in entities)
        {
            V_CotizacionesPorAprobarDto resultItem = new V_CotizacionesPorAprobarDto();

            resultItem.Cotizacion = item.Cotizacion;
            resultItem.Renglon = (int)item.Renglon;
            resultItem.Propuesta = (int)item.Propuesta;
            resultItem.IdCliente = item.IdCliente;
            resultItem.RazonSocial = item.RazonSocial;
            resultItem.Rif = item.Rif;
            resultItem.Producto = item.Producto;
            if (item.TipoProducto == "S")
            {
                resultItem.Producto =  MapProductoAll(entities,item.Cotizacion);
            }
            resultItem.CodigoProducto = item.CodigoProducto;
            resultItem.Vendedor = item.Vendedor;
            resultItem.Fecha = item.Fecha;
            resultItem.Oficina = item.Oficina.ToString();
            resultItem.NombreOficina = item.NombreOficina;
            resultItem.SearchText = item.SearchText;
            resultItem.TotalPropuestaUsd = (decimal)item.TotalPropuestaUsd;
            resultItem.TotalPropuestaUsdString =item.TotalPropuestaUsd.ToString();
            resultItem.FechaString = item.FechaString;
            resultItem.ObsSolicitudPrecio = item.ObsSolicitudPrecio;
            resultItem.IdSolicitudPrecio = (long)item.IdSolicitudPrecio;
            resultItem.TasaExcepcion = (decimal)item.TasaExcepcion;
            resultItem.FechaPago = item.FechaPago;
            resultItem.FechaPagoString = item.FechaPagoString;
            resultItem.ImprimirFacturaEnUSD = item.ImprimirFacturaEnUSD;
            resultItem.EstatusPlanta = item.EstatusPlanta;
            resultItem.Recibo = item.Recibo;
            resultItem.AppSubCategoryId = item.AppSubCategoryId;
            resultItem.Fiscal = item.Fiscal;
            resultItem.TieneRifAdjunto = item.TieneRifAdjunto;
            resultItem.Orden = item.Orden;
            resultItem.CotizacionCorta =  (int)item.CotizacionCorta;
            resultItem.SolicitudDeCredito = (decimal)item.SolicitudDeCredito;
            resultItem.ObservacionesCreditoExcepcion = item.ObservacionesCreditoExcepcion;

            resultItem.Aprobado = false;
            resultItem.Rechazado = false;
            resultItem.SolicitudCerrada = false;
                
            resultItem.FechaCompromiso = null;
            
            var excepcion = await _unitOfWork.CreSolicitudExcepcionRepository.GetById((decimal)item.SolicitudDeCredito);
            if (excepcion != null)
            {
                resultItem.Aprobado = (bool)excepcion.Aprobado;
                resultItem.Rechazado = (bool)excepcion.Rechazado;
                resultItem.SolicitudCerrada = (bool)excepcion.SolicitudCerrada;
                
                resultItem.FechaCompromiso = excepcion.FechaCompromiso;
            }

            var existe = result.Where(x =>x.Cotizacion==item.Cotizacion && x.CodigoProducto == item.CodigoProducto)
                .Any();
            if (!existe)
            {
                result.Add(resultItem);
            }
           
        }
        return result;
       
       
       
    }
 
    public async Task<List<V_CotizacionesPorAprobarDto>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter)
    {
       var cotizacionesPorAprobar= await _unitOfWork.VCotizacionesPorAprobarRepository.GetAllBySearchText(filter);
       var result = await Map(cotizacionesPorAprobar);
       return result;
       
       
       
    }
    public async Task<List<V_CotizacionesPorAprobarDto>> GetAllBySearchTextAll(AppGeneralQuotesQueryFilter filter)
    {
        var cotizacionesPorAprobar= await _unitOfWork.VCotizacionesPorAprobarRepository.GetAllBySearchTextAll(filter);
        var result = await MapAll(cotizacionesPorAprobar);
        return result;
       
       
       
    }
    
    public async Task<ApiResponse<string>>  CerrarExcepcion(ActualizarCotizacion dto)
    {
        ApiResponse<string> response = new ApiResponse<string>("");
        Metadata metadata = new Metadata()
        {
            IsValid = true,
            Message = ""
        };
        try
        {
            
            
            var cotizacionesPorAprobar= await _unitOfWork.VCotizacionesPorAprobarRepository.CerrarExcepcion(dto);
            
            response.Data = cotizacionesPorAprobar.Data;
            response.Meta = cotizacionesPorAprobar.Meta;
            return response;
        }
        catch (Exception e)
        {
            response.Data = "";
            metadata.Message = e.Message;
            metadata.success = false;
            metadata.IsValid = false;
            response.Meta = metadata;
            
            return response;
        }
     
        
        
    }
    public async Task<ApiResponse<string>>  PasePlanta(ActualizarCotizacion dto)
    {
        ApiResponse<string> response = new ApiResponse<string>("");
        Metadata metadata = new Metadata()
        {
            IsValid = true,
            Message = ""
        };
        try
        {
            
            
            var cotizacionesPorAprobar= await _unitOfWork.VCotizacionesPorAprobarRepository.EtlPasePlanta(dto);
            
            response.Data = cotizacionesPorAprobar.Data;
            response.Meta = cotizacionesPorAprobar.Meta;
            return response;
        }
        catch (Exception e)
        {
            response.Data = "";
            metadata.Message = e.Message;
            metadata.success = false;
            metadata.IsValid = false;
            response.Meta = metadata;
            
            return response;
        }
     
        
        
    }
    
    public async Task<ApiResponse<bool>> UpdateCotizacion(ActualizarCotizacion dto)
    {
        
        ApiResponse<bool> response = new ApiResponse<bool>(false);
        Metadata metadata = new Metadata()
        {
            IsValid = true,
            Message = ""
        };
        try
        {


            var cliente =  _unitOfWork.MtrClienteRepository.GetById(dto.IdCliente);
            if (cliente == null)
            {
                response.Data = false;
                metadata.Message = "Cliente no existe";
                metadata.success = false;
                metadata.IsValid = false;
                response.Meta = metadata;
                return response;
            }

            var cotizacion = await _unitOfWork.CotizacionRepository.GetByCotizacion(dto.Cotizacion);
            if (cotizacion == null)
            {
                response.Data = false;
                metadata.Message = "Cotizacion no existe";
                metadata.success = false;
                metadata.IsValid = false;
                response.Meta = metadata;
                return response;
            }

            if (dto.IdCliente != "000000" && cotizacion.CodVendedor.Trim().ToUpper() != cliente.Vendedor1.Trim().ToUpper())
            {
                response.Data = false;
                metadata.Message = "Cliente no pertenece a el vendedor de la cotizacion";
                metadata.success = false;
                metadata.IsValid = false;
                response.Meta = metadata;
                return response;
            }
            
            var cotizacionesPorAprobar= await _unitOfWork.VCotizacionesPorAprobarRepository.UpdateCotizacion(dto);
            response.Data = cotizacionesPorAprobar;
            metadata.Message = "";
            metadata.success = true;
            metadata.IsValid = true;
            response.Meta = metadata;
            return response;
        }
        catch (Exception e)
        {
          
            response.Data = false;
            metadata.Message = e.Message;
            metadata.success = false;
            metadata.IsValid = false;
            response.Meta = metadata;
            
            return response;
        }
      
    }
    
    
}