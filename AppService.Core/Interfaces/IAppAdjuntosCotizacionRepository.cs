using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces;

public interface IAppAdjuntosCotizacionRepository
{
    Task<List<AppAdjuntosCotizacion>> GetByCotizacion(string cotizacion, int renglon);

}