using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VBacklogCpry010
    {
        public long Orden { get; set; }
        public long JobId { get; set; }
        public string NombreCliente { get; set; }
        public int? CodigoCliente { get; set; }
        public long? CantidadOrdenada { get; set; }
        public short? Partes { get; set; }
        public int? TintasFrente { get; set; }
        public int? TintasRespado { get; set; }
        public string Vendedor { get; set; }
        public string Oficina { get; set; }
        public string Region { get; set; }
        public string Estacion { get; set; }
        public long? CantidadEstacion { get; set; }
        public decimal? UsdCantidadEstacion { get; set; }
        public decimal? TotalPropuestaUsd { get; set; }
        public string JobRetenido { get; set; }
        public string TipoJob { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public string MaquinaPrensa { get; set; }
        public string AbreviadoMaquinaPrensa { get; set; }
        public short? TurnosPrensa { get; set; }
        public decimal? HorasPTurnoPrensa { get; set; }
        public decimal PiesXhoraRealPrensa { get; set; }
        public string MaquinaColectora { get; set; }
        public string AbreviadoMaquinaColectora { get; set; }
        public short? TurnosColectora { get; set; }
        public decimal? HorasPTurnoColectora { get; set; }
        public decimal PiesXhoraRealColectora { get; set; }
        public string CodProducto { get; set; }
        public decimal? PiesTotalesEstacion { get; set; }
        public decimal HorasTiempoProduccionColectora { get; set; }
        public decimal DiasProduccionColectora { get; set; }
        public decimal HorasTiempoProduccionPrensa { get; set; }
        public decimal DiasProduccionPrensa { get; set; }
        public long? CantFactura { get; set; }
        public long? CantProgramada { get; set; }
        public long? ProgramadaFacturada { get; set; }
    }
}
