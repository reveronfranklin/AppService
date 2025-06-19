using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VRetencione
    {
        public decimal Porcentaje { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaRecuperacion { get; set; }
        public DateTime FechaRetencion { get; set; }
        public double Factura { get; set; }
        public string IdCliente { get; set; }
        public DateTime? FechaFactura { get; set; }
    }
}
