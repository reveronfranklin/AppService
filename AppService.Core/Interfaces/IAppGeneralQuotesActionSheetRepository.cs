using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.Entities;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces;

public interface IAppGeneralQuotesActionSheetRepository
{
    Task<AppGeneralQuotesActionSheet> GetByCotizacion(string cotizacion);
    Task<AppGeneralQuotesActionSheet> Create(string cotizacion);

}