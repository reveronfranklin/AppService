using System.Threading.Tasks;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Cotizaciones;

public interface IAppGeneralQuotesActionSheetRepository
{
    Task<AppGeneralQuotesActionSheet> GetByCotizacion(string cotizacion);
}