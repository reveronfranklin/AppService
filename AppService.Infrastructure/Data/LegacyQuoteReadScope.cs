namespace AppService.Infrastructure.Data
{
    // Keyless read projection only; never added to historical EF entities.
    public sealed class LegacyQuoteReadScope
    {
        public int Id { get; set; }
        public string Cotizacion { get; set; }
    }
}
