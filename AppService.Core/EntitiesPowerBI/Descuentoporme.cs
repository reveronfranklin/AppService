using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class Descuentoporme
    {
        public decimal Id { get; set; }
        public int? AloContable { get; set; }
        public int? MesContable { get; set; }
        public string Porcentaje { get; set; }
    }
}
