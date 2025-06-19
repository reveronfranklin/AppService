using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class Ventas
    {
     
        public decimal? VentaDolares { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public decimal? AloContable { get; set; }
        public decimal? MesContable { get; set; }
        public int? CodOficina { get; set; }
        public string NomOficina { get; set; }
        public string Region { get; set; }
        public string NombreRegion { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public string NomVendedor { get; set; }
      
    }
}
