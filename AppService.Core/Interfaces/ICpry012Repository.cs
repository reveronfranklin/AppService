using AppService.Core.EntitiesPlanta;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface ICpry012Repository
    {
        Task<Cpry012> GetByOrdenAsync(long orden);
        void Update(Core.EntitiesPlanta.Cpry012 entity);
        void UpdateProductoEnOrden(Core.EntitiesPlanta.Cpry012 entity);
    }
}
