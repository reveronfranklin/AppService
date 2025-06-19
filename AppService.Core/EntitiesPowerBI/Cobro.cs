using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class Cobro
    {
        public decimal Id { get; set; }
        public long? Orden { get; set; }
        public string Transaccion { get; set; }
        public decimal? Documento { get; set; }
        public string TransaccionAfecta { get; set; }
        public decimal? DocumentoAfecta { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Base { get; set; }
        public decimal? Impuesto { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? DolarBcv { get; set; }
        public decimal? DolarPromedio { get; set; }
        public decimal? DolarParalelo { get; set; }
        public decimal? TasaBcv { get; set; }
        public decimal? TasaPromedio { get; set; }
        public decimal? TasaParalelo { get; set; }
        public int? IdMoneda { get; set; }
        public string Moneda { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public int? CodOficina { get; set; }
        public string NomOficina { get; set; }
        public string Region { get; set; }
        public string NombreRegion { get; set; }
        public string Nombre { get; set; }
        public string NomVendedor { get; set; }
        public string RegionJunta { get; set; }
        public decimal? PorcentajeIva { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? AloContable { get; set; }
        public int? MesContable { get; set; }
        public string Concepto { get; set; }
        public decimal? UsdVentaOrden { get; set; }
        public decimal? TasaParaleloOrden { get; set; }
        public decimal? DolarParaleloOrden { get; set; }
    }
}
