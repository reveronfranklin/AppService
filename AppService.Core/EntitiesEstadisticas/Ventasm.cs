﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesEstadisticas
{
    public partial class Ventasm
    {
        public int Recnum { get; set; }
        /// <summary>
        /// Numero de Orden
        /// </summary>
        public decimal? Orden { get; set; }
        public decimal? Correlativo { get; set; }
        public decimal? Renglon { get; set; }
        /// <summary>
        /// Codigo de Producto
        /// </summary>
        public string Producto { get; set; }
        public string NomProducto { get; set; }
        public string Tipo { get; set; }
        public decimal? Clase { get; set; }
        /// <summary>
        /// Codigo de Cliente
        /// </summary>
        public decimal? Cliente { get; set; }
        public string Nombre { get; set; }
        /// <summary>
        /// Codigo de Vendedor
        /// </summary>
        public string Vendedor { get; set; }
        public string NomVendedor { get; set; }
        /// <summary>
        /// Codigo de Supervisor
        /// </summary>
        public string Supervisor { get; set; }
        public string TipoVend { get; set; }
        public string Grupo { get; set; }
        /// <summary>
        /// Cotizacion
        /// </summary>
        public string Cotizacion { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public decimal? Ano { get; set; }
        public decimal? Mes { get; set; }
        public decimal? Partes { get; set; }
        public decimal MillaresP { get; set; }
        /// <summary>
        /// Precio de Venta Unitario
        /// </summary>
        public decimal? PrecioVenta { get; set; }
        public decimal? PrecioLista { get; set; }
        /// <summary>
        /// Total Venta
        /// </summary>
        public decimal? ValorVenta { get; set; }
        /// <summary>
        /// Total Lista
        /// </summary>
        public decimal? ValorLista { get; set; }
        /// <summary>
        /// Mes Contable
        /// </summary>
        public decimal? MesContable { get; set; }
        /// <summary>
        /// Año Contable
        /// </summary>
        public decimal? AloContable { get; set; }
        public string ZonaVta { get; set; }
        public string RegionVta { get; set; }
        public string TipoPapel { get; set; }
        public string MetodoLista { get; set; }
        public string TipoProdu { get; set; }
        public string Territorio { get; set; }
        public decimal? NotaCambio { get; set; }
        public decimal? Formas { get; set; }
        public string Basica { get; set; }
        public string Opuesta { get; set; }
        public decimal? Conteo { get; set; }
        public string Codmos { get; set; }
        public string Fiscal { get; set; }
        public string CodPro { get; set; }
        public string CodproRpg { get; set; }
        public string Flag { get; set; }
        public string Codcorp { get; set; }
        public decimal? MesReorden { get; set; }
        public decimal? AnoReorden { get; set; }
        public string LineaProduc { get; set; }
        /// <summary>
        /// Nombre de Oficina
        /// </summary>
        public string NombOfic { get; set; }
        public string Descripcion { get; set; }
        public string NombreDeForma { get; set; }
        public decimal? ListaDol { get; set; }
        public decimal? VentaDol { get; set; }
        public decimal? Tasa { get; set; }
        public DateTime? FechaReorden { get; set; }
        public decimal? Concesion { get; set; }
        public decimal? Tabla { get; set; }
        public decimal? POverger { get; set; }
        public decimal? Overger { get; set; }
        public decimal? PComision { get; set; }
        public decimal? Comision { get; set; }
        public decimal? POverride { get; set; }
        public decimal? Override { get; set; }
        public string Familia { get; set; }
        public string ManualAuto { get; set; }
        public decimal? Bsmc { get; set; }
        public decimal? Pormc { get; set; }
        public string CodTeleop { get; set; }
        public string Rangobajomargen { get; set; }
        public string Rangoaltomargen { get; set; }
        public decimal? PiesTotal { get; set; }
        public string RangoPiestotal { get; set; }
        public decimal? PorcMcPapel { get; set; }
        /// <summary>
        /// Porcentaje de MC Financiero
        /// </summary>
        public decimal? PorcMcFinan { get; set; }
        public decimal? BsMcPapel { get; set; }
        public decimal? BsMcFinan { get; set; }
        public decimal? PorcMcPapelsc { get; set; }
        public decimal? PorcMcFinansc { get; set; }
        public decimal? BsMcPapelsc { get; set; }
        public decimal? BsMcFinansc { get; set; }
        public decimal? SdfV { get; set; }
        public decimal? SdfTl { get; set; }
        public decimal? SdfGo { get; set; }
        public decimal? SdfGr { get; set; }
        public decimal? SdfGic { get; set; }
        public decimal? SdfCic { get; set; }
        public decimal? SdfGop { get; set; }
        public string CodGo { get; set; }
        public string CodGr { get; set; }
        public string CodGic { get; set; }
        public string CodCic { get; set; }
        public string CodGop { get; set; }
        public decimal? ComisionGo { get; set; }
        public decimal? ComisionGic { get; set; }
        public decimal? ComisionCic { get; set; }
        public decimal? ComisionGop { get; set; }
        public decimal? PorcGo { get; set; }
        public decimal? PorcGic { get; set; }
        public decimal? PorcCic { get; set; }
        public decimal? PorcGop { get; set; }
        public string Nocalculamc { get; set; }
        public decimal? RprecioVenta { get; set; }
        public decimal? RprecioLista { get; set; }
        public decimal? RvalorVenta { get; set; }
        public decimal? RvalorLista { get; set; }
        public decimal? Rtasa { get; set; }
        public decimal? Roverger { get; set; }
        public decimal? Rcomision { get; set; }
        public decimal? Roverride { get; set; }
        public decimal? Rbsmc { get; set; }
        public decimal? RbsMcPapel { get; set; }
        public decimal? RbsMcFinan { get; set; }
        public decimal? RbsMcPapelsc { get; set; }
        public decimal? RbsMcFinansc { get; set; }
        public decimal? RsdfV { get; set; }
        public decimal? RsdfTl { get; set; }
        public decimal? RsdfGo { get; set; }
        public decimal? RsdfGr { get; set; }
        public decimal? RsdfGic { get; set; }
        public decimal? RsdfCic { get; set; }
        public decimal? RsdfGop { get; set; }
        public decimal? RcomisionGo { get; set; }
        public decimal? RcomisionGic { get; set; }
        public decimal? RcomisionCic { get; set; }
        public decimal? RcomisionGop { get; set; }
        /// <summary>
        /// Condicion de Pago
        /// </summary>
        public int? Condpago { get; set; }
        public int? CodOficina { get; set; }
        public decimal? PorcMcFinanEmb { get; set; }
        public decimal? BsMcFinanEmb { get; set; }
        public decimal? PorcMcFinanscEmb { get; set; }
        public decimal? BsMcFinanscEmb { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public int? IdCuota { get; set; }
        public decimal? IdSolicitud { get; set; }
        public decimal? L03calComision { get; set; }
        public decimal? X10calPvjusto { get; set; }
        public decimal PiesTotales { get; set; }
        public decimal PiesLineales { get; set; }
        public decimal PiesVenta { get; set; }
        public decimal KilosOrden { get; set; }
        public decimal PiesCuadrado { get; set; }
        public decimal? VentaDolRef { get; set; }
        public decimal? TasaDolRef { get; set; }
        public int? IdCuotaComision { get; set; }
        public DateTime? FechaDolRef { get; set; }
        public decimal? PrecioUnitarioUsd { get; set; }
        public decimal? Usdmc { get; set; }
        public decimal? RtasaDolRef { get; set; }
        public string CotizacionCorta { get; set; }
        public string CotizacionLarga { get; set; }
    }
}
