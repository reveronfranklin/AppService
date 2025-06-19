using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VDwConsolidadoAnalisisDeVencimiento
    {
        public double? Ano { get; set; }
        public double? Mes { get; set; }
        public double? CodigoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string CodigoVendedor { get; set; }
        public string NombreVendedor { get; set; }
        public short? Oficina { get; set; }
        public string NombOfic { get; set; }
        public double? Saldo { get; set; }
        public decimal? Retencion { get; set; }
        public string Documento { get; set; }
        public string NombreRegion { get; set; }
        public DateTime? FechaFactura { get; set; }
        public decimal? Tasa { get; set; }
        public decimal? TasaHoy { get; set; }
    }
}
