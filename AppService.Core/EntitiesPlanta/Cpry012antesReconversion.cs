﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPlanta
{
    public partial class Cpry012antesReconversion
    {
        public decimal Recnum { get; set; }
        public long Orden { get; set; }
        public short? Pais { get; set; }
        public int? Cliente { get; set; }
        public int NoTransmision { get; set; }
        public DateTime FecRegVentas { get; set; }
        public DateTime? FecCreditos { get; set; }
        public DateTime? FecTransmision { get; set; }
        public DateTime? FecRecibPlant { get; set; }
        public DateTime? FecProgramada { get; set; }
        public string Estado { get; set; }
        public DateTime? FecComproClte { get; set; }
        public string TipoOrden { get; set; }
        public long? CantOrdenada { get; set; }
        public long? CantProducida { get; set; }
        public long? CantFacturada { get; set; }
        public string NombreCliente { get; set; }
        public string DescProducto { get; set; }
        public string CodProducto { get; set; }
        public decimal? PrecioLista { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal? TotalLista { get; set; }
        public decimal? TotalVenta { get; set; }
        public string LineaProducto { get; set; }
        public string FamiliaProduct { get; set; }
        public string NombreProducto { get; set; }
        public int? MedidaBase { get; set; }
        public int? MedidaVariable { get; set; }
        public short? PartesFormula { get; set; }
        public string CondicionProdu { get; set; }
        public string FlagProdInter { get; set; }
        public short? NuTintasFrent { get; set; }
        public string PrimTintaFte { get; set; }
        public string SegTintaFte { get; set; }
        public string TercTintaFte { get; set; }
        public string CuarTintaFte { get; set; }
        public string QuintTintaFte { get; set; }
        public string SexTintaFte { get; set; }
        public short? NuTintasResp { get; set; }
        public string PrimTintaResp { get; set; }
        public string SegTintaResp { get; set; }
        public string TercTintaResp { get; set; }
        public long? CantProgramada { get; set; }
        public string Vendedor { get; set; }
        public string TipoEmpaque { get; set; }
        public DateTime? FecCierreOrd { get; set; }
        public short? CodBacklogRet { get; set; }
        public string FacturaDolar { get; set; }
        public string FlagFactura { get; set; }
        public short? LineaIncoming { get; set; }
        public string TipoProducto { get; set; }
        public string Supervisor { get; set; }
        public string FlagActivo { get; set; }
        public string LugarProducir { get; set; }
        public decimal? TotalListaExt { get; set; }
        public decimal? TotalVentaExt { get; set; }
        public string LugarEnviar { get; set; }
        public long? CantAProducir { get; set; }
        public long? OrdenAnterior { get; set; }
        public DateTime? FechaCalculo { get; set; }
        public string OrdenComb { get; set; }
        public DateTime FechaArchivo { get; set; }
        public int? ConsArchivo { get; set; }
        public string PrefPrenStd { get; set; }
        public string PrefPrenEsp { get; set; }
        public string PrefColeStd { get; set; }
        public string PrefColeEsp { get; set; }
        public string PoncheEsp { get; set; }
        public string EmpacarEn { get; set; }
        public int? CantXPaq { get; set; }
        public string Doblez { get; set; }
        public string TipoCaja { get; set; }
        public int? CantCajas { get; set; }
        public int? CantXCajas { get; set; }
        public decimal? Refinado { get; set; }
        public string RefinadoEn { get; set; }
        public string GomaRegPtes { get; set; }
        public string EngomaCarbPre { get; set; }
        public string EngomaCarbCol { get; set; }
        public string Cortado { get; set; }
        public string Solido { get; set; }
        public string EmbReverso { get; set; }
        public string Regular { get; set; }
        public string TranfTapeRem { get; set; }
        public string TranfTapePerm { get; set; }
        public string TranfTapeEf { get; set; }
        public int? NumeroTiras { get; set; }
        public string PartesTiras { get; set; }
        public string PosTirasF { get; set; }
        public string PosTirasR { get; set; }
        public string SalidaPreCab { get; set; }
        public string SalidaPrePie { get; set; }
        public string Impresion { get; set; }
        public string AnchosRod1 { get; set; }
        public string AnchosPts1 { get; set; }
        public string AnchosRod2 { get; set; }
        public string AnchosPts2 { get; set; }
        public short? RollosRod1 { get; set; }
        public decimal? RollosCtd1 { get; set; }
        public string RollosPts1 { get; set; }
        public short? RollosRod2 { get; set; }
        public decimal? RollosCtd2 { get; set; }
        public string RollosPts2 { get; set; }
        public short? RollosRod3 { get; set; }
        public decimal? RollosCtd3 { get; set; }
        public string RollosPts3 { get; set; }
        public string Fiscal { get; set; }
        public string InstFacturar { get; set; }
        public string PoncMargIzq { get; set; }
        public string PoncMargDer { get; set; }
        public string TalonExtra { get; set; }
        public string CteEntregar { get; set; }
        public string DirecEntregar { get; set; }
        public string RifEntregar { get; set; }
        public string CteFacturar { get; set; }
        public string DirecFacturar { get; set; }
        public string RifFacturar { get; set; }
        public string FlagV19 { get; set; }
        public string Cotizacion { get; set; }
        public string StatusOrden { get; set; }
        public string DirecEntregar2 { get; set; }
        public string DirecEntregar3 { get; set; }
        public string DirecFacturar2 { get; set; }
        public string DirecFacturar3 { get; set; }
        public decimal PorcMc { get; set; }
        public long? CtdEntregada { get; set; }
        public decimal? VentaUnitDol { get; set; }
        public decimal? ListaUnitDol { get; set; }
        public int? FormasUndFact { get; set; }
        public string VinetaImpresa { get; set; }
        public string Anulada { get; set; }
        public short? CondPago { get; set; }
        public decimal? Tasa { get; set; }
        public decimal? VentaTotDolar { get; set; }
        public decimal? ListaTotDolar { get; set; }
        public DateTime? FechaDolar { get; set; }
        public string SiColetilla { get; set; }
        public string SiIndexado { get; set; }
        public string SiDolares { get; set; }
        public long? CtdNc { get; set; }
        public int Anticipo { get; set; }
        public short LineaAnt { get; set; }
        public string ClaseOrden { get; set; }
        public string RifImprimir { get; set; }
        public string CodTeleop { get; set; }
        public string Excepcion { get; set; }
        public decimal? PorcCom { get; set; }
        public decimal? POverrideG { get; set; }
        public decimal? POverrideCg { get; set; }
        public decimal? Comision { get; set; }
        public decimal? POverride { get; set; }
        public decimal? PCobranza { get; set; }
        public decimal? POverger { get; set; }
        public decimal? POverrideGo { get; set; }
        public short? Tabla { get; set; }
        public string FlagSoportev { get; set; }
        public string Web { get; set; }
        public string Adelantada { get; set; }
        public string AutorizaAde { get; set; }
        public string ImprimeDol { get; set; }
        public short? CodAplicacion { get; set; }
        public decimal? Impout { get; set; }
        public decimal? Impvar { get; set; }
        public decimal? PorcMcPapel { get; set; }
        public decimal? PorcMcFinan { get; set; }
        public decimal? BsMcPapel { get; set; }
        public decimal? BsMcFinan { get; set; }
        public decimal? CambioCompo { get; set; }
        public decimal? PorcMcPapelsc { get; set; }
        public decimal? PorcMcFinansc { get; set; }
        public decimal? BsMcPapelsc { get; set; }
        public decimal? BsMcFinansc { get; set; }
        public int? Idcalendar { get; set; }
        public decimal? AnticipoJde { get; set; }
        public decimal? LineaAntJde { get; set; }
        public string Nocalculamc { get; set; }
        public decimal? RprecioLista { get; set; }
        public decimal? RprecioVenta { get; set; }
        public decimal? RtotalLista { get; set; }
        public decimal? RtotalVenta { get; set; }
        public decimal? RtotalListaEx { get; set; }
        public decimal? RtotalVentaEx { get; set; }
        public decimal? Rcomision { get; set; }
        public decimal? RbsMcPapel { get; set; }
        public decimal? RbsMcFinan { get; set; }
        public decimal? RbsMcPapelsc { get; set; }
        public decimal? RbsMcFinansc { get; set; }
        public decimal? Rtasa { get; set; }
        public string Idcalendar2 { get; set; }
        public decimal? An8facturar { get; set; }
        public decimal? An8entregar { get; set; }
        public decimal? PorcMcPapelE { get; set; }
        public decimal? PorcMcFinanE { get; set; }
        public decimal? BsMcPapelE { get; set; }
        public decimal? BsMcFinanE { get; set; }
        public decimal? PorcMcpapelsce { get; set; }
        public decimal? PorcMcfinansce { get; set; }
        public decimal? BsMcPapelsce { get; set; }
        public decimal? BsMcFinansce { get; set; }
        public decimal? CantidadRodada { get; set; }
        public string Ws { get; set; }
        public decimal? IdSolicitud { get; set; }
        public decimal? IdSolicitud2 { get; set; }
        public string RecalcularMargen { get; set; }
        public decimal? IdDirFacturar { get; set; }
        public decimal? IdDirEntregar { get; set; }
        public string EstadoFacturar { get; set; }
        public string MunicipioFacturar { get; set; }
        public string EstadoEntregar { get; set; }
        public string MunicipioEntregar { get; set; }
        public decimal? PorcRespSocial { get; set; }
        public decimal? PorcComRegulada { get; set; }
        public decimal? PorcMcMinimo { get; set; }
        public int? IdCombinada { get; set; }
        public decimal? IdSolicitudCosto { get; set; }
        public bool? Plantilla { get; set; }
        public decimal? CostoSap { get; set; }
        public decimal? IdSolicitudReoperacion { get; set; }
        public decimal? PrecioUnitarioUsd { get; set; }
        public decimal? TotalPropuestaUsd { get; set; }
        public decimal? TasaUsd { get; set; }
        public DateTime? FechaTasa { get; set; }
        public decimal? RbsMcPapelE { get; set; }
        public decimal? RbsMcFinanE { get; set; }
        public decimal? RbsMcPapelsce { get; set; }
        public decimal? RbsMcFinansce { get; set; }
        public decimal? TotalVentareconvertido { get; set; }
    }
}
