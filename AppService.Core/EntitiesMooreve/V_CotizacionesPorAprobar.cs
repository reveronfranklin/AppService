using System;

namespace AppService.Core.EntitiesMooreve;

public class V_CotizacionesPorAprobar
{
    public string Cotizacion { get; set; }
    public int Renglon { get; set; }
    public int Propuesta { get; set; }
    public string IdCliente { get; set; }
    public string RazonSocial { get; set; }
    public string Rif { get; set; }
    public string Producto { get; set; }
    public string CodigoProducto { get; set; }
    public string Vendedor { get; set; }
    public DateTime? Fecha { get; set; }
    public string Oficina { get; set; }
    public string NombreOficina { get; set; }
    public string SearchText { get; set; }
    public decimal TotalPropuestaUsd { get; set; }
    public string FechaString { get; set; }
    public string ObsSolicitudPrecio { get; set; }
    public long IdSolicitudPrecio { get; set; }
    
    public decimal TasaExcepcion { get; set; }
    public DateTime? FechaPago { get; set; }
    public string FechaPagoString { get; set; }
    public bool ImprimirFacturaEnUSD { get; set; }
    public string EstatusPlanta { get; set; }
    public string Estatus { get; set; }
    public string Recibo { get; set; }
    public int? AppSubCategoryId { get; set; }
    public string Fiscal { get; set; }
    public bool TieneRifAdjunto { get; set; }
    public long Orden { get; set; }
    
    
    public decimal SolicitudDeCredito { get; set; }
    public bool SolicitudCerrada { get; set; }
    public string ObservacionesCreditoExcepcion { get; set; }
    public string TipoProducto { get; set; }
    public decimal CotizacionCorta { get; set; }
  

}



public class V_CotizacionesPorAprobarDto
{
    public string Cotizacion { get; set; }
    public int Renglon { get; set; }
    public int Propuesta { get; set; }
    public string IdCliente { get; set; }
    public string RazonSocial { get; set; }
    public string Rif { get; set; }
    public string Producto { get; set; }
    public string CodigoProducto { get; set; }
    public string Vendedor { get; set; }
    public DateTime? Fecha { get; set; }
    public string Oficina { get; set; }
    public string NombreOficina { get; set; }
    public string SearchText { get; set; }
    public decimal TotalPropuestaUsd { get; set; }
    public string TotalPropuestaUsdString { get; set; }
    public string FechaString { get; set; }
    public string ObsSolicitudPrecio { get; set; }
    public long IdSolicitudPrecio { get; set; }
    
    public decimal TasaExcepcion { get; set; }
    public DateTime? FechaPago { get; set; }
    public string FechaPagoString { get; set; }
    public bool ImprimirFacturaEnUSD { get; set; }
    public string EstatusPlanta { get; set; }
    public string Estatus { get; set; }
    public string Recibo { get; set; }
    public int? AppSubCategoryId { get; set; }
    public string Fiscal { get; set; }
    public bool TieneRifAdjunto { get; set; }
    public long Orden { get; set; }
    
    
    public decimal SolicitudDeCredito { get; set; }
    public string ObservacionesCreditoExcepcion { get; set; }
    public bool Aprobado { get; set; }
    public bool Rechazado { get; set; }
    public bool SolicitudCerrada { get; set; }
    public DateTime? FechaCompromiso { get; set; }
    public int CotizacionCorta { get; set; }
    
 
    
    public string DercripcionStatusExcepcion { get { return getDescripcion(ObservacionesCreditoExcepcion,Aprobado,Rechazado,SolicitudCerrada); } }


    private string getDescripcion(string observacionesCreditoExcepcion,bool aprobado, bool rechazado,bool solicitudCerrada)
    {
        string result = string.Empty;
        if (observacionesCreditoExcepcion.Length > 0)
        {
            if (!aprobado && solicitudCerrada)
            {
                result = "PENDIENTE";
            }
            if (aprobado && solicitudCerrada)
            {
                result = "APROBADO";
            }
            if (aprobado && !solicitudCerrada)
            {
                result = "PENDIENTE";
            }
            if (rechazado)
            {
                result = "RECHAZADO";
            }
           
        }
      
        return result;

    }

    

}

