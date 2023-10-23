﻿using AppService.Core.CustomEntities;
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
using System.Linq;
using System.Threading.Tasks;

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
          IAppRecipesByAppDetailQuotesService appRecipesByAppDetailQuotesService)
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
        }

        public async Task<List<AppDetailQuotes>> GetAll() => await this._unitOfWork.AppDetailQuotesRepository.GetAll();

        public async Task<ApiResponse<List<AppDetailQuotesGetDto>>> GetListAppDetailQuoteByAppGeneralQuotesId(int appGeneralQuotesId)
        {
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };

            List<AppDetailQuotesGetDto> resultDto = new List<AppDetailQuotesGetDto>();

            ApiResponse<List<AppDetailQuotesGetDto>> response = new ApiResponse<List<AppDetailQuotesGetDto>>(resultDto);

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

                    item.StatusAprobacionDto = await this.StatusAprobacion(byId3);
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
                appDetailQuotes.CreatedAt = DateTime.Now;
                appDetailQuotes.FechaRegistro = DateTime.Now;
                appDetailQuotes.UserCreate = "";
                appDetailQuotes.Producto = appProducts.Code;
                appDetailQuotes.ObsSolicitud = appDetailQuotesDto.ObsSolicitud;
                appDetailQuotes.MedidaBasica = new Decimal?(appDetailQuotesDto.MedidaBasica);
                appDetailQuotes.MedidaOpuesta = new Decimal?(appDetailQuotesDto.MedidaOpuesta);
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
                appDetailQuotes.CalculoId = appDetailQuotesDto.CalculoId;
                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorProducto)
                {
                    var precio =
                        await _appRecipesByAppDetailQuotesService.GetPrecioProductoCantidadOfficeProduct(appDetailQuotes.IdProducto, appDetailQuotes.Cantidad);
                    if (precio != null)
                    {
                        appDetailQuotes.CalculoId = precio.CalculoId;
                    }
                }
                appDetailQuotes.UnitPriceBaseProduction = appDetailQuotesDto.PrecioLista;
                appDetailQuotes.UnitPriceBaseProductionMaximo = appDetailQuotesDto.UnitPriceBaseProductionMaximo;

                if (appDetailQuotesDto.Presentacion == null) appDetailQuotesDto.Presentacion = "";
                if (appDetailQuotesDto.Salida == null) appDetailQuotesDto.Salida = "";
                if (appDetailQuotesDto.Forma == null) appDetailQuotesDto.Forma = "";
                appDetailQuotes.Presentacion = appDetailQuotesDto.Presentacion;
                appDetailQuotes.Salida = appDetailQuotesDto.Salida;
                appDetailQuotes.Forma = appDetailQuotesDto.Forma;

                appDetailQuotesInserted = await this.Insert(appDetailQuotes);
                await this._unitOfWork.SaveChangesAsync();
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
                
            
                
                AppDetailQuotes appDetailQuotes = await this.GetById(appDetailQuotesUpdateDto.Id);
                if (appDetailQuotes == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "No existe Detalle para ese producto!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
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
                if (appDetailQuotesUpdateDto.PrecioLista <= 0M)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Debe indicar precio lista";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
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
                appDetailQuotes.SolicitarPrecio = new bool?(appDetailQuotesUpdateDto.SolicitarPrecio);

                appDetailQuotes.Medidas = await getMedidas(appDetailQuotes.IdProducto);
                appDetailQuotes.Papeles = await _unitOfWork.Wpry240Repository.GetPapeles(appDetailQuotes.Cotizacion);
                appDetailQuotes.TipoPapeles = await _unitOfWork.Wpry240Repository.GetPapelesTipo(appDetailQuotes.Cotizacion);
                appDetailQuotes.Tintas = await _unitOfWork.Wpry241Repository.GetTintas(appDetailQuotes.Cotizacion);
                appDetailQuotes.CantPartes = await _unitOfWork.Wpry240Repository.GetCantPartes(appDetailQuotes.Cotizacion);
                appDetailQuotes.CantTintas = 0;
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




                AppDetailQuotes appDetailQuotesUpdated = await this.Update(appDetailQuotes);


                AppGeneralQuotes general = await this._unitOfWork.AppGeneralQuotesRepository.GetByIdForUpdate(appDetailQuotes.AppGeneralQuotesId);
                if (general != null)
                {
                    general.FechaActualiza = DateTime.Now;

                    this._unitOfWork.AppGeneralQuotesRepository.Update(general);
                    await this._unitOfWork.SaveChangesAsync();
                }



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
                    _unitOfWork.AppDetailQuotesRepository.Update(appDetailUpdate);
                    await _unitOfWork.SaveChangesAsync();

                }
            }

        }

        public async Task ArmaDataReportEtiquetasPrime(string cotizacion)
        {
            var appDetailQuotes = await _unitOfWork.AppDetailQuotesRepository.GetByQuotesCotizacion(cotizacion);
            if (appDetailQuotes.Count > 0)
            {
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
                        reporteNew.Cliente = $"({generalQuotes.IdCliente}) {generalQuotes.IdClienteNavigation.Nombre.Trim()} RIF {generalQuotes.IdClienteNavigation.NoRegTribut.Trim()}";
                        reporteNew.Direccion = $"{generalQuotes.IdClienteNavigation.Direccion.Trim()} {generalQuotes.IdClienteNavigation.Direccion1.Trim()}";
                        
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
                        reporte.Cliente = $"({generalQuotes.IdCliente}) {generalQuotes.IdClienteNavigation.Nombre.Trim()} RIF {generalQuotes.IdClienteNavigation.NoRegTribut.Trim()}";
                        reporte.Direccion = $"{generalQuotes.IdClienteNavigation.Direccion.Trim()} {generalQuotes.IdClienteNavigation.Direccion1.Trim()}";
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
            result = (decimal)calculo.Quantity;


            return result;
        }

        public async Task<decimal> GetPrecioClise(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            decimal result;
            var calculo = listCalculo.Where(x => x.Code == "PRECIOGOMASTIL").FirstOrDefault();
            result = (decimal)calculo.Quantity;


            return result;
        }
        public async Task<decimal> GetPrecioTroquel(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            decimal result;
            var calculo = listCalculo.Where(x => x.Code == "TROQUEL").FirstOrDefault();
            result = (decimal)calculo.TotalCost;


            return result;
        }


        public async Task<string> GetSustratoBase(List<AppRecipesByAppDetailQuotesHistory> listCalculo)
        {
            string result;
            var calculo= listCalculo.Where(x=> x.Code== "PAPELPRIMERAPARTE").FirstOrDefault();
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

           


            Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);

            if (renglon == null)
            {
                await this._cotizacionService.IntegrarCotizacion(appDetailQuotes.AppGeneralQuotesId, true);
                renglon=await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);

            }

            var aprobacion= await this._aprobacionesServices.CreateAprobacionAprobada(renglon.Cotizacion, renglon.Renglon, 1, appGanarPerderDto.UsuarioConectado);
            if (aprobacion.Meta.IsValid)
            {
                ApiResponse<Wsmy647> apiResponse = await this._aprobacionesServices.ActivarWORKFLOW(renglon.Cotizacion, renglon.Renglon, 1, appGanarPerderDto.UsuarioConectado, appDetailQuotes);
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

            await this._cotizacionService.IntegrarCotizacion(appDetailQuotes.AppGeneralQuotesId, true);
            metadata.IsValid = true;
            metadata.Message = "Cotizacion Actualizada!!! ";
            response.Meta = metadata;
            response.Data = false;
            return response;
        }

        public async Task<StatusAprobacionDto> StatusAprobacion(
          AppDetailQuotes appDetailQuotes)
        {
            StatusAprobacionDto result = new StatusAprobacionDto();
            AppGeneralQuotes general = await this._unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);
            Wsmy502 cotizacionProducto = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appDetailQuotes.IdProductoNavigation.ExternalCode);
            if (cotizacionProducto != null)
            {
                Wsmy639 wsmy639Response = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(cotizacionProducto.Cotizacion, cotizacionProducto.Renglon, 1);
                if (wsmy639Response != null)
                {
                    if (wsmy639Response.FlagAprobado.Value && !wsmy639Response.FlagCerrado.Value)
                    {
                        wsmy639Response.FlagAprobado = new bool?(false);
                        this._unitOfWork.AprobacionesRepository.Update(wsmy639Response);
                        await this._unitOfWork.SaveChangesAsync();
                    }
                    result.FlagAprobado = wsmy639Response.FlagAprobado;
                    result.FlagCerrado = wsmy639Response.FlagCerrado;
                    result.ValorVentaAprobar = wsmy639Response.ValorVentaAprobar;
                    result.ValorVentaAprobarUsd = wsmy639Response.ValorVentaAprobarUsd;
                  
                    if (result.FlagAprobado.Value && result.FlagCerrado.Value)
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
                        var appProduct = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
                        if (appProduct != null)
                        {
                            result.precioEstimacion = 0;
                            if (appProduct.RequiereEstimacion == null) appProduct.RequiereEstimacion = false;
                            if ((bool)appProduct.RequiereEstimacion)
                            {
                                result.precioEstimacion = wsmy639Response.ValorVentaAprobarUsd;
                            }
                        }
                    }
                    if (!result.FlagAprobado.Value && result.FlagCerrado.Value)
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "Rechazado";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                    }

                    if (!result.FlagAprobado.Value && !result.FlagCerrado.Value)
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "PENDIENTE";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                    }

                    if (wsmy639Response.IdEstatus=="RECHA")
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "SOLICITUD RECHAZADA POR EL DPTO PRECIOS";
                        result.precioEstimacion = 0;
                        result.ValorVentaAprobar = 0;
                        result.ValorVentaAprobarUsd = 0;
                    }
                    if (wsmy639Response.IdEstatus == "APRO")
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
                        var appProduct = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
                        if (appProduct != null)
                        {
                            result.precioEstimacion = 0;
                            if ((bool)appProduct.RequiereEstimacion)
                            {
                                result.precioEstimacion = wsmy639Response.ValorVentaAprobarUsd;
                                result.ValorVentaAprobar = wsmy639Response.ValorVentaAprobar;
                                result.ValorVentaAprobarUsd = wsmy639Response.ValorVentaAprobarUsd;
                            }
                        }
                    }


                    Decimal precioUsd1 = appDetailQuotes.PrecioUsd;
                    Decimal? nullable = appDetailQuotes.UnitPriceBaseProduction;
                    Decimal valueOrDefault1 = nullable.GetValueOrDefault();
                    if (precioUsd1 >= valueOrDefault1 & nullable.HasValue)
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
                    }
                    long? idMtrTipoMoneda = general.IdMtrTipoMoneda;
                    long num = 1;
                    if (idMtrTipoMoneda.GetValueOrDefault() == num & idMtrTipoMoneda.HasValue)
                    {
                        Decimal precioUsd2 = appDetailQuotes.PrecioUsd;
                        nullable = wsmy639Response.ValorVentaAprobarUsd;
                        Decimal valueOrDefault2 = nullable.GetValueOrDefault();
                        if (precioUsd2 < valueOrDefault2 & nullable.HasValue)
                        {
                            result.Aprobado = false;
                            result.Color = "danger";
                            result.StatusString = "ENVIAR APROBACION";
                        }
                    }
                }
                else
                {
                    result.ValorVentaAprobar = 0;
                    result.ValorVentaAprobarUsd = 0;
                    result.FlagAprobado = new bool?(false);
                    result.FlagCerrado = new bool?(false);
                    Decimal precioUsd = appDetailQuotes.PrecioUsd;
                    Decimal? priceBaseProduction = appDetailQuotes.UnitPriceBaseProduction;
                    Decimal valueOrDefault3 = priceBaseProduction.GetValueOrDefault();
                    if (precioUsd < valueOrDefault3 & priceBaseProduction.HasValue)
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        result.StatusString = "ENVIAR APROBACION";
                    }
                    else
                    {
                        result.Aprobado = true;
                        result.Color = "prymary";
                        result.StatusString = "APROBADO";
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
                    Decimal? cantidadMinima = appDetailQuotes.IdProductoNavigation.CantidadMinima;
                    Decimal valueOrDefault4 = cantidadMinima.GetValueOrDefault();
                    if (cantidad < valueOrDefault4 & cantidadMinima.HasValue)
                    {
                        result.Aprobado = false;
                        result.Color = "danger";
                        StatusAprobacionDto statusAprobacionDto = result;
                        cantidadMinima = appDetailQuotes.IdProductoNavigation.CantidadMinima;
                        string str = "ENVIAR APROBACION POR CANTIDAD MINIMA: " + cantidadMinima.ToString();
                        statusAprobacionDto.StatusString = str;
                    }


                }
                wsmy639Response = (Wsmy639)null;
            }
            else
            {
                result.ValorVentaAprobar = 0;
                result.ValorVentaAprobarUsd = 0;
                Decimal precioUsd = appDetailQuotes.PrecioUsd;
                Decimal? nullable = appDetailQuotes.UnitPriceBaseProduction;
                Decimal valueOrDefault5 = nullable.GetValueOrDefault();
                if (precioUsd < valueOrDefault5 & nullable.HasValue)
                {
                    result.Aprobado = false;
                    result.Color = "danger";
                    result.StatusString = "ENVIAR APROBACION";
                }
                else
                {
                    result.Aprobado = true;
                    result.Color = "prymary";
                    result.StatusString = "APROBADO";
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
                nullable = appDetailQuotes.IdProductoNavigation.CantidadMinima;
                Decimal valueOrDefault6 = nullable.GetValueOrDefault();
                if (cantidad < valueOrDefault6 & nullable.HasValue)
                {
                    result.Aprobado = false;
                    result.Color = "danger";
                    StatusAprobacionDto statusAprobacionDto = result;
                    nullable = appDetailQuotes.IdProductoNavigation.CantidadMinima;
                    string str = "ENVIAR APROBACION POR CANTIDAD MINIMA: " + nullable.ToString();
                    statusAprobacionDto.StatusString = str;
                }
            }
            StatusAprobacionDto statusAprobacionDto1 = result;
            result = (StatusAprobacionDto)null;
            general = (AppGeneralQuotes)null;
            return statusAprobacionDto1;
        }

        public async Task<bool> RequiereAprobacionAppGeneralQuotesId(int appGeneralQuotesId)
        {
            bool resultDto = false;
            try
            {
                foreach (AppDetailQuotes appDetailQuotes in await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotesId))
                {
                    if (!(await this.StatusAprobacion(await this.GetById(appDetailQuotes.Id))).Aprobado)
                    {
                        resultDto = true;
                        return resultDto;
                    }
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