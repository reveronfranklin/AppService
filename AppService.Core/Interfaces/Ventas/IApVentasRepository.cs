using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Shared;
using AppService.Core.DTOs.Ventas;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Ventas;

public interface IAppVentasRepository
{
    Task<ResultDto<List<AppVentas>>> GetPaginate(AppVentasFilter filter);
    Task<AppVentas> GetByOrdenProducto(decimal orden, string producto);
    Task<AppVentas> GetByOrden(decimal orden);
}