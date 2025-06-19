using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VEmbarquesVenta
    {
        public decimal Id { get; set; }
        public string Detalle { get; set; }
        public string Embarques { get; set; }
        public string Ventas { get; set; }
        public string Cobrado { get; set; }
        public string EstacionCxc { get; set; }
        public string EstacionPlanta { get; set; }
    }
}
