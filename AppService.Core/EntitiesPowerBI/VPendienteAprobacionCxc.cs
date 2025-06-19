using System;
using System.Collections.Generic;

namespace AAppService.Core.EntitiesPowerBI
{
    public partial class VPendienteAprobacionCxc
    {
        public decimal PorcVentasMinimo { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public string Cotizacion { get; set; }
        public string CodigoVendedor { get; set; }
        public string CodCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NombreVendedor { get; set; }
        public short CodigoOficina { get; set; }
        public string NomOficina { get; set; }
        public string NombreRegion { get; set; }
        public decimal MontoSolicitado { get; set; }
        public decimal TotalUsd { get; set; }
        public decimal TotalBs { get; set; }
        public string Producto { get; set; }
        public string CodigoProducto { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Descripcion { get; set; }
    }
}
