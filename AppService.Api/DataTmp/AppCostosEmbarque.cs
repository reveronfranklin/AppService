using System;
using System.Collections.Generic;

namespace AppService.Api.DataTmp
{
    public partial class AppCostosEmbarque
    {
        public decimal Id { get; set; }
        public string TipoDocumento { get; set; }
        public string DescripcionDocumento { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public int? Documento { get; set; }
        public decimal? Monto { get; set; }
        public int? Cliente { get; set; }
        public string NombreCliente { get; set; }
        public string CodVendedor { get; set; }
        public string NombreVededor { get; set; }
        public int? CodOficina { get; set; }
        public int? CodRegion { get; set; }
        public string NombreRegion { get; set; }
        public string Cotizacion { get; set; }
        public int? AppGeneralQuotesId { get; set; }
        public int? AppDetailQuotesId { get; set; }
        public int? CalculoId { get; set; }
        public string Articulo { get; set; }
        public int? AppProductsId { get; set; }
        public int? AppTipoCalculo { get; set; }
        public string Titulo { get; set; }
        public string CodeVariable { get; set; }
        public decimal? Valor { get; set; }
    }
}
