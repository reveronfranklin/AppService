using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.QueryFilters;

namespace AppService.Core.Interfaces;

public interface IVCotizacionesPorAprobarRepository
{
   
    Task<List<V_CotizacionesPorAprobar>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter);
}