﻿using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class CobRangoDiasPago
    {
        public int Id { get; set; }
        public int? Desde { get; set; }
        public int? Hasta { get; set; }
        public string Titulo { get; set; }
    }
}
