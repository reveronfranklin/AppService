using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPagosManualesService
{
    Task<ResultDto<List<Wsmy685>>> GetPaginate(PagosManualesFilter filter);
    Task<ResultDto<Wsmy685>> Update(PagosManualesUpdateDto dto);
    Task<ResultDto<Wsmy685>> Create(PagosManualesUpdateDto dto);
    Task<ResultDto<PagosManualesDeleteDto>> Delete(PagosManualesDeleteDto dto);
}