using AppService.Core.Entities;
using AppService.Core.EntitiesMateriales;
using AppService.Core.EntitiesMooreve;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Features.Especificaciones.GetAllFilter
{
    public interface IGetAllFilterReadRepository
    {
        Task<AppDetailQuotes> GetAppDetailQuoteById(int idAppDetailQuote);
        Task<AppProducts> GetProductById(int idProducto);
        Task<Wsmy406> GetAplicacionByCodAplicacion(int codAplicacion);

        Task<List<Wpry240>> GetPartes(string cotizacion, int renglon, int propuesta);
        Task<List<Wpry241>> GetTintasByPartes(string cotizacion, int renglon, int propuesta, List<int> partesIds);
        Task<List<Csmy011>> GetTintasActivas();

        Task<List<AppVariablesEspecificacionesPartes>> GetVariablesPartes(int codAplicacion);
        Task<List<AppVariablesEspecificacionesPartes>> GetVariablesGenerales(int codAplicacion);
        Task<List<AppValoresVariablesEspecificacionesPartes>> GetValoresByVariables(List<string> idVariables);

        Task<List<Wpry251>> GetWpry251ByScope(string cotizacion, int renglon, int propuesta, List<int> partesIds, List<string> idVariables);
        Task<List<Wimy001>> GetPapelesByTipoGramaje(List<TipoPapelGramajeKey> keys);

        Task<MedidasLookup> GetMedidas(int idProducto);
        Task<Wpry229> GetTipoOrden(string cotizacion, int renglon, int propuesta);
    }
}
