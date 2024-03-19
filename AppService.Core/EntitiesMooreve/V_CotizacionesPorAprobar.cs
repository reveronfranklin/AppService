using System;

namespace AppService.Core.EntitiesMooreve;

public class V_CotizacionesPorAprobar
{
    public string Cotizacion { get; set; }
    public int Renglon { get; set; }
    public string IdCliente { get; set; }
    public string RazonSocial { get; set; }
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

}