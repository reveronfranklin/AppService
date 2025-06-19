using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class Ordene
    {
        public long Orden { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Venta { get; set; }
        public decimal? DolarBcv { get; set; }
        public decimal? DolarPromedio { get; set; }
        public decimal? DolarParalelo { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public int? CodOficina { get; set; }
        public string NomOficina { get; set; }
        public string Region { get; set; }
        public string NombreRegion { get; set; }
        public string Nombre { get; set; }
        public string NomVendedor { get; set; }
        public string RegionJunta { get; set; }
        public decimal? TotalPropuestaUsd { get; set; }
        public int? AloContable { get; set; }
        public int? MesContable { get; set; }
        public DateTime? FechaTasaReferencial { get; set; }
        public decimal? TasaBcv { get; set; }
        public decimal? TasaPromedio { get; set; }
        public decimal? TasaParalelo { get; set; }
        public decimal? TotalCobrado { get; set; }
        public decimal? TotalCobradoBcv { get; set; }
        public decimal? TotalCobradoParalelo { get; set; }
        public decimal? TotalCobradoPromedio { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? CantidadEmbarcada { get; set; }
        public decimal? DiferenciaCantidad { get; set; }
        public decimal? TotalFacturadoBs { get; set; }
        public decimal? TotalCobradoBs { get; set; }
        public decimal? DiferenciaFacturadoBsCobradoBs { get; set; }
        public DateTime? FechaContableUltimoCobro { get; set; }
        public int? AñoContableUltimoCobro { get; set; }
        public int? MesContableUltimoCobro { get; set; }
        public decimal? TotalPropuestaUsdMinimo { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public string Cotizacion { get; set; }
        public DateTime? FechaEmisionUltimoCobro { get; set; }
        public int? AñoEmisionUltimoCobro { get; set; }
        public int? MesEmisionUltimoCobro { get; set; }
        public string CodigoVendedor { get; set; }
    }
}
