using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Shared;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Cotizaciones;

public interface IAppQuotesPaginateRepository
{
    Task<ResultDto<List<AppQuotesPaginate>>> GetPaginate(AppQuotesPaginateFilter filter);
}