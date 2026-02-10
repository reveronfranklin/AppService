using AppService.Core.EntitiesMooreve;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IAprobacionesRepository
    {

        Task<List<Wsmy639>> GetAll();

        Task<Wsmy639> GetById(long id);


        Task<Wsmy639> GetByCotizacionRenglonPropuesta(string cotizacion, int renglon, int propuesta);

        Task Add(Wsmy639 entity);

        void Update(Wsmy639 entity);

        Task Delete(long id);
         Task DeleteWorkFlow(long id);
        Task<Wsmy639> GetByCotizacionProducto(string cotizacion, string producto);

       

        Task<Wsmy639> CreaAprobacion(string cotizacion, int renglon, int propuesta, string usuarioConectado,
            string mensajeSolicitarPrecio);
        Task<Wsmy639> CreaAprobacionAprobada(string cotizacion, int renglon, int propuesta, string usuarioConectado);

    }
}
