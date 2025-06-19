using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VTiemposWepy208
    {
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public DateTime Fecha { get; set; }
        public long Orden { get; set; }
        public string NombreCliente { get; set; }
        public int? Cliente { get; set; }
        public long Operador { get; set; }
        public string NombreOperador { get; set; }
        public long CantidadRodada { get; set; }
        public long? FormasRodada { get; set; }
        public long? CantOrdenada { get; set; }
        public short Maquina { get; set; }
        public string DescMaquina { get; set; }
        public string LineaProducto { get; set; }
        public string Producto { get; set; }
        public string DescProducto { get; set; }
        public short Anchos { get; set; }
        public int Basica { get; set; }
        public int Opuesta { get; set; }
        public decimal? TotalPropuestaUsd { get; set; }
        public long? VentaRodado { get; set; }
        public long? PiesRodados { get; set; }
        public long? PiesTotales { get; set; }
        public decimal? Tiempo { get; set; }
        public short CodigoTiempo { get; set; }
        public string DescTiempo { get; set; }
        public string DescCosto { get; set; }
    }
}
