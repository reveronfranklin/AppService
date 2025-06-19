using AppService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IAppDetailQuotesRepository
    {

        void UpdatePrecios(int appDetailsId, decimal precioMinimo, decimal precioMaximo, int calculoId, int solicitarPrecio);
       
        void UpdateCondicionPago(int appGeneralQuotesId, short condicionPago);
        Task UpdatePrecioMinimo(int appDetailQuotesId, decimal precioMinimo, decimal precioMaximo, int calculoId);
        Task<List<AppDetailQuotes>> GetAll();

        Task<List<AppDetailQuotes>> GetByAppGeneralQuotesId(int appGeneralQuotesId);

        Task<AppDetailQuotes> GetById(int id);

        Task Add(AppDetailQuotes entity);

        void Update(AppDetailQuotes entity);
        Task Delete(int id);
        Task<AppDetailQuotes> GetByQuetesProduct(string cotizacion, int idProduct);


        Task<bool> ExisteEnEspera(int appGeneralQuotesId);


        Task<bool> ProductExistInQuotesDetail(int idProduct);

        Task<List<AppDetailQuotes>> GeSimpletByAppGeneralQuotesId(int appGeneralQuotesId);
        Task<List<AppDetailQuotes>> GetByQuotesCotizacion(string cotizacion);
        Task<AppDetailQuotes> GetByGeneralQuotesExternalcode(int idGeneralQuotes, string externalCode);
        Task<List<AppDetailQuotes>> GetBySubCategopry(int subCategory);
        Task UpdateFlete(int appDetailQuotesId, decimal porcFlete, decimal flete);
        Task<List<AppDetailQuotes>> GetByYearMonth(int year, int month);
    }
}
