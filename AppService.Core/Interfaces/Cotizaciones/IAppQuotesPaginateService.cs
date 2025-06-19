using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Cotizaciones;

public interface IAppQuotesPaginateService
{
    Task<DTOs.Shared.ResultDto<List<AppQuotesPaginate>>> GetPaginate(AppQuotesPaginateFilter filter);
}