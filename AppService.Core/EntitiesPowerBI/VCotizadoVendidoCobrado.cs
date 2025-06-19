using System;
using System.Collections.Generic;

namespace AppService.Core.EntitiesPowerBI
{
    public partial class VCotizadoVendidoCobrado
    {
        public decimal Id { get; set; }
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public string FechaRegistro { get; set; }
        public string CodigoVendedor { get; set; }
        public string NombreVendedor { get; set; }
        public string NombreOficina { get; set; }
        public string NombreRegion { get; set; }
        public int? NroCotizacion { get; set; }
        public decimal? DolaresCotizado { get; set; }
        public int? ClienteCotizado { get; set; }
        public int? OrdenesVendidas { get; set; }
        public decimal? DolaresVendidos { get; set; }
        public int? ClienteNuevo { get; set; }
        public int? ClienteReactivado { get; set; }
        public int? Clienteactivo { get; set; }
        public decimal? DolaresCobrado { get; set; }
        public int? ClienteCobrado { get; set; }
        public int? NroCobranza { get; set; }
    }
}
