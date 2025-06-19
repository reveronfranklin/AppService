using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppService.Core.EntitiesMooreve;

public class CotizacionesViewData
{
    
      public int Id { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(20)]
    public string Cotizacion { get; set; }

    [Column(TypeName = "int")]
    public int? Renglon { get; set; }

    [Column(TypeName = "int")]
    public int? Propuesta { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(10)] // Changed to 10 to match the table definition
    public string IdCliente { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(200)]
    public string RazonSocial { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(255)]
    public string Producto { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(12)] // Changed to 12 to match the table definition
    public string CodigoProducto { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(200)]
    public string Vendedor { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(20)]
    public string Rif { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(20)]
    public string Fiscal { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(50)]
    public string Estatus { get; set; }

    [Column(TypeName = "bit")]
    public bool TieneRifAdjunto { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [Column(TypeName = "int")]
    public int? Oficina { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(200)]
    public string NombreOficina { get; set; }

    [Column(TypeName = "nvarchar")]
    public string SearchText { get; set; }

    [Column(TypeName = "decimal")]
    public decimal? TotalPropuestaUsd { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(20)]
    public string FechaString { get; set; }

    [Column(TypeName = "nvarchar")]
    public string ObsSolicitudPrecio { get; set; }

    [Column(TypeName = "numeric")]

    public decimal? IdSolicitudPrecio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaPago { get; set; }

    [Column(TypeName = "decimal")]
    public decimal? TasaExcepcion { get; set; }

    [Column(TypeName = "bit")]
    public bool ImprimirFacturaEnUSD { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(20)]
    public string FechaPagoString { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(50)]
    public string EstatusPlanta { get; set; }

    [Column(TypeName = "nvarchar")]
    public string Recibo { get; set; }

    [Column(TypeName = "int")]
    public int? AppSubCategoryId { get; set; }

    [Column(TypeName = "bigint")]
    public long Orden { get; set; }

    [Column(TypeName = "numeric")]
    public decimal? SolicitudDeCredito { get; set; }

    [Column(TypeName = "nvarchar")]
    public string ObservacionesCreditoExcepcion { get; set; }

    [Column(TypeName = "bit")]
    public bool? SolicitudCerrada { get; set; }

    [Column(TypeName = "nvarchar")]
    [MaxLength(1)]
    public string TipoProducto { get; set; }

    [Column(TypeName = "numeric")]
    public decimal? CotizacionCorta { get; set; }
  

}



