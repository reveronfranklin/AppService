using System.Threading.Tasks;
namespace AppService.Core.Interfaces
{
    // Server-owned classification. Null means allowed by the configured policy.
    public interface ILegacyQuoteModelAccess
    {
        Task<string> CheckAsync(int? generalId = null, int? detailId = null, string quotation = null);
        // Default is deliberately strict for older implementations; the SQL
        // adapter explicitly supports external quote codes without an RRD row.
        Task<string> CheckExternalAsync(string quotation) => CheckAsync(quotation: quotation);
        Task<string> CheckOdooDetailAsync(int mooreId)
            => Task.FromResult("No se pudo validar la identidad del detalle Odoo.");
    }
}
