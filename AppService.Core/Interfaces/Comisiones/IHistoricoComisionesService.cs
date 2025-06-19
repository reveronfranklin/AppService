using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Responses;

namespace AppService.Core.Interfaces.Comisiones;

public interface IHistoricoComisionesService
{
    Task<ApiResponse<List<VHistoricoComsiones>>> GetByPeriodo(PeriodoFilter filter);
    Task<ApiResponse<List<ResumenComisionResponse>>> GetByPeriodoResumen(PeriodoFilter filter);
}