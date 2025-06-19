using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Shared;
using AppService.Core.DTOs.Ventas;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Ventas;

public interface IAppVentasService
{
    Task<ResultDto<List<AppVentas>>> GetPaginate(AppVentasFilter filter);
}