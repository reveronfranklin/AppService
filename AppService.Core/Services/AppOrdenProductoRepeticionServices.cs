using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Repeticiones;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.Map;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AppService.Core.Services
{
    public class AppOrdenProductoRepeticionServices : IAppOrdenProductoRepeticionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;
        private readonly IAppProductsService _appProductsService;
        private readonly IAppUnitsService _appUnitsService;
        private readonly IAppCache _cache;
        public AppOrdenProductoRepeticionServices(IUnitOfWork unitOfWork,
                                                 IMapper mapper,
                                                 IOptions<PaginationOptions> options,
                                                  IAppProductsService appProductsService,
                                                     IAppUnitsService appUnitsService,
                                                 IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._paginationOptions = options.Value;

            this._appProductsService = appProductsService;
            this._appUnitsService = appUnitsService;
            _cache = cache;
        }

        public async Task<AppOrdenProductoRepeticion> GetByOrden(long orden)
        {

            var repeticion = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetByOrden(orden);

            // AppConfigAppGetDto appConfigDto = _mapper.Map<AppConfigAppGetDto>(appConfig);

            return repeticion;
        }
        public async Task<AppOrdenProductoRepeticion> Update(AppOrdenProductoRepeticion entity)
        {

            var repeticionFind = await GetByOrden(entity.Orden);
            if (repeticionFind == null)
            {
                throw new Exception("Documento No existe");
            }
            repeticionFind.AppproductsId = entity.AppproductsId;
            _unitOfWork.AppOrdenProductoRepeticionRepository.Update(repeticionFind);
            await _unitOfWork.SaveChangesAsync();

            return await GetByOrden(repeticionFind.Orden);


        }
        
        public async Task SetCacheAsync(string key, string value)
        {
            await _cache.SetAsync(key, value,TimeSpan.FromHours(2));
        }
        public void RemoveCache(string key)
        {
            _cache.Remove(key);
        }
        public async Task<string> GetCacheAsync(string key)
        {
            return await _cache.GetAsync(key);
        }

        public async Task<ApiResponse<ListaRepeticiones>> GetAllFilter(AppOrdenProductoRepeticionFilterDto filter)
        {
            ListaRepeticiones resultDto = new ListaRepeticiones();
            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };

            ApiResponse<ListaRepeticiones> response = new ApiResponse<ListaRepeticiones>(resultDto);

            /*var listRepeticiones=await GetCacheAsync($"ListRepeticiones{filter.IdCliente}");
            if (listRepeticiones != null)
            {
                response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<ListaRepeticiones>> (listRepeticiones);
                return response;
            }*/
           
            
            
            var appOrdenProductoRepeticion = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetByCliente(filter.IdCliente);
            if (appOrdenProductoRepeticion.Count > 0)
            {
                List<AppOrdenProductoRepeticionGetDto> listOrdenProducto = new List<AppOrdenProductoRepeticionGetDto>();
                //foreach (var item in appOrdenProductoRepeticion.Where(x => x.AppproductsId > 0).ToList())
                foreach (var item in appOrdenProductoRepeticion)
                {
                    AppOrdenProductoRepeticionGetDto itemOrdenProducto = new AppOrdenProductoRepeticionGetDto();
                    itemOrdenProducto.Id = item.Id;
                    itemOrdenProducto.NombreCliente = item.NombreCliente.Trim();
                    itemOrdenProducto.IdCliente = item.IdCliente;
                    itemOrdenProducto.Orden = item.Orden.ToString();
                    itemOrdenProducto.Fecha = item.Fecha;
                    itemOrdenProducto.FechaString = FechaString(item.Fecha);
                    itemOrdenProducto.AppproductsId = item.AppproductsId;
                    itemOrdenProducto.AppproductsDecription = item.AppproductsDecription.Trim();
                    itemOrdenProducto.CodProducto = item.COD_PRODUCTO.Trim();
                    itemOrdenProducto.NombreProducto = item.NombreProducto.Trim();
                    itemOrdenProducto.NombreForma = item.NombreForma.Trim();
                    itemOrdenProducto.CantTintas = item.Cant_Tintas;
                    itemOrdenProducto.PartesFormula = item.PARTES_FORMULA;
                    itemOrdenProducto.MedidaBase = item.MEDIDA_BASE;
                    itemOrdenProducto.MedidaVariable = item.MEDIDA_VARIABLE;
                    itemOrdenProducto.MedidaBaseDecimal = item.MEDIDA_BASE_DECIMAL;
                    itemOrdenProducto.MedidaVariableDecimal = item.MEDIDA_VARIABLE_DECIMAL;
                    itemOrdenProducto.BasicaHumano = item.BasicaHumano;
                    itemOrdenProducto.OpuestaHumano = item.OpuestaHumano;
                    itemOrdenProducto.PapelPrimeraParte = item.PAPELPRIMERAPARTE;
                    itemOrdenProducto.PapelSegundaParte = item.PAPELSEGUNDAPARTE;
                    itemOrdenProducto.PapelTerceraParte = item.PAPELTERCERAPARTE;
                    itemOrdenProducto.PapelCuartaParte = item.PAPELCUARTAPARTE;
                    itemOrdenProducto.PapelQuintaParte = item.PAPELQUINTAPARTE;

                    itemOrdenProducto.Forma = "";
                    itemOrdenProducto.Salida= "";
                    itemOrdenProducto.Presentacion= "";
                    
                    var orden = await _unitOfWork.Cpry012Repository.GetByOrdenAsync(item.Orden);
                    if(orden != null)
                    {
                        
                        
                        var cotizacion = orden.Cotizacion.Trim().Substring(0, 13);
                        var appDetailQuote = await _unitOfWork.AppDetailQuotesRepository.GetByQuetesProduct(cotizacion, item.AppproductsId);
                        if (appDetailQuote != null)
                        {
                            itemOrdenProducto.Forma =appDetailQuote.Forma;
                            itemOrdenProducto.Salida= appDetailQuote.Salida;
                            itemOrdenProducto.Presentacion= appDetailQuote.Presentacion;
                        }
                        
                       
                        if (orden.NuTintasResp == null) orden.NuTintasResp = 0;
                        if (orden.NuTintasFrent == null) orden.NuTintasFrent = 0;
                        var cantidadTintas = orden.NuTintasFrent + orden.NuTintasResp;
                  

                    }


                    if (item.CANT_ORDENADA == null)
                    {
                        item.CANT_ORDENADA = 0;
                    }
                    itemOrdenProducto.CantidadOrdenada = item.CANT_ORDENADA;
                    itemOrdenProducto.Millares = item.CANT_ORDENADA / 1000;
                    if (item.PrecioUnitarioUsd == null)
                    {
                        item.PrecioUnitarioUsd = 0;
                    }
                    itemOrdenProducto.PrecioUnitarioUsd = item.PrecioUnitarioUsd;
                    if (item.TotalPropuestaUsd == null)
                    {
                        item.TotalPropuestaUsd = 0;
                    }
                    itemOrdenProducto.TotalPropuestaUsd = item.TotalPropuestaUsd;
                    itemOrdenProducto.MedidaBasicaCm = item.MedidaBasicaCm;
                    itemOrdenProducto.MedidaOpuestaCm = item.MedidaOpuestaCm;


                 


                    AppProducts appProductsFind = await this._appProductsService.GetById(item.AppproductsId);
                    if (appProductsFind != null)
                    {
                        AppProductConversion unitAlternativeUnit = await this._unitOfWork.AppProductConversionRepository.GetByProductBaseUnitAlternativeUnit(item.AppproductsId, appProductsFind.ProductionUnitId.Value, (int)appProductsFind.AppUnitsId);
                        if (unitAlternativeUnit != null)
                            itemOrdenProducto.AppProductConversionGetDto = MapAppProductConversion.MapAppProductConversionToAppProductConversionGetDto(unitAlternativeUnit);

                        AppProductsGetDto appProductsGetDto = this._mapper.Map<AppProductsGetDto>((object)appProductsFind);

                        AppUnits byId = await this._appUnitsService.GetById(appProductsFind.ProductionUnitId.Value);
                        if (byId != null)
                            appProductsGetDto.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId);
                        appProductsGetDto.Link = appProductsFind.UrlImage == "" || appProductsFind.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + appProductsFind.UrlImage;
                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(item.AppproductsId);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            List<AppPriceDto> appPriceDtoList = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                            appProductsGetDto.PrecioPorRango = true;
                            appProductsGetDto.AppPriceDto = appPriceDtoList;
                        }
                        itemOrdenProducto.AppProductsGetDto = appProductsGetDto;

                    }

                    listOrdenProducto.Add(itemOrdenProducto);

                }
                resultDto.AppOrdenProductoRepeticionGetDto = listOrdenProducto.OrderByDescending(x => x.Fecha).ToList();

                var listAppRepeticionClienteProducto = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClienteProductoByCliente(filter.IdCliente);
                var lisAppRepeticionClienteNombreForma = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClienteNombreFormaByCliente(filter.IdCliente);
                var listAppRepeticionClienteBasica = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClienteBasicaByCliente(filter.IdCliente);
                var listAppRepeticionClienteOpuesta = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClienteOpuestaByCliente(filter.IdCliente);
                var listAppRepeticionClientePartes = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClientePartesByCliente(filter.IdCliente);
                var listAppRepeticionClienteTintas = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClienteTintasByCliente(filter.IdCliente);
                var listAppRepeticionClientePapelPrimeraParte = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClientePapelPrimeraParteByCliente(filter.IdCliente);
                var listAppRepeticionClientePapelSegundaParte = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClientePapelSegundaParteByCliente(filter.IdCliente);
                var listAppRepeticionClientePapelTerceraParte = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClientePapelTerceraParteByCliente(filter.IdCliente);
                var listAppRepeticionClientePapelCuartaParte = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClientePapelCuartaParteByCliente(filter.IdCliente);
                var listAppRepeticionClientePapelQuintaParte = await _unitOfWork.AppOrdenProductoRepeticionRepository.GetAppRepeticionClientePapelQuintaParteByCliente(filter.IdCliente);

                resultDto.AppRepeticionClienteProducto = listAppRepeticionClienteProducto;
                resultDto.AppRepeticionClienteNombreForma = lisAppRepeticionClienteNombreForma;
                resultDto.AppRepeticionClienteBasica = listAppRepeticionClienteBasica;
                resultDto.AppRepeticionClienteOpuesta = listAppRepeticionClienteOpuesta;

                resultDto.AppRepeticionClientePartes = listAppRepeticionClientePartes;
                resultDto.AppRepeticionClienteTintas = listAppRepeticionClienteTintas;
                resultDto.AppRepeticionClientePapelPrimeraParte = listAppRepeticionClientePapelPrimeraParte;
                resultDto.AppRepeticionClientePapelSegundaParte = listAppRepeticionClientePapelSegundaParte;
                resultDto.AppRepeticionClientePapelTerceraParte = listAppRepeticionClientePapelTerceraParte;
                resultDto.AppRepeticionClientePapelCuartaParte = listAppRepeticionClientePapelCuartaParte;
                resultDto.AppRepeticionClientePapelQuintaParte = listAppRepeticionClientePapelQuintaParte;

            }

            response.Data = resultDto;
            metadata.IsValid = true;
            metadata.Message = "";
            response.Meta = metadata;
            await SetCacheAsync($"ListRepeticiones{filter.IdCliente}", System.Text.Json.JsonSerializer.Serialize(response));
            return response;


        }
        public async Task<ApiResponse<bool>> PuedeModificarProducto(PuedeModificarProductoOrdenFilterDto filter)
        {
            var response = new ApiResponse<bool>(false);
            var metadata = new Metadata
            {
                IsValid = false,
                Message = string.Empty
            };

            var usuarioConectado = filter?.UsuarioConectado?.Trim();
            if (string.IsNullOrWhiteSpace(usuarioConectado))
            {
                metadata.Message = "Usuario conectado requerido";
                response.Meta = metadata;
                return response;
            }

            var permission = await _unitOfWork.AppOrdenProductoRepeticionRepository
                .PuedeModificarProductoAsync(usuarioConectado);

            response.Data = permission.PuedeModificar;
            metadata.IsValid = string.Equals(permission.Message, "Success", StringComparison.OrdinalIgnoreCase);
            metadata.Message = metadata.IsValid ? string.Empty : permission.Message;

            response.Meta = metadata;
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateProductoOrden(UpdateProductoOrdenFilterDto filter)
        {
            var permissionResponse = await PuedeModificarProducto(new PuedeModificarProductoOrdenFilterDto
            {
                UsuarioConectado = filter?.UsuarioConectado
            });
            if (!permissionResponse.Meta.IsValid)
            {
                return permissionResponse;
            }
            if (!permissionResponse.Data)
            {
                permissionResponse.Meta.IsValid = false;
                permissionResponse.Meta.Message =
                    "Los vendedores no estan autorizados para modificar el producto de una orden";
                return permissionResponse;
            }

            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };
           
            ApiResponse<bool> response = new ApiResponse<bool>(false);
            if (filter.Orden <= 0)
            {
                metadata.IsValid = false;
                metadata.Message = "Orden requerida";
                response.Meta = metadata;
                return response;
            }

            if (filter.IdProducto <= 0 || await _appProductsService.GetById(filter.IdProducto) == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Producto no valido";
                response.Meta = metadata;
                return response;
            }

            var repeticion = await GetByOrden(filter.Orden);
            if (repeticion == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Orden no encontrada";
                response.Meta = metadata;
                return response;
            }

            repeticion.AppproductsId = filter.IdProducto;
            await Update(repeticion);

            var idCliente = repeticion.IdCliente?.Trim();
            if (!string.IsNullOrWhiteSpace(idCliente))
            {
                RemoveCache($"ListRepeticiones{idCliente}");
            }

            var orden = await _unitOfWork.Cpry012Repository.GetByOrdenAsync(filter.Orden);
            if (orden != null)
            {
                orden.AppproductsId = filter.IdProducto;
                _unitOfWork.Cpry012Repository.UpdateProductoEnOrden(orden);
            }

            if (!string.IsNullOrWhiteSpace(idCliente))
            {
                AppOrdenProductoRepeticionFilterDto filterRepeticion = new AppOrdenProductoRepeticionFilterDto();
                filterRepeticion.IdCliente = idCliente;
                await GetAllFilter(filterRepeticion);
            }

            response.Data = true;
            metadata.IsValid = true;
            metadata.Message = "";
            response.Meta = metadata;
            return response;


        }

        

        private string FechaString(DateTime fecha)
        {
            string result = "";
            string año = "";
            string mes = "";
            string dia = "";
            string hora = "";
            string minuto = "";
            string segundo = "";
            año = fecha.Year.ToString();
            if (fecha.Month.ToString().Length < 2)
            {
                mes = fecha.Month.ToString().PadLeft(2, '0');
            }
            else
            {
                mes = fecha.Month.ToString();
            }

            if (fecha.Day.ToString().Length < 2)
            {
                dia = fecha.Day.ToString().PadLeft(2, '0');
            }
            else
            {
                dia = fecha.Day.ToString();
            }

            if (fecha.Hour.ToString().Length < 2)
            {
                hora = fecha.Hour.ToString().PadLeft(2, '0');
            }
            else
            {
                hora = fecha.Hour.ToString();
            }

            if (fecha.Minute.ToString().Length < 2)
            {
                minuto = fecha.Minute.ToString().PadLeft(2, '0');
            }
            else
            {
                minuto = fecha.Minute.ToString();
            }

            if (fecha.Second.ToString().Length < 2)
            {
                segundo = fecha.Second.ToString().PadLeft(2, '0');
            }
            else
            {
                segundo = fecha.Second.ToString();
            }

            result = dia + "-" + mes + "-" + año;

            return result;
        }


    }
}
