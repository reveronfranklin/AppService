using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.DTOs.Comisiones.PcOrdenesSinCalculoComision;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcOrdenesSinCalculoComisionServices
{
    Task<DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>> GetPaginate(PagosManualesFilter filter);
    Task<ResultDto<PcOrdenesSinCalculoComision>> Update(PcOrdenesSinCalculoComisionUpdateDto dto);
    Task<ResultDto<PcOrdenesSinCalculoComision>> Create(PcOrdenesSinCalculoComisionUpdateDto dto);

    Task<ResultDto<PcOrdenesSinCalculoComisionUpdateDto>> Delete(
        PcOrdenesSinCalculoComisionUpdateDto dto);
    
}