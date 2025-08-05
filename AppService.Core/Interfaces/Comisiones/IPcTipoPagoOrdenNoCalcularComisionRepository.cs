using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcTipoPagoOrdenNoCalcularComisionRepository
{
    Task<Core.DTOs.Shared.ResultDto<List<PCTipoPagoOrdenNoCalcularComision>>> GetPaginate(PagosManualesFilter filter);
    Task<PCTipoPagoOrdenNoCalcularComision> GetById(long id);
    Task<PCTipoPagoOrdenNoCalcularComision> GetByOrden(long orden);
    Task<PCTipoPagoOrdenNoCalcularComision> GetByTipoPagoOrden(int tipoPago, long orden);
    Task Add(PCTipoPagoOrdenNoCalcularComision entity);
    void Update(PCTipoPagoOrdenNoCalcularComision entity);
    Task Delete(long id);
    Task<string> UpdateSearchText();
}