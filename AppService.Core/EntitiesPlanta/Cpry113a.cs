﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPlanta
{
    public partial class Cpry113a
    {
        public decimal Recnum { get; set; }
        public string PapelLista { get; set; }
        public string PapelMat { get; set; }
        public string PapelFifo { get; set; }
        public string Eliminado { get; set; }
        public decimal CostoPapel { get; set; }
        public string Descripcion { get; set; }
        public string TipoPapel { get; set; }
        public decimal CostoPapelDol { get; set; }
        public decimal? RcostoPapel { get; set; }
    }
}
