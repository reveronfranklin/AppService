using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VVenta
    {
        public decimal Id { get; set; }
        public decimal? Orden { get; set; }
        public DateTime? FechaOrden { get; set; }
        public string Producto { get; set; }
        public decimal? VentaDolRef { get; set; }
        public decimal? TasaDolRef { get; set; }
        public DateTime? FechaDolRef { get; set; }
        public decimal? TasaParalelo { get; set; }
        public decimal? UsdParalelo { get; set; }
        public decimal? Usdmc { get; set; }
        public decimal? Bsmc { get; set; }
        public decimal? Pormc { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public decimal? AloContable { get; set; }
        public decimal? MesContable { get; set; }
        public int? CodOficina { get; set; }
        public string NomOficina { get; set; }
        public string LineaProduc { get; set; }
        public string Region { get; set; }
        public decimal? IdSolicitud { get; set; }
        public string NombreRegion { get; set; }
        public string Nombre { get; set; }
        public string TipoCalculo { get; set; }
        public string Vendedor { get; set; }
        public string NomVendedor { get; set; }
        public string Subcategoria1 { get; set; }
        public decimal? PiesCuadrado { get; set; }
        public string Descripcion { get; set; }
        public int? IdCuota { get; set; }
        public decimal? Conteo { get; set; }
        public string Codmos { get; set; }
        public decimal? KilosOrden { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal? ValorVenta { get; set; }
        public decimal? PrecioVentareconvertido { get; set; }
        public decimal? ValorVentareconvertido { get; set; }
        public string RegionJunta { get; set; }
        public string Tipo { get; set; }
        public string NombreSector { get; set; }
        public string NombreRamo { get; set; }
        public string Fiscal { get; set; }
        public string Porcentajedescuento { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? Decuentousd { get; set; }
        public double? CodJdePadre { get; set; }
        public string CodigoCliente { get; set; }
        public string ClientePadre { get; set; }
        public string EstatusCliente { get; set; }
        public string DescripcionLineaNegocio { get; set; }
        public decimal? PorcVentasMinimo { get; set; }
        public decimal? PorcVentasMaximo { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public string DescripcionVariacion { get; set; }
        public DateTime? FechaContable { get; set; }
        public decimal? Alo { get; set; }
        public decimal? Mes { get; set; }
        public decimal? TasaPromedio { get; set; }
        public decimal? UsdPromedio { get; set; }
        public string OrdenEstrategica { get; set; }
        public int? IdCuotaComision { get; set; }
    }
}
