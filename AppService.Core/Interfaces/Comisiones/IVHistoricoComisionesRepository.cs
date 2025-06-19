using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.Entities;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IVHistoricoComisionesRepository
{
    Task<List<VHistoricoComsiones>> GetByPeriodo(int idPeriodo, string update);
    Task<List<VHistoricoComsiones>> GetBySearchText(PeriodoFilter filter, List<MtrVendedor> vendedores, string update);

    Task<List<VHistoricoComsiones>> GetByPeriodoSearchText(PeriodoFilter filter, List<MtrVendedor> vendedores,
        string update);
}