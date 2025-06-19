using System.Threading.Tasks;
using AppService.Core.DTOs.Precios;
using AppService.Core.DTOs.Shared;

namespace AppService.Core.Interfaces;

public interface IAppGetPriceService
{
    Task<DTOs.Shared.ResultDto<OptionsPreciosProductosResponse>> GetPrice(
        OptionsPreciosProductos options);

}