using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VVtascuota
    {
        public decimal UsdVentasEstrategica { get; set; }
        public decimal Piescuadrado { get; set; }
        public decimal Conteo { get; set; }
        public int Clientes { get; set; }
        public decimal Kilosorden { get; set; }
        public decimal UsdParalelo { get; set; }
        public decimal UsdPromedio { get; set; }
        public int Id { get; set; }
        public int? Año { get; set; }
        public int? Mes { get; set; }
        public string Vendedor { get; set; }
        public int? IdProductoCuota { get; set; }
        public decimal? CuotaUsd { get; set; }
        public decimal? VentaUsd { get; set; }
        public decimal? VentaUsdFormato { get; set; }
        public string NombreVendedor { get; set; }
        public string DescripcionCuota { get; set; }
        public string CodigoRegion { get; set; }
        public string NombreRegion { get; set; }
        public string NomOficina { get; set; }
        public string RegionJunta { get; set; }
        public int Fiscal { get; set; }
        public string DescripcionLineaNegocio { get; set; }
        public int? LineaNegocio { get; set; }
        public decimal? Budget { get; set; }
    }
}
