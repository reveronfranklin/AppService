using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.PowerBi;
using AppService.Core.EntitiesPowerBI;

namespace AppService.Core.Interfaces.PowerBi;

public interface IPowerBiOrdenesService
{
    Task<List<Ordene>> GetAll(PowerBiOrdenesFilter filter);
    Task<List<VVenta>> GetAllVentas();
    Task<List<AppService.Core.EntitiesPowerBI.Ventas>> GetAllVentasReducido();
}