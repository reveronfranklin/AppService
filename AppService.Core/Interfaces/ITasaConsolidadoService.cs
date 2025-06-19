using System.Collections.Generic;
using AppService.Core.DTOs.Shared;
using AppService.Core.EntitiesMooreve;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;

namespace AppService.Core.Interfaces;

public interface ITasaConsolidadoService
{
    Task<ResultDto<List<TasaConsolidadasResponseDto>>> GetAll();
    
    Task<ResultDto<bool>> UpdateTasas(TasasConsolidadasUpdateDto tasas);
    Task<ResultDto<bool>> DeleteTasas(TasasConsolidadasUpdateDto tasas);
}