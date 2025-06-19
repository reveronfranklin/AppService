using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VHistoricoComsione
    {
        public string IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string IdVendedor { get; set; }
        public string NombreVendedor { get; set; }
        public string NombreOficina { get; set; }
        public decimal? Orden { get; set; }
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public decimal? BsComision { get; set; }
        public decimal? BsBaseComsion { get; set; }
        public decimal? ComCuotasCump { get; set; }
        public decimal? ComCuotasGenerales { get; set; }
        public decimal? Tasa { get; set; }
        public decimal? UsdComision { get; set; }
        public decimal? UsdBaseComision { get; set; }
        public decimal? UsdComisionCantidadCuotasCumplidas { get; set; }
        public decimal? UsdComisionRangoCumplimientoCuotaGeneral { get; set; }
        public DateTime? FechaTasa { get; set; }
        public string DescripcionTipoPago { get; set; }
        public string LineaNegocio { get; set; }
        public string Descripcion { get; set; }
        public string Producto { get; set; }
        public string Subcategoria { get; set; }
        public decimal? PorComisionAplicado { get; set; }
        public decimal? Nuevacomision { get; set; }
        public decimal? CantidadOrden { get; set; }
        public decimal? Porcentaje { get; set; }
        public decimal? Concesion { get; set; }
        public DateTime? FechaOrden { get; set; }
        public decimal? Sobreprecio { get; set; }
        public string Concepto { get; set; }
        public string Periodo { get; set; }
        public decimal? MontoRecibo { get; set; }
        public decimal? VentaOrdenUsd { get; set; }
        public int? IdPeriodo { get; set; }
        public int Id { get; set; }
        public string SearchText { get; set; }
        public int? Documento { get; set; }
        public string DescripcionPeriodo { get; set; }
        public string ObsManuales { get; set; }
    }
}
