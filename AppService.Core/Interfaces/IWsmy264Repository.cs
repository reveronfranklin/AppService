using System.Threading.Tasks;
using AppService.Core.EntitiesClientes;

namespace AppService.Core.Interfaces;

public interface IWsmy264Repository
{
    Task<Wsmy264> GetById(decimal id);
    Task Add(Wsmy264 entity);
    void Update(Wsmy264 entity);
   void UpdateQuery(Wsmy264 entity);
    Task Delete(decimal id);
    Task<decimal> NextId(decimal codigo);
    void AddQuery(Wsmy264 entity);
    Task<Wsmy264> GetByCodigoIdDireccion(decimal id, decimal codigo);
}