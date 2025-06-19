using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class RangoVariacionVentaMinimo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal RangoDesde { get; set; }
        public decimal RangoHasta { get; set; }
    }
}
