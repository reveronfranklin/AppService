﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesEstadisticas
{
    public partial class TotalSup
    {
        public string NombreSup { get; set; }
        public decimal? Lista { get; set; }
        public decimal? Venta { get; set; }
        public decimal? Conc { get; set; }
        public decimal? Bsmc { get; set; }
        public decimal? Porcmc { get; set; }
        public decimal? Papel { get; set; }
    }
}
