using AppService.Core.Entities;
using AppService.Core.QueryFilters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IAppGeneralQuotesRepository
    {
        Task<List<AppGeneralQuotes>> GetListCotizacionesUltimoMes();
        Task<List<AppGeneralQuotes>> GetAll(AppGeneralQuotesQueryFilter filter);

        Task<AppGeneralQuotes> GetById(int id);

        Task<bool> AppDeleteSolcitudCreditoCotizacion
            (string cotizacion);
        Task<AppGeneralQuotes> GetByIdForUpdate(int id);

        Task<AppGeneralQuotes> GetByCotizacions(string cotizacion);

        Task Add(AppGeneralQuotes entity);

        void Update(AppGeneralQuotes entity);

        Task Delete(int id);

        string ProximaCotizacion(string Cod_Vendedor);

        Task<bool> PermiteAdicionarDetalle(int idGeneralQuote);

        void MarcarIntegrado(bool marca, int id);
        Task<int> VerificarStatus(int idGeneralQuote);
        Task<List<AppGeneralQuotes>> GetByCotizacionesPendientesIntegrar();

        Task<List<string>> GetListCotizaciones();

    }
}
