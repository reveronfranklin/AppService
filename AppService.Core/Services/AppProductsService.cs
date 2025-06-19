﻿
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Odoo.Producto.OdooProducts;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Core.Services
{
    public class AppProductsService : IAppProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppUnitsService _appUnitsService;
        private readonly IAppProductVariableSearchTextService _appProductVariableSearchTextService;
        private readonly IMtrTipoMonedaService _mtrTipoMonedaService;
        private readonly IAppSubCategoryService _appSubCategoryService;
        private readonly IOdooClient _odooClient;
        private readonly IAppProductConversionService _appProductConversionService;
        private readonly PaginationOptions _paginationOptions;

        public AppProductsService(
          IUnitOfWork unitOfWork,
          IOptions<PaginationOptions> options,
          IMapper mapper,
          IAppUnitsService appUnitsService,
          IAppProductVariableSearchTextService appProductVariableSearchTextService,
          IMtrTipoMonedaService mtrTipoMonedaService,
          IAppSubCategoryService appSubCategoryService,
          IOdooClient odooClient,
          IAppProductConversionService appProductConversionService)
        {
            this._unitOfWork = unitOfWork;
            this._appUnitsService = appUnitsService;
            this._appProductVariableSearchTextService = appProductVariableSearchTextService;
            this._mtrTipoMonedaService = mtrTipoMonedaService;
            this._appSubCategoryService = appSubCategoryService;
            _odooClient = odooClient;
            _appProductConversionService = appProductConversionService;
            this._paginationOptions = options.Value;
            this._mapper = mapper;
        }

        public async Task<List<AppProducts>> GetAll() => await this._unitOfWork.AppProductsRepository.GetAll();

        public async Task<ApiResponse<List<AppProductsGetDto>>> GetAll(
          AppProdutsQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? this._paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? this._paginationOptions.DefaultPageSize : filters.PageSize;
            List<AppProductsGetDto> data = new List<AppProductsGetDto>();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<List<AppProductsGetDto>> response = new ApiResponse<List<AppProductsGetDto>>(data);
            try
            {
                List<AppProducts> allFilter = await this._unitOfWork.AppProductsRepository.GetAllFilter(filters);
                if (allFilter != null)
                {
                    List<AppProductsGetDto> appProductsDto = this._mapper.Map<List<AppProductsGetDto>>((object)allFilter);
                    foreach (AppProductsGetDto item in appProductsDto)
                    {
                        AppProductConversionFilter appProductConversionFilter = new AppProductConversionFilter();
                        appProductConversionFilter.AppProductsId = item.Id;
                        var conversiones = _appProductConversionService.GetAllByProduct(appProductConversionFilter);
                        item.Conversiones = conversiones.Data;

                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(item.Id);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            item.PrecioPorRango = true;
                            item.AppPriceDto = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                            var porcentajePrecioMaximo = await _unitOfWork.AppConfigAppRepository.GetByKey("UTILIDADOPMAX");
                            foreach (var itemPrecio in item.AppPriceDto)
                            {
                                decimal porcentaje = decimal.Parse(porcentajePrecioMaximo.Valor);
                                var adicional = (itemPrecio.Precio * porcentaje) /100;
                                itemPrecio.PrecioMaximo = itemPrecio.Precio + adicional;
                            }
                        }
                        else
                            item.PrecioPorRango = false;

                        item.Link = item.UrlImage == "" || item.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + item.UrlImage;
                        AppUnits byId1 = await this._appUnitsService.GetById(item.AppUnitsId.Value);
                        if (byId1 != null)
                            item.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);
                        AppUnits byId2 = await this._appUnitsService.GetById(item.ProductionUnitId.Value);
                        if (byId2 != null)
                            item.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                        MtrTipoMoneda byId3 = await this._mtrTipoMonedaService.GetById(item.PrymaryMtrMonedaId.Value);
                        if (byId3 != null)
                            item.PrymaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId3);
                        MtrTipoMoneda byId4 = await this._mtrTipoMonedaService.GetById(item.SecundaryMtrMonedaId.Value);
                        if (byId4 != null)
                            item.SecundaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId4);
                        AppSubCategory byId5 = await this._appSubCategoryService.GetById(item.AppSubCategoryId.Value);
                        if (byId5 != null)
                            item.AppSubCategoryGetDto = this._mapper.Map<AppSubCategoryGetDto>((object)byId5);
                    }
                    response.Data = appProductsDto;
                    response.Meta = metadata;
                    return response;
                }
                metadata.IsValid = true;
                metadata.Message = "No Data....";
                response.Data = (List<AppProductsGetDto>)null;
                response.Meta = metadata;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Data = (List<AppProductsGetDto>)null;
                response.Meta = metadata;
                return response;
            }
        }

        public async Task<AppProductsGetDto> GetByIdProducto(int productoId)
        {
            AppProductsGetDto result = new AppProductsGetDto();
            var product = await _unitOfWork.AppProductsRepository.GetById(productoId);
            if (product != null)
            {
                AppProductsGetDto appProductsDto = this._mapper.Map<AppProductsGetDto>((object)product);
                        AppProductConversionFilter appProductConversionFilter = new AppProductConversionFilter();
                        appProductConversionFilter.AppProductsId = appProductsDto.Id;
                        var conversiones = _appProductConversionService.GetAllByProduct(appProductConversionFilter);
                        appProductsDto.Conversiones = conversiones.Data;

                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(appProductsDto.Id);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            appProductsDto.PrecioPorRango = true;
                            appProductsDto.AppPriceDto = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                            var porcentajePrecioMaximo = await _unitOfWork.AppConfigAppRepository.GetByKey("UTILIDADOPMAX");
                            foreach (var itemPrecio in appProductsDto.AppPriceDto)
                            {
                                decimal porcentaje = decimal.Parse(porcentajePrecioMaximo.Valor);
                                var adicional = (itemPrecio.Precio * porcentaje) /100;
                                itemPrecio.PrecioMaximo = itemPrecio.Precio + adicional;
                            }
                        }
                        else
                            appProductsDto.PrecioPorRango = false;

                        appProductsDto.Link = appProductsDto.UrlImage == "" || appProductsDto.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + appProductsDto.UrlImage;
                        AppUnits byId1 = await this._appUnitsService.GetById(appProductsDto.AppUnitsId.Value);
                        if (byId1 != null)
                            appProductsDto.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);
                        AppUnits byId2 = await this._appUnitsService.GetById(appProductsDto.ProductionUnitId.Value);
                        if (byId2 != null)
                            appProductsDto.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                        MtrTipoMoneda byId3 = await this._mtrTipoMonedaService.GetById(appProductsDto.PrymaryMtrMonedaId.Value);
                        if (byId3 != null)
                            appProductsDto.PrymaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId3);
                        MtrTipoMoneda byId4 = await this._mtrTipoMonedaService.GetById(appProductsDto.SecundaryMtrMonedaId.Value);
                        if (byId4 != null)
                            appProductsDto.SecundaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId4);
                        AppSubCategory byId5 = await this._appSubCategoryService.GetById(appProductsDto.AppSubCategoryId.Value);
                        if (byId5 != null)
                            appProductsDto.AppSubCategoryGetDto = this._mapper.Map<AppSubCategoryGetDto>((object)byId5);
                        result = appProductsDto;
            }

         
            return result;
        }
        
        public async Task<ResultDto<List<AppProductsGetDto>>> GetAllBySubCategory(
          AppProdutsQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? this._paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? this._paginationOptions.DefaultPageSize : filters.PageSize;
            List<AppProductsGetDto> data = new List<AppProductsGetDto>();

           ResultDto<List<AppProductsGetDto>> response = new ResultDto<List<AppProductsGetDto>>(null);
            try
            {
                var allFilter = await this._unitOfWork.AppProductsRepository.GetAllFilterPaginate(filters);
                if (allFilter.Data != null)
                {
                    List<AppProductsGetDto> appProductsDto = this._mapper.Map<List<AppProductsGetDto>>((object)allFilter.Data);
                    foreach (AppProductsGetDto item in appProductsDto)
                    {
                        AppProductConversionFilter appProductConversionFilter = new AppProductConversionFilter();
                        appProductConversionFilter.AppProductsId = item.Id;
                        var conversiones = _appProductConversionService.GetAllByProduct(appProductConversionFilter);
                        item.Conversiones = conversiones.Data;

                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(item.Id);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            item.PrecioPorRango = true;
                            item.AppPriceDto = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                            var porcentajePrecioMaximo = await _unitOfWork.AppConfigAppRepository.GetByKey("UTILIDADOPMAX");
                            foreach (var itemPrecio in item.AppPriceDto)
                            {
                                decimal porcentaje = decimal.Parse(porcentajePrecioMaximo.Valor);
                                var adicional = (itemPrecio.Precio * porcentaje) /100;
                                itemPrecio.PrecioMaximo = itemPrecio.Precio + adicional;
                            }
                        }
                        else
                            item.PrecioPorRango = false;

                        item.Link = item.UrlImage == "" || item.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + item.UrlImage;
                        AppUnits byId1 = await this._appUnitsService.GetById(item.AppUnitsId.Value);
                        if (byId1 != null)
                            item.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);
                        AppUnits byId2 = await this._appUnitsService.GetById(item.ProductionUnitId.Value);
                        if (byId2 != null)
                            item.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                        MtrTipoMoneda byId3 = await this._mtrTipoMonedaService.GetById(item.PrymaryMtrMonedaId.Value);
                        if (byId3 != null)
                            item.PrymaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId3);
                        MtrTipoMoneda byId4 = await this._mtrTipoMonedaService.GetById(item.SecundaryMtrMonedaId.Value);
                        if (byId4 != null)
                            item.SecundaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId4);
                        AppSubCategory byId5 = await this._appSubCategoryService.GetById(item.AppSubCategoryId.Value);
                        if (byId5 != null)
                            item.AppSubCategoryGetDto = this._mapper.Map<AppSubCategoryGetDto>((object)byId5);
                    }
                    response.Data = appProductsDto;
                    response.IsValid=allFilter.IsValid;
                    response.Message = allFilter.Message;
                    response.CantidadRegistros = allFilter.CantidadRegistros;
                    response.Page = allFilter.Page;
                    response.TotalPage = allFilter.TotalPage;
                    return response;
                }
             
                response.Data = (List<AppProductsGetDto>)null;
                response.Message ="No Data....";
                return response;
            }
            catch (Exception ex)
            {
                response.IsValid=false;
                response.Data = (List<AppProductsGetDto>)null;
                response.Message =  ex.InnerException.Message;
                return response;
            }
        }

        
        public async Task<ApiResponse<List<AppProductsGetDto>>> GetAllByVariable(
          List<AppVariableSearchCompareQueryFilter> filter,
          int subCategoryId)
        {
            List<AppProductsGetDto> data = new List<AppProductsGetDto>();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<List<AppProductsGetDto>> response = new ApiResponse<List<AppProductsGetDto>>(data);
            try
            {
                if (filter.Count <= 0)
                {
                    metadata.IsValid = true;
                    metadata.Message = "No Data....";
                    response.Data = (List<AppProductsGetDto>)null;
                    response.Meta = metadata;
                    return response;
                }
                List<AppProducts> allByVarible = await this._unitOfWork.AppProductsRepository.GetAllByVarible(filter, subCategoryId);
                if (allByVarible != null)
                {
                    List<AppProductsGetDto> appProductsDto = this._mapper.Map<List<AppProductsGetDto>>((object)allByVarible);
                    foreach (AppProductsGetDto item in appProductsDto)
                    {
                        AppProductConversionFilter appProductConversionFilter = new AppProductConversionFilter();
                        appProductConversionFilter.AppProductsId = item.Id;
                        var conversiones = _appProductConversionService.GetAllByProduct(appProductConversionFilter);
                        item.Conversiones = conversiones.Data;

                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(item.Id);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            item.PrecioPorRango = true;
                            item.AppPriceDto = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                        }
                        else
                            item.PrecioPorRango = false;
                        AppUnits byId1 = await this._appUnitsService.GetById(item.AppUnitsId.Value);
                        if (byId1 != null)
                            item.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);
                        AppUnits byId2 = await this._appUnitsService.GetById(item.ProductionUnitId.Value);
                        if (byId2 != null)
                            item.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                        item.Link = item.UrlImage == "" || item.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + item.UrlImage;
                    }
                    response.Data = appProductsDto;
                    response.Meta = metadata;
                    return response;
                }
                metadata.IsValid = true;
                metadata.Message = "No Data....";
                response.Data = (List<AppProductsGetDto>)null;
                response.Meta = metadata;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Data = (List<AppProductsGetDto>)null;
                response.Meta = metadata;
                return response;
            }
        }

        
        public async Task<ResultDto<List<AppProductsGetDto>>> GetAllByVariableSearch(
          List<AppVariableSearchCompareQueryFilter> filter,
          int subCategoryId)
        {
            List<AppProductsGetDto> data = new List<AppProductsGetDto>();
         
            ResultDto<List<AppProductsGetDto>> response = new ResultDto<List<AppProductsGetDto>>(data);
            try
            {
                if (filter.Count <= 0)
                {
                    response.IsValid = true;
                    response.Message = "No Data....";
                    response.Data = (List<AppProductsGetDto>)null;
                    response.CantidadRegistros = 0;
                    response.Page = 0;
                    response.TotalPage = 0;
                    return response;
                }
                List<AppProducts> allByVarible = await this._unitOfWork.AppProductsRepository.GetAllByVarible(filter, subCategoryId);
                if (allByVarible != null)
                {
                    List<AppProductsGetDto> appProductsDto = this._mapper.Map<List<AppProductsGetDto>>((object)allByVarible);
                    foreach (AppProductsGetDto item in appProductsDto)
                    {
                        AppProductConversionFilter appProductConversionFilter = new AppProductConversionFilter();
                        appProductConversionFilter.AppProductsId = item.Id;
                        var conversiones = _appProductConversionService.GetAllByProduct(appProductConversionFilter);
                        item.Conversiones = conversiones.Data;

                        List<AppPrice> allByAppProduct = await this._unitOfWork.AppPriceRepository.GetAllByAppProduct(item.Id);
                        if (allByAppProduct != null && allByAppProduct.Count > 0)
                        {
                            item.PrecioPorRango = true;
                            item.AppPriceDto = this._mapper.Map<List<AppPriceDto>>((object)allByAppProduct);
                        }
                        else
                            item.PrecioPorRango = false;
                        AppUnits byId1 = await this._appUnitsService.GetById(item.AppUnitsId.Value);
                        if (byId1 != null)
                            item.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);
                        AppUnits byId2 = await this._appUnitsService.GetById(item.ProductionUnitId.Value);
                        if (byId2 != null)
                            item.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                        item.Link = item.UrlImage == "" || item.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + item.UrlImage;
                    }
                    response.Data = appProductsDto;
                    response.IsValid = true;
                    response.Message = "";
                    response.CantidadRegistros = appProductsDto.Count;
                    response.Page = 1;
                    response.TotalPage = 1;
                    return response;
                }
                response.IsValid = true;
                response.Message = "No Data....";
                response.Data = (List<AppProductsGetDto>)null;
                response.CantidadRegistros = 0;
                response.Page = 0;
                response.TotalPage = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.Message = ex.InnerException.Message;;
                response.Data = (List<AppProductsGetDto>)null;
                response.CantidadRegistros = 0;
                response.Page = 0;
                response.TotalPage = 0;
                return response;
            }
        }

        
        public async Task<AppProducts> GetById(int id) => await this._unitOfWork.AppProductsRepository.GetById(id);

        public async Task<AppProducts> GetByCode(string code) => await this._unitOfWork.AppProductsRepository.GetByCode(code);

        public async Task<Decimal> GetPrecio(int appProductId, Decimal cantidad)
        {
            Decimal result = 0M;
            if (!await this._unitOfWork.AppPriceRepository.ProductExist(appProductId))
            {
                AppProducts byId = await this.GetById(appProductId);
                if (byId != null)
                    result = byId.UnitPrice;
            }
            else
            {
                AppPrice productoCantidad = await this._unitOfWork.AppPriceRepository.GetByProductoCantidad(appProductId, cantidad);
                if (productoCantidad != null)
                {
                    result = productoCantidad.Precio;
                }
                else
                {
                    AppPrice firstPriceByProduct = await this._unitOfWork.AppPriceRepository.GetFirstPriceByProduct(appProductId);
                    if (firstPriceByProduct != null)
                        result = firstPriceByProduct.Precio;
                }
            }
            return result;
        }

        public async Task<AppProducts> Insert(AppProducts appProducts)
        {
            var civy004 = _unitOfWork.Csmy036Repository.GetCivy004ByCode(appProducts.ExternalCode.Trim());
            if (civy004 != null)
            {
                appProducts.Inventariable = true;
            }
            else
            {
                appProducts.Inventariable = false;
            }

            await this._unitOfWork.AppProductsRepository.Add(appProducts);
            await this._unitOfWork.SaveChangesAsync();
            return appProducts;
        }

        public async Task<ApiResponse<AppProductsGetDto>> InsertProducts(AppProductsCreateDto appProductsCreateDto)
        {
            AppProductsGetDto resultDto = new AppProductsGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppProductsGetDto> response = new ApiResponse<AppProductsGetDto>(resultDto);
            try
            {
                if (appProductsCreateDto.UrlImage == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Url Imagen no valida";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appProductsCreateDto.ExternalCode == "")
                {
                    metadata.IsValid = false;
                    metadata.Message = "Codigo Externo no es valido";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(appProductsCreateDto.ExternalCode.Trim());
                if (csmy036 == null)
                {

                    metadata.IsValid = false;
                    metadata.Message = "Error en configuracion de producto,(External Code Invalido) ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                if (await this.GetByCode(appProductsCreateDto.Code) != null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Codigo de producto ya existe, verifique por favor";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._appUnitsService.GetById(appProductsCreateDto.AppUnitsId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Unidad venta del Producto No existe";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._appUnitsService.GetById(appProductsCreateDto.ProductionUnitId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Unidad Produccion del Producto No existe";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._unitOfWork.MtrTipoMonedaRepository.GetById(appProductsCreateDto.PrymaryMtrMonedaId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Moneda Primaria No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._unitOfWork.MtrTipoMonedaRepository.GetById(appProductsCreateDto.SecundaryMtrMonedaId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Moneda Secundaria No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._unitOfWork.AppSubCategoryRepository.GetById(appProductsCreateDto.AppSubCategoryId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Subcategoria No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }


                AppProducts appProducts = this._mapper.Map<AppProducts>((object)appProductsCreateDto);


                var civy004 = _unitOfWork.Csmy036Repository.GetCivy004ByCode(appProducts.ExternalCode.Trim());
                if (civy004 != null)
                {
                    appProducts.Inventariable = true;
                }
                else
                {
                    appProducts.Inventariable = false;
                }

                appProducts.ExternalCode = appProducts.ExternalCode.Trim();
                appProducts.RequiereEstimacion = appProductsCreateDto.RequiereEstimacion;
                appProducts.CreatedAt = DateTime.Now;
                appProducts.UserCreate = appProductsCreateDto.UsuarioConectado;
                appProducts.UserUpdate = appProductsCreateDto.UsuarioConectado;
                appProducts.UpdatedAt = new DateTime?(DateTime.Now);
                if (appProducts.VariablesSearchText == null)
                    appProducts.VariablesSearchText = "";
                AppProducts source = await this.Insert(appProducts);

                var sendToOdoo = await _unitOfWork.AppConfigAppRepository.GetByKey("SendProductToOdoo");
                if (source != null)
                {


                    if (sendToOdoo.Valor.Trim() == "1")
                    {
                        string tipoDeCalculo = source.TipoCalculo.ToString();
                        if (source.TipoCalculo > 3)
                        {
                            tipoDeCalculo = "3";
                        }
                        var odooProduct = await GetOdooProduct(source.Description1, source.Id, tipoDeCalculo, source.UnitPrice);
                        //JsonSerializerOptions options = new JsonSerializerOptions
                        //{
                        //    IgnoreNullValues = true,
                        //    WriteIndented = true,
                        //    PropertyNameCaseInsensitive = true
                        //};
                        //string json = JsonSerializer.Serialize(odooProduct, options);
                        string json1 = JsonConvert.SerializeObject(odooProduct);
                        StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");
                        Metadata result = await _odooClient.Post(data);
                    }


                    resultDto = this._mapper.Map<AppProductsGetDto>((object)source);
                    AppUnits byId = await this._appUnitsService.GetById(source.AppUnitsId.Value);
                    if (byId != null)
                        resultDto.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId);
                    resultDto.Link = resultDto.UrlImage == "" || resultDto.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + resultDto.UrlImage;
                    metadata.IsValid = true;
                    metadata.Message = "Producto: " + resultDto.Code + " creado Satisfactoriamente!!";
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

        public async Task<AppProducts> Update(AppProducts appProducts)
        {
            try
            {
                var civy004 = _unitOfWork.Csmy036Repository.GetCivy004ByCode(appProducts.ExternalCode.Trim());
                if (civy004 != null)
                {
                    appProducts.Inventariable = false;
                }
                else
                {
                    appProducts.Inventariable = true;
                }


                if (await this.GetById(appProducts.Id) == null) return null;
                this._unitOfWork.AppProductsRepository.Update(appProducts);
                await this._unitOfWork.SaveChangesAsync();
                return await this.GetById(appProducts.Id);
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                return (AppProducts)null;
            }
        }
        public async Task<decimal> CantidaTintasProducto(int idProduct)
        {
            var recipes = await _unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(idProduct, "CANT_TINTAS");
            if (recipes.Count > 0)
            {
                var recipe = recipes[0];
                return (decimal)recipe.Quantity;
            }
            else
            {
                return 0;
            }
        }

        public async Task SendAllToOdoo()
        {
            var appProducts = await _unitOfWork.AppProductsRepository.GetAll();
            await UpdateProductsToOdoo(appProducts);
        }

        public async Task<ApiResponse<AppProductsGetDto>> UpdateProducts(AppProductsUpdateDto appProductsUpdateDto)
        {
            AppProductsGetDto resultDto = new AppProductsGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppProductsGetDto> response = new ApiResponse<AppProductsGetDto>(resultDto);
            try
            {
                AppProducts appProductsFind = await this.GetById(appProductsUpdateDto.Id);
                if (appProductsFind == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Codigo de producto no existe, verifique por favor";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (appProductsUpdateDto.ExternalCode == "")
                {
                    metadata.IsValid = false;
                    metadata.Message = "Codigo Externo no es valido";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(appProductsUpdateDto.ExternalCode.Trim());
                if (csmy036 == null)
                {

                    metadata.IsValid = false;
                    metadata.Message = "Error en configuracion de producto,(External Code Invalido) ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                if (await this._appUnitsService.GetById(appProductsUpdateDto.AppUnitsId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Unidad venta del Producto No existe";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._appUnitsService.GetById(appProductsUpdateDto.ProductionUnitId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Unidad Produccion del Producto No existe";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._unitOfWork.MtrTipoMonedaRepository.GetById(appProductsUpdateDto.PrymaryMtrMonedaId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Moneda Primaria No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._unitOfWork.MtrTipoMonedaRepository.GetById(appProductsUpdateDto.SecundaryMtrMonedaId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Moneda Secundaria No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                if (await this._unitOfWork.AppSubCategoryRepository.GetById(appProductsUpdateDto.AppSubCategoryId.Value) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Subcategoria No Existe!!! ";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                var precioAnterior = appProductsFind.UnitPrice;

                appProductsFind.ValidadCantidadFija = appProductsUpdateDto.ValidadCantidadFija;
                bool? validadCantidadFija = appProductsUpdateDto.ValidadCantidadFija;
                bool flag = true;
                if (validadCantidadFija.GetValueOrDefault() == flag & validadCantidadFija.HasValue)
                {
                    List<AppRecipes> productIdVariableCode = await this._unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(appProductsUpdateDto.Id, "CANTIDAD");
                    if (productIdVariableCode != null && productIdVariableCode.Count > 0)
                    {
                        appProductsFind.CantidadFija = productIdVariableCode == null ? new Decimal?(0M) : productIdVariableCode.FirstOrDefault<AppRecipes>().Quantity;
                    }

                }
                appProductsFind.RequiereEstimacion = appProductsUpdateDto.RequiereEstimacion;
                appProductsFind.ProductionUnitId = appProductsUpdateDto.ProductionUnitId;
                appProductsFind.AppUnitsId = appProductsUpdateDto.AppUnitsId;
                appProductsFind.AppSubCategoryId = appProductsUpdateDto.AppSubCategoryId;
                appProductsFind.PrymaryMtrMonedaId = appProductsUpdateDto.PrymaryMtrMonedaId;
                appProductsFind.SecundaryMtrMonedaId = appProductsUpdateDto.SecundaryMtrMonedaId;
                appProductsFind.UnitPrice = appProductsUpdateDto.UnitPrice;
                appProductsFind.Description1 = appProductsUpdateDto.Description1.Trim();
                appProductsFind.Description2 = appProductsUpdateDto.Description2.Trim();
                appProductsFind.ExternalCode = appProductsUpdateDto.ExternalCode.Trim();
                appProductsFind.Code = appProductsUpdateDto.Code;
                appProductsFind.UserUpdate = appProductsUpdateDto.UsuarioConectado;
                appProductsFind.UpdatedAt = new DateTime?(DateTime.Now);
                appProductsFind.CantidadMinima = appProductsUpdateDto.CantidadMinima;
                var civy004 = _unitOfWork.Csmy036Repository.GetCivy004ByCode(appProductsFind.ExternalCode.Trim());
                if (civy004 != null)
                {
                    appProductsFind.Inventariable = true;
                }
                else
                {
                    appProductsFind.Inventariable = false;
                }

                AppProducts source = await this.Update(appProductsFind);



                var prices = await _unitOfWork.AppPriceRepository.GetAllByAppProduct(appProductsUpdateDto.Id);
                if (prices.Count == 1)
                {
                    foreach (var item in prices)
                    {
                        item.Precio = appProductsUpdateDto.UnitPrice;
                        _unitOfWork.AppPriceRepository.Update(item);
                        _unitOfWork.SaveChanges();

                    }
                }
                if (prices.Count == 0 && appProductsFind.Inventariable == true)
                {
                    AppPrice newPrice = new AppPrice();
                    newPrice.AppproductsId = appProductsFind.Id;
                    newPrice.Desde = 1;
                    newPrice.Hasta = 99999999;
                    newPrice.Precio = appProductsFind.UnitPrice;
                    await _unitOfWork.AppPriceRepository.Add(newPrice);
                    _unitOfWork.SaveChanges();
                }



                var sendToOdoo = await _unitOfWork.AppConfigAppRepository.GetByKey("SendProductToOdoo");


                if (source != null)
                {



                    if (sendToOdoo.Valor.Trim() == "1")
                    {
                        string tipoDeCalculo = appProductsFind.TipoCalculo.ToString();
                        if (appProductsFind.TipoCalculo > 3)
                        {
                            tipoDeCalculo = "3";
                        }

                        var odooProduct = await GetOdooProduct(appProductsFind.Description1, appProductsFind.Id, tipoDeCalculo, appProductsFind.UnitPrice);
                        //JsonSerializerOptions options = new JsonSerializerOptions
                        //{
                        //    IgnoreNullValues = true,
                        //    WriteIndented = true,
                        //    PropertyNameCaseInsensitive = true
                        //};
                        // string json = JsonSerializer.Serialize(odooProduct, options);
                        string json1 = JsonConvert.SerializeObject(odooProduct);
                        StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");
                        Metadata result = await _odooClient.Post(data);


                    }



                    resultDto = this._mapper.Map<AppProductsGetDto>((object)source);
                    AppUnits byId = await this._appUnitsService.GetById(source.AppUnitsId.Value);
                    if (byId != null)
                        resultDto.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId);
                    resultDto.Link = resultDto.UrlImage == "" || resultDto.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + resultDto.UrlImage;
                    metadata.IsValid = true;
                    metadata.Message = "Producto: " + resultDto.Code + " Actualizado Satisfactoriamente!!";
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


        public async Task UpdateProductsToOdoo(List<AppProducts> appProducts)
        {

            foreach (var item in appProducts)
            {
                if(item.Id == 319774)
                {
                    var detener = "";
                }

                string tipoDeCalculo = item.TipoCalculo.ToString();
                if (item.TipoCalculo > 3)
                {
                    tipoDeCalculo = "3";
                }

                var odooProduct = await GetOdooProduct(item.Description1, item.Id, tipoDeCalculo, item.UnitPrice);
                //JsonSerializerOptions options = new JsonSerializerOptions
                //{
                //    IgnoreNullValues = true,
                //    WriteIndented = true,
                //    PropertyNameCaseInsensitive = true
                //};
                string json1 = JsonConvert.SerializeObject(odooProduct);
                StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");
                Metadata result = await _odooClient.Post(data);

            }

        }
        public async Task<bool> Delete(int id)
        {
            await this._unitOfWork.AppProductsRepository.Delete((int)id);
            await this._unitOfWork.SaveChangesAsync();


            return true;
        }


        public async Task<ApiResponse<bool>> DeleteProduct(AppProductsDeleteDto dto)
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
                if (await this.GetById((int)dto.Id) == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Producto No existe!!!";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }

                var productExistInQuotesDetail = await _unitOfWork.AppDetailQuotesRepository.ProductExistInQuotesDetail(dto.Id);
                if (!productExistInQuotesDetail)
                {
                    resultDto = await this.Delete(dto.Id);
                    metadata.IsValid = true;
                    metadata.Message = "Producto Eliminado Satisfactoriamente!";
                    response.Meta = metadata;
                    response.Data = resultDto;

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

        public async Task<ApiResponse<AppProductsGetDto>> GetProduct(
          int id)
        {
            AppProductsGetDto resultDto = new AppProductsGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppProductsGetDto> response = new ApiResponse<AppProductsGetDto>(resultDto);
            try
            {
                AppProducts appProductsFind = await this.GetById(id);
                if (appProductsFind == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Codigo de producto no existe, verifique por favor";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                resultDto = this._mapper.Map<AppProductsGetDto>((object)appProductsFind);
                AppUnits byId1 = await this._appUnitsService.GetById(appProductsFind.AppUnitsId.Value);
                if (byId1 != null)
                    resultDto.AppUnitsGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId1);
                AppUnits byId2 = await this._appUnitsService.GetById(appProductsFind.ProductionUnitId.Value);
                if (byId2 != null)
                    resultDto.ProductionUnitGetDto = this._mapper.Map<AppUnitsGetDto>((object)byId2);
                MtrTipoMoneda byId3 = await this._mtrTipoMonedaService.GetById(appProductsFind.PrymaryMtrMonedaId.Value);
                if (byId3 != null)
                    resultDto.PrymaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId3);
                MtrTipoMoneda byId4 = await this._mtrTipoMonedaService.GetById(appProductsFind.SecundaryMtrMonedaId.Value);
                if (byId4 != null)
                    resultDto.SecundaryMtrMonedaGetDto = this._mapper.Map<MtrTipoMonedaDto>((object)byId4);
                AppSubCategory byId5 = await this._appSubCategoryService.GetById(appProductsFind.AppSubCategoryId.Value);
                if (byId5 != null)
                    resultDto.AppSubCategoryGetDto = this._mapper.Map<AppSubCategoryGetDto>((object)byId5);
                resultDto.Link = resultDto.UrlImage == "" || resultDto.UrlImage == null ? this._paginationOptions.UrlGetFiles + "NoImage.png" : this._paginationOptions.UrlGetFiles + resultDto.UrlImage;
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

        public async Task<ApiResponse<AppProductsGetDto>> CopyProduct(
          int id)
        {
            AppProductsGetDto resultDto = new AppProductsGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppProductsGetDto> response = new ApiResponse<AppProductsGetDto>(resultDto);
            try
            {
                AppProducts byId = await this.GetById(id);
                if (byId == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Codigo de producto no existe, verifique por favor";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                AppProductsCreateDto appProductsCreateDto = this._mapper.Map<AppProductsCreateDto>((object)byId);
                Guid guid = Guid.NewGuid();
                appProductsCreateDto.Code += guid.ToString();
                appProductsCreateDto.Code = appProductsCreateDto.Code.Substring(0, 20);

                appProductsCreateDto.UsuarioConectado = byId.UserCreate;
                appProductsCreateDto.Description1 += appProductsCreateDto.Code;
                if (appProductsCreateDto.RequiereEstimacion == null) appProductsCreateDto.RequiereEstimacion = false;
                response = await this.InsertProducts(appProductsCreateDto);
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





        public ArgClass GetDetailProduct(string name, long code, string sales_behavior, decimal unitPrice)
        {



            ArgClass result = new ArgClass();

            result.Name = name; // "Short run 9 1/2 x 11 2 partes 2 tintas quimico 60 blanco";
            result.StandardPrice = unitPrice;
            result.SalesBehavior = sales_behavior;
            result.ListPrice = unitPrice;
            result.DefaultCode = code.ToString(); //145;
            result.SaleOk = true;
            result.DetailedType = "product";
            result.InvoicePolicy = "order";

            result.UomId = new Id { DefaultCode = "1" };
            result.UomPoId = new Id { DefaultCode = "1" };

            Id[] arrayTaxes = new Id[2];
            arrayTaxes[0] = new Id { DefaultCode = "1" };
            arrayTaxes[1] = new Id { DefaultCode = "2" };
            result.TaxesId = arrayTaxes;

            //Id catId = new Id();
            //catId.DefaultCode = "all";
            result.CategId = new Id { DefaultCode = "1" };





            return result;
        }

        public async Task<OdooProducts> GetOdooProduct(string name, long code, string sales_behavior, decimal unitPrice)
        {
            /*
            OdooServerProd
            OdooServerDev
            OdooTokenProd
            OdooTokenDev*/

            var server = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooServerProd");
            var token = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooTokenProd");

            ArrayList arr = new ArrayList();
            arr.Add(server.Valor.Trim());
            arr.Add(2);
            arr.Add(token.Valor.Trim());
            arr.Add("integration.model");
            arr.Add("product_create_update");

            var listOfStrings = new List<string>();
            string[] arrayOfStrings = listOfStrings.ToArray();
            arr.Add(arrayOfStrings);

            ArrayList arrProduct = new ArrayList();
            arrProduct.Add(GetDetailProduct(name, code, sales_behavior, unitPrice));
            arr.Add(arrProduct);


            OdooProducts odooProducts = new OdooProducts();
            odooProducts.jsonrpc = "2.0";
            odooProducts.method = "call";
            odooProducts.Params = new Params
            {
                Service = "object",
                Method = "execute",
                Args = arr

            };

            return odooProducts;

        }
    }
}
