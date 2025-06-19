using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesPowerBI;

namespace AppService.Core.Interfaces.PowerBi;

public interface IPowerBiOrdenesRepository
{
    Task<List<Ordene>> GetAll(string usuario, bool esVendedor, int year);
    Task<List<VVenta>> GetAllVentas(decimal year);
    Task<List<AppService.Core.EntitiesPowerBI.Ventas>> GetAllVentasReducido(decimal year);
}