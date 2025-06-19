using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VTemporalComisione
    {
        public string Vendedor { get; set; }
        public string Cliente { get; set; }
        public long? Orden { get; set; }
        public decimal? BaseComsion { get; set; }
        public decimal? ComCuotasCump { get; set; }
        public decimal? ComCuotasGenerales { get; set; }
        public decimal? BsComision { get; set; }
        public string DescripcionTipoPago { get; set; }
        public decimal? Concesion { get; set; }
        public DateTime? FechaOrden { get; set; }
    }
}
