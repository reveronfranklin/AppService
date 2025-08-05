using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcTipoPagoRepository
{
    Task<List<PCTipoPago>> GetAll();
    Task<List<PCTipoPago>> GetAllManuales();
    Task<PCTipoPago> GetById(int id);
    
}