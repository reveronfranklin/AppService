using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.DTOs.Comisiones.PcOrdenesSinCalculoComision;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcTipoPagoOrdenNoCalcularComisionService
{
    Task<DTOs.Shared.ResultDto<List<PCTipoPagoOrdenNoCalcularComision>>> GetPaginate(
        PagosManualesFilter filter);

    Task<ResultDto<PCTipoPagoOrdenNoCalcularComision>> Update(PcTipoPagoOrdenNoCalcularComisionUpdateDto dto);

    Task<ResultDto<PCTipoPagoOrdenNoCalcularComision>> Create(PcTipoPagoOrdenNoCalcularComisionUpdateDto dto);

    Task<ResultDto<PcTipoPagoOrdenNoCalcularComisionUpdateDto>> Delete(
        PcTipoPagoOrdenNoCalcularComisionUpdateDto dto);
    
}