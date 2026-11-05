using AppService.Core.CustomEntities;
using AppService.Core.DTOs.Especificaciones;
using AppService.Core.EntitiesMateriales;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Core.Features.Especificaciones.GetAllFilter
{
    public class GetAllFilterHandler
    {
        private readonly IGetAllFilterReadRepository _readRepository;

        public GetAllFilterHandler(IGetAllFilterReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<ApiResponse<EspecificacionesGetDto>> Handle(GetAllFilterQuery query)
        {
            EspecificacionesGetDto resultDto = new EspecificacionesGetDto();
            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""
            };

            ApiResponse<EspecificacionesGetDto> response = new ApiResponse<EspecificacionesGetDto>(resultDto);

            try
            {
                var filter = query.Filter;
                var appDetailQuote = await _readRepository.GetAppDetailQuoteById(filter.IdAppDetailQuote);
                if (appDetailQuote == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No existe la cotizacion";
                    response.Data = null;
                    response.Meta = metadata;
                    return response;
                }

                var partes = await _readRepository.GetPartes(filter.Cotizacion, filter.Renglon, filter.Propuesta) ?? new List<Wpry240>();
                var tintasValidas = await _readRepository.GetTintasActivas() ?? new List<Csmy011>();
                var wpry229 = await _readRepository.GetTipoOrden(filter.Cotizacion, filter.Renglon, filter.Propuesta);

                var appProduct = await _readRepository.GetProductById(appDetailQuote.IdProducto);
                if (appProduct == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No existe el producto asociado a la cotizacion";
                    response.Data = null;
                    response.Meta = metadata;
                    return response;
                }

                var aplicacionProducto = await _readRepository.GetAplicacionByCodAplicacion(appProduct.CodAplicacion);
                if (aplicacionProducto == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No existe la aplicacion para el producto";
                    response.Data = null;
                    response.Meta = metadata;
                    return response;
                }

                MedidasLookup medidasLookup = null;
                if (!(appDetailQuote.MedidaBasica > 0))
                {
                    medidasLookup = await _readRepository.GetMedidas(filter.IdProducto);
                }

                var variablesPartes = await _readRepository.GetVariablesPartes(aplicacionProducto.CodAplicacion) ?? new List<AppVariablesEspecificacionesPartes>();
                var variablesGenerales = await _readRepository.GetVariablesGenerales(aplicacionProducto.CodAplicacion) ?? new List<AppVariablesEspecificacionesPartes>();

                var partesIds = partes.Select(x => x.IdParte).Distinct().ToList();
                var tintasParte = await _readRepository.GetTintasByPartes(filter.Cotizacion, filter.Renglon, filter.Propuesta, partesIds) ?? new List<Wpry241>();

                var variablesIds = variablesPartes.Select(x => (x.IdVariable ?? string.Empty).Trim())
                    .Concat(variablesGenerales.Select(x => (x.IdVariable ?? string.Empty).Trim()))
                    .Where(x => x.Length > 0)
                    .Distinct()
                    .ToList();

                var valores = await _readRepository.GetValoresByVariables(variablesIds) ?? new List<AppValoresVariablesEspecificacionesPartes>();
                var wpry251Partes = await _readRepository.GetWpry251ByScope(filter.Cotizacion, filter.Renglon, filter.Propuesta, partesIds, variablesIds) ?? new List<Wpry251>();

                // Se mantiene la logica original para generales (renglon/propuesta = 1/1).
                var wpry251Generales = await _readRepository.GetWpry251ByScope(filter.Cotizacion, 1, 1, new List<int> { 0 }, variablesIds) ?? new List<Wpry251>();

                var papelesKeys = partes
                    .Select(x => new TipoPapelGramajeKey
                    {
                        TipoPapel = (x.TipoPapel ?? string.Empty).Trim(),
                        Gramaje = (x.Gramaje ?? string.Empty).Trim()
                    })
                    .Where(x => x.TipoPapel.Length > 0 && x.Gramaje.Length > 0)
                    .GroupBy(x => $"{x.TipoPapel}|{x.Gramaje}")
                    .Select(x => x.First())
                    .ToList();

                var papeles = await _readRepository.GetPapelesByTipoGramaje(papelesKeys) ?? new List<Wimy001>();

                var valoresByVariable = valores
                    .GroupBy(x => (x.IdVariable ?? string.Empty).Trim())
                    .ToDictionary(x => x.Key, x => x.ToList());

                var papelesByKey = papeles
                    .GroupBy(x => $"{(x.TipoPapel ?? string.Empty).Trim()}|{(x.Gramaje ?? string.Empty).Trim()}")
                    .ToDictionary(x => x.Key, x => x.ToList());

                var tintasByParte = tintasParte
                    .GroupBy(x => x.IdParte)
                    .ToDictionary(x => x.Key, x => x.ToList());

                var selectedPartes = new Wpry251SelectionLookup();
                foreach (var item in wpry251Partes)
                {
                    selectedPartes.Keys.Add(Wpry251SelectionLookup.BuildKey(item.IdParte, item.IdVariable, item.Valor));
                }

                var selectedGenerales = new Wpry251SelectionLookup();
                foreach (var item in wpry251Generales)
                {
                    selectedGenerales.Keys.Add(Wpry251SelectionLookup.BuildKey(item.IdParte, item.IdVariable, item.Valor));
                }

                List<PartesGetDto> resultPartesDto = new List<PartesGetDto>();
                foreach (var item in partes)
                {
                    PartesGetDto itemPartesGetDto = new PartesGetDto
                    {
                        Cotizacion = item.Cotizacion,
                        Renglon = item.Renglon,
                        Propuesta = item.Propuesta,
                        IdParte = item.IdParte,
                        IdPapel = item.IdPapel,
                        FrasesMarginales = item.FrasesMarginales,
                        TipoPapel = item.TipoPapel,
                        Gramaje = item.Gramaje,
                        TintasFrente = string.Empty,
                        TintasRespaldo = string.Empty
                    };

                    if (appDetailQuote.MedidaBasica > 0)
                    {
                        itemPartesGetDto.MedidaBasica = appDetailQuote.MedidaBasica.ToString();
                        itemPartesGetDto.MedidaOpuesta = appDetailQuote.MedidaOpuesta.ToString();
                    }
                    else
                    {
                        itemPartesGetDto.MedidaBasica = medidasLookup != null ? medidasLookup.MedidaBasica : null;
                        itemPartesGetDto.MedidaOpuesta = medidasLookup != null ? medidasLookup.MedidaOpuesta : null;
                    }

                    List<TintasGetDto> listTintasGetDto = new List<TintasGetDto>();
                    if (tintasByParte.TryGetValue(item.IdParte, out var tintasItem))
                    {
                        foreach (var itemTintas in tintasItem)
                        {
                            listTintasGetDto.Add(new TintasGetDto
                            {
                                Cotizacion = item.Cotizacion,
                                Renglon = item.Renglon,
                                Propuesta = item.Propuesta,
                                IdParte = item.IdParte,
                                IdUbicacion = itemTintas.IdUbicacion,
                                IdTinta = itemTintas.IdTinta
                            });
                        }

                        itemPartesGetDto.TintasFrente = string.Join(";", tintasItem.Where(x => x.IdUbicacion == 1).Select(x => x.IdTinta));
                        itemPartesGetDto.TintasRespaldo = string.Join(";", tintasItem.Where(x => x.IdUbicacion != 1).Select(x => x.IdTinta));
                    }

                    var papelKey = $"{(item.TipoPapel ?? string.Empty).Trim()}|{(item.Gramaje ?? string.Empty).Trim()}";
                    List<PapelesTipoGramaje> papelesValidos = new List<PapelesTipoGramaje>();
                    if (papelesByKey.TryGetValue(papelKey, out var papelesItem))
                    {
                        foreach (var papel in papelesItem)
                        {
                            papelesValidos.Add(new PapelesTipoGramaje
                            {
                                IdPapel = papel.Codigo,
                                TipoPapel = (item.TipoPapel ?? string.Empty).Trim(),
                                Gramaje = (item.Gramaje ?? string.Empty).Trim()
                            });
                        }
                    }

                    List<AppVariablesEspecificacionesPartesGetDto> variablesPartesDto = new List<AppVariablesEspecificacionesPartesGetDto>();
                    foreach (var variable in variablesPartes)
                    {
                        List<AppValoresVariablesEspecificacionesPartesGetDto> valoresDto = new List<AppValoresVariablesEspecificacionesPartesGetDto>();
                        var idVariable = (variable.IdVariable ?? string.Empty).Trim();
                        if (valoresByVariable.TryGetValue(idVariable, out var valoresVariable))
                        {
                            foreach (var valor in valoresVariable)
                            {
                                var selectedKey = Wpry251SelectionLookup.BuildKey(item.IdParte, idVariable, valor.Valor);
                                var isChecked = selectedPartes.Keys.Contains(selectedKey);
                                valoresDto.Add(new AppValoresVariablesEspecificacionesPartesGetDto
                                {
                                    Id = valor.Id,
                                    IdVariable = valor.IdVariable,
                                    Valor = valor.Valor,
                                    FlagMultipleValor = valor.FlagMultipleValor,
                                    ValorReal = isChecked ? valor.Valor : string.Empty,
                                    Cheked = isChecked
                                });
                            }
                        }

                        variablesPartesDto.Add(new AppVariablesEspecificacionesPartesGetDto
                        {
                            Id = variable.Id,
                            CodAplicacion = variable.CodAplicacion,
                            IdVariable = variable.IdVariable,
                            NombreVariable = variable.NombreVariable,
                            FlagObligatorio = variable.FlagObligatorio,
                            FlagGralParte = variable.FlagGralParte,
                            Orden = variable.Orden,
                            AppValoresVariablesEspecificacionesPartesGetDto = valoresDto
                        });
                    }

                    itemPartesGetDto.ListTintasGetDto = listTintasGetDto;
                    itemPartesGetDto.PapelesValidos = papelesValidos;
                    itemPartesGetDto.AppVariablesEspecificacionesPartesGetDto = variablesPartesDto;
                    resultPartesDto.Add(itemPartesGetDto);
                }

                List<TintasValidasGetDto> listTintasValidasGetDto = tintasValidas
                    .Select(x => new TintasValidasGetDto
                    {
                        Codigo = (x.Codigo ?? string.Empty).Trim(),
                        FlagNoContar = x.FlagNoContar == "X"
                    })
                    .ToList();

                List<AppVariablesEspecificacionesGeneralGetDto> generalesDto = new List<AppVariablesEspecificacionesGeneralGetDto>();
                foreach (var variable in variablesGenerales)
                {
                    List<AppValoresVariablesEspecificacionesGeneralGetDto> valoresDto = new List<AppValoresVariablesEspecificacionesGeneralGetDto>();
                    var idVariable = (variable.IdVariable ?? string.Empty).Trim();
                    if (valoresByVariable.TryGetValue(idVariable, out var valoresVariable))
                    {
                        foreach (var valor in valoresVariable)
                        {
                            var selectedKey = Wpry251SelectionLookup.BuildKey(0, idVariable, valor.Valor);
                            var isChecked = selectedGenerales.Keys.Contains(selectedKey);
                            valoresDto.Add(new AppValoresVariablesEspecificacionesGeneralGetDto
                            {
                                Id = valor.Id,
                                IdVariable = valor.IdVariable,
                                Valor = valor.Valor,
                                FlagMultipleValor = valor.FlagMultipleValor,
                                ValorReal = isChecked ? valor.Valor : string.Empty,
                                Cheked = isChecked
                            });
                        }
                    }

                    generalesDto.Add(new AppVariablesEspecificacionesGeneralGetDto
                    {
                        Id = variable.Id,
                        CodAplicacion = variable.CodAplicacion,
                        IdVariable = variable.IdVariable,
                        NombreVariable = variable.NombreVariable,
                        FlagObligatorio = variable.FlagObligatorio,
                        FlagGralParte = variable.FlagGralParte,
                        Orden = variable.Orden,
                        AppValoresVariablesEspecificacionesGeneralGetDto = valoresDto
                    });
                }

                resultDto.IdTipoOrden = wpry229?.TipoOrden != null ? (short)wpry229.TipoOrden : (short)0;
                resultDto.ListPartesGetDto = resultPartesDto;
                resultDto.ListTintasValidasGetDto = listTintasValidasGetDto;
                resultDto.AppVariablesEspecificacionesGeneralGetDto = generalesDto;

                response.Data = resultDto;
                response.Meta = metadata;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.Message;
                response.Data = null;
                response.Meta = metadata;
                return response;
            }
        }
    }
}
