﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPlanta
{
    public partial class Pendienteporclientesstockdetallado
    {
        public int Cliente { get; set; }
        public decimal? PorFacturar { get; set; }
        public decimal? Iva { get; set; }
        public int? Porciva { get; set; }
        public long Orden { get; set; }
        public string CodProducto { get; set; }
    }
}
