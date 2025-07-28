using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcOrdenesSinCalculoComisionRepository
{
    Task<DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>> GetPaginate(PagosManualesFilter filter);

    Task<PcOrdenesSinCalculoComision> GetById(long id);
    Task<PcOrdenesSinCalculoComision> GetByOrden(long orden);
    Task Add(PcOrdenesSinCalculoComision entity);
    void Update(PcOrdenesSinCalculoComision entity);
    Task Delete(long id);
    Task<string> UpdateSearchText();
    

}