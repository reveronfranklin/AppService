using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesMooreve
{
    public partial class PCTipoPago
    {
      

        public int Id { get; set; }
        public string TipoPago { get; set; }
        public string Descripcion { get; set; }
        public bool FlagCalcular { get; set; }
        public bool? AplicaGerente { get; set; }
        public string ConceptoNominaPago { get; set; }
        public string ConceptoNominaDescuento { get; set; }

     
    }
}
