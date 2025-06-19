using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.AppCalculadora;
using AppService.Core.DTOs.Shared;

namespace AppService.Core.Interfaces;

public interface IAppCalculadoraService
{
    Task<ResultDto<List<AppCalculadoraResponseDto>>> GetByVendedor(AppCalculadoraFilterDto filter);
    Task<ResultDto<bool>> Create(AppCalculadoraUpdateDto dto);
    Task<ResultDto<bool>> Update(AppCalculadoraUpdateDto dto);
    Task<ResultDto<bool>> Delete(AppCalculadoraDeleteDto dto);

}