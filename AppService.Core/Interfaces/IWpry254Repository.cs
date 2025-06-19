using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces;

public interface IWpry254Repository
{
    Task<Wpry254> GetByCotizacionNombreArchivo(string cotizacion, string nombreArchivo);
    Task Add(Wpry254 entity);
    Task DeleteByFileName(string fileName);
}