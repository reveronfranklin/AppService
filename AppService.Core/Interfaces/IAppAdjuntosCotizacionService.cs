using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.CustomEntities;
using AppService.Core.EntitiesMooreve;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;

namespace AppService.Core.Interfaces;

public interface IAppAdjuntosCotizacionService
{
    Task<ApiResponse<List<AppAdjuntosCotizacion>>> GetAdjuntosCotizacion(AppAdjuntosCotizacionFilter filters);
}