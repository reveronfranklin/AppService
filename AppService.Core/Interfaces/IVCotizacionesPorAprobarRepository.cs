using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Aprobaciones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;

namespace AppService.Core.Interfaces;

public interface IVCotizacionesPorAprobarRepository
{
   
    Task<List<V_CotizacionesPorAprobar>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter);
    Task<List<CotizacionesViewData>> GetAllBySearchTextAll(AppGeneralQuotesQueryFilter filter);
    Task<bool> UpdateCotizacion(ActualizarCotizacion dto);
    Task<ApiResponse<string>> EtlPasePlanta(ActualizarCotizacion dto);
    Task<ApiResponse<string>> CerrarExcepcion(ActualizarCotizacion dto);
    Task<ApiResponse<string>> VerificarCredito(string cotizacion, string usuarioConectado);
}