using System;
using System.Collections.Generic;
using System.Text;

namespace AppService.Core.QueryFilters
{
    //nueva
    public class AppAdjuntosCotizacionFilter
    {        

        //Paginacion
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        //Filtros
        public string Cotizacion { get; set; }
        public int Renglon { get; set; }
    }
}
