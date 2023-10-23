using System;
namespace AppService.Core.Entities
{
	public class AppReporteCotizacionEtiquetasPrime
	{

        public decimal Id { get; set; }
        public int AppGeneralQuotesId { get; set; }
        public int AppDetailQuotesId { get; set; }
        public string Cotizacion { get; set; }
        public string Fecha { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Producto { get; set; }
        public string Impresion { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Forma { get; set; }
        public string SustratoBase { get; set; }
        public string Acabado { get; set; }
        public string CantidadImpresion { get; set; }
        public string Tamano { get; set; }
        public string Presentacion { get; set; }
        public string CantidadProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public string UnitarioProducto { get; set; }
        public string TotalProducto { get; set; }
        public string CantidadClise { get; set; }
        public string DescripcionClise { get; set; }
        public string UnitarioClise { get; set; }
        public string TotalCliese { get; set; }
        public string CantidadTroquel { get; set; }
        public string DescripcionTroquel { get; set; }
        public string UnitarioTroquel { get; set; }
        public string TotalTroquel { get; set; }
        public string Total { get; set; }
        public string CondicionPago { get; set; }
        public string Vigencia { get; set; }
        public string Vendedor { get; set; }
        public int CalculoId { get; set; }


    }
}

