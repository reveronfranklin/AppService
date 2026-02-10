namespace AppService.Core.Entities
{
   public class AppGeneralQuotesActionSheet
    {
        public string Cotizacion { get; set; } = string.Empty;
        public int? Estatus { get; set; }
        public bool? Actualizar { get; set; }
        public bool? EnviarAlCliente { get; set; }
        public bool? EnviarAprobacionPrecio { get; set; }
        public bool? GanarPerder { get; set; }
        public bool? Postergar { get; set; }
        public bool? Eliminar { get; set; }
        public bool? Cancel { get; set; }
        public bool? ExistQuotes { get; set; } = true;
        public bool? Imprimir { get; set; }
        public bool? RetornarAGrabacion { get; set; }
        public bool? TieneOrden { get; set; }
    }

}
