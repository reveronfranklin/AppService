﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesEstadisticas
{
    public partial class EstadisticoCobranzas01
    {
        public string CodVendedor { get; set; }
        public decimal Cuota { get; set; }
        public decimal Mes { get; set; }
        public decimal Ano { get; set; }
        public decimal? Monto { get; set; }
        public string CodigoGrupo { get; set; }
    }
}
