using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Odoo.Cotizaciones.Recibir;
using AppService.Core.Entities;
using AppService.Core.EntitiesFacturacion;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Enumerations;
using AppService.Core.Interfaces;
using AppService.Core.Map;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.DTOs.Precios;
using AppService.Core.Utility;
using AutoMapper.Configuration.Conventions;

namespace AppService.Core.Services
{
    public class AppDetailQuotesService : IAppDetailQuotesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppProductsService _appProductsService;
        private readonly IAppUnitsService _appUnitsService;
        private readonly IAppStatusQuoteService _appStatusQuoteService;
        private readonly ITPaTasaReferencialServices _tPaTasaReferencialServices;
        private readonly IAppDetailQuotesConversionUnitService _appDetailQuotesConversionUnitService;
        private readonly IAppTemplateConversionUnitService _appTemplateConversionUnitService;
        private readonly ICotizacionService _cotizacionService;
        private readonly IAprobacionesServices _aprobacionesServices;
        private readonly IAppRecipesByAppDetailQuotesService _appRecipesByAppDetailQuotesService;
        private readonly IAppGetPriceService _appGetPriceService;
        private readonly IMunicipioServices _municipioServices;

        private readonly PaginationOptions _paginationOptions;

        public AppDetailQuotesService(
          IUnitOfWork unitOfWork,
          IOptions<PaginationOptions> options,
          IMapper mapper,
          IAppProductsService appProductsService,
          IAppUnitsService appUnitsService,
          IAppStatusQuoteService appStatusQuoteService,
          ITPaTasaReferencialServices tPaTasaReferencialServices,
          IAppDetailQuotesConversionUnitService appDetailQuotesConversionUnitService,
          IAppTemplateConversionUnitService appTemplateConversionUnitService,
          ICotizacionService cotizacionService,
          IAprobacionesServices aprobacionesServices,
          IAppRecipesByAppDetailQuotesService appRecipesByAppDetailQuotesService,
          IAppGetPriceService appGetPriceService,
          IMunicipioServices municipioServices
       
          )
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._appProductsService = appProductsService;
            this._appUnitsService = appUnitsService;
            this._appStatusQuoteService = appStatusQuoteService;
            this._tPaTasaReferencialServices = tPaTasaReferencialServices;
            this._appDetailQuotesConversionUnitService = appDetailQuotesConversionUnitService;
            this._appTemplateConversionUnitService = appTemplateConversionUnitService;
            this._cotizacionService = cotizacionService;
            this._aprobacionesServices = aprobacionesServices;
            this._paginationOptions = options.Value;
            _appRecipesByAppDetailQuotesService = appRecipesByAppDetailQuotesService;
            _appGetPriceService = appGetPriceService;
            _municipioServices = municipioServices;
        }

        public async Task<List<AppDetailQuotes>> GetAll() => await this._unitOfWork.AppDetailQuotesRepository.GetAll();

        /*public async Task<Decimal> GetFleteByIdMunicipo(Decimal id)
        {
            decimal result = 0;

            Winy243 municipio = await this._unitOfWork.Winy243Repository.GetById(id);
            if (municipio != null)
            {
                result = (decimal)municipio.PorcFlete;
            }


            return result;
        }*/
        
        
        public async Task<ApiResponse<List<AppDetailQuotesGetDto>>> GetListAppDetailQuoteByAppGeneralQuotesId(int appGeneralQuotesId)
        {
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };

            List<AppDetailQuotesGetDto> resultDto = new List<AppDetailQuotesGetDto>();

            ApiResponse<List<AppDetailQuotesGetDto>> response = new ApiResponse<List<AppDetailQuotesGetDto>>(resultDto);

            var appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appGeneralQuotesId);
            
         
            try
            {
                resultDto = this._mapper.Map<List<AppDetailQuotesGetDto>>((object)await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotesId));
                foreach (AppDetailQuotesGetDto item in resultDto)
                {
                    AppProducts appProductsFind = await this._appProductsService.GetById(item.IdProducto);
                    if (appProductsFind != null)
                    {
                        AppProductConversion unitAlternativeUnit = await this._unitOfWork.AppProductConversionRepository.GetByProductBaseUnitAlternativeUnit(item.IdProducto, appProductsFind.ProductionUnitId.Value, item.IdUnidad);
                        if (unitAlternativeUnit != null)
                            item.AppProductConversionGetDto = MapAppProductConversion.MapAppProductConversionToAppProductConversionGetDto(unitAlternativeUnit);

                        AppProductsGetDto appProductsGetDto = this._mapper.Map<AppProductsGetDto>((object)appProductsFind);
                        //appProductsGetDto.TipoCalculo = appProductsFind.TipoCalculo;
                        AppUnits byId = await this._appUnitsService.GetById(appProductsFind.ProductionUnitId.Value);
                        if (byId != null)
                            appProductsGetDto.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId);
                        appProductsGetDto.Link = appProductsFind.UrlImage == "" || appProductsFind.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + appProductsFind.UrlImage;
                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(item.IdProducto);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            List<AppPriceDto> appPriceDtoList = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                            appProductsGetDto.PrecioPorRango = true;
                            var porcentajePrecioMaximo = await _unitOfWork.AppConfigAppRepository.GetByKey("UTILIDADOPMAX");
                            foreach (var itemPrecio in appPriceDtoList)
                            {
                                decimal porcentaje = decimal.Parse(porcentajePrecioMaximo.Valor);
                                var adicional = (itemPrecio.Precio * porcentaje) / 100;
                                itemPrecio.PrecioMaximo = itemPrecio.Precio + adicional;
                            }

                            appProductsGetDto.AppPriceDto = appPriceDtoList;
                        }
                        item.AppProductsGetDto = appProductsGetDto;
                        appProductsGetDto = (AppProductsGetDto)null;
                    }

                    AppUnits byId1 = await this._appUnitsService.GetById(item.IdUnidad);
                    if (byId1 != null)
                        item.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);

                    AppStatusQuote byId2 = await this._appStatusQuoteService.GetById(item.IdEstatus);
                    if (byId2 != null)
                        item.AppStatusQuoteGetDto = this._mapper.Map<AppStatusQuoteGetDto>((object)byId2);

                    AppDetailQuotes byId3 = await this.GetById(item.Id);
                    var flete = await GetFlete((decimal)appGeneralQuotes.IdMunicipio, item.IdProducto);
                    if (item.PorcFlete == 0)
                    {
                        item.PorcFlete= flete;
                        item.Flete = (item.UnitPriceBaseProduction * flete) / 100;
                    }

                    if (item.UnitPriceConverted == 0)
                    {
                        item.UnitPriceConverted = item.UnitPriceBaseProduction;
                    }
                    
                    item.StatusAprobacionDto = await this.StatusAprobacion(byId3,flete);
                    if (byId3.OrdenAnterior == null)
                    {
                        byId3.OrdenAnterior = 0;

                    }
                    item.OrdenAnterior = (long)byId3.OrdenAnterior;
                    var cantTintas = await _appProductsService.CantidaTintasProducto(item.IdProducto);
                    item.CantidadTintas = cantTintas;

                    item.TieneTintasCargadas = true;
                    var civy004 = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(appProductsFind.ExternalCode);
                    if (civy004 == null)
                    {
                        var tintas = await _unitOfWork.Wpry241Repository.GetByCotizacion(item.Cotizacion);
                        if (tintas.Count <= 0)
                        {
                            item.TieneTintasCargadas = false;
                        }
                    }

                    if (byId3.CalculoId == null)
                    {
                        byId3.CalculoId = 0;
                    }
                    item.CalculoId = (int)byId3.CalculoId;

                    appProductsFind = (AppProducts)null;
                }

                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
        }

        public async Task<decimal> GetFlete(decimal idMunicipio, int idProducto)
        {
            decimal porcflete = 0;
            var producto = await _unitOfWork.AppProductsRepository.GetById(idProducto);
            if (producto != null)
            {
                if (producto.PorcFlete > 0)
                {
                    porcflete=producto.PorcFlete;
                    return porcflete;
                }
            }
            var municipio = await _unitOfWork.Winy243Repository.GetById((decimal)idMunicipio);
            if (municipio != null)
            {
                porcflete = (decimal)municipio.PorcFlete;
                return porcflete;
            }
            return porcflete;
        }

        public async Task UpdateFlete(int year,int month)
        {
            var all = await _unitOfWork.AppDetailQuotesRepository.GetByYearMonth(year,month);
      
            if (all.Count() > 0)
            {
                foreach (var item in all)
                {
                    var general = await _unitOfWork.AppGeneralQuotesRepository.GetById(item.AppGeneralQuotesId);
                    if (general!=null)
                    {
                        var porcFlete = await GetFlete((decimal)general.IdMunicipio, item.IdProducto);
                        
                        var flete = (item.UnitPriceBaseProduction * porcFlete)/100;
                   
                        flete = (decimal?)(Math.Truncate((double)(flete * 100)) / 100);
                        
                        _unitOfWork.AppDetailQuotesRepository.UpdateFlete(item.Id,porcFlete,(decimal)flete);
                    }
                }
            }
        }
   
        public async Task RecalcularPreciosLista(int appGeneralQuotesId)
        {
            var generalQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appGeneralQuotesId);
            
            List<AppDetailQuotes> listAppDetailQuotes = await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotesId);
            foreach (AppDetailQuotes item in listAppDetailQuotes)
            {
                var producto = await _appProductsService.GetByIdProducto(item.IdProducto);
             
                
                var municipio = await _unitOfWork.Winy243Repository.GetById((decimal)generalQuotes.IdMunicipio);
                MunicipioGetDto municipioDto = new MunicipioGetDto();
                municipioDto.Recnum = municipio.Recnum;
                municipioDto.DescMunicipio = municipio.DescMunicipio;
                var porcFlete =await GetFlete((decimal)generalQuotes.IdMunicipio, item.IdProducto);
                municipioDto.PorcFlete = porcFlete;
                
                var idCondicionPago = 0;
                if (item.IdCondPago>0)
                {
                    idCondicionPago = item.IdCondPago;
                }
                else
                {
                    idCondicionPago=generalQuotes.IdCondPago;
                }

                var condicion = await _unitOfWork.MtrCondicionPagoRepository.GetById((short)idCondicionPago);
                
                OptionsPreciosProductos optionsPreciosProductos = new OptionsPreciosProductos();
                optionsPreciosProductos.Cantidad = (decimal)item.CantidadSolicitada;
                optionsPreciosProductos.Producto = producto;
                optionsPreciosProductos.MedidaBasica = (decimal)item.MedidaBasica;
                optionsPreciosProductos.MedidaOpuesta = (decimal)item.MedidaOpuesta;
                optionsPreciosProductos.CondicionPago = idCondicionPago;
                optionsPreciosProductos.Municipio = municipioDto;
                optionsPreciosProductos.IdUnidad=item.IdUnidad;
                optionsPreciosProductos.AppDetailQuotesId=item.Id;
                var precio = await _appGetPriceService.GetPrice(optionsPreciosProductos);
               /* var unitPriceBaseProduction =
                    precio.Data.PrecioMinimo + (precio.Data.PrecioMinimo* condicion.PocGapAplicarPrecio) / 100;*/
             
                int solicitarPrecio = 0;
                var precioMasFlete =precio.Data.PrecioMinimo + precio.Data.Flete;
                
            
               if (precioMasFlete > item.PrecioUsd)
                {
                    solicitarPrecio = 1;
                }

                if (precio.Data.PorDebajoDeCantidadMinima)
                {
                    solicitarPrecio = 1;
                }
            
                _unitOfWork.AppDetailQuotesRepository.UpdatePrecios(item.Id,precio.Data.PrecioMinimo,precio.Data.PrecioMaximo,precio.Data.IdCalculo,solicitarPrecio);
                
            }
                
            
        }
        
        public async Task<List<AppDetailQuotes>> RecalculaAppDetailQuoteByAppGeneralQuotesId(
          int appGeneralQuotesId)
        {
            try
            {
                List<AppDetailQuotes> listAppDetailQuotes = await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotesId);
                foreach (AppDetailQuotes item in listAppDetailQuotes)
                {
                    if (item.IdEstatusNavigation != null && item.IdEstatusNavigation.FlagModificar == "X")
                    {
                        AppService.Core.EntitiesMC.TPaTasaReferencial tasaByFecha = await this._tPaTasaReferencialServices.GetTasaByFecha(DateTime.Now);
                        long? idMtrTipoMoneda = item.AppGeneralQuotes.IdMtrTipoMoneda;
                        long num1 = 1;
                        if (idMtrTipoMoneda.GetValueOrDefault() == num1 & idMtrTipoMoneda.HasValue)
                        {
                            item.PrecioUsd = item.Precio / tasaByFecha.Tasa.Value;
                            item.TotalUsd = item.PrecioUsd * item.Cantidad;
                            item.PrecioUsd = Math.Truncate(item.PrecioUsd * 10000M) / 10000M;
                            item.TotalUsd = Math.Truncate(item.TotalUsd * 10000M) / 10000M;
                        }
                        else
                        {
                            item.Precio = item.PrecioUsd * tasaByFecha.Tasa.Value;
                            item.Total = item.Precio * item.Cantidad;
                            item.Precio = Math.Truncate(item.Precio * 10000M) / 10000M;
                            item.Total = Math.Truncate(item.Total * 10000M) / 10000M;
                        }
                        if (item.IdProductoNavigation != null)
                        {
                            int? quantityPerPackage = item.IdProductoNavigation.QuantityPerPackage;
                            int num2 = 0;
                            if (quantityPerPackage.GetValueOrDefault() > num2 & quantityPerPackage.HasValue)
                                item.QuantityPerPackage = item.IdProductoNavigation.QuantityPerPackage;
                        }
                        AppPrice productoCantidad = await this._unitOfWork.AppPriceRepository.GetByProductoCantidad(item.IdProducto, item.Cantidad);
                        item.UnitPriceBaseProduction = productoCantidad == null ? new Decimal?(item.IdProductoNavigation.UnitPrice) : new Decimal?(productoCantidad.Precio);
                        AppDetailQuotes appDetailQuotes = await this.Update(item);
                    }
                }
                return listAppDetailQuotes;
            }
            catch (Exception ex)
            {

                return (List<AppDetailQuotes>)null;
            }
        }

        public async Task<AppDetailQuotes> GetById(int id) => await this._unitOfWork.AppDetailQuotesRepository.GetById(id);

        public async Task<AppDetailQuotes> Insert(AppDetailQuotes appDetailQuotes)
        {
            AppDetailQuotes appDetailQuotes1;
            try
            {
                await this._unitOfWork.AppDetailQuotesRepository.Add(appDetailQuotes);
                await this._unitOfWork.SaveChangesAsync();
                appDetailQuotes1 = appDetailQuotes;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                throw;
            }
            return appDetailQuotes1;
        }

        public async Task<ApiResponse<AppDetailQuotesGetDto>> InsertAppDetailQuotes(
          AppDetailQuotesCreateDto appDetailQuotesDto)
        {
            AppDetailQuotesGetDto resultDto = new AppDetailQuotesGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppDetailQuotesGetDto> response = new ApiResponse<AppDetailQuotesGetDto>(resultDto);
            try
            {
                AppGeneralQuotes appGeneralQuotesFind = await this._unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotesDto.AppGeneralQuotesId);
                if (appGeneralQuotesFind == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No tiene Encabezado de Cotizacion!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                AppProducts appProducts = await this._appProductsService.GetById(appDetailQuotesDto.IdProducto);
                if (appProducts == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Producto No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(appProducts.ExternalCode.Trim());
                if (csmy036 == null)
                {

                    metadata.IsValid = false;
                    metadata.Message = "Error en configuracion de producto,(External Code Invalido) ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                bool? requiereDatosEntrada = appProducts.RequiereDatosEntrada;
                bool flag = true;
                if (requiereDatosEntrada.GetValueOrDefault() == flag & requiereDatosEntrada.HasValue && (appDetailQuotesDto.MedidaBasica <= 0M || appDetailQuotesDto.MedidaOpuesta <= 0M))
                {
                    metadata.IsValid = false;
                    metadata.Message = "Producto Raquiere Medidas!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._appUnitsService.GetById(appDetailQuotesDto.IdUnidad) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Unidad No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesDto.Cantidad <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cantidad no valida!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                /*if (appDetailQuotesDto.PrecioLista > appDetailQuotesDto.UnitPriceBaseProductionMaximo)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio minimo es mayor al precio maximo!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }*/
                
            
                
                Decimal precioLista = appDetailQuotesDto.PrecioLista;
                AppService.Core.EntitiesMC.TPaTasaReferencial tasaByFecha = await this._tPaTasaReferencialServices.GetTasaByFecha(DateTime.Now);
                long? idMtrTipoMoneda = appGeneralQuotesFind.IdMtrTipoMoneda;
                long num1 = 1;
                Decimal? nullable1;
                if (idMtrTipoMoneda.GetValueOrDefault() == num1 & idMtrTipoMoneda.HasValue)
                {
                    AppDetailQuotesCreateDto detailQuotesCreateDto = appDetailQuotesDto;
                    Decimal precio = appDetailQuotesDto.Precio;
                    nullable1 = tasaByFecha.Tasa;
                    Decimal num2 = nullable1.Value;
                    Decimal num3 = precio / num2;
                    detailQuotesCreateDto.PrecioUsd = num3;
                    appDetailQuotesDto.TotalUsd = appDetailQuotesDto.PrecioUsd * appDetailQuotesDto.Cantidad;
                    appDetailQuotesDto.PrecioUsd = Math.Truncate(appDetailQuotesDto.PrecioUsd * 10000M) / 10000M;
                    appDetailQuotesDto.TotalUsd = Math.Truncate(appDetailQuotesDto.TotalUsd * 10000M) / 10000M;
                }
                else
                {
                    AppDetailQuotesCreateDto detailQuotesCreateDto = appDetailQuotesDto;
                    Decimal precioUsd = appDetailQuotesDto.PrecioUsd;
                    nullable1 = tasaByFecha.Tasa;
                    Decimal num4 = nullable1.Value;
                    Decimal num5 = precioUsd * num4;
                    detailQuotesCreateDto.Precio = num5;
                    appDetailQuotesDto.Total = appDetailQuotesDto.Precio * appDetailQuotesDto.Cantidad;
                    appDetailQuotesDto.Precio = Math.Truncate(appDetailQuotesDto.Precio * 10000M) / 10000M;
                    appDetailQuotesDto.Total = Math.Truncate(appDetailQuotesDto.Total * 10000M) / 10000M;
                }
                if (!appProducts.RequiereDatosEntrada.Value)
                {
                    appDetailQuotesDto.ValorConvertido = (Decimal)appProducts.QuantityPerPackage.Value;
                    appDetailQuotesDto.UnitPriceBaseProduction = precioLista;
                    appDetailQuotesDto.UnitPriceConverted = precioLista;
                    AppDetailQuotesCreateDto detailQuotesCreateDto1 = appDetailQuotesDto;
                    int? quantityPerPackage = appProducts.QuantityPerPackage;
                    Decimal num6 = (Decimal)quantityPerPackage.Value;
                    detailQuotesCreateDto1.CantidadPorUnidadProduccion = num6;
                    AppDetailQuotesCreateDto detailQuotesCreateDto2 = appDetailQuotesDto;
                    quantityPerPackage = appProducts.QuantityPerPackage;
                    int num7 = quantityPerPackage.Value;
                    detailQuotesCreateDto2.QuantityPerPackage = num7;
                }
                if (appDetailQuotesDto.Precio <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesDto.Total <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesDto.PrecioUsd <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio Usd no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesDto.TotalUsd <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio Usd no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if(appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorRollo)
                {
                    var formasEnCalculo = await _unitOfWork.AppRecipesByAppDetailQuotesRepository.GetListRecipesByProductCodeVariableCodeHistorico((int)appDetailQuotesDto.CalculoId, appProducts.Code, "CANTFORMAS");
                    if (formasEnCalculo != null && formasEnCalculo.Count > 0)
                    {
                        appDetailQuotesDto.CantidadPorUnidadProduccion= (decimal)formasEnCalculo.FirstOrDefault().Quantity;
                     
                    }
                }


                AppDetailQuotes appDetailQuotes = this._mapper.Map<AppDetailQuotes>((object)appDetailQuotesDto);
                appDetailQuotes.IdCondPago = appDetailQuotesDto.CondicionPago;
                appDetailQuotes.CreatedAt = DateTime.Now;
                appDetailQuotes.FechaRegistro = DateTime.Now;
                appDetailQuotes.UserCreate = "";
                appDetailQuotes.Producto = appProducts.Code;
                appDetailQuotes.ObsSolicitud = appDetailQuotesDto.ObsSolicitud;
                appDetailQuotes.MedidaBasica = new Decimal?(appDetailQuotesDto.MedidaBasica);
                appDetailQuotes.MedidaOpuesta = new Decimal?(appDetailQuotesDto.MedidaOpuesta);
                if (appDetailQuotes.MedidaBasica != null && appDetailQuotes.MedidaBasica > 0)
                {
                    var basica = decimal.ToDouble((decimal)appDetailQuotes.MedidaBasica);
                    var opuesta = decimal.ToDouble((decimal)appDetailQuotes.MedidaOpuesta);
                    var medidaBasicaFraccion =
                        ConvertirMedidas.ConvertirCmAPulgadas(basica,true);
                    var medidaOpuestaFraccion =
                        ConvertirMedidas.ConvertirCmAPulgadas(opuesta,true);
                    appDetailQuotes.MedidaBasicaFraccionPulgada = medidaBasicaFraccion.HumanRepresentation;
                    appDetailQuotes.MedidaOpuestaFraccionPulgada = medidaOpuestaFraccion.HumanRepresentation;
                }
                else
                {
                    appDetailQuotes.MedidaBasicaFraccionPulgada = "";
                    appDetailQuotes.MedidaOpuestaFraccionPulgada = "";
                }
                
                appDetailQuotes.CantidadPorUnidadProduccion = new Decimal?(appDetailQuotesDto.CantidadPorUnidadProduccion);
                appDetailQuotes.ValorConvertido = new Decimal?(appDetailQuotesDto.ValorConvertido);

                AppDetailQuotes appDetailQuotes1 = appDetailQuotes;
                nullable1 = appDetailQuotes.UnitPriceBaseProduction;
                Decimal? valorConvertido = appDetailQuotes.ValorConvertido;
                Decimal? nullable2 = nullable1.HasValue & valorConvertido.HasValue ? new Decimal?(nullable1.GetValueOrDefault() * valorConvertido.GetValueOrDefault()) : new Decimal?();
                appDetailQuotes1.UnitPriceConverted = nullable2;
                AppDetailQuotes appDetailQuotesInserted = new AppDetailQuotes();
                if (await this._unitOfWork.AppDetailQuotesRepository.GetByQuetesProduct(appDetailQuotesDto.Cotizacion, appDetailQuotesDto.IdProducto) != null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Producto Existe en la cotizacion!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                appDetailQuotes.OrdenAnterior = appDetailQuotesDto.OrdenAnterior;
                appDetailQuotes.Medidas = await getMedidas(appProducts.Id);
                if (appDetailQuotesDto.CalculoId == null)
                {
                    appDetailQuotesDto.CalculoId = 0;
                }
                appDetailQuotes.CalculoId = appDetailQuotesDto.CalculoId;
               /* if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorProducto)
                {
                    var precio =
                        await _appRecipesByAppDetailQuotesService.GetPrecioProductoCantidadOfficeProduct(appDetailQuotes.IdProducto, appDetailQuotes.Cantidad);
                    if (precio != null)
                    {
                        appDetailQuotes.CalculoId = precio.CalculoId;
                        appDetailQuotesDto.PrecioLista = precio.Precio;
                        appDetailQuotesDto.UnitPriceBaseProductionMaximo = precio.Precio;
                    }
                }*/
                appDetailQuotes.UnitPriceBaseProduction = appDetailQuotesDto.PrecioLista;
                appDetailQuotes.UnitPriceBaseProductionMaximo = appDetailQuotesDto.UnitPriceBaseProductionMaximo;

                if (appDetailQuotesDto.Presentacion == null) appDetailQuotesDto.Presentacion = "";
                if (appDetailQuotesDto.Salida == null) appDetailQuotesDto.Salida = "";
                if (appDetailQuotesDto.Forma == null) appDetailQuotesDto.Forma = "";
                appDetailQuotes.Presentacion = appDetailQuotesDto.Presentacion;
                appDetailQuotes.Salida = appDetailQuotesDto.Salida;
                appDetailQuotes.Forma = appDetailQuotesDto.Forma;

                if (appDetailQuotes.IdCondPago == 0)
                {
                    appDetailQuotes.IdCondPago = 40;
                }

                var porcFlete = await GetFlete((decimal)appGeneralQuotesFind.IdMunicipio, appDetailQuotes.IdProducto);
                appDetailQuotes.PorcFlete = porcFlete;
                appDetailQuotes.Flete = ((decimal)appDetailQuotes.UnitPriceBaseProduction * porcFlete) / 100;
                
                appDetailQuotesInserted = await this.Insert(appDetailQuotes);
                await this._unitOfWork.SaveChangesAsync();
                if (appDetailQuotesDto.CondicionPago == 0)
                {
                    appDetailQuotesDto.CondicionPago = 40;
                }
                _unitOfWork.AppDetailQuotesRepository.UpdateCondicionPago(appDetailQuotesDto.AppGeneralQuotesId,appDetailQuotesDto.CondicionPago);
                
                //TODO PRUEBA INTEGRAR COTIZACION POR LOTE
                await this._cotizacionService.IntegrarCotizacion(appDetailQuotesInserted.AppGeneralQuotesId, true);
                    if (appDetailQuotesInserted != null)
                {
                    if (appDetailQuotesInserted.IdProductoNavigation.AppSubCategoryId == 9)
                    {
                        await ArmaDataReportEtiquetasPrime(appDetailQuotesInserted.Cotizacion);
                    }
                   
                    resultDto = this._mapper.Map<AppDetailQuotesGetDto>((object)appDetailQuotesInserted);
                    AppProducts byId1 = await this._appProductsService.GetById(appDetailQuotesInserted.IdProducto);
                    if (byId1 != null)
                        resultDto.AppProductsGetDto = this._mapper.Map<AppProductsGetDto>((object)byId1);
                    AppUnits byId2 = await this._appUnitsService.GetById(appDetailQuotesInserted.IdUnidad);
                    if (byId2 != null)
                        resultDto.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                    AppStatusQuote byId3 = await this._appStatusQuoteService.GetById(appDetailQuotesInserted.IdEstatus);
                    if (byId3 != null)
                        resultDto.AppStatusQuoteGetDto = this._mapper.Map<AppStatusQuoteGetDto>((object)byId3);
                    resultDto.OrdenAnterior = (long)appDetailQuotesInserted.OrdenAnterior;
                    metadata.IsValid = true;
                    metadata.Message = "A la Cotizacion: " + resultDto.Cotizacion + " se le adiciono producto Satisfactoriamente!!";
                }
                else
                {
                    metadata.IsValid = false;
                    metadata.Message = "Registro No actualizado";
                }
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
        }

        public async Task<AppDetailQuotes> Update(AppDetailQuotes appDetailQuotes)
        {
            try
            {
              
                this._unitOfWork.AppDetailQuotesRepository.Update(appDetailQuotes);
                await this._unitOfWork.SaveChangesAsync();
                return await this.GetById(appDetailQuotes.Id);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                return (AppDetailQuotes)null;
            }
        }

        public async Task<ApiResponse<AppDetailQuotesGetDto>> ValidarPrecio(int appDetailQuotesId,decimal precio,decimal lista)
        {
            AppDetailQuotesGetDto resultDto = new AppDetailQuotesGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppDetailQuotesGetDto> response = new ApiResponse<AppDetailQuotesGetDto>(resultDto);
            AppDetailQuotes appDetailQuotes = await this.GetById(appDetailQuotesId);
            if (appDetailQuotes == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Id Cotizacion No Existe!!! ";
                response.Meta = metadata;
                response.Data = null;
                return response;
            }

            var appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);
            var porcflete = await GetFlete((decimal)appGeneralQuotes.IdMunicipio,appDetailQuotes.IdProducto);

         
           
            Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);

            if (renglon == null)
            {
                //TODO PRUEBA INTEGRAR COTIZACION POR LOTE
                await this._cotizacionService.IntegrarCotizacion(appDetailQuotes.AppGeneralQuotesId, true);
                renglon=await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);

            }
            
            if (renglon != null)
            {
                var aprobacionObj =
                    await _aprobacionesServices.GetByCotizacionRenglonPrpopuesta(
                        renglon.Cotizacion, renglon.Renglon, 1);
                if (aprobacionObj != null)
                {
                    var preciousd = precio;
                    var aprobado = aprobacionObj.ValorVentaAprobarUsd;
                  
                    if (preciousd <aprobado)
                    {
                        await RegresarAGrabacionCotizacion(appDetailQuotes.AppGeneralQuotesId);
                        metadata.IsValid = false;
                        metadata.Message = $"Precio no puede ser menor a el aprobado!!! {aprobado}, cotizacion enviada a Grabacion";
                        response.Meta = metadata;
                        response.Data = null;
                        return response;
                    }
                }
                else
                {
                    
                    var preciousd = precio;
                    decimal unitPriceBaseProduction = lista;
                    var flete = (unitPriceBaseProduction* porcflete) / 100;
                    decimal precioLista = Math.Round(unitPriceBaseProduction+flete, 2);
                    if (precioLista>preciousd) 
                    {
                        
                        await RegresarAGrabacionCotizacion(appDetailQuotes.AppGeneralQuotesId);
                        metadata.IsValid = false;
                        metadata.Message = $"Precio no puede ser menor a  la lista!!! {lista}, cotizacion enviada a Grabacion";
                        response.Meta = metadata;
                        response.Data = null;
                        return response;
                    }
                }
            }

            return response;

        }


        public async Task UpdatePrecioListaEstimadas()
        {
            
            var detail =  await this._unitOfWork.AppDetailQuotesRepository.GetAll();
            var filtrado = detail.Where(x => x.FechaRegistro.Year == 2024 && x.FechaRegistro.Month == 8).ToList();
            foreach (var appDetailQuotesUpdateDto in filtrado )
            {

                decimal precioLista;
                
                AppDetailQuotes appDetailQuotes = await this.GetById(appDetailQuotesUpdateDto.Id);
                AppProducts appProducts = await this._appProductsService.GetById(appDetailQuotesUpdateDto.IdProducto);
              
                if (appProducts.RequiereEstimacion == true)
                {
                    Wsmy502 cotizacionProducto = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appProducts.ExternalCode);
                    if (cotizacionProducto != null)
                    {
                        Wsmy639 wsmy639Response = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(cotizacionProducto.Cotizacion, cotizacionProducto.Renglon, 1);
                        if (wsmy639Response != null)
                        {
                            if (wsmy639Response.IdEstatus == "APRO")
                            {
                                precioLista = (decimal)wsmy639Response.ValorVentaAprobarUsd;
                              
                            }else
                            {
                                precioLista = 0;
                               
                            }
                            appDetailQuotes.UnitPriceBaseProduction = new Decimal?(precioLista);
                            appDetailQuotes.UnitPriceConverted = new Decimal?(precioLista);
                            appDetailQuotes.UnitPriceBaseProductionMaximo = new Decimal?(precioLista);
                            this._unitOfWork.AppDetailQuotesRepository.Update(appDetailQuotes);
                            await this._unitOfWork.SaveChangesAsync();

                            await _cotizacionService.IntegrarCotizacion(appDetailQuotes.AppGeneralQuotesId, true);
                        }
                    }
                }
            }
            
            
        }
        
        public async Task<ApiResponse<AppDetailQuotesGetDto>> UpdateAppDetailQuotes(
          AppDetailQuotesUpdateDto appDetailQuotesUpdateDto)
        {

            AppDetailQuotesGetDto resultDto = new AppDetailQuotesGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
       
            ApiResponse<AppDetailQuotesGetDto> response = new ApiResponse<AppDetailQuotesGetDto>(resultDto);
            try
            {
                bool recalcularPrecio = false;
            
             
                
                AppDetailQuotes appDetailQuotes = await this.GetById(appDetailQuotesUpdateDto.Id);
                if (appDetailQuotes == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No existe Detalle para ese producto!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                AppGeneralQuotes generalQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);
                
                if (appDetailQuotes.IdCondPago != appDetailQuotesUpdateDto.CondicionPago)
                {
                    recalcularPrecio = true;
                }

                if (appDetailQuotes.PrecioUsd != appDetailQuotesUpdateDto.PrecioUsd)
                {
                    await EliminarAprobacionRechazada(appDetailQuotes);
                }



                if (appDetailQuotes.AppGeneralQuotes == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No tiene Encabezado de Cotizacion!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                AppProducts appProducts = await this._appProductsService.GetById(appDetailQuotesUpdateDto.IdProducto);
                if (appProducts == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Producto No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(appProducts.ExternalCode.Trim());
                if (csmy036 == null)
                {

                    metadata.IsValid = false;
                    metadata.Message = "Error en configuracion de producto,(External Code Invalido) ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                bool? requiereDatosEntrada = appProducts.RequiereDatosEntrada;
                bool flag = true;
                if (requiereDatosEntrada.GetValueOrDefault() == flag & requiereDatosEntrada.HasValue && (appDetailQuotesUpdateDto.MedidaBasica <= 0M || appDetailQuotesUpdateDto.MedidaOpuesta <= 0M))
                {
                    metadata.IsValid = false;
                    metadata.Message = "Producto Raquiere Medidas!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._appUnitsService.GetById(appDetailQuotesUpdateDto.IdUnidad) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Unidad No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesUpdateDto.Cantidad <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cantidad no valida!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesUpdateDto.AppConversionUnitGenericCreateDto == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Debe indicar la conversion de unidades ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
              
                if (appProducts.RequiereEstimacion == true)
                {
                    Wsmy502 cotizacionProducto = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appProducts.ExternalCode);
                    if (cotizacionProducto != null)
                    {
                        Wsmy639 wsmy639Response = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(cotizacionProducto.Cotizacion, cotizacionProducto.Renglon, 1);
                        if (wsmy639Response != null)
                        {
                            if (wsmy639Response.IdEstatus == "APRO")
                            {
                                appDetailQuotesUpdateDto.PrecioLista = (decimal)wsmy639Response.ValorVentaAprobarUsd;
                                appDetailQuotesUpdateDto.UnitPriceBaseProductionMaximo =
                                    (decimal)wsmy639Response.ValorVentaAprobarUsd;
                            }else
                            {
                                appDetailQuotesUpdateDto.PrecioLista = 0;
                                appDetailQuotesUpdateDto.UnitPriceBaseProductionMaximo = 0;
                            }
                           
                        }
                    }
                }
             
              


                Decimal precioLista = appDetailQuotesUpdateDto.PrecioLista;
                AppService.Core.EntitiesMC.TPaTasaReferencial tasaByFecha = await this._tPaTasaReferencialServices.GetTasaByFecha(DateTime.Now);
                long? idMtrTipoMoneda = appDetailQuotes.AppGeneralQuotes.IdMtrTipoMoneda;
                long num1 = 1;
                if (idMtrTipoMoneda.GetValueOrDefault() == num1 & idMtrTipoMoneda.HasValue)
                {
                    appDetailQuotesUpdateDto.Precio = Math.Truncate(appDetailQuotesUpdateDto.Precio * 100M) / 100M;
                    appDetailQuotesUpdateDto.Total = Math.Truncate(appDetailQuotesUpdateDto.Total * 100M) / 100M;
                    appDetailQuotesUpdateDto.PrecioUsd = appDetailQuotesUpdateDto.Precio / tasaByFecha.Tasa.Value;
                    appDetailQuotesUpdateDto.TotalUsd = appDetailQuotesUpdateDto.PrecioUsd * appDetailQuotesUpdateDto.Cantidad;
                    appDetailQuotesUpdateDto.PrecioUsd = Math.Truncate(appDetailQuotesUpdateDto.PrecioUsd * 100M) / 100M;
                    appDetailQuotesUpdateDto.TotalUsd = Math.Truncate(appDetailQuotesUpdateDto.TotalUsd * 100M) / 100M;
                }
                else
                {
                    appDetailQuotesUpdateDto.PrecioUsd = Math.Truncate(appDetailQuotesUpdateDto.PrecioUsd * 100M) / 100M;
                    appDetailQuotesUpdateDto.TotalUsd = Math.Truncate(appDetailQuotesUpdateDto.TotalUsd * 100M) / 100M;
                    appDetailQuotesUpdateDto.Precio = appDetailQuotesUpdateDto.PrecioUsd * tasaByFecha.Tasa.Value;
                    appDetailQuotesUpdateDto.Total = appDetailQuotesUpdateDto.Precio * appDetailQuotesUpdateDto.Cantidad;
                    appDetailQuotesUpdateDto.Precio = Math.Truncate(appDetailQuotesUpdateDto.Precio * 100M) / 100M;
                    appDetailQuotesUpdateDto.Total = Math.Truncate(appDetailQuotesUpdateDto.Total * 100M) / 100M;
                }
                if (appDetailQuotesUpdateDto.Precio <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesUpdateDto.Total <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesUpdateDto.PrecioUsd <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio Usd no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appDetailQuotesUpdateDto.TotalUsd <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Precio Usd no valido!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorRollo)
                {
                    var formasEnCalculo = await _unitOfWork.AppRecipesByAppDetailQuotesRepository.GetListRecipesByProductCodeVariableCodeHistorico((int)appDetailQuotesUpdateDto.CalculoId, appProducts.Code, "CANTFORMAS");
                    if (formasEnCalculo != null && formasEnCalculo.Count > 0)
                    {
                        appDetailQuotesUpdateDto.CantidadPorUnidadProduccion = (decimal)formasEnCalculo.FirstOrDefault().Quantity;

                    }
                }

                appDetailQuotes.IdProducto = appDetailQuotesUpdateDto.IdProducto;
                appDetailQuotes.IdCondPago = appDetailQuotesUpdateDto.CondicionPago;
                appDetailQuotes.Producto = appProducts.Code;
                appDetailQuotes.IdUnidad = appDetailQuotesUpdateDto.IdUnidad;
                appDetailQuotes.NombreComercialProducto = appDetailQuotesUpdateDto.NombreComercialProducto;
                appDetailQuotes.Observaciones = appDetailQuotesUpdateDto.Observaciones;
                appDetailQuotes.Precio = appDetailQuotesUpdateDto.Precio;
                appDetailQuotes.PrecioUsd = appDetailQuotesUpdateDto.PrecioUsd;
                appDetailQuotes.Total = appDetailQuotesUpdateDto.Total;
                appDetailQuotes.TotalUsd = appDetailQuotesUpdateDto.TotalUsd;
                appDetailQuotes.Cantidad = appDetailQuotesUpdateDto.Cantidad;
                appDetailQuotes.CantidadSolicitada = new Decimal?(appDetailQuotesUpdateDto.CantidadSolicitada);
                appDetailQuotes.CantidadPorUnidadProduccion = appDetailQuotesUpdateDto.CantidadPorUnidadProduccion;
                appDetailQuotes.ValorConvertido = appDetailQuotesUpdateDto.ValorConvertido;
                appDetailQuotes.FechaRegistro = DateTime.Now;
                appDetailQuotes.UserUpdate = appDetailQuotesUpdateDto.UsuarioConectado;
                appDetailQuotes.DiasEntrega = appDetailQuotesUpdateDto.DiasEntrega;
                appDetailQuotes.ObsSolicitud = appDetailQuotesUpdateDto.ObsSolicitud;
                appDetailQuotes.ValorConvertido = appDetailQuotesUpdateDto.ValorConvertido;
                appDetailQuotes.UnitPriceBaseProduction = new Decimal?(precioLista);
                appDetailQuotes.UnitPriceConverted = new Decimal?(precioLista);
                appDetailQuotes.QuantityPerPackage = appProducts.QuantityPerPackage;
                appDetailQuotes.UnitPriceBaseProduction = new Decimal?(precioLista);
                appDetailQuotes.MedidaBasica = new Decimal?(appDetailQuotesUpdateDto.MedidaBasica);
                appDetailQuotes.MedidaOpuesta = new Decimal?(appDetailQuotesUpdateDto.MedidaOpuesta);
                
                if (appDetailQuotes.MedidaBasica != null && appDetailQuotes.MedidaBasica > 0)
                {
                    var basica = decimal.ToDouble((decimal)appDetailQuotes.MedidaBasica);
                    var opuesta = decimal.ToDouble((decimal)appDetailQuotes.MedidaOpuesta);

                    var factorPulgada = 2.54;
                    var basicaPulgada = Math.Truncate(basica / factorPulgada);
                    var medidaBasicaConvertida = new Fractional.Fractional((decimal)basicaPulgada, false);

                    
                    var medidaBasicaFraccion =
                        ConvertirMedidas.ConvertirCmAPulgadas(basica,true);
                    var medidaOpuestaFraccion =
                        ConvertirMedidas.ConvertirCmAPulgadas(opuesta,true);
                    appDetailQuotes.MedidaBasicaFraccionPulgada = medidaBasicaFraccion.HumanRepresentation;
                    appDetailQuotes.MedidaOpuestaFraccionPulgada = medidaOpuestaFraccion.HumanRepresentation;
                }
                else
                {
                    appDetailQuotes.MedidaBasicaFraccionPulgada = "";
                    appDetailQuotes.MedidaOpuestaFraccionPulgada = "";
                }

              
                
                appDetailQuotes.SolicitarPrecio = new bool?(appDetailQuotesUpdateDto.SolicitarPrecio);

                appDetailQuotes.Medidas = await getMedidas(appDetailQuotes.IdProducto);
                appDetailQuotes.Papeles = await _unitOfWork.Wpry240Repository.GetPapeles(appDetailQuotes.Cotizacion);
                appDetailQuotes.TipoPapeles = await _unitOfWork.Wpry240Repository.GetPapelesTipo(appDetailQuotes.Cotizacion);
                appDetailQuotes.Tintas = await _unitOfWork.Wpry241Repository.GetTintas(appDetailQuotes.Cotizacion);
                appDetailQuotes.CantPartes = await _unitOfWork.Wpry240Repository.GetCantPartes(appDetailQuotes.Cotizacion);
                appDetailQuotes.CantTintas = 0;
                if (appDetailQuotesUpdateDto.CalculoId == null)
                {
                    appDetailQuotesUpdateDto.CalculoId = 0;
                }

          
                appDetailQuotes.CalculoId = appDetailQuotesUpdateDto.CalculoId;
                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorProducto)
                {
                    var precio =
                        await _appRecipesByAppDetailQuotesService.GetPrecioProductoCantidadOfficeProduct(appDetailQuotes.IdProducto, appDetailQuotes.Cantidad);
                    if (precio != null)
                    {
                        appDetailQuotes.CalculoId = precio.CalculoId;
                    }
                }
  
                
                if (appDetailQuotes.Tintas.Length > 0)
                {
                    string[] tintas = appDetailQuotes.Tintas.Split(",");
                    appDetailQuotes.CantTintas = tintas.Count();
                }


            
                appDetailQuotes.UnitPriceBaseProductionMaximo = appDetailQuotesUpdateDto.UnitPriceBaseProductionMaximo;

                if (appDetailQuotesUpdateDto.Presentacion == null) appDetailQuotesUpdateDto.Presentacion = "";
                if (appDetailQuotesUpdateDto.Salida == null) appDetailQuotesUpdateDto.Salida = "";
                if (appDetailQuotesUpdateDto.Forma == null) appDetailQuotesUpdateDto.Forma = "";
                appDetailQuotes.Presentacion = appDetailQuotesUpdateDto.Presentacion;
                appDetailQuotes.Salida = appDetailQuotesUpdateDto.Salida;
                appDetailQuotes.Forma = appDetailQuotesUpdateDto.Forma;


                var porcFlete = await GetFlete((decimal)generalQuotes.IdMunicipio, appDetailQuotes.IdProducto);
                appDetailQuotes.PorcFlete = porcFlete;
                appDetailQuotes.Flete = ((decimal)appDetailQuotes.UnitPriceBaseProduction * porcFlete) / 100;
                appDetailQuotes.Flete  = (decimal?)(Math.Truncate((double)( appDetailQuotes.Flete * 100)) / 100);
                
                if (appDetailQuotes.IdCondPago == 0)
                {
                    appDetailQuotes.IdCondPago = 40;
                }

                AppDetailQuotes appDetailQuotesUpdated = await this.Update(appDetailQuotes);
                await RecalcularPreciosLista(appDetailQuotes.AppGeneralQuotesId);
                /*if (appDetailQuotesUpdateDto.CalculoId == 0)
                {
                    await RecalcularPreciosLista(appDetailQuotes.AppGeneralQuotesId);
                }*/
                
                _unitOfWork.AppDetailQuotesRepository.UpdateCondicionPago(appDetailQuotesUpdated.AppGeneralQuotesId,appDetailQuotesUpdated.IdCondPago);


                AppGeneralQuotes general = await this._unitOfWork.AppGeneralQuotesRepository.GetByIdForUpdate(appDetailQuotes.AppGeneralQuotesId);
                if (general != null)
                {
                    general.FechaActualiza = DateTime.Now;
                    general.IdCondPago = appDetailQuotes.IdCondPago;

                    this._unitOfWork.AppGeneralQuotesRepository.Update(general);
                    await this._unitOfWork.SaveChangesAsync();
                }
            

                /*if (recalcularPrecio)
                {
                    await RecalcularPreciosLista(appDetailQuotes.AppGeneralQuotesId);

                }*/
                 
              

            
                await this._cotizacionService.IntegrarCotizacion(appDetailQuotesUpdated.AppGeneralQuotesId, true);
                if (appDetailQuotesUpdated != null)
                {
                    if (appDetailQuotesUpdated.IdProductoNavigation.AppSubCategoryId == 9)
                    {
                        await ArmaDataReportEtiquetasPrime(appDetailQuotesUpdated.Cotizacion);
                    }
                    resultDto = this._mapper.Map<AppDetailQuotesGetDto>((object)appDetailQuotesUpdated);
                    AppProducts byId1 = await this._appProductsService.GetById(appDetailQuotesUpdated.IdProducto);
                    if (byId1 != null)
                        resultDto.AppProductsGetDto = this._mapper.Map<AppProductsGetDto>((object)byId1);
                    AppUnits byId2 = await this._appUnitsService.GetById(appDetailQuotesUpdated.IdUnidad);
                    if (byId2 != null)
                        resultDto.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                    AppStatusQuote byId3 = await this._appStatusQuoteService.GetById(appDetailQuotesUpdated.IdEstatus);
                    if (byId3 != null)
                        resultDto.AppStatusQuoteGetDto = this._mapper.Map<AppStatusQuoteGetDto>((object)byId3);
                    if (appDetailQuotesUpdated.OrdenAnterior == null) appDetailQuotesUpdated.OrdenAnterior = 0;
                    resultDto.OrdenAnterior = (long)appDetailQuotesUpdated.OrdenAnterior;
                    metadata.IsValid = true;
                    metadata.Message = "Cotizacion: " + resultDto.Cotizacion + " actualizado Satisfactoriamente!!";
                }
                else
                {
                    metadata.IsValid = false;
                    metadata.Message = "Registro No actualizado";
                }
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
        }
        
      
        public async Task RecalcularCotizaciones(int subCategory)
        {
            var cotizaciones = await _unitOfWork.AppDetailQuotesRepository.GetBySubCategopry(subCategory);
            if (cotizaciones.Count > 0)
            {
                foreach (var item in cotizaciones)
                {

                    var municipio = item.AppGeneralQuotes.IdMunicipio;
                    appRecipesByAppDetailQuotesQueryFilter filters = new appRecipesByAppDetailQuotesQueryFilter();
                    filters.IdMunicipio = (decimal)municipio;
                    filters.AppProuctId = item.IdProducto;
                    filters.Cantidad = (int)item.CantidadSolicitada;
                    filters.Largo = item.MedidaBasica;
                    filters.Ancho = item.MedidaOpuesta;
                    var precio = await _appRecipesByAppDetailQuotesService.GetPrice(filters);
                    AppDetailQuotesUpdateDto appDetailQuotesUpdateDto = new AppDetailQuotesUpdateDto();
                    appDetailQuotesUpdateDto.Id = item.Id;
                    appDetailQuotesUpdateDto.Producto = item.Producto;
                    appDetailQuotesUpdateDto.IdProducto = item.IdProducto;
                    appDetailQuotesUpdateDto.NombreComercialProducto = item.NombreComercialProducto;

                    /*
                       
                      
                        public string NombreComercialProducto { get; set; }
                        public int IdEstatus { get; set; }
                        public decimal Cantidad { get; set; }
                        public decimal CantidadSolicitada { get; set; }
                        public decimal Precio { get; set; }
                        public decimal Total { get; set; }
                        public decimal PrecioUsd { get; set; }
                        public decimal TotalUsd { get; set; }
                        public int IdUnidad { get; set; }
                        public string Observaciones { get; set; }
                        public int DiasEntrega { get; set; }
                        public string UsuarioConectado { get; set; }
                        public decimal PrecioLista { get; set; }
                        public int QuantityPerPackage { get; set; }
                        public bool SolicitarPrecio { get; set; }
                        public string ObsSolicitud { get; set; }
                        public decimal? ValorConvertido { get; set; }
                        public decimal? UnitPriceBaseProduction { get; set; }
                        public decimal? UnitPriceConverted { get; set; }
                        public decimal? CantidadPorUnidadProduccion { get; set; }
                        public long OrdenAnterior { get; set; }
                        public Decimal MedidaBasica { get; set; }
                        public Decimal MedidaOpuesta { get; set; }
                        public int CalculoId { get; set; }
                        public Decimal? UnitPriceBaseProductionMaximo { get; set; } = 0;
                        public string Forma { get; set; } = string.Empty;
                        public string Salida { get; set; } = string.Empty;
                        public string Presentacion { get; set; } = string.Empty;

                     */
                }
            }
        }

        public async Task RecalcularCotizacionesOfficeProduct(int subCategory)
        {
            var cotizaciones = await _unitOfWork.AppDetailQuotesRepository.GetBySubCategopry(subCategory);
            if (cotizaciones.Count > 0)
            {
                var cotizacionesFiltered = cotizaciones.Where(x => x.CalculoId == 0 && x.CreatedAt.Year==2023 && x.CreatedAt.Month>=8).ToList();
                foreach (var item in cotizacionesFiltered)
                {
                    try
                    {
                        AppProducts appProducts = await this._appProductsService.GetById(item.IdProducto);
                        if (appProducts != null)
                        {
                            if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorProducto)
                            {
                                var precio =
                                    await _appRecipesByAppDetailQuotesService.GetPrecioProductoCantidadOfficeProduct(item.IdProducto, item.Cantidad);
                                if (precio != null)
                                {
                                    AppDetailQuotes appDetailQuotes = await this.GetById(item.Id);
                                    if (appDetailQuotes != null)
                                    {
                                        if(precio.CalculoId!=null)
                                        {
                                            appDetailQuotes.CalculoId = precio.CalculoId;
                                            AppDetailQuotes appDetailQuotesUpdated = await this.Update(appDetailQuotes);
                                        }
                                    
                                    }
                               
                            
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
               
                    
                    
                }
            }
        }
        public async Task UpdateDataReport(string cotizacion)
        {
           
            var appDetailQuotes = await _unitOfWork.AppDetailQuotesRepository.GetByQuotesCotizacion(cotizacion);
            if (appDetailQuotes.Count > 0)
            {
                foreach (var item in appDetailQuotes)
                {
                    var appDetailUpdate = await _unitOfWork.AppDetailQuotesRepository.GetById(item.Id);

                    appDetailUpdate.Medidas = await getMedidas(appDetailUpdate.IdProducto);
                    appDetailUpdate.Papeles = await _unitOfWork.Wpry240Repository.GetPapeles(appDetailUpdate.Cotizacion);
                    appDetailUpdate.TipoPapeles = await _unitOfWork.Wpry240Repository.GetPapelesTipo(appDetailUpdate.Cotizacion);
                    appDetailUpdate.Tintas = await _unitOfWork.Wpry241Repository.GetTintas(appDetailUpdate.Cotizacion);
                    appDetailUpdate.CantPartes = await _unitOfWork.Wpry240Repository.GetCantPartes(appDetailUpdate.Cotizacion);

                    appDetailUpdate.CantTintas = 0;
                    if (item.Tintas.Length > 0)
                    {

                        string[] tintas = appDetailUpdate.Tintas.Split(",");
                        item.CantTintas = tintas.Count();
                    }

                    if (appDetailUpdate.MedidaBasica != null && appDetailUpdate.MedidaBasica > 0)
                    {
                        var basica = decimal.ToDouble((decimal)appDetailUpdate.MedidaBasica);
                        var opuesta = decimal.ToDouble((decimal)appDetailUpdate.MedidaOpuesta);
                        var medidaBasicaFraccion =
                            ConvertirMedidas.ConvertirCmAPulgadas(basica,true);
                        var medidaOpuestaFraccion =
                            ConvertirMedidas.ConvertirCmAPulgadas(opuesta,true);
                        appDetailUpdate.MedidaBasicaFraccionPulgada = medidaBasicaFraccion.HumanRepresentation;
                        appDetailUpdate.MedidaOpuestaFraccionPulgada = medidaOpuestaFraccion.HumanRepresentation;
                    }
                    else
                    {
                        appDetailUpdate.MedidaBasicaFraccionPulgada = "";
                        appDetailUpdate.MedidaOpuestaFraccionPulgada = "";
                    }
                  
                    _unitOfWork.AppDetailQuotesRepository.Update(appDetailUpdate);
                    //TODO PRUEBA INTEGRAR COTIZACION POR LOTEawait _unitOfWork.SaveChangesAsync();
                    if (item.IdProductoNavigation.AppSubCategoryId == 9)
                    {
                        await ArmaDataReportEtiquetasPrime(item.Cotizacion);
                    }
                    await this._cotizacionService.IntegrarCotizacion(item.AppGeneralQuotesId, true);
                }
       
          
            }

        }

        public async Task ArmaDataReportEtiquetasPrime(string cotizacion)
        {
            var appDetailQuotes = await _unitOfWork.AppDetailQuotesRepository.GetByQuotesCotizacion(cotizacion);
            if (appDetailQuotes.Count > 0)
            {
                var firstRecord = appDetailQuotes.First();
                //await RecalcularPreciosLista(firstRecord.AppGeneralQuotesId);
                foreach (var item in appDetailQuotes)
                {
                    var listCalculo = await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetListRecipesByIdCalculoCodeHistorico((int)item.CalculoId);
                    var generalQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(item.AppGeneralQuotesId);
                    var contacto = await _unitOfWork.MtrContactosRepository.GetById(generalQuotes.IdContacto);
                    var condicionPago = await _unitOfWork.MtrCondicionPagoRepository.GetById(generalQuotes.IdCondPago);
                    var vendedor = _unitOfWork.MtrVendedorRepository.GetById(generalQuotes.IdVendedor);
                    string fecha="";
                    string dia = generalQuotes.Fecha.Day.ToString("00");
                    string mes = generalQuotes.Fecha.Month.ToString("00");
                    string ano = generalQuotes.Fecha.Year.ToString();
                    fecha = $"{dia}/{mes}/{ano}";
                    var reporte = await _unitOfWork.AppReporteCotizacionEtiquetasPrimeRepository.GetByCotizacion(cotizacion);
                    if (reporte == null)
                    {
                        AppReporteCotizacionEtiquetasPrime reporteNew = new AppReporteCotizacionEtiquetasPrime();

                        reporteNew.Cotizacion = item.Cotizacion;
                        reporteNew.AppDetailQuotesId = item.Id;
                        reporteNew.AppGeneralQuotesId = item.AppGeneralQuotesId;
                        reporteNew.CalculoId = (int)item.CalculoId;
                       

                        reporteNew.Fecha = $"{fecha}";
                        //DATOS DEL CLIENTE

                        if (generalQuotes.IdCliente == "000000")
                        {
                            reporteNew.Cliente = $"({generalQuotes.IdCliente}) {generalQuotes.RazonSocial.Trim()} RIF {generalQuotes.Rif.Trim()}";
                            reporteNew.Direccion = $"{generalQuotes.Direccion.Trim()}";

                        }
                        else
                        {
                            reporteNew.Cliente = $"({generalQuotes.IdCliente}) {generalQuotes.IdClienteNavigation.Nombre.Trim()} RIF {generalQuotes.IdClienteNavigation.NoRegTribut.Trim()}";
                            reporteNew.Direccion = $"{generalQuotes.IdClienteNavigation.Direccion.Trim()} {generalQuotes.IdClienteNavigation.Direccion1.Trim()}";

                        }
                     
                        reporteNew.Contacto = $"{contacto.Nombre.Trim()}";
                        reporteNew.Telefono = $"{contacto.Telefono1.Trim()}";


                        //DATOS DEL PRODUCTO
                        reporteNew.Producto = $"{item.IdProductoNavigation.Description1} {item.IdProductoNavigation.Description2}";
                        reporteNew.Impresion = "FLEXOGRAFICA";
                        reporteNew.Codigo = $"{item.IdProductoNavigation.ExternalCode}-{item.IdProductoNavigation.Code}";
                        reporteNew.Nombre = $"{item.NombreComercialProducto}";
                        reporteNew.Forma = $"{item.Forma}";
                        reporteNew.SustratoBase = await GetSustratoBase(listCalculo);
                        reporteNew.Acabado = await GetAcabado(listCalculo);
                        var cantTintas = await GetCantTintas(listCalculo);
                        reporteNew.CantidadImpresion = $"{cantTintas} Tintas"; 
                        reporteNew.Tamano= $"{item.MedidaBasica*10} mm x {item.MedidaOpuesta*10} mm";
                        reporteNew.Presentacion = $"{item.Presentacion} , Salida {item.Salida}"; 

                        //PRECIOS DEL PRODUCTO
                        var cantidadSolicitada = (decimal)item.Cantidad;
                        reporteNew.CantidadProducto = $"{cantidadSolicitada.ToString("N", CultureInfo.InvariantCulture)}";
                        reporteNew.UnitarioProducto = $"{item.PrecioUsd.ToString("N", CultureInfo.InvariantCulture)}";
                        reporteNew.DescripcionProducto= $"{item.IdProductoNavigation.Description1}";
                        reporteNew.TotalProducto = $"{item.TotalUsd.ToString("N", CultureInfo.InvariantCulture)}";


                        var precioClise = await GetPrecioClise(listCalculo);
                        var precioTroquel = await GetPrecioTroquel(listCalculo);
                        reporteNew.CantidadClise = "1.00";
                        reporteNew.DescripcionClise = "Clises";
                        reporteNew.UnitarioClise = $"{precioClise.ToString("N", CultureInfo.InvariantCulture)}";
                        reporteNew.TotalCliese = reporteNew.UnitarioClise;

                        reporteNew.CantidadTroquel = "1.00";
                        reporteNew.DescripcionTroquel = "Troquel";
                        reporteNew.UnitarioTroquel = $"{precioTroquel.ToString("N", CultureInfo.InvariantCulture)}";
                        reporteNew.TotalTroquel = reporteNew.UnitarioTroquel;
                        var total = item.TotalUsd + precioTroquel + precioClise;
                        reporteNew.Total = $"{total.ToString("N", CultureInfo.InvariantCulture)}";

                        if (item.OrdenAnterior <= 0)
                        {
                            reporteNew.CantidadClise = "1.00";
                            reporteNew.DescripcionClise = "Clises";
                            reporteNew.UnitarioClise = $"{precioClise.ToString("N", CultureInfo.InvariantCulture)}";
                            reporteNew.TotalCliese = reporteNew.UnitarioClise;

                            reporteNew.CantidadTroquel = "1.00";
                            reporteNew.DescripcionTroquel = "Troquel";
                            reporteNew.UnitarioTroquel = $"{precioTroquel.ToString("N", CultureInfo.InvariantCulture)}";
                            reporteNew.TotalTroquel = reporteNew.UnitarioTroquel;
                            
                            reporteNew.Total = $"{item.TotalUsd.ToString("N", CultureInfo.InvariantCulture)}";
                        }
                        else
                        {
                            reporteNew.CantidadClise = "";
                            reporteNew.DescripcionClise = "";
                            reporteNew.UnitarioClise = "";
                            reporteNew.TotalCliese = "";

                            reporteNew.CantidadTroquel = "";
                            reporteNew.DescripcionTroquel = "";
                            reporteNew.UnitarioTroquel = ""; 
                            reporteNew.TotalTroquel = "";
                            total = item.TotalUsd ;
                            reporteNew.Total = $"{total.ToString("N", CultureInfo.InvariantCulture)}";
                        }

                        reporteNew.CondicionPago = $"Condicion de Pago:{condicionPago.Descripcion.Trim()}"; 
                        reporteNew.Vendedor = $"Sr(a):{vendedor.Codigo.Trim()} " +
                                                      $"{vendedor.Nombre.Trim()} ({vendedor.TlfCelular.Trim()}) " +
                                                      $"Email: {vendedor.EMail.Trim()}";
                        reporteNew.Vigencia = $"Vigencia de la Cotizacion 10 dias.";
                        await _unitOfWork.AppReporteCotizacionEtiquetasPrimeRepository.Add(reporteNew);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        //DATOS DEL CLIENTE
                        if (generalQuotes.IdCliente == "000000")
                        {
                            reporte.Cliente = $"({generalQuotes.IdCliente}) {generalQuotes.RazonSocial.Trim()} RIF {generalQuotes.Rif.Trim()}";
                            reporte.Direccion = $"{generalQuotes.Direccion.Trim()}";

                        }
                        else
                        {
                            reporte.Cliente = $"({generalQuotes.IdCliente}) {generalQuotes.IdClienteNavigation.Nombre.Trim()} RIF {generalQuotes.IdClienteNavigation.NoRegTribut.Trim()}";
                            reporte.Direccion = $"{generalQuotes.IdClienteNavigation.Direccion.Trim()} {generalQuotes.IdClienteNavigation.Direccion1.Trim()}";

                        }
                        
                        reporte.Contacto = $"{contacto.Nombre.Trim()}";
                        reporte.Telefono = $"{contacto.Telefono1}";
                        reporte.CalculoId = (int)item.CalculoId;
                        reporte.Fecha = $"{fecha}";
                        //DATOS DEL PRODUCTO
                        reporte.Producto = $"{item.IdProductoNavigation.Description1} {item.IdProductoNavigation.Description2}";
                        reporte.Impresion = "FLEXOGRAFICA";
                        reporte.Codigo = $"{item.IdProductoNavigation.ExternalCode}-{item.IdProductoNavigation.Code}";
                        reporte.Nombre = $"{item.NombreComercialProducto}";
                        reporte.Forma = $"{item.Forma}";
                        reporte.SustratoBase = await GetSustratoBase(listCalculo);
                        reporte.Acabado = await GetAcabado(listCalculo);
                        var cantTintas = await GetCantTintas(listCalculo);
                        reporte.CantidadImpresion = $"{cantTintas} Tintas";
                        reporte.Tamano = $"{item.MedidaBasica * 10} mm x {item.MedidaOpuesta * 10} mm";
                        reporte.Presentacion = $"{item.Presentacion} , Salida {item.Salida}";

                        //PRECIOS DEL PRODUCTO
                        var cantidadSolicitada = (decimal)item.Cantidad;
                        reporte.CantidadProducto = $"{cantidadSolicitada.ToString("N", CultureInfo.InvariantCulture)}";
                        reporte.UnitarioProducto = $"{item.PrecioUsd.ToString("N", CultureInfo.InvariantCulture)}";
                        reporte.DescripcionProducto = $"{item.IdProductoNavigation.Description1}";
                        reporte.TotalProducto = $"{item.TotalUsd.ToString("N", CultureInfo.InvariantCulture)}";

                    

                        if (item.OrdenAnterior <=0)
                        {
                            var precioClise = await GetPrecioClise(listCalculo);
                            var precioTroquel = await GetPrecioTroquel(listCalculo);


                          
                          
                            reporte.CantidadClise = "1.00";
                            reporte.DescripcionClise = "Clises";
                            reporte.UnitarioClise = $"{precioClise.ToString("N", CultureInfo.InvariantCulture)}";
                            reporte.TotalCliese = reporte.UnitarioClise;

                            reporte.CantidadTroquel = "1.00";
                            reporte.DescripcionTroquel = "Troquel";
                            reporte.UnitarioTroquel = $"{precioTroquel.ToString("N", CultureInfo.InvariantCulture)}";
                            reporte.TotalTroquel = reporte.UnitarioTroquel;
                            reporte.Total = $"{item.TotalUsd.ToString("N", CultureInfo.InvariantCulture)}";

                        }
                        else
                        {
                            reporte.CantidadClise = "";
                            reporte.DescripcionClise = "";
                            reporte.UnitarioClise = "";
                            reporte.TotalCliese = "";

                            reporte.CantidadTroquel = "";
                            reporte.DescripcionTroquel = "";
                            reporte.UnitarioTroquel = "";
                            reporte.TotalTroquel = "";

                          
                            reporte.Total = $"{item.TotalUsd.ToString("N", CultureInfo.InvariantCulture)}";
                        }

                        reporte.CondicionPago = $"Condicion de Pago:{condicionPago.Descripcion.Trim()}";
                        reporte.Vendedor = $"Sr(a):{vendedor.Codigo.Trim()} " +
                                                      $"{vendedor.Nombre.Trim()} ({vendedor.TlfCelular.Trim()}) " +
                                                      $"Email: {vendedor.EMail.Trim()}";

                        reporte.Vigencia = $"Vigencia de la Cotizacion 10 dias.";
                        _unitOfWork.AppReporteCotizacionEtiquetasPrimeRepository.Update(reporte);
                        await _unitOfWork.SaveChangesAsync();
                    }

                 
                   

                }
            }

        }


        public async Task<decimal> GetCantTintas(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            decimal result;
            var calculo = listCalculo.Where(x => x.Code == "CANT_TINTAS").FirstOrDefault();
            if (calculo == null)
            {
                result = 0;
                return result;
            }
            result = (decimal)calculo.Quantity;


            return result;
        }

        public async Task<decimal> GetPrecioClise(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            decimal result;
            var calculo = listCalculo.Where(x => x.Code == "PRECIOGOMASTIL").FirstOrDefault();
            if (calculo == null)
            {
                result = 0;
                return result;
            }
            result = (decimal)calculo.Quantity;


            return result;
        }
        public async Task<decimal> GetPrecioTroquel(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            decimal result;
            var calculo = listCalculo.Where(x => x.Code == "TROQUEL").FirstOrDefault();
            if (calculo == null)
            {
                result = 0;
                return result;
            }
            
            result = (decimal)calculo.TotalCost;


            return result;
        }


        public async Task<string> GetSustratoBase(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            string result;
            var calculo= listCalculo.Where(x=> x.Code== "PAPELPRIMERAPARTE").FirstOrDefault();

            if (calculo == null)
            {
                result = "";
                return result;
            }
            var ingrediente = await _unitOfWork.AppIngredientsRepository.GetById((int)calculo.AppIngredientsId);
            if (ingrediente != null)
            {
                result = ingrediente.Description;
            }
            else {
                result = "";
            }


            return result;
        }
        public async Task<string> GetAcabado(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            string result;
            var calculo = listCalculo.Where(x => x.Code == "LAMINADO").FirstOrDefault();
            if (calculo == null)
            {
                result = "";
                return result;
            }
            
            var ingrediente = await _unitOfWork.AppIngredientsRepository.GetById((int)calculo.AppIngredientsId);
            if (ingrediente != null)
            {
                result = ingrediente.Description;
            }
            else
            {
                result = "";
            }


            return result;
        }

        
        public async Task<string> getMedidas(int idProduct)
        {
            string result = string.Empty;
            result = await _unitOfWork.AppRecipesRepository.getMedidas(idProduct);

            return result;
        }

        public async Task<ApiResponse<bool>> DeleteDetailQuotes(
          AppDetailQuotesDeleteDto appDetailQuotesDeleteDto)
        {
            bool resultDto = false;
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<bool> response = new ApiResponse<bool>(resultDto);
            try
            {
                AppDetailQuotes byId = await this.GetById(appDetailQuotesDeleteDto.Id);
                if (byId == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cotizacion No Existe!!! " + appDetailQuotesDeleteDto.Cotizacion;
                    response.Meta = metadata;
                    response.Data = false;
                    return response;
                }
                if (byId.IdEstatusNavigation != null)
                {
                    if (byId.IdEstatusNavigation.FlagModificar == "X")
                    {


                        await this._cotizacionService.DeleteCotizacionRenglon(byId);
                        await this.DeteleAppDetailQuotesByDetailQuotesId(appDetailQuotesDeleteDto.Id);
                        var details = await _unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(byId.AppGeneralQuotesId);
                        if (details.Count == 0)
                        {
                            var general = await _unitOfWork.AppGeneralQuotesRepository.GetById(byId.AppGeneralQuotesId);
                            if (general != null)
                            {
                                general.IdEstatus = 1;
                                _unitOfWork.AppGeneralQuotesRepository.Update(general);
                                _unitOfWork.SaveChanges();

                            }
                        }
                        int num = await this.Delete(appDetailQuotesDeleteDto.Id) ? 1 : 0;
                        metadata.IsValid = true;
                        metadata.Message = "Cotizacion: " + appDetailQuotesDeleteDto.Cotizacion + " Eliminada Satisfactoriamente!!";
                        response.Meta = metadata;
                        response.Data = resultDto;
                    }
                    else
                    {
                        metadata.IsValid = false;
                        metadata.Message = "Cotizacion: " + appDetailQuotesDeleteDto.Cotizacion + " No puede ser eliminada.Esta en Estatus: " + byId.IdEstatusNavigation.Descripcion;
                        response.Meta = metadata;
                        response.Data = resultDto;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                await this._unitOfWork.AppDetailQuotesRepository.Delete(id);
                await this._unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                return false;
            }
        }

        public async Task DeteleAppDetailQuotesByDetailQuotesId(int appDetailQuotesId)
        {
            try
            {
                List<AppDetailQuotesConversionUnit> appDetailQuotesId1 = await this._unitOfWork.AppDetailQuotesConversionUnitRepository.GetByAppDetailQuotesId(appDetailQuotesId);
                if (appDetailQuotesId1 == null || appDetailQuotesId1.Count <= 0)
                    return;
                foreach (AppDetailQuotesConversionUnit quotesConversionUnit in appDetailQuotesId1)
                    await this._unitOfWork.AppDetailQuotesConversionUnitRepository.Delete(quotesConversionUnit.Id);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                throw;
            }
        }
        
        public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName) {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists) {
                info.Create();
            }

            string path = Path.Combine(filePath, fileName);
            using(FileStream outputFileStream = new FileStream(path, FileMode.Create)) {
                inputStream.CopyTo(outputFileStream);
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private async Task<OfdAdjuntoCriterio> AddAdjuntoCriterio(long IdAdjunto, Int16 IdCriterioBusqueda, string Valor)
        {
            OfdAdjuntoCriterio AdjCri = new OfdAdjuntoCriterio();
            try
            {
                AdjCri.IdAdjunto = IdAdjunto;
                AdjCri.IdCriterioBusqueda = IdCriterioBusqueda;
                AdjCri.Valor = Valor;
                AdjCri.IdUsuarioCreacion = "SISTEMA";
                AdjCri.FechaCreacion = DateTime.Now;
                var created = await _unitOfWork.OfdAdjuntoCriterioRepository.Add(AdjCri);
                await _unitOfWork.SaveChangesAsync();
                return created;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
      public async Task AddReportCotizacionOficinaDigital(string cotizacion, bool flagTotal, bool flagFormasCaja, bool flagIva, bool observaciones, bool imprimirUsd, int? subcategoria = null)
        {
            await UpdateDataReport(cotizacion);
            var cotizacionObj =await  _unitOfWork.CotizacionRepository.GetByCotizacion(cotizacion);
            ReporteCotizacionDto dto = new ReporteCotizacionDto();

            dto.ParametroCotizacion = "Cotizacion";
            dto.Cotizacion = cotizacion;


            dto.ParametroFlagTotal = "FlagTotal";
            dto.FlagTotal = flagTotal;


            dto.ParametroFlagFormasCaja = "FlagFormasCaja";
            dto.FlagFormasCaja = flagFormasCaja;


            dto.ParametroFlagIva = "FlagIva";
            dto.FlagIva = flagIva;

            dto.ParametroObservaciones = "Observaciones";
            dto.Observaciones = observaciones;

            dto.ParametroImprimirUsd = "ImprimirUsd";
            dto.ImprimirUsd = imprimirUsd;


            //Todo agregar urls,credenciales al settings  y hacer metodo async

            //URL Base de mi servidor de reporte con la peticion de parametro en la url
            //string urlBase = "https://myrshost/ReportServer?/myreport&PARAMETRO=";
            string myreport = "";
            if (subcategoria > 2)
            {
                myreport = "Ventas/Cotizador Plus/CotizacionPlusFormas";
            }

            if (subcategoria == 1 || subcategoria == 10)
            {
                myreport = "Ventas/Cotizador Plus/AppCotizacionPlusFormas";

            }
            if (subcategoria == 2)
            {
                myreport = "Ventas/Cotizador Plus/CotizacionPlusStock";
            }
            if (subcategoria == 9)
            {
                myreport = "Ventas/Cotizador Plus/AppReporteCotizacionEtiquetasPrime ";
            }


            // string myreport = "Ventas/Cotizador Plus/CotizacionFormasVentas";

            string urlBase = "http://vmooreapp2/ReportServer_FSVEMCYN03D?/" + myreport;
            // aqui es donde indicas en que formato quieres obtener el reporte
            string formatoReporte = "&rs:Format=pdf";

            // la url final
            string url = urlBase
                + "&" + dto.ParametroCotizacion + "=" + dto.Cotizacion
                + "&" + dto.ParametroFlagTotal + "=" + dto.FlagTotal
                + "&" + dto.ParametroFlagFormasCaja + "=" + dto.FlagFormasCaja
                + "&" + dto.ParametroFlagIva + "=" + dto.FlagIva
                + "&" + dto.ParametroObservaciones + "=" + dto.Observaciones
                + "&" + dto.ParametroImprimirUsd + "=" + dto.ImprimirUsd
                + formatoReporte;




            try
            {
                string nameFile =dto.Cotizacion + "_" + "_No_firmada.pdf" ;
                urlBase = url.Replace("\n", "");
                System.Net.WebRequest request = WebRequest.Create(urlBase);
                // pasas las credenciales para conectarte al servidor de reporte de reporting services
                NetworkCredential credentials = new NetworkCredential(@"RR105841", "Polar2023*", "Moore");
                request.Credentials = credentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
               
                byte[] bytes = ReadFully(stream);

                //byte[] bytes = new byte[0];
            
                var fileFind = await _unitOfWork.OfdAdjuntoRepository.GetByFileName(nameFile);
                if (fileFind != null)
                {
                    await _unitOfWork.OfdAdjuntoRepository.DeleteByFileName(nameFile);
             
                }



                await _unitOfWork.Wpry254Repository.DeleteByFileName(nameFile);
                Wpry254 wpry254 = new Wpry254();
                wpry254.Cotizacion = cotizacionObj.Cotizacion;
                wpry254.Renglon = 1;
                wpry254.Propuesta = 1;
                wpry254.Archivo=bytes;
                wpry254.IdTipoDocumento = 6;
                wpry254.FechaCreacion = DateTime.Now;
                wpry254.NombreArchivo = nameFile;
                wpry254.NoReplicar = true;
                wpry254.IdUsuario = "helpdesk";
                await _unitOfWork.Wpry254Repository.Add(wpry254);
                await _unitOfWork.SaveChangesAsync();
                
                OfdAdjunto ofdAdjunto = new OfdAdjunto();
                ofdAdjunto.NombreArchivo = nameFile;
                ofdAdjunto.Archivo = bytes;
                ofdAdjunto.IdUsuarioCreacion = "helpdesk";
                ofdAdjunto.FechaCreacion = DateTime.Now;
                ofdAdjunto.IdTipoDocumento = 6;
                ofdAdjunto.IdAdjuntoCotizadorPluss = wpry254.Id;
                var created = await _unitOfWork.OfdAdjuntoRepository.Add(ofdAdjunto);
                await _unitOfWork.SaveChangesAsync();
                
                //_unitOfWork.OfdAdjuntoRepository.ActualizaOfdAdjuntoEnWpry254(created.IdAdjunto);
                if (created.IdAdjunto > 0)
                {
                    
                    short idCriterioBusquedaOrden = 5;
                    short idCriterioBusquedaCotizacion = 6;
                    short idCriterioBusquedaCliente = 7;
                    short idCriterioBusquedaRif = 8;
                    short idCriterioBusquedaFactura = 9;
                    short idCriterioBusquedaNotaEntrega = 11;
                    short idCriterioBusquedaNotaCredito = 13;
                    short idCriterioBusquedaNotaDebito = 14;
                    string valor = string.Empty;
                    if ( cotizacionObj.Cotizacion.Length > 0)
                    {

                        valor =  cotizacionObj.Cotizacion.ToString();
                        await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaCotizacion, valor);
                    }
                    if (cotizacionObj.CodCliente.Length > 0)
                    {
                        valor = cotizacionObj.CodCliente.ToString();
                        await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaCliente, valor);
                    }
                    if (cotizacionObj.Rif.Length > 0)
                    {
                        valor = cotizacionObj.Rif.ToString();
                        await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaRif, valor);
                    }
                }
               

               

            }
            catch (Exception ex)
            {
                //return ex.Message;
            }

        }

      
        public async Task<ApiResponse<AppGeneralQuotesGetDto>> RegresarAGrabacionCotizacion(
          int generalQuoteId)
        {
            AppGeneralQuotesGetDto resultDto = new AppGeneralQuotesGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppGeneralQuotesGetDto> response = new ApiResponse<AppGeneralQuotesGetDto>(resultDto);
            try
            {
                AppGeneralQuotes appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(generalQuoteId);
                if (appGeneralQuotes == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cotizacion No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                var cotizacionTieneOrden = await _unitOfWork.PropuestaRepository.CotizacionTieneOrden(appGeneralQuotes.Cotizacion);
                if (cotizacionTieneOrden)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cotizacion Tiene Orden!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                appGeneralQuotes.IdEstatus = 1;
                appGeneralQuotes.Fecha = DateTime.Now;
                appGeneralQuotes.FechaActualiza =DateTime.Now;
                _unitOfWork.AppGeneralQuotesRepository.Update(appGeneralQuotes);
                await this._unitOfWork.SaveChangesAsync();
                List<AppDetailQuotes> appDetailQuotesList = await this._unitOfWork.AppDetailQuotesRepository.GeSimpletByAppGeneralQuotesId(generalQuoteId);
                if (appDetailQuotesList != null)
                {
                    foreach (AppDetailQuotes entity in appDetailQuotesList)
                    {
                        entity.IdEstatus = 1;
                        this._unitOfWork.AppDetailQuotesRepository.Update(entity);
                        await this._unitOfWork.SaveChangesAsync();
                        var aprobaciones =await _unitOfWork.AprobacionesRepository.GetByCotizacionRenglonPropuesta(entity.Cotizacion,1,1);

                        if (aprobaciones != null)
                        {
                            await _unitOfWork.AprobacionesRepository.Delete(aprobaciones.Id);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }

               
                //TODO PRUEBA INTEGRAR COTIZACION POR LOTE
                await this._cotizacionService.IntegrarCotizacion(appGeneralQuotes.Id, true);
           
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }
        }
        public async Task<ApiResponse<bool>> GanarPerder(
          AppGanarPerderDto appGanarPerderDto)
        {
            bool data = false;
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<bool> response = new ApiResponse<bool>(data);
            AppStatusQuote statusGanada = await this._appStatusQuoteService.GetStatusGanada();
            AppStatusQuote statusPerdida = await this._appStatusQuoteService.GetStatusPerdida();
            AppDetailQuotes appDetailQuotesUpdated = await this.GetById(appGanarPerderDto.AppDetailQuotesId);
            if (appDetailQuotesUpdated != null)
            {
                await ActualizarPrecioAprobado(appDetailQuotesUpdated);
            }
        
            
            int status = 0;
            if (appGanarPerderDto.CondicionId == 1)
                status = statusGanada.Id;
            else if (appGanarPerderDto.CondicionId == 2)
                status = statusPerdida.Id;
            AppDetailQuotes appDetailQuotes = await this.GetById(appGanarPerderDto.AppDetailQuotesId);
            if (appDetailQuotes == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Id Cotizacion No Existe!!! ";
                response.Meta = metadata;
                response.Data = false;
                return response;
            }

       
            
            if (appDetailQuotes.UnitPriceBaseProduction == 0)
            {
                metadata.IsValid = false;
                metadata.Message = "Precio Lista Invalido,Entre a la pantalla de detalle de producto, verifique El Precio Minimo y de GUARDAR";
                response.Meta = metadata;
                response.Data = false;
                return response;
            }
            var producto = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
            if (producto == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Producto no existe";
                response.Meta = metadata;
                response.Data = false;
                return response;
            }

            if (appGanarPerderDto.CondicionId == 1)
            {
                var productoStock = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(producto.ExternalCode);
                if (productoStock == null)
                {
                    var tintas = await _unitOfWork.Wpry241Repository.CotizacionTieneTintas(appDetailQuotes.Cotizacion);
                    if (tintas ==false)
                    {
                        metadata.IsValid = false;
                        metadata.Message = "Cotizacion No Contiene Tintas!!!, Si es sin Impresion por favor asigne la tinta: S/IMP ";
                        response.Meta = metadata;
                        response.Data = false;
                        return response;
                    }
                }
            }
          
           
            
            
            var appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);
           
            var porcflete = await GetFlete((decimal)appGeneralQuotes.IdMunicipio, producto.Id);
            
           
            Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);

            if (renglon == null)
            {
                //TODO PRUEBA INTEGRAR COTIZACION POR LOTE
                await this._cotizacionService.IntegrarCotizacion(appDetailQuotes.AppGeneralQuotesId, true);
                renglon=await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);

            }
            var aprobacionObj =
                await _aprobacionesServices.GetByCotizacionRenglonPrpopuesta(
                    renglon.Cotizacion, renglon.Renglon, 1);
            if (renglon != null && appGanarPerderDto.CondicionId == 1)
            {
              
                if (aprobacionObj != null)
                {
                    var preciousd = appDetailQuotes.PrecioUsd;
                    var aprobado = aprobacionObj.ValorVentaAprobarUsd;
                 
                 
                    decimal unitPriceBaseProduction = (decimal)appDetailQuotes.UnitPriceBaseProduction;
                    var flete = (unitPriceBaseProduction* porcflete) / 100;
                    
                    flete = Math.Truncate(flete * 100) / 100;
                    decimal precioLista = Math.Round(unitPriceBaseProduction+flete, 2);
                    

                    if (precioLista>preciousd && aprobado==0)
                    {
                        metadata.IsValid = false;
                        metadata.Message = $"No tiene Precio Aprobado";
                        response.Meta = metadata;
                        response.Data = false;
                        return response;
                    }
                    
                    if (preciousd <aprobado)
                    {
                        metadata.IsValid = false;
                        metadata.Message = $"Precio no puede ser menor a el aprobado!!! {aprobado}, cotizacion enviada a Grabacion";
                        response.Meta = metadata;
                        response.Data = false;
                        return response;
                    }
                }
                else
                {
                    
                    var preciousd = appDetailQuotes.PrecioUsd;
                    decimal unitPriceBaseProduction = (decimal)appDetailQuotes.UnitPriceConverted;
                    var flete = (unitPriceBaseProduction* porcflete) / 100;
                    
                    flete = Math.Truncate(flete * 100) / 100;
                    decimal precioLista = Math.Round(unitPriceBaseProduction+flete, 2);
                    if (precioLista>preciousd) 
                    {
                       // await RegresarAGrabacionCotizacion(appDetailQuotes.AppGeneralQuotesId);
                        metadata.IsValid = false;
                        metadata.Message = $"Precio no puede ser menor a  la lista!!! {precioLista}, cotizacion enviada a Grabacion";
                        response.Meta = metadata;
                        response.Data = false;
                        return response;
                    }
                }
            }

            var product = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
            if (product != null)
            {
                var civy004 = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(product.ExternalCode);
                if (civy004 != null)
                {
                    if (appGanarPerderDto.CondicionId == 1 && aprobacionObj == null)
                    {
                        var aprobacion= await this._aprobacionesServices.CreateAprobacionAprobada(renglon.Cotizacion, renglon.Renglon, 1, appGanarPerderDto.UsuarioConectado);
                        if (aprobacion.Meta.IsValid)
                        {
                            ApiResponse<Wsmy647> apiResponse = await this._aprobacionesServices.ActivarWORKFLOW(renglon.Cotizacion, renglon.Renglon, 1, appGanarPerderDto.UsuarioConectado, appDetailQuotes);

                        }
                    }
                }
            }
           
            
         


            appDetailQuotes.IdEstatus = status;
            appDetailQuotes.RazonGanadaPerdida = new int?(appGanarPerderDto.MotivoId);
            appDetailQuotes.Competidor = new int?(appGanarPerderDto.CompetidorId);
            AppDetailQuotes appDetailQuotes1 = await this.Update(appDetailQuotes);
            await this._unitOfWork.SaveChangesAsync();
            AppGeneralQuotes byId = await this._unitOfWork.AppGeneralQuotesRepository.GetByIdForUpdate(appDetailQuotes.AppGeneralQuotesId);
            if (byId != null)
            {
                byId.IdEstatus = status;
                byId.FechaActualiza = DateTime.Now;
                this._unitOfWork.AppGeneralQuotesRepository.Update(byId);
                await this._unitOfWork.SaveChangesAsync();
            }
            //TODO PRUEBA INTEGRAR COTIZACION POR LOTE
            await this._cotizacionService.IntegrarCotizacion(appDetailQuotes.AppGeneralQuotesId, true);
            /*await _unitOfWork.CotizacionRepository
                .EnviarCotizacionAdministradora(renglon.Cotizacion, renglon.Renglon, 1,
                    appGanarPerderDto.UsuarioConectado);*/
            
         
            if (product != null)
            {
                var subcategoria = product.AppSubCategoryId;
                await AddReportCotizacionOficinaDigital(appDetailQuotes.Cotizacion, true, true, false, true, true,subcategoria);
            }
           
            metadata.IsValid = true;
            metadata.Message = "Cotizacion Actualizada!!! ";
            response.Meta = metadata;
            response.Data = false;
            return response;
        }


        public async Task ActualizarPrecioAprobado( AppDetailQuotes appDetailQuotes)
        {
            Wsmy502 cotizacionProducto = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);
            if (cotizacionProducto != null)
            {
                Wsmy639 wsmy639Response = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(cotizacionProducto.Cotizacion, cotizacionProducto.Renglon, 1);
                if (wsmy639Response != null)
                {
                    var precioAprobado =  wsmy639Response.ValorVentaAprobarUsd;

                    _unitOfWork.AppDetailQuotesRepository.UpdatePrecioMinimo(appDetailQuotes.Id, (decimal)precioAprobado,
                        (decimal)precioAprobado, 0);

                }
            }
        }


        public async Task EliminarAprobacionRechazada(
            AppDetailQuotes appDetailQuotes)
        {
            Wsmy502 cotizacionProducto = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);
            if (cotizacionProducto != null)
            {
                Wsmy639 wsmy639Response = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(cotizacionProducto.Cotizacion, cotizacionProducto.Renglon, 1);
                if (wsmy639Response != null)
                {
                    if (wsmy639Response.IdEstatus=="RECH")
                    {
                        await _unitOfWork.Wsmy369Repository.LimpiaCotizacion(cotizacionProducto.Cotizacion);
                    }
                }
            }
            
            
        }
        public async Task<StatusAprobacionDto> StatusAprobacion(
          AppDetailQuotes appDetailQuotes,decimal porcflete)
        {
            StatusAprobacionDto result = new StatusAprobacionDto();
            AppGeneralQuotes general = await this._unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);
           
            
            Wsmy502 cotizacionProducto = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);
            if (cotizacionProducto != null)
            {

                var appProduct = appDetailQuotes.IdProductoNavigation; // _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
                if (appProduct != null)
                {
                    if (appProduct.PorcFlete > 0)
                    {
                        porcflete= appProduct.PorcFlete;
                    }
                }
                
                
                Wsmy639 wsmy639Response = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(cotizacionProducto.Cotizacion, cotizacionProducto.Renglon, 1);
                if (wsmy639Response != null)
                {
                    
                 
                    result.FlagAprobado = wsmy639Response.FlagAprobado;
                    result.FlagCerrado = wsmy639Response.FlagCerrado;
                    result.ValorVentaAprobar = wsmy639Response.ValorVentaAprobar;
                    result.ValorVentaAprobarUsd = wsmy639Response.ValorVentaAprobarUsd;
                  
                    if (wsmy639Response.IdEstatus=="")
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "SOLICITUD PENDIENTE DE APROBACION";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                        return result;
                    }
                

                    if (wsmy639Response.IdEstatus=="RECH")
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "SOLICITUD RECHAZADA POR EL DPTO PRECIOS";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                        return result;
                    }
                    decimal unitPriceBaseProduction = (decimal)appDetailQuotes.UnitPriceBaseProduction;
                    var flete = (unitPriceBaseProduction* porcflete) / 100;
                    flete = Math.Truncate(100 * flete) / 100;
                    decimal lista = Math.Round(unitPriceBaseProduction+flete, 2);
                    if (lista > 0 && appDetailQuotes.PrecioUsd >=  lista)
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
                        return result;
                    }

                    if (appProduct.RequiereEstimacion==true && wsmy639Response.IdEstatus != "APRO")
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "SOLICITUD NO ESTA APROBADA";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                    }
                    if (wsmy639Response.IdEstatus == "APRO")
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
                      
                        if (appProduct != null)
                        {
                            result.ValorVentaAprobar = wsmy639Response.ValorVentaAprobar;
                            result.ValorVentaAprobarUsd = wsmy639Response.ValorVentaAprobarUsd;
                            result.precioEstimacion = 0;
                            appRecipesByAppDetailQuotesQueryFilter filter = new appRecipesByAppDetailQuotesQueryFilter();

                            filter.Ancho =appDetailQuotes.MedidaOpuesta;
                            filter.Largo = appDetailQuotes.MedidaBasica;
                            filter.UnidadId = appDetailQuotes.IdUnidad;
                            filter.Cantidad = (int)appDetailQuotes.CantidadSolicitada;
                            filter.AppProuctId = appDetailQuotes.IdProducto;
                            filter.AppDetailQuotesId = appDetailQuotes.Id;
                            filter.IdMunicipio = (decimal)general.IdMunicipio;
                            var precio=await _appRecipesByAppDetailQuotesService.GetPrice(filter);
                            decimal cantidadAlternativa = 0;
                            if (precio.Data != null)
                            {

                                cantidadAlternativa = (decimal)precio.Data.CantidadConvertidaAlternativa;
                            }
                            if ((bool)appProduct.RequiereEstimacion || appProduct.CantidadMinima >cantidadAlternativa )
                            {
                                
                                result.precioEstimacion = wsmy639Response.ValorVentaAprobarUsd;
                                result.ValorVentaAprobar = wsmy639Response.ValorVentaAprobar;
                                result.ValorVentaAprobarUsd = wsmy639Response.ValorVentaAprobarUsd;
                                await _unitOfWork.PropuestaRepository.UpdateListaCotizacion(
                                    cotizacionProducto.Cotizacion, (decimal)wsmy639Response.ValorVentaAprobarUsd, flete);
                            }
                        }
                        
                    }
                   
                    
                    
                    
                    
                   
                }
                else
                {
                    result.ValorVentaAprobar = 0;
                    result.ValorVentaAprobarUsd = 0;
                    result.FlagAprobado = new bool?(false);
                    result.FlagCerrado = new bool?(false);
                  
                    long? idMtrTipoMoneda = general.IdMtrTipoMoneda;
                    long num = 1;
                    if (idMtrTipoMoneda.GetValueOrDefault() == num & idMtrTipoMoneda.HasValue)
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "ENVIAR APROBACION";
                    }

                    appDetailQuotes.UnitPriceConverted ??= 0;
                    decimal unitPriceBaseProduction = (decimal)appDetailQuotes.UnitPriceConverted;
                    var flete = (unitPriceBaseProduction* porcflete) / 100;
                    flete = Math.Truncate(100 * flete) / 100;
                    decimal lista = Math.Round(unitPriceBaseProduction+(decimal)flete, 2);
                    if (appDetailQuotes.PrecioUsd >=  lista)
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
                        return result;
                    }
                    else
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "ENVIAR APROBACION";
                    }
                    if (appDetailQuotes.Cantidad < appDetailQuotes.IdProductoNavigation.CantidadMinima)
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        StatusAprobacionDto statusAprobacionDto = result;
                        string str = $"ENVIAR APROBACION POR CANTIDAD MINIMA: {appDetailQuotes.IdProductoNavigation.CantidadMinima}";
                        statusAprobacionDto.StatusString = str;
                    }
                    if (appProduct.RequiereEstimacion==true )
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "REQUIERE ESTIMACION - SOLICITUD NO ESTA APROBADA";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                    }

                }
                wsmy639Response = (Wsmy639)null;
            }
            else
            {
                result.ValorVentaAprobar = 0;
                result.ValorVentaAprobarUsd = 0;
                var flete = (appDetailQuotes.UnitPriceBaseProduction * porcflete) / 100;
                if (appDetailQuotes.PrecioUsd >=  appDetailQuotes.UnitPriceBaseProduction+flete)
                {
                    result.Aprobado = true;
                    result.Color = "prymary";
                    result.StatusString = "APROBADO";
                    return result;
                }
                else
                {
                    result.Aprobado = false;
                    result.Color = "danger";
                    result.StatusString = "ENVIAR APROBACION";
                }
                long? idMtrTipoMoneda = general.IdMtrTipoMoneda;
                long num = 1;
                if (idMtrTipoMoneda.GetValueOrDefault() == num & idMtrTipoMoneda.HasValue)
                {
                    result.Aprobado = false;
                    result.Color = "danger";
                    result.StatusString = "ENVIAR APROBACION";
                }
                
                
                Decimal cantidad = appDetailQuotes.Cantidad;
             
                if (appDetailQuotes.Cantidad < appDetailQuotes.IdProductoNavigation.CantidadMinima)
                {
                    result.Aprobado = false;
                    result.Color = "danger";
                    StatusAprobacionDto statusAprobacionDto = result;
                    string str =
                        $"ENVIAR APROBACION POR CANTIDAD MINIMA: {appDetailQuotes.IdProductoNavigation.CantidadMinima}";
                    statusAprobacionDto.StatusString = str;
                }
            }
            StatusAprobacionDto statusAprobacionDto1 = result;
            result = (StatusAprobacionDto)null;
            general = (AppGeneralQuotes)null;
            return statusAprobacionDto1;
        }

        public async Task<bool> RequiereAprobacionAppGeneralQuotesId(int appGeneralQuotesId,AppGeneralQuotes  appGeneralQuotes)
        {
            bool resultDto = false;
            try
            {
                
                //var appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appGeneralQuotesId);
                if (appGeneralQuotes.IdEstatus >= 5)
                {
                    resultDto = false;
                    return resultDto;
                }
             
                var appDetailQuotes =
                    await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotesId);
                if (appDetailQuotes.Count <= 0)
                {
                    resultDto = true;
                    return resultDto;
                }
                foreach (var item in appDetailQuotes)
                {

                    //var producto = await _unitOfWork.AppProductsRepository.GetById(item.IdProducto);
                    decimal flete ;
                    if (item.PorcFlete == null || item.PorcFlete.Value == 0)
                    {
                      flete=  await GetFlete((decimal)appGeneralQuotes.IdMunicipio,item.IdProducto);
                    }
                    else
                    {
                        flete = (decimal)item.PorcFlete;
                    }
                  
                  
                    if (item.SolicitarPrecio==false )
                    {
                        resultDto = false;
                    }
                    else
                    {
                        var statusAprobaion =await this.StatusAprobacion(item,(decimal)flete);
                        if (!statusAprobaion.Aprobado)
                        {
                            resultDto = true;
                            return true;

                        }
                    }
                   
                    
                    
                    //var detail = await this.GetById(item.Id);
                   
                }
                
              
                return resultDto;
            }
            catch (Exception ex)
            {
                return resultDto;
            }
        }


        public async Task<string> DescripcionProductosCotizadosAppGeneralQuotesId(int appGeneralQuotesId)
        {
            string resultDto = String.Empty;
            try
            {
                foreach (AppDetailQuotes appDetailQuotes in await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotesId))
                {

                    resultDto = resultDto + " " + appDetailQuotes.NombreComercialProducto;
                    return resultDto;

                }
                return resultDto;
            }
            catch (Exception ex)
            {
                return resultDto;
            }
        }


        //#####################################################################################################################################
        // S E R V I C E S  O D D O 
        //#####################################################################################################################################
        public async Task<ApiResponse<DetailGetDto>> OdooDetailUpdate(DetailUpdateDto dto)
        {
            DetailGetDto resultDto = new DetailGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<DetailGetDto> response = new ApiResponse<DetailGetDto>(resultDto);

            try
            {
                var detail = await GetById(dto.Id);
                if (detail != null)//update
                {
                    AppDetailQuotesUpdateDto appDetailQuotesUpdateDto = new AppDetailQuotesUpdateDto();
                    //Armar el dto a enviar a UpdateAppDetailQuotes()



                    var resultUpdate = await UpdateAppDetailQuotes(appDetailQuotesUpdateDto);
                    if (resultUpdate.Meta.IsValid)
                    {
                        DetailGetDto detailGetDto = new DetailGetDto();
                        //Armar el dto Result
                        detailGetDto.Id = resultUpdate.Data.Id;
                        detailGetDto.Cotizacion = resultUpdate.Data.Cotizacion;
                        detailGetDto.GeneralId = resultUpdate.Data.AppGeneralQuotesId;

                        metadata = resultUpdate.Meta;
                        response.Meta = metadata;
                        response.Data = detailGetDto;
                        return response;
                    }
                    else
                    {
                        metadata = resultUpdate.Meta;
                        response.Meta = metadata;
                        response.Data = null;
                        return response;
                    }

                }
                else//insert
                {
                    AppDetailQuotesCreateDto appDetailQuotesCreateDto = new AppDetailQuotesCreateDto();

                    //Armar dto AppDetailQuotesCreateDto a enviar a InsertAppDetailQuotes()


                    var resultUpdate = await InsertAppDetailQuotes(appDetailQuotesCreateDto);
                    if (resultUpdate.Meta.IsValid)
                    {
                        DetailGetDto detailGetDto = new DetailGetDto();
                        //Armar el dto Result
                        detailGetDto.Id = resultUpdate.Data.Id;
                        detailGetDto.Cotizacion = resultUpdate.Data.Cotizacion;
                        detailGetDto.GeneralId = resultUpdate.Data.AppGeneralQuotesId;

                        metadata = resultUpdate.Meta;
                        response.Meta = metadata;
                        response.Data = detailGetDto;
                        return response;
                    }
                    else
                    {
                        metadata = resultUpdate.Meta;
                        response.Meta = metadata;
                        response.Data = null;
                        return response;
                    }

                }

            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Meta = metadata;
                response.Data = resultDto;
                return response;
            }

        }




    }
}
