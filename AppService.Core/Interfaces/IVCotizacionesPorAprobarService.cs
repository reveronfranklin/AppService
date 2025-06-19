using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Aprobaciones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;

namespace AppService.Core.Interfaces;

public interface IVCotizacionesPorAprobarService
{
    Task<List<V_CotizacionesPorAprobarDto>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter);
    Task<List<V_CotizacionesPorAprobarDto>> GetAllBySearchTextAll(AppGeneralQuotesQueryFilter filter);
    Task<ApiResponse<bool>> UpdateCotizacion(ActualizarCotizacion dto);
    Task<ApiResponse<string>> PasePlanta(ActualizarCotizacion dto);
    Task<ApiResponse<string>> CerrarExcepcion(ActualizarCotizacion dto);
}