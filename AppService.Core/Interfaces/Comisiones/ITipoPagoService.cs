using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Shared;
using AppService.Core.EntitiesMooreve;
using PcTipoPagoResponseDto = AppService.Core.DTOs.Comisiones.PcTipoPagoResponseDto;

namespace AppService.Core.Interfaces.Comisiones;

public interface ITipoPagoService
{
    Task<ResultDto<List<PCTipoPago>>> GetAll();
    Task<ResultDto<List<PcTipoPagoResponseDto>>> GetAllManuales();

}