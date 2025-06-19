using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class ContabilidadCebesYCeco
    {
        public double? Ano { get; set; }
        public double? Mes { get; set; }
        public double? Cuenta { get; set; }
        public string DescripcionCuenta { get; set; }
        public string Cecos { get; set; }
        public string Cebes { get; set; }
        public double? Monto { get; set; }
        public double? Tasa { get; set; }
        public double? Usd { get; set; }
        public string TipoCuenta { get; set; }
        public string ClasificacionMadre { get; set; }
    }
}
