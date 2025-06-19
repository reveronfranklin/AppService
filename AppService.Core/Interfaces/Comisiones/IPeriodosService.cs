using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.Responses;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPeriodosService
{
    Task<ApiResponse<List<PeriodosDto>>> GetAll();

}