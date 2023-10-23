using System;
using System.Collections.Generic;

namespace AppService.Core.Entities
{
    public partial class AppTituloVariable
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int? IdSubCategoria { get; set; }
        public string CodeVariable { get; set; }
        public int? OrdenReporte { get; set; }
    }
}
