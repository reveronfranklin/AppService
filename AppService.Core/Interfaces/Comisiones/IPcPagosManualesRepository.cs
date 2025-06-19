using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcPagosManualesRepository
{
    Task<List<Wsmy685>> GetAll();
    Task<Wsmy685> GetById(long id);
    Task Add(Wsmy685 entity);
    void Update(Wsmy685 entity);
    Task Delete(long id);
    Task<ResultDto<List<Wsmy685>>> GetPaginate(PagosManualesFilter filter);
}