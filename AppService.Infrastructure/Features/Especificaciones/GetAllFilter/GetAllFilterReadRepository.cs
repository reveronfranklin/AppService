using AppService.Core.Entities;
using AppService.Core.EntitiesMateriales;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Features.Especificaciones.GetAllFilter;
using AppService.Infrastructure.Data;
using AppService.Infrastructure.DataMateriales;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Features.Especificaciones.GetAllFilter
{
    public class GetAllFilterReadRepository : IGetAllFilterReadRepository
    {
        private readonly RRDContext _rrdContext;
        private readonly MooreveContext _mooreveContext;
        private readonly MaterialesContext _materialesContext;

        public GetAllFilterReadRepository(
            RRDContext rrdContext,
            MooreveContext mooreveContext,
            MaterialesContext materialesContext)
        {
            _rrdContext = rrdContext;
            _mooreveContext = mooreveContext;
            _materialesContext = materialesContext;
        }

        public async Task<AppDetailQuotes> GetAppDetailQuoteById(int idAppDetailQuote)
        {
            return await _rrdContext.AppDetailQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idAppDetailQuote);
        }

        public async Task<AppProducts> GetProductById(int idProducto)
        {
            return await _rrdContext.AppProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idProducto);
        }

        public async Task<Wsmy406> GetAplicacionByCodAplicacion(int codAplicacion)
        {
            return await _mooreveContext.Wsmy406
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CodAplicacion == codAplicacion);
        }

        public async Task<List<Wpry240>> GetPartes(string cotizacion, int renglon, int propuesta)
        {
            return await _mooreveContext.Wpry240
                .AsNoTracking()
                .Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta)
                .OrderBy(x => x.IdParte)
                .ToListAsync();
        }

        public async Task<List<Wpry241>> GetTintasByPartes(string cotizacion, int renglon, int propuesta, List<int> partesIds)
        {
            if (partesIds == null || partesIds.Count == 0)
            {
                return new List<Wpry241>();
            }

            return await _mooreveContext.Wpry241
                .AsNoTracking()
                .Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta && partesIds.Contains(x.IdParte))
                .ToListAsync();
        }

        public async Task<List<Csmy011>> GetTintasActivas()
        {
            return await _mooreveContext.Csmy011
                .AsNoTracking()
                .Where(x => x.FlagEliminada != "X")
                .OrderByDescending(x => x.Frecuencia)
                .ToListAsync();
        }

        public async Task<List<AppVariablesEspecificacionesPartes>> GetVariablesPartes(int codAplicacion)
        {
            return await _mooreveContext.AppVariablesEspecificacionesPartes
                .AsNoTracking()
                .Where(x => x.CodAplicacion == codAplicacion && x.FlagGralParte == "")
                .OrderBy(x => x.Orden)
                .ToListAsync();
        }

        public async Task<List<AppVariablesEspecificacionesPartes>> GetVariablesGenerales(int codAplicacion)
        {
            return await _mooreveContext.AppVariablesEspecificacionesPartes
                .AsNoTracking()
                .Where(x => x.CodAplicacion == codAplicacion && x.FlagGralParte != "")
                .OrderBy(x => x.Orden)
                .ToListAsync();
        }

        public async Task<List<AppValoresVariablesEspecificacionesPartes>> GetValoresByVariables(List<string> idVariables)
        {
            if (idVariables == null || idVariables.Count == 0)
            {
                return new List<AppValoresVariablesEspecificacionesPartes>();
            }

            return await _mooreveContext.AppValoresVariablesEspecificacionesPartes
                .AsNoTracking()
                .Where(x => idVariables.Contains(x.IdVariable))
                .ToListAsync();
        }

        public async Task<List<Wpry251>> GetWpry251ByScope(string cotizacion, int renglon, int propuesta, List<int> partesIds, List<string> idVariables)
        {
            if (partesIds == null || partesIds.Count == 0 || idVariables == null || idVariables.Count == 0)
            {
                return new List<Wpry251>();
            }

            return await _mooreveContext.Wpry251
                .AsNoTracking()
                .Where(x =>
                    x.Cotizacion == cotizacion &&
                    x.Renglon == renglon &&
                    x.Propuesta == propuesta &&
                    partesIds.Contains(x.IdParte) &&
                    idVariables.Contains(x.IdVariable))
                .ToListAsync();
        }

        public async Task<List<Wimy001>> GetPapelesByTipoGramaje(List<TipoPapelGramajeKey> keys)
        {
            if (keys == null || keys.Count == 0)
            {
                return new List<Wimy001>();
            }

            var tipoSet = keys.Select(x => x.TipoPapel).Distinct().ToList();
            var gramajeSet = keys.Select(x => x.Gramaje).Distinct().ToList();
            var exactSet = keys.Select(x => $"{x.TipoPapel}|{x.Gramaje}").ToHashSet();

            var candidates = await _materialesContext.Wimy001s
                .AsNoTracking()
                .Where(x => tipoSet.Contains(x.TipoPapel) && gramajeSet.Contains(x.Gramaje))
                .ToListAsync();

            return candidates
                .Where(x => exactSet.Contains($"{(x.TipoPapel ?? string.Empty).Trim()}|{(x.Gramaje ?? string.Empty).Trim()}"))
                .ToList();
        }

        public async Task<MedidasLookup> GetMedidas(int idProducto)
        {
            var medidas = await _rrdContext.AppRecipes
                .AsNoTracking()
                .Where(x => x.AppproductsId == idProducto && (x.Code.StartsWith("MEDIDABASICA") || x.Code.StartsWith("MEDIDAOPUESTA")))
                .Select(x => new { x.Code, x.DescriptionSearch })
                .ToListAsync();

            var result = new MedidasLookup();
            var basica = medidas.FirstOrDefault(x => x.Code =="MEDIDABASICA");
            var opuesta = medidas.FirstOrDefault(x => x.Code=="MEDIDAOPUESTA");

            result.MedidaBasica = basica != null ? basica.DescriptionSearch : null;
            result.MedidaOpuesta = opuesta != null ? opuesta.DescriptionSearch : null;

            return result;
        }

        public async Task<Wpry229> GetTipoOrden(string cotizacion, int renglon, int propuesta)
        {
            return await _mooreveContext.Wpry229
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta);
        }
    }
}
