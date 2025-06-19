using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VPendienteAprobacionPlantum
    {
        public string Estacion { get; set; }
        public decimal NumCot { get; set; }
        public string CotizacionOriginal { get; set; }
        public DateTime? FechaPasada { get; set; }
        public string CodVend { get; set; }
        public string Oficina { get; set; }
        public string NomOficina { get; set; }
        public string NombreRegion { get; set; }
        public string CodigoClient { get; set; }
        public string NomCliente { get; set; }
        public string Vendedor { get; set; }
        public decimal? Totalpropuesta { get; set; }
        public decimal? Totalpropuestausd { get; set; }
        public string Producto { get; set; }
        public string CodigoProducto { get; set; }
        public string Lineanegocio { get; set; }
    }
}
