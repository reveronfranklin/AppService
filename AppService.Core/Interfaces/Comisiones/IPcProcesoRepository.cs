using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcProcesoRepository
{
    Task<List<PcProceso>> GetAll();
    Task<PcProceso> GetById(long id);
    Task Add(PcProceso entity);
    void Update(PcProceso entity);
    Task Delete(long id);

}