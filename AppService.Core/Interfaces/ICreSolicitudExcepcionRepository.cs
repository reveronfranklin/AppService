using System.Threading.Tasks;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces;

public interface ICreSolicitudExcepcionRepository
{

    Task<CreSolicitudExcepcion> GetById(decimal solicitudCredito);
}