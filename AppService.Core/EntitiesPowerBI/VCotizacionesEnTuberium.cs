using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VCotizacionesEnTuberium
    {
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public string NomOficina { get; set; }
        public string NombreRegion { get; set; }
        public string CodVendedor { get; set; }
        public string Vendedor { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public string Cotizacion { get; set; }
        public int? Renglon { get; set; }
        public int? Propuesta { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualiza { get; set; }
        public int? Estatus { get; set; }
        public string Descripcion { get; set; }
        public decimal? TotalPropuestaUsd { get; set; }
        public string CodProducto { get; set; }
        public string Producto { get; set; }
        public string SubCategoria { get; set; }
        public string ProductoCuota { get; set; }
        public string RazonSocial { get; set; }
        public string Codmos { get; set; }
        public string Motivo { get; set; }
        public string Competidor { get; set; }
        public long? Orden { get; set; }
        public string LineaNegocio { get; set; }
        public decimal? PorcVentasMinimo { get; set; }
        public decimal? PorcVentasMaximo { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public string DescripcionVariacion { get; set; }
        public string CodigoRegion { get; set; }
        public short? CodOficina { get; set; }
        public string CondicionPago { get; set; }
    }
}
