﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesFacturacion
{
    public partial class Ciny208
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
        public DateTime? FechaFactura { get; set; }
        public DateTime? FechaOrden { get; set; }
        public string Pais { get; set; }
        public string LineaProduccio { get; set; }
        public string FamiliaProducc { get; set; }
        public string Medida { get; set; }
        public string FlagExistencia { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FecRecPlaneac { get; set; }
        public string FlagTipoJob { get; set; }
        public DateTime? FecComproClte { get; set; }
        public string OrigenProducto { get; set; }
        public string FlagRetenido { get; set; }
        public string Presentacion { get; set; }
        public string FlagTipoOrden { get; set; }
        public int Remision { get; set; }
        public string FlagOficina { get; set; }
        public string Millares { get; set; }
        public string UnidadFact { get; set; }
        public decimal FormasUnidad { get; set; }
        public string PrefijoDesde { get; set; }
        public string NumeroDesde { get; set; }
        public string SufijoDesde { get; set; }
        public string PrefijoHasta { get; set; }
        public string NumeroHasta { get; set; }
        public string SufijoHasta { get; set; }
        public int Bultos { get; set; }
        public decimal ImpuestoDol { get; set; }
        public string Usuario { get; set; }
        public decimal? TipoCambio { get; set; }
        public decimal? Iva { get; set; }
        public decimal? Unidadesent { get; set; }
        public decimal? Preciounidad { get; set; }
        public decimal? Copy { get; set; }
        public DateTime? FechaProg { get; set; }
        public int? BultoDesp { get; set; }
        public decimal? KiloBulto { get; set; }
        public int? BultoTran { get; set; }
        public decimal? Rimpuesto1 { get; set; }
        public decimal? RvalorVenta { get; set; }
        public decimal? RvalorLista { get; set; }
        public decimal? RprecioVenta { get; set; }
        public decimal? RprecioLista { get; set; }
        public decimal? Rpreciounidad { get; set; }
        public string PrefijoCli { get; set; }
        public string SufijoCli { get; set; }
        public string NumCliDesde { get; set; }
        public string NumCliHasta { get; set; }
        public string GeneraDespacho { get; set; }
        public long? IdPrelista { get; set; }
        public string FacturasAdelantadas { get; set; }
        public decimal? IdRegistroPrelista { get; set; }
        public decimal? VentaConciliado { get; set; }
        public decimal? DocumentoSap { get; set; }
    }
}
