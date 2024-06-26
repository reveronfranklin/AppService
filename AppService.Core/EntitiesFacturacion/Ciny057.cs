﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesFacturacion
{
    public partial class Ciny057
    {
        public decimal Recnum { get; set; }
        public int Factura { get; set; }
        public short CentroFact { get; set; }
        public short TipoDoc { get; set; }
        public string Articulo { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal Impuesto1 { get; set; }
        public decimal Impuesto2 { get; set; }
        public decimal ValorVenta { get; set; }
        public decimal ValorVentad { get; set; }
        public decimal ValorLista { get; set; }
        public decimal ValorListad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioLista { get; set; }
        public long Orden { get; set; }
        public string Descripcion1 { get; set; }
        public string Descripcion2 { get; set; }
        public string TipoProducto { get; set; }
        public string FlagImprenta { get; set; }
        public string TotalParcial { get; set; }
        public int NoEntrega { get; set; }
        public long JobId { get; set; }
        public DateTime FechaFactura { get; set; }
        public DateTime? FechaOrden { get; set; }
        public string Pais { get; set; }
        public string LineaProduccio { get; set; }
        public string FamiliaProducc { get; set; }
        public DateTime? FecRecPlaneac { get; set; }
        public string FlagTipoJob { get; set; }
        public DateTime? FecComproClte { get; set; }
        public string OrigenProducto { get; set; }
        public string FlagRetenido { get; set; }
        public string FlagTipoOrden { get; set; }
        public string DireccionFact { get; set; }
        public string NombreFact { get; set; }
        public string RifFact { get; set; }
        public string DireccionEmb { get; set; }
        public string NombreEmb { get; set; }
        public string RifEmb { get; set; }
        public decimal Iva { get; set; }
        public string Linea1 { get; set; }
        public string Partes { get; set; }
        public string MetodoPrecio1 { get; set; }
        public string TipoPapel { get; set; }
        public string Lipamepa { get; set; }
        public string Familia { get; set; }
        public string UnidadFacturar { get; set; }
        public decimal FormasUnidad { get; set; }
        public decimal Bultos { get; set; }
        public string PrefijoDesde { get; set; }
        public string NumeroDesde { get; set; }
        public string SufijoDesde { get; set; }
        public string PrefijoHasta { get; set; }
        public string NumeroHasta { get; set; }
        public string SufijoHasta { get; set; }
        public string FlagFaltante { get; set; }
        public int FormasBajotiro { get; set; }
        public string Millares { get; set; }
        public string CtaLista { get; set; }
        public string CtaConc { get; set; }
        public string Estadisticas { get; set; }
        public decimal VentaUniDol { get; set; }
        public decimal ListaUniDol { get; set; }
        public decimal ImpuestoDol { get; set; }
        public string RegistroTribut { get; set; }
        public string Usuario { get; set; }
        public decimal? Copy { get; set; }
        public decimal? TipoCambio { get; set; }
        public decimal? CantEntregar { get; set; }
        public string PrefijoDesdeE { get; set; }
        public string NumeroDesdeE { get; set; }
        public string SufijoDesdeE { get; set; }
        public string PrefijoHastaE { get; set; }
        public string NumeroHastaE { get; set; }
        public string SufijoHastaE { get; set; }
        public decimal? Unidadesfact { get; set; }
        public decimal? Unidadesent { get; set; }
        public decimal? Preciounidad { get; set; }
        public string ImpStock { get; set; }
        public decimal? RemisionOfi { get; set; }
        public string GeneraDespacho { get; set; }
        public DateTime? FechaProg { get; set; }
        public int? BultoDesp { get; set; }
        public decimal? KiloBulto { get; set; }
        public int? BultoTran { get; set; }
        public decimal? Bsmc { get; set; }
        public decimal? PorcMcPapel { get; set; }
        public decimal? PorcMcFinan { get; set; }
        public decimal? BsMcPapel { get; set; }
        public decimal? BsMcFinan { get; set; }
        public decimal? Batch { get; set; }
        public string Almacen { get; set; }
        public decimal? PorcMcPapelsc { get; set; }
        public decimal? PorcMcFinansc { get; set; }
        public decimal? BsMcPapelsc { get; set; }
        public decimal? BsMcFinansc { get; set; }
        public decimal? Rimpuesto1 { get; set; }
        public decimal? RvalorVenta { get; set; }
        public decimal? RvalorLista { get; set; }
        public decimal? RprecioVenta { get; set; }
        public decimal? RprecioLista { get; set; }
        public decimal? Rpreciounidad { get; set; }
        public decimal? Rbsmc { get; set; }
        public decimal? RbsMcPapel { get; set; }
        public decimal? RbsMcFinan { get; set; }
        public decimal? RbsMcPapelsc { get; set; }
        public decimal? RbsMcFinansc { get; set; }
        public string PrefijoCli { get; set; }
        public string SufijoCli { get; set; }
        public string NumCliDesde { get; set; }
        public string NumCliHasta { get; set; }
        public string PrefijoCliE { get; set; }
        public string SufijoCliE { get; set; }
        public string NumCliDesdeE { get; set; }
        public string NumCliHastaE { get; set; }
        public decimal? IdSolicitud { get; set; }
        public string ReportadoSeniat { get; set; }
        public string RecalcularMargen { get; set; }
        public int? Año { get; set; }
        public int? Mes { get; set; }
        public int? MetodoCalculo { get; set; }
        public decimal? IdSolicitudCosto { get; set; }
        public decimal? Costo { get; set; }
        public long? IdPrelista { get; set; }
        public decimal? ValorVentaCpj { get; set; }
        public decimal? IdSolicitudCostoCpj { get; set; }
        public decimal? Pieslineales { get; set; }
        public decimal? CantidadEntregada { get; set; }
        public string Posicionsap { get; set; }
        public string Pedidosap { get; set; }
        public decimal? IdRegistroPrelista { get; set; }
        public string Vbeln { get; set; }
        public string GeneraListaDespacho { get; set; }
        public string Cotizacion { get; set; }
        public decimal? VentaEntregada { get; set; }
        public decimal? RventaEntregada { get; set; }
    }
}
