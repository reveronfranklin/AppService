
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.EntitiesFacturacion;
using AppService.Core.Enumerations;
using AppService.Core.Interfaces;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Core.Services
{
    public class AppRecipesByAppDetailQuotesService : IAppRecipesByAppDetailQuotesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppProductsService _appProductsService;
        private readonly IHelperService _helperService;
        private readonly IAppVariablesService _appVariablesService;
        private readonly IAppVariableSearchService _appVariableSearchService;
        private readonly IAppConfigAppService appConfigAppService;

        private readonly PaginationOptions _paginationOptions;

        public AppRecipesByAppDetailQuotesService(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IOptions<PaginationOptions> options,
          IAppProductsService appProductsService,
          IHelperService helperService,
          IAppVariablesService appVariablesService,
          IAppVariableSearchService appVariableSearchService,
          IAppConfigAppService appConfigAppService
         )
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._appProductsService = appProductsService;
            this._helperService = helperService;
            this._appVariablesService = appVariablesService;
            this._appVariableSearchService = appVariableSearchService;
            this.appConfigAppService = appConfigAppService;
            this._paginationOptions = options.Value;
        }

        public async Task<List<AppRecipesByAppDetailQuotes>> GetListByCalculoId(
          int calculoId)
        {
            return await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetAllByCalculoId(calculoId);
        }

        public async Task<AppRecipesByAppDetailQuotes> GetById(int id) => await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetById(id);

        public async Task<AppRecipesByAppDetailQuotes> Update(
          AppRecipesByAppDetailQuotes appRecipes)
        {
            if (await this.GetById(appRecipes.Id) == null)
                throw new Exception("Documento No existe");
            this._unitOfWork.AppRecipesByAppDetailQuotesRepository.Update(appRecipes);
            await this._unitOfWork.SaveChangesAsync();
            return await this.GetById(appRecipes.Id);
        }

        public async Task DeleteRange(List<AppRecipesByAppDetailQuotes> entity)
        {
            this._unitOfWork.AppRecipesByAppDetailQuotesRepository.DeleteRange(entity);
            await this._unitOfWork.SaveChangesAsync();
        }


        public async Task UpdatePrecioCotizacion()
        {
            var cotizacionesAll = await _unitOfWork.AppDetailQuotesRepository.GetAll();
            var cotizaciones = cotizacionesAll.Where(x => x.IdEstatus < 5 && x.FechaRegistro.Year==2024 && x.FechaRegistro.Month==8 && x.UnitPriceBaseProduction==0).ToList();
            if (cotizaciones.Count > 0)
            {
                foreach (var item in cotizaciones)
                {
                    decimal idMunicipio = 1;
                    var generalQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(item.AppGeneralQuotesId);
                    if (generalQuotes != null)
                    {
                        if (generalQuotes.IdMunicipio == 0)
                        {
                            idMunicipio = (decimal)generalQuotes.IdMunicipio;
                        }
                        else
                        {
                            MtrDirecciones direccionEntregar = await _unitOfWork.MtrDireccionesRepository.GetById(generalQuotes.IdDireccionEntregar);
                            if (direccionEntregar != null)
                            {
                                Winy243 byEstadoMunicipio = await this._unitOfWork.Winy243Repository.GetByEstadoMunicipio(direccionEntregar.Estado, direccionEntregar.Municipio);
                                idMunicipio = byEstadoMunicipio.Recnum;
                            }
                        }
                        
                       
                    }
                    appRecipesByAppDetailQuotesQueryFilter filter = new appRecipesByAppDetailQuotesQueryFilter();

                    filter.Ancho =item.MedidaOpuesta;
                    filter.Largo = item.MedidaBasica;
                    filter.UnidadId = item.IdUnidad;
                    filter.Cantidad = (int)item.CantidadSolicitada;
                    filter.AppProuctId = item.IdProducto;
                    filter.AppDetailQuotesId = item.Id;
                    filter.IdMunicipio = idMunicipio;
                    await GetPrice(filter);
                }
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

        
        public async Task<ApiResponse<AppPriceGetDto>> GetPrice(appRecipesByAppDetailQuotesQueryFilter filter)
        {
            var tasa = await _unitOfWork.TPaTasaReferencialRepository.GetTasaByFecha(DateTime.Now);
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<AppPriceGetDto> result = new ApiResponse<AppPriceGetDto>(null);

          
            
            if (filter.Cantidad <= 0)
            {

                result.Data = null;
                metadata.IsValid = false;
                metadata.Message = "Cantidad Invalida";
                result.Meta = metadata;
                return result;

            }

            var municipio = await _unitOfWork.Winy243Repository.GetById(filter.IdMunicipio);
            if (municipio == null)
            {
                result.Data = null;
                metadata.IsValid = false;
                metadata.Message = "Id municipio no existe";
                result.Meta = metadata;
                return result;
            }



            AppProducts appProducts = await _appProductsService.GetById(filter.AppProuctId);
            if (appProducts != null)
            {


                
                
                var porcFlete = await GetFlete(filter.IdMunicipio,filter.AppProuctId);
                

                if (appProducts.PorcFlete > 0)
                {
                    porcFlete=appProducts.PorcFlete;
                }
                
                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorProductoCantidad)
                {
                    var precioProductoCantidad = await GetPrecioProductoCantidad(filter.AppProuctId, filter.Cantidad,(int)filter.CondicionDePago);
                    precioProductoCantidad.Precio = Utility.TruncateDec.TruncateDecimal(precioProductoCantidad.Precio, 2);
                    precioProductoCantidad.PrecioMaximo = Utility.TruncateDec.TruncateDecimal(precioProductoCantidad.PrecioMaximo, 2);
                    precioProductoCantidad.Flete = (precioProductoCantidad.Precio * porcFlete) / 100;

                    precioProductoCantidad.Flete = Utility.TruncateDec.TruncateDecimal(precioProductoCantidad.Flete, 2);
                    precioProductoCantidad.PrecioMasFlete = precioProductoCantidad.Precio + precioProductoCantidad.Flete;

                    precioProductoCantidad.Tasa = (decimal)tasa.Tasa;

                    result.Data = precioProductoCantidad;
                    metadata.IsValid = true;
                    metadata.Message = "Precio Generado Satisfactoriamente";
                    result.Meta = metadata;

                }

                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorRollo)
                {
                    var precioProductoCantidad = await GetPrecioProductoCantidadRollo(filter.AppProuctId, filter.Cantidad,filter.CondicionDePago);
                    precioProductoCantidad.Precio = Utility.TruncateDec.TruncateDecimal(precioProductoCantidad.Precio, 2);
                    precioProductoCantidad.PrecioMaximo = Utility.TruncateDec.TruncateDecimal(precioProductoCantidad.PrecioMaximo, 2);
                    precioProductoCantidad.Flete = (precioProductoCantidad.Precio * porcFlete) / 100;

                    precioProductoCantidad.Flete = Utility.TruncateDec.TruncateDecimal(precioProductoCantidad.Flete, 2);
                    precioProductoCantidad.PrecioMasFlete = precioProductoCantidad.Precio + precioProductoCantidad.Flete;

                    precioProductoCantidad.Tasa = (decimal)tasa.Tasa;

                    result.Data = precioProductoCantidad;
                    metadata.IsValid = true;
                    metadata.Message = "Precio Generado Satisfactoriamente";
                    result.Meta = metadata;

                }

                



                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorProducto)
                {


                    //parate
                    AppPriceGetDto resultPrice = new AppPriceGetDto();
                    resultPrice =await  GetPrecioProductoCantidad(filter.AppProuctId, filter.Cantidad,(int)filter.CondicionDePago);
                    resultPrice.AppproductsId = filter.AppProuctId;
                    resultPrice.CalculoId = 0;
                    resultPrice.Desde = filter.Cantidad;
                    resultPrice.Hasta = filter.Cantidad;
                    resultPrice.Precio = appProducts.UnitPrice;
                 
                    resultPrice.Precio = Utility.TruncateDec.TruncateDecimal(resultPrice.Precio, 2);
                    resultPrice.PrecioMaximo = appProducts.UnitPrice;
                    resultPrice.Tasa = (decimal)tasa.Tasa;
                    var defaultConversion = await GetDefaultConversionByProduct(filter.AppProuctId);
                    if (defaultConversion != null)
                    {
                        Conversion conversion = new Conversion((decimal)defaultConversion.XNumerador, (decimal)defaultConversion.YDenominador, filter.Cantidad);
                        resultPrice.CantidadConvertida = conversion.getCantidadAlternativa(); ;
                    }
                    else
                    {
                        resultPrice.CantidadConvertida = filter.Cantidad;
                    }


                    resultPrice.Flete = (resultPrice.Precio * porcFlete) / 100;
                    resultPrice.Flete = Utility.TruncateDec.TruncateDecimal(resultPrice.Flete, 2);
                    resultPrice.PrecioMasFlete = resultPrice.Precio + resultPrice.Flete;
                    result.Data = resultPrice;
                    metadata.Message = "Precio Generado Satisfactoriamente";
                    result.Meta = metadata;
                    result.Meta.IsValid = true;





                }

                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.RequiereEntradaLargoAncho)
                {

                    if (filter.Largo == null || filter.Largo <= 0)
                    {

                        result.Data = null;
                        metadata.IsValid = false;
                        metadata.Message = "Medida largo Invalida";
                        result.Meta = metadata;
                        return result;

                    }
                    if (filter.Ancho == null || filter.Ancho <= 0)
                    {

                        result.Data = null;
                        metadata.IsValid = false;
                        metadata.Message = "Medida ancho Invalida";
                        result.Meta = metadata;
                        return result;

                    }

                    var resultConversion = await CalculaConversion(filter.Cantidad, (decimal)filter.Largo, (decimal)filter.Ancho);
                    var price = await _unitOfWork.AppPriceRepository.GetByProductoCantidad(filter.AppProuctId, resultConversion.resulCantidad);

                    AppPriceGetDto resultPrice = new AppPriceGetDto();
                    resultPrice.AppproductsId = filter.AppProuctId;
                    resultPrice.CalculoId = 0;
                    resultPrice.Desde = filter.Cantidad;
                    resultPrice.Hasta = filter.Cantidad;
                    resultPrice.Precio = Utility.TruncateDec.TruncateDecimal(price.Precio, 2);
              
                    resultPrice.Flete = (resultPrice.Precio * porcFlete) / 100;
                    resultPrice.Flete = Utility.TruncateDec.TruncateDecimal(resultPrice.Flete, 2);
                    resultPrice.PrecioMasFlete = resultPrice.Precio + resultPrice.Flete;
                    resultPrice.Tasa = (decimal)tasa.Tasa;
                    resultPrice.CantidadConvertida = Utility.TruncateDec.TruncateDecimal(resultConversion.resulCantidad, 3);
                    result.Data = resultPrice;
                    metadata.Message = "Precio Generado Satisfactoriamente";
                    result.Meta = metadata;
                    result.Meta.IsValid = true;

                }

                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorMontaje){

                    if (filter.UnidadId == null) filter.UnidadId = 4;
                     if (filter.Largo == null || filter.Largo <= 0)
                    {

                        result.Data = null;
                        metadata.IsValid = false;
                        metadata.Message = "Medida largo Invalida";
                        result.Meta = metadata;
                        return result;

                    }
                    if (filter.Ancho == null || filter.Ancho <= 0)
                    {

                        result.Data = null;
                        metadata.IsValid = false;
                        metadata.Message = "Medida ancho Invalida";
                        result.Meta = metadata;
                        return result;

                    }
                    long ordenAnterior;
                    if (filter.AppDetailQuotesId == null) {
                        filter.AppDetailQuotesId = 0;
                        ordenAnterior = 0;
                    }
                    else
                    {
                        var detail = await _unitOfWork.AppDetailQuotesRepository.GetById((int)filter.AppDetailQuotesId);
                        if (detail != null)
                        {
                            ordenAnterior = (long)detail.OrdenAnterior;
                        }
                        else
                        {
                            ordenAnterior = 0;
                        }
                    }
                    
                   

                    var resultConversion = await CalculaConversion(filter.Cantidad, (decimal)filter.Largo, (decimal)filter.Ancho);
                    if (filter.UnidadId == null) filter.UnidadId = 4;
                    var price = await this.GetPrecioProductoCantidadMedidaBasicaCmMedidaOpuestam(filter.AppProuctId,filter.Cantidad, (decimal)filter.Largo, (decimal)filter.Ancho, ordenAnterior,(int)filter.UnidadId,filter.CondicionDePago);
                    AppPriceGetDto resultPrice = new AppPriceGetDto();
                    resultPrice.AppproductsId = filter.AppProuctId;
                    resultPrice.CalculoId = price.CalculoId;
                    resultPrice.Desde = filter.Cantidad;
                    resultPrice.Hasta = filter.Cantidad;
                    resultPrice.Precio = Utility.TruncateDec.TruncateDecimal(price.Precio, 2);
                    resultPrice.PrecioMaximo = Utility.TruncateDec.TruncateDecimal(price.PrecioMaximo, 2);
                    resultPrice.Flete = (resultPrice.Precio * porcFlete) / 100;
                    resultPrice.Flete = Utility.TruncateDec.TruncateDecimal(resultPrice.Flete, 2);
                    resultPrice.PrecioMasFlete = resultPrice.Precio + resultPrice.Flete;
                    resultPrice.Tasa = (decimal)tasa.Tasa;
                    resultPrice.CantidadConvertida = Utility.TruncateDec.TruncateDecimal((decimal)price.CantidadConvertida, 3);
                    resultPrice.PrecioPorUnidad= Utility.TruncateDec.TruncateDecimal((decimal)price.PrecioPorUnidad, 3);
                    resultPrice.CantidadPorUnidad= Utility.TruncateDec.TruncateDecimal((decimal)price.CantidadPorUnidad, 3);
                    resultPrice.CantidadConvertidaAlternativa = Utility.TruncateDec.TruncateDecimal((decimal)price.CantidadConvertidaAlternativa, 3);
                    resultPrice.CantidadPorUnidadAlternativa = Utility.TruncateDec.TruncateDecimal((decimal)price.CantidadPorUnidadAlternativa, 3);
                    if(resultPrice.CantidadConvertidaAlternativa==null || resultPrice.CantidadConvertidaAlternativa == 0)
                    {
                        resultPrice.CantidadConvertidaAlternativa = resultPrice.CantidadConvertida;
                    }
                    resultPrice.UnidadId = price.UnidadId;
                    resultPrice.PorDebajoDeCantidadMinima = false;
                    if (appProducts.CantidadMinima > resultPrice.CantidadConvertidaAlternativa)
                    {
                        resultPrice.PorDebajoDeCantidadMinima = true;
                    }
                    
                        //resultPrice.CantidadConvertida = Utility.TruncateDec.TruncateDecimal(resultConversion.resulCantidad, 3);
                    result.Data = resultPrice;
                    metadata.Message = "Precio Generado Satisfactoriamente";
                    result.Meta = metadata;
                    result.Meta.IsValid = true;

                }

                if (appProducts.TipoCalculo == (int)TipoCalculoEnum.PrecioPorDesarrolloPrime)
                {

                    if (filter.Largo == null || filter.Largo <= 0)
                    {

                        result.Data = null;
                        metadata.IsValid = false;
                        metadata.Message = "Medida largo Invalida";
                        result.Meta = metadata;
                        return result;

                    }
                    if (filter.Ancho == null || filter.Ancho <= 0)
                    {

                        result.Data = null;
                        metadata.IsValid = false;
                        metadata.Message = "Medida ancho Invalida";
                        result.Meta = metadata;
                        return result;

                    }
                    long ordenAnterior;
                    if (filter.AppDetailQuotesId == null)
                    {
                        filter.AppDetailQuotesId = 0;
                        ordenAnterior = 0;
                    }
                    else
                    {
                        var detail = await _unitOfWork.AppDetailQuotesRepository.GetById((int)filter.AppDetailQuotesId);
                        if (detail != null)
                        {
                            ordenAnterior = (long)detail.OrdenAnterior;
                        }
                        else
                        {
                            ordenAnterior = 0;
                        }
                    }

                    // var resultConversion = await CalculaConversion(filter.Cantidad, (decimal)filter.Largo, (decimal)filter.Ancho);
                    var price = await this.GetPrecioEtiquetasPrime(filter.AppProuctId, filter.Cantidad, (decimal)filter.Largo, (decimal)filter.Ancho, ordenAnterior,filter.CondicionDePago);
                    AppPriceGetDto resultPrice = new AppPriceGetDto();
                    resultPrice.AppproductsId = filter.AppProuctId;
                    resultPrice.CalculoId = price.CalculoId;
                    resultPrice.Desde = filter.Cantidad;
                    resultPrice.Hasta = filter.Cantidad;
                    resultPrice.Precio = Utility.TruncateDec.TruncateDecimal(price.Precio, 2);
                    resultPrice.PrecioMaximo = Utility.TruncateDec.TruncateDecimal(price.PrecioMaximo, 2);
                    resultPrice.Flete = (resultPrice.Precio * porcFlete) / 100;
                    resultPrice.Flete = Utility.TruncateDec.TruncateDecimal(resultPrice.Flete, 2);
                    resultPrice.PrecioMasFlete = resultPrice.Precio + resultPrice.Flete;
                    resultPrice.Tasa = (decimal)tasa.Tasa;
                    resultPrice.CantidadConvertida = Utility.TruncateDec.TruncateDecimal((decimal)price.CantidadConvertida, 3);
                    resultPrice.PrecioPorUnidad = Utility.TruncateDec.TruncateDecimal((decimal)price.PrecioPorUnidad, 3);
                    resultPrice.CantidadPorUnidad = Utility.TruncateDec.TruncateDecimal((decimal)price.CantidadPorUnidad, 3);
                    resultPrice.CantidadConvertidaAlternativa = resultPrice.CantidadConvertida;
                    resultPrice.PorDebajoDeCantidadMinima = false;
                    if (appProducts.CantidadMinima > resultPrice.CantidadConvertidaAlternativa)
                    {
                        resultPrice.PorDebajoDeCantidadMinima = true;
                    }  
                    
               

                    result.Data = resultPrice;
                    metadata.Message = "Precio Generado Satisfactoriamente";
                    result.Meta = metadata;
                    result.Meta.IsValid = true;

                }
            }
            else
            {
                result.Data = null;
                metadata.IsValid = false;
                metadata.Message = "Codigo de producto no existe";
                result.Meta = metadata;
            }

           // await _unitOfWork.AppDetailQuotesRepository.UpdatePrecioMinimo((int)filter.AppDetailQuotesId, result.Data.Precio,
            //    result.Data.PrecioMaximo,(int)result.Data.CalculoId);
            
            result.Data.Tasa = (decimal)tasa.Tasa;
            result.Data.PrecioBs = Utility.TruncateDec.TruncateDecimal(result.Data.Precio * (decimal)tasa.Tasa, 2);

            result.Data.FleteBs = Utility.TruncateDec.TruncateDecimal(result.Data.Flete * (decimal)tasa.Tasa, 2);
            result.Data.PrecioMasFleteBs = Utility.TruncateDec.TruncateDecimal(result.Data.PrecioMasFlete * (decimal)tasa.Tasa, 2);
            result.Data.SubTotalBs = Utility.TruncateDec.TruncateDecimal(result.Data.SubTotal * (decimal)tasa.Tasa, 2);

            return result;
        }



        public class ParametrosMaquina
        {

            public decimal AdicionalProduccion { get; set; }
            public decimal AdicionalProduccionOpuesta { get; set; }
            public decimal MedidaOpuestaRollo { get; set; }
            public decimal MedidaBasicaRollo { get; set; }

            public ParametrosMaquina(decimal adicionalProduccion, decimal adicionalProduccionOpuesta, decimal medidaOpuestaRollo, decimal medidaBasicaRollo)
            {
                AdicionalProduccion = adicionalProduccion;
                AdicionalProduccionOpuesta = adicionalProduccionOpuesta;
                MedidaOpuestaRollo = medidaOpuestaRollo;
                MedidaBasicaRollo = medidaBasicaRollo;

            }



        }

        public async Task<ResultConversionUnidadesMetrosCuadrados> CalculaConversion(decimal cantidadSolicitada, decimal medidaBasica, decimal medidaOpuesta)
        {

            try
            {
              //  var parametrosDto = await appConfigAppService.getParametrosMaquina();


               /*ParametrosMaquina parametrosMaquinas = new ParametrosMaquina(
                    parametrosDto.AdicionalProduccion,
                    parametrosDto.AdicionalProduccionOpuesta,
                    parametrosDto.MedidaOpuestaRollo, parametrosDto.MedidaBasicaRollo);*/
               ParametrosMaquina parametrosMaquinas = new ParametrosMaquina(
                   0,
                   0,
                   0, 0);

                if (cantidadSolicitada <= 0)
                {
                    return null;
                }

                if (cantidadSolicitada <= 0)
                {
                    return null;
                }
                if (medidaBasica <= 0)
                {
                    return null;
                }
                if (medidaOpuesta <= 0)
                {
                    return null;
                }



                var conversion = new ConversionUnidadesMetrosCuadrados(
                    parametrosMaquinas.AdicionalProduccion,
                    parametrosMaquinas.AdicionalProduccionOpuesta,
                    parametrosMaquinas.MedidaBasicaRollo,
                    parametrosMaquinas.MedidaOpuestaRollo);


                conversion.cantidadBase = cantidadSolicitada;
                conversion.medidaBasica = medidaBasica;
                conversion.medidaOpuesta = medidaOpuesta;
                var result = conversion.getCantidadPorUnidad();
                ResultConversionUnidadesMetrosCuadrados resultObject = new ResultConversionUnidadesMetrosCuadrados();
                resultObject.cantidadPorUnidad = result;
                resultObject.area = conversion.area;
                if (result == 0) result = 1;
                resultObject.resulCantidad = cantidadSolicitada / result;
                return resultObject;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

      


        }

        public async Task<AppProductConversion> GetDefaultConversionByProduct(int appProductId,int unidadId=0)
        {
            AppProductConversion result = new AppProductConversion();
            var product = await _unitOfWork.AppProductsRepository.GetById(appProductId);
            if (product != null)
            {
                if (unidadId == 0) unidadId = (int)product.AppUnitsId;
                var appProductConversion = await _unitOfWork.AppProductConversionRepository.GetByProductBaseUnitAlternativeUnit
                        (appProductId, (int)product.ProductionUnitId, unidadId);
                result = appProductConversion;

            }

            return result;
        }

        public async Task<AppPriceGetDto> GetPrecioProductoCantidad(int productId, Decimal cantidad, int condicionDePago)
        {
            AppPriceGetDto result = new AppPriceGetDto();
            int calculoId = await this.GenerarCalculoPorProductoCantidad(productId, cantidad,condicionDePago);
           //await this.CalulateRecipeByCalculoId(calculoId);
            await CalulateRecipeByCalculoIdInMemory(calculoId);
            Decimal precio = this._unitOfWork.AppRecipesByAppDetailQuotesRepository.TotalCost(calculoId);
            result.AppproductsId = productId;
            result.CalculoId = new int?(calculoId);
            result.Desde = cantidad;
            result.Hasta = cantidad;
            precio = Utility.TruncateDec.TruncateDecimal(precio, 2);
            result.Precio = precio;

            var defaultConversion = await GetDefaultConversionByProduct(productId);
            Conversion conversion = new Conversion((decimal)defaultConversion.XNumerador, (decimal)defaultConversion.YDenominador, cantidad);
            result.CantidadConvertida = conversion.getCantidadAlternativa();
            result.CantidadConvertidaAlternativa = result.CantidadConvertida;
            
            List<AppRecipesByAppDetailQuotes> calculoList = await this.GetListByCalculoId(calculoId);
            if (calculoList.Count > 0)
            {
                await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.AddRangeHistory(this.CrearListaHistoricoCalculo(calculoList));
                await this._unitOfWork.SaveChangesAsync();
                await this.DeleteRange(calculoList);
            }
            var precioMaximo = calculoList.Where(x => x.Code == "PRECIOUNITARIOMAXIMO").FirstOrDefault();
            result.PrecioMaximo = result.Precio;
            if (precioMaximo != null)
            {
                result.PrecioMaximo = (decimal)precioMaximo.TotalCost;
            }

            var formasRollo = calculoList.Where(x => x.Code == "CANTFORMAS").FirstOrDefault();
            if (formasRollo!=null)
            {
                result.CantidadConvertida = formasRollo.Quantity;

            }
            
            result.PorDebajoDeCantidadMinima = false;
            
            var producto = await _unitOfWork.AppProductsRepository.GetById(productId);
            if (producto != null)
            {
                if (producto.CantidadMinima > result.CantidadConvertidaAlternativa)
                {
                    result.PorDebajoDeCantidadMinima = true;
                }
            }
            
            AppPriceGetDto productoCantidad = result;
            result = (AppPriceGetDto)null;
            calculoList = (List<AppRecipesByAppDetailQuotes>)null;
            return productoCantidad;
       
        }

        public async Task<AppPriceGetDto> GetPrecioProductoCantidadRollo( int productId,Decimal cantidad,int condicionDePago)
        {
            AppPriceGetDto result = new AppPriceGetDto();
            int calculoId = await this.GenerarCalculoPorProductoCantidad(productId, cantidad,condicionDePago);
            //await this.CalulateRecipeByCalculoId(calculoId);
            await CalulateRecipeByCalculoIdInMemory(calculoId);
            Decimal precio = this._unitOfWork.AppRecipesByAppDetailQuotesRepository.TotalCost(calculoId);
            result.AppproductsId = productId;
            result.CalculoId = new int?(calculoId);
            result.Desde = cantidad;
            result.Hasta = cantidad;
            precio = Utility.TruncateDec.TruncateDecimal(precio, 2);
            result.Precio = precio;
          

            List<AppRecipesByAppDetailQuotes> calculoList = await this.GetListByCalculoId(calculoId);
            if (calculoList.Count > 0)
            {
                await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.AddRangeHistory(this.CrearListaHistoricoCalculo(calculoList));
                await this._unitOfWork.SaveChangesAsync();
                await this.DeleteRange(calculoList);
            }
            var precioMaximo = calculoList.Where(x => x.Code == "PRECIOUNITARIOMAXIMO").FirstOrDefault();
            result.PrecioMaximo = result.Precio;
            if (precioMaximo != null)
            {
                result.PrecioMaximo = (decimal)precioMaximo.TotalCost;
            }

            /*var formasRollo = calculoList.Where(x => x.Code == "CANTFORMAS").FirstOrDefault();
            if (formasRollo != null)
            {
                result.CantidadPorUnidad = formasRollo.Quantity;

            }*/
            result.CantidadConvertida = cantidad;
            result.CantidadConvertidaAlternativa = cantidad;
            result.CantidadPorUnidadAlternativa = cantidad;
            result.CantidadPorUnidad = 0; // cantidad / result.CantidadConvertida;
            result.PrecioPorUnidad = 0; //result.Precio / result.CantidadPorUnidad;
            
            result.PorDebajoDeCantidadMinima = false;
            var producto = await _unitOfWork.AppProductsRepository.GetById(productId);
            if (producto != null)
            {
                if (producto.CantidadMinima > result.CantidadConvertidaAlternativa)
                {
                    result.PorDebajoDeCantidadMinima = true;
                }
            }
            
            AppPriceGetDto productoCantidad = result;
            result = (AppPriceGetDto)null;
            calculoList = (List<AppRecipesByAppDetailQuotes>)null;
            return productoCantidad;

        }


         public async Task<AppPriceGetDto> GetPrecioProductoCantidadOfficeProduct( int productId,Decimal cantidad)
        {
            AppPriceGetDto result = new AppPriceGetDto();
           // int calculoId = await this.GenerarCalculoPorProductoCantidad(productId, cantidad);
            //await CalulateRecipeByCalculoIdInMemory(calculoId);
            var price = await _unitOfWork.AppPriceRepository.GetByProductoCantidad(productId ,cantidad);
            result.AppproductsId = productId;
            result.CalculoId = 0;
            result.Desde = cantidad;
            result.Hasta = cantidad;
           var precio = Utility.TruncateDec.TruncateDecimal(price.Precio, 2);
            result.Precio =precio;
            
            result.PrecioMaximo = result.Precio;
            result.CantidadConvertida = cantidad;
            result.CantidadPorUnidad = 0; // cantidad / result.CantidadConvertida;
            result.PrecioPorUnidad = 0; //result.Precio / result.CantidadPorUnidad;
            AppPriceGetDto productoCantidad = result;
            result = (AppPriceGetDto)null;
            return productoCantidad;

        }

        

        public async Task<AppPriceGetDto> GetPrecioProductoCantidadMedidaBasicaCmMedidaOpuestam(
          int productId,
          Decimal cantidad,
          Decimal medidaBasicaCm,
          Decimal medidaOpuestaCm,
            long ordenAnterior,int unidadId,int condicionPago)
        {
            AppPriceGetDto result = new AppPriceGetDto();
            int calculoId = await this.GenerarCalculoPorProductoCantidadMedidasCm(productId, cantidad,medidaBasicaCm,medidaOpuestaCm,ordenAnterior,condicionPago);
            //await this.CalulateRecipeByCalculoId(calculoId);
            await CalulateRecipeByCalculoIdInMemory(calculoId);
            // Decimal precio = this._unitOfWork.AppRecipesByAppDetailQuotesRepository.TotalCost(calculoId);
            result.AppproductsId = productId;
            result.CalculoId = new int?(calculoId);
            result.Desde = cantidad;
            result.Hasta = cantidad;
            //precio = Utility.TruncateDec.TruncateDecimal(precio, 2);
            result.Precio = 0;

            var defaultConversion = await GetDefaultConversionByProduct(productId,unidadId);
            if (defaultConversion != null)
            {
                Conversion conversion = new Conversion((decimal)defaultConversion.XNumerador, (decimal)defaultConversion.YDenominador, cantidad);
                result.CantidadConvertida = conversion.getCantidadAlternativa();
            }
           

            List<AppRecipesByAppDetailQuotes> calculoList = await this.GetListByCalculoId(calculoId);
            if (calculoList.Count > 0)
            {
                await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.AddRangeHistory(this.CrearListaHistoricoCalculo(calculoList));
                await this._unitOfWork.SaveChangesAsync();
                await this.DeleteRange(calculoList);
            }

            var LiteralM2Teoricos = "AREATEORICATOTALM2";

            var LiteralPrecioM2 = "COSTOM2";
            var LiteralPrecioM22x65= "CostoM2(2x6.5)";
            var LiteralPrecioM285= "CostoM2(8.5)";



            var M2CONSUMIDOS = "M2CONSUMIDOS";
            var M2consumidos2x65 = "M2consumidos2x6.5)";
            var M2consumidos85 = "M2consumidos8.5";

            var  ETQPORM22X65= "ETQPORM2(2X6.5)";
            var  ETQPORM285= "ETQPORM2(8.5)";
            var  ETIQUETASPORM2 = "ETIQUETASPORM2";

            var AREATEORICATOTALM2= calculoList.Where(x => x.Code == LiteralM2Teoricos).FirstOrDefault();

            var PrecioM2 = calculoList.Where(x => x.Code == LiteralPrecioM2).FirstOrDefault();
            var M2Consumido1 = calculoList.Where(x => x.Code == M2CONSUMIDOS).FirstOrDefault();

            var PrecioM22x65 = calculoList.Where(x => x.Code == LiteralPrecioM22x65).FirstOrDefault();
            var M2Consumido2 = calculoList.Where(x => x.Code == M2consumidos2x65).FirstOrDefault();

            var PrecioM285 = calculoList.Where(x => x.Code == LiteralPrecioM285).FirstOrDefault();
            var M2Consumido3 = calculoList.Where(x => x.Code == M2consumidos85).FirstOrDefault();


            var ETQPORM22X65Obj = calculoList.Where(x => x.Code == ETQPORM22X65).FirstOrDefault();
            var ETQPORM285obj = calculoList.Where(x => x.Code == ETQPORM285).FirstOrDefault();
            var ETIQUETASPORM2obj = calculoList.Where(x => x.Code == ETIQUETASPORM2).FirstOrDefault();

            List<AppVariablesMontaje> listResultMontajes = new List<AppVariablesMontaje>();

            AppVariablesMontaje resultMontajes1 = new AppVariablesMontaje();
            resultMontajes1.Id = 1;
            resultMontajes1.Precio = (decimal)PrecioM2.TotalCost;
            resultMontajes1.M2 = (decimal)M2Consumido1.Quantity;
            listResultMontajes.Add(resultMontajes1);

            AppVariablesMontaje resultMontajes2 = new AppVariablesMontaje();
            resultMontajes2.Id = 2;
            resultMontajes2.Precio = (decimal)PrecioM22x65.TotalCost;
            resultMontajes2.M2 = (decimal)M2Consumido2.Quantity;
            listResultMontajes.Add(resultMontajes2);

            AppVariablesMontaje resultMontajes3 = new AppVariablesMontaje();
            resultMontajes3.Id = 3;
            resultMontajes3.Precio = (decimal)PrecioM285.TotalCost;
            resultMontajes3.M2 = (decimal)M2Consumido3.Quantity;
            listResultMontajes.Add(resultMontajes3);

            var stringPrecioMillar = "PRECIOMILLAR";
            var precioMillar = calculoList.Where(x => x.Code == stringPrecioMillar).FirstOrDefault();
            var stringMillares = "Millares";
            var cantidadMillares = calculoList.Where(x => x.Code == stringMillares).FirstOrDefault();

            result.CantidadConvertidaAlternativa = (decimal)M2Consumido1.Quantity;
            result.CantidadPorUnidadAlternativa = ETIQUETASPORM2obj.Quantity;
            if (unidadId == 4)
            {
                result.Precio = (decimal)PrecioM2.TotalCost;
               
                result.CantidadConvertida = (decimal)M2Consumido1.Quantity;
                result.CantidadPorUnidad = ETIQUETASPORM2obj.Quantity;
                var precioMaximo = calculoList.Where(x => x.Code == "PRECIOUNITARIOMAXIMO").FirstOrDefault();
                result.PrecioMaximo = result.Precio;
                if (precioMaximo != null)
                {
                    result.PrecioMaximo = (decimal)precioMaximo.TotalCost;
                }
            }else if(unidadId == 615)
            {
                result.Precio = (decimal)precioMillar.TotalCost;
                result.CantidadConvertida = (decimal)cantidadMillares.Quantity;
                result.CantidadPorUnidad = 1000;

                // var precioMaximo = calculoList.Where(x => x.Code == "PRECIOUNITARIOMAXIMO").FirstOrDefault();
                var precioMaximo = calculoList.Where(x => x.Code == "PRECIOMAXIMOMILLAR").FirstOrDefault();
                result.PrecioMaximo = result.Precio;
                if (precioMaximo != null)
                {
                    result.PrecioMaximo = (decimal)precioMaximo.TotalCost;
                }

            }



            var porcentajesM2 = await _unitOfWork.AppPorcentajeAdicionalM2Repository.GetDesdeHasta((decimal)AREATEORICATOTALM2.Quantity);
            if (porcentajesM2 != null)
            {

                var adicional = (result.Precio * porcentajesM2.Porcentaje) / 100;
                result.Precio = result.Precio - adicional;

            }

            //result.CantidadPorUnidad = cantidad/result.CantidadConvertida;
            if (result.CantidadPorUnidad == 0)
            {
                result.CantidadPorUnidad = 1;
            }
            result.PrecioPorUnidad = result.Precio/ result.CantidadPorUnidad;
            result.UnidadId = unidadId;
            AppPriceGetDto productoCantidad = result;
            result = (AppPriceGetDto)null;
            calculoList = (List<AppRecipesByAppDetailQuotes>)null;
            return productoCantidad;
        }


        public async Task<AppPriceGetDto> GetPrecioEtiquetasPrime(
         int productId,
         Decimal cantidad,
         Decimal medidaBasicaCm,
         Decimal medidaOpuestaCm,long ordenAnterior,int condicionPago)
        {
            AppPriceGetDto result = new AppPriceGetDto();
            int calculoId = await this.GenerarCalculoPorProductoCantidadMedidasCm(productId, cantidad, medidaBasicaCm, medidaOpuestaCm,ordenAnterior,condicionPago);
            await this.CalulateRecipeByCalculoId(calculoId);
            //await CalulateRecipeByCalculoIdInMemory(calculoId);
            // Decimal precio = this._unitOfWork.AppRecipesByAppDetailQuotesRepository.TotalCost(calculoId);
            result.AppproductsId = productId;
            result.CalculoId = new int?(calculoId);
            result.Desde = cantidad;
            result.Hasta = cantidad;
            //precio = Utility.TruncateDec.TruncateDecimal(precio, 2);
            result.Precio = 0;

            var defaultConversion = await GetDefaultConversionByProduct(productId);
            Conversion conversion = new Conversion((decimal)defaultConversion.XNumerador, (decimal)defaultConversion.YDenominador, cantidad);
            result.CantidadConvertida = conversion.getCantidadAlternativa();
            result.CantidadConvertidaAlternativa = result.CantidadConvertida;
            List<AppRecipesByAppDetailQuotes> calculoList = await this.GetListByCalculoId(calculoId);
            if (calculoList.Count > 0)
            {
                await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.AddRangeHistory(this.CrearListaHistoricoCalculo(calculoList));
                await this._unitOfWork.SaveChangesAsync();
                await this.DeleteRange(calculoList);
            }

            
            var precioVenta = calculoList.Where(x => x.Code == "PRECIOMILLAR").FirstOrDefault();  
            if (precioVenta != null)
            {
                result.Precio = (decimal)precioVenta.TotalCost;
            }

            var precioMaximo = calculoList.Where(x => x.Code == "PRECIOUNITARIOMAXIMO").FirstOrDefault();
            result.PrecioMaximo = result.Precio;
            if (precioMaximo != null)
            {
                result.PrecioMaximo = (decimal)precioMaximo.TotalCost;
            }   

            var  millares = calculoList.Where(x => x.Code == "CANT_MILLAR").FirstOrDefault();
          
            if (millares != null)
            {
                result.CantidadConvertida = (decimal)millares.Quantity;
                
            }
            result.CantidadConvertidaAlternativa = result.CantidadConvertida;
            result.CantidadPorUnidad = conversion.y_denominador;

            //result.CantidadPorUnidad = cantidad/result.CantidadConvertida;
            result.PrecioPorUnidad = result.Precio / result.CantidadPorUnidad;

            AppPriceGetDto productoCantidad = result;
            result = (AppPriceGetDto)null;
            calculoList = (List<AppRecipesByAppDetailQuotes>)null;
            return productoCantidad;
        }




        public async Task<AppPriceGetDto> GetPrecioProductoCantidadMedidaBasicaCmMedidaOpuestamCopia(
        int productId,
        Decimal cantidad,
        Decimal medidaBasicaCm,
        Decimal medidaOpuestaCm,
        int condicionPago)
        {
            AppPriceGetDto result = new AppPriceGetDto();
            int calculoId = await this.GenerarCalculoPorProductoCantidadMedidasCm(productId, cantidad, medidaBasicaCm, medidaOpuestaCm,condicionPago);
            //await this.CalulateRecipeByCalculoId(calculoId);
            await CalulateRecipeByCalculoIdInMemory(calculoId);
            // Decimal precio = this._unitOfWork.AppRecipesByAppDetailQuotesRepository.TotalCost(calculoId);
            result.AppproductsId = productId;
            result.CalculoId = new int?(calculoId);
            result.Desde = cantidad;
            result.Hasta = cantidad;
            //precio = Utility.TruncateDec.TruncateDecimal(precio, 2);
            result.Precio = 0;

            var defaultConversion = await GetDefaultConversionByProduct(productId);
            Conversion conversion = new Conversion((decimal)defaultConversion.XNumerador, (decimal)defaultConversion.YDenominador, cantidad);
            result.CantidadConvertida = conversion.getCantidadAlternativa();

            List<AppRecipesByAppDetailQuotes> calculoList = await this.GetListByCalculoId(calculoId);
            if (calculoList.Count > 0)
            {
                await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.AddRangeHistory(this.CrearListaHistoricoCalculo(calculoList));
                await this._unitOfWork.SaveChangesAsync();
                await this.DeleteRange(calculoList);
            }

            var LiteralPrecioM2 = "COSTOM2";
            var LiteralPrecioM22x65 = "CostoM2(2x6.5)";
            var LiteralPrecioM285 = "CostoM2(8.5)";



            var M2CONSUMIDOS = "M2CONSUMIDOS";
            var M2consumidos2x65 = "M2consumidos2x6.5)";
            var M2consumidos85 = "M2consumidos8.5";

            var ETQPORM22X65 = "ETQPORM2(2X6.5)";
            var ETQPORM285 = "ETQPORM2(8.5)";
            var ETIQUETASPORM2 = "ETIQUETASPORM2";



            var PrecioM2 = calculoList.Where(x => x.Code == LiteralPrecioM2).FirstOrDefault();
            var M2Consumido1 = calculoList.Where(x => x.Code == M2CONSUMIDOS).FirstOrDefault();

            var PrecioM22x65 = calculoList.Where(x => x.Code == LiteralPrecioM22x65).FirstOrDefault();
            var M2Consumido2 = calculoList.Where(x => x.Code == M2consumidos2x65).FirstOrDefault();

            var PrecioM285 = calculoList.Where(x => x.Code == LiteralPrecioM285).FirstOrDefault();
            var M2Consumido3 = calculoList.Where(x => x.Code == M2consumidos85).FirstOrDefault();


            var ETQPORM22X65Obj = calculoList.Where(x => x.Code == ETQPORM22X65).FirstOrDefault();
            var ETQPORM285obj = calculoList.Where(x => x.Code == ETQPORM285).FirstOrDefault();
            var ETIQUETASPORM2obj = calculoList.Where(x => x.Code == ETIQUETASPORM2).FirstOrDefault();

            List<AppVariablesMontaje> listResultMontajes = new List<AppVariablesMontaje>();

            AppVariablesMontaje resultMontajes1 = new AppVariablesMontaje();
            resultMontajes1.Id = 1;
            resultMontajes1.Precio = (decimal)PrecioM2.TotalCost;
            resultMontajes1.M2 = (decimal)M2Consumido1.Quantity;
            listResultMontajes.Add(resultMontajes1);

            AppVariablesMontaje resultMontajes2 = new AppVariablesMontaje();
            resultMontajes2.Id = 2;
            resultMontajes2.Precio = (decimal)PrecioM22x65.TotalCost;
            resultMontajes2.M2 = (decimal)M2Consumido2.Quantity;
            listResultMontajes.Add(resultMontajes2);

            AppVariablesMontaje resultMontajes3 = new AppVariablesMontaje();
            resultMontajes3.Id = 3;
            resultMontajes3.Precio = (decimal)PrecioM285.TotalCost;
            resultMontajes3.M2 = (decimal)M2Consumido3.Quantity;
            listResultMontajes.Add(resultMontajes3);

            var montajeMayor40 = listResultMontajes.Where(x => x.M2 > 40).FirstOrDefault();
            if (montajeMayor40 == null)
            {
                if (medidaOpuestaCm > 7)
                {
                    result.Precio = (decimal)PrecioM285.TotalCost;
                    result.CantidadConvertida = (decimal)M2Consumido3.Quantity;
                    result.CantidadPorUnidad = ETQPORM285obj.Quantity;


                }
                else
                {
                    result.Precio = (decimal)PrecioM22x65.TotalCost;
                    result.CantidadConvertida = (decimal)M2Consumido2.Quantity;
                    result.CantidadPorUnidad = ETQPORM22X65Obj.Quantity;
                }
            }
            else
            {
                result.Precio = (decimal)PrecioM2.TotalCost;
                result.CantidadConvertida = (decimal)M2Consumido1.Quantity;
                result.CantidadPorUnidad = ETIQUETASPORM2obj.Quantity;

            }



            var sumarAdicionalPorVolumen = await _unitOfWork.AppPorcentajeAdicionalM2Repository.AplicarValorBase((decimal)result.CantidadConvertida);
            if (sumarAdicionalPorVolumen)
            {
                var porcentajesM2 = await _unitOfWork.AppPorcentajeAdicionalM2Repository.GetDesdeHasta((decimal)result.CantidadConvertida);
                if (porcentajesM2 != null)
                {

                    var adicional = (result.Precio * porcentajesM2.Porcentaje) / 100;
                    result.Precio = result.Precio + adicional;

                }

            }

            //result.CantidadPorUnidad = cantidad/result.CantidadConvertida;
            result.PrecioPorUnidad = result.Precio / result.CantidadPorUnidad;

            AppPriceGetDto productoCantidad = result;
            result = (AppPriceGetDto)null;
            calculoList = (List<AppRecipesByAppDetailQuotes>)null;
            return productoCantidad;
        }



        public List<AppRecipesByAppDetailQuotesHistory> CrearListaHistoricoCalculo(
          List<AppRecipesByAppDetailQuotes> entities)
        {
            List<AppRecipesByAppDetailQuotesHistory> detailQuotesHistoryList = new List<AppRecipesByAppDetailQuotesHistory>();
            foreach (AppRecipesByAppDetailQuotes entity in entities)
            {
                AppRecipesByAppDetailQuotesHistory detailQuotesHistory = new AppRecipesByAppDetailQuotesHistory()
                {
                    CalculoId = entity.CalculoId,
                    AppproductsId = entity.AppproductsId,
                    AppVariableId = entity.AppVariableId,
                    Description = entity.Description,
                    AppIngredientsId = entity.AppIngredientsId,
                    Quantity = entity.Quantity,
                    TotalCost = entity.TotalCost,
                    Formula = entity.Formula,
                    FormulaValue = entity.FormulaValue,
                    SumValue = entity.SumValue,
                    OrderCalculate = entity.OrderCalculate,
                    Code = entity.Code,
                    IncludeInSearch = entity.IncludeInSearch,
                    Secuencia = entity.Secuencia,
                    AfectaCosto = entity.AfectaCosto,
                    VariablesSearchText = entity.VariablesSearchText,
                    TruncarEntero = entity.TruncarEntero,
                    EsVariableDeEntrada = entity.EsVariableDeEntrada,
                    RetornarElMayor=entity.RetornarElMayor,
                    RetornarElMenor=entity.RetornarElMenor
                };
                detailQuotesHistoryList.Add(detailQuotesHistory);
            }
            return detailQuotesHistoryList;
        }

        public async Task<int> GenerarCalculoPorProductoCantidad(int productId, Decimal cantidad,int condicionDePago)
        {
            decimal porcentajeCondicioPago = 0;
            var condicion= await _unitOfWork.MtrCondicionPagoRepository.GetById((short)condicionDePago);
            if (condicion != null)
            {
                porcentajeCondicioPago = condicion.PocGapAplicarPrecio;
            }
            int result = 0;
            List<AppRecipesByAppDetailQuotes> byAppDetailQuotesList = new List<AppRecipesByAppDetailQuotes>();
            List<AppRecipes> recipes = await this._unitOfWork.AppRecipesRepository.GetAllRecipesByProductId(productId);
            if (recipes != null)
            {
                int calculoId = await this.NextId();
                result = calculoId;
                if ((await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetAllByCalculoId(calculoId)).Count == 0)
                {
                    foreach (AppRecipes appRecipes in recipes)
                    {
                        AppRecipesByAppDetailQuotes entity = new AppRecipesByAppDetailQuotes()
                        {
                            CalculoId = calculoId,
                            AppproductsId = appRecipes.AppproductsId,
                            AppVariableId = appRecipes.AppVariableId,
                            Description = appRecipes.Description,
                            AppIngredientsId = appRecipes.AppIngredientsId,
                            Quantity = appRecipes.Quantity,
                            TotalCost = appRecipes.TotalCost,
                            Formula = appRecipes.Formula,
                            FormulaValue = appRecipes.FormulaValue,
                            SumValue = appRecipes.SumValue,
                            OrderCalculate = appRecipes.OrderCalculate,
                            Code = appRecipes.Code,
                            IncludeInSearch = appRecipes.IncludeInSearch,
                            Secuencia = appRecipes.Secuencia,
                            AfectaCosto = appRecipes.AfectaCosto,
                            VariablesSearchText = appRecipes.VariablesSearchText,
                            TruncarEntero = appRecipes.TruncarEntero,
                            EsVariableDeEntrada = appRecipes.EsVariableDeEntrada
                        };
                        int? appVariableId = appRecipes.AppVariableId;
                        int num = 92;
                        if (appVariableId.GetValueOrDefault() == num & appVariableId.HasValue)
                            entity.Quantity = new Decimal?(cantidad);
                        if (entity.Code == "%CONDICIONDEPAGO")
                        {
                            entity.Quantity = porcentajeCondicioPago;
                        }
                        await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.Add(entity);
                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
            }
            int num1 = result;
            recipes = (List<AppRecipes>)null;
            return num1;
        }

         public async Task<int> GenerarCalculoPorProductoCantidadMedidasCm(int productId, Decimal cantidad,decimal medidaBasicaCm,decimal medidaOpuestaCm,long ordenAnterior=0,int condicionDePago=40)
        {
            int result = 0;
            List<AppRecipesByAppDetailQuotes> byAppDetailQuotesList = new List<AppRecipesByAppDetailQuotes>();
            List<AppRecipes> recipes = await this._unitOfWork.AppRecipesRepository.GetAllRecipesByProductId(productId);
            if (recipes != null)
            {
                decimal porcentajeCondicioPago = 0;
                var condicion= await _unitOfWork.MtrCondicionPagoRepository.GetById((short)condicionDePago);
                if (condicion != null)
                {
                    porcentajeCondicioPago = condicion.PocGapAplicarPrecio;
                }
                
                int calculoId = await this.NextId();
                result = calculoId;
                if ((await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetAllByCalculoId(calculoId)).Count == 0)
                {
                    foreach (AppRecipes appRecipes in recipes)
                    {
                        AppRecipesByAppDetailQuotes entity = new AppRecipesByAppDetailQuotes()
                        {
                            CalculoId = calculoId,
                            AppproductsId = appRecipes.AppproductsId,
                            AppVariableId = appRecipes.AppVariableId,
                            Description = appRecipes.Description,
                            AppIngredientsId = appRecipes.AppIngredientsId,
                            Quantity = appRecipes.Quantity,
                            TotalCost = appRecipes.TotalCost,
                            Formula = appRecipes.Formula,
                            FormulaValue = appRecipes.FormulaValue,
                            SumValue = appRecipes.SumValue,
                            OrderCalculate = appRecipes.OrderCalculate,
                            Code = appRecipes.Code,
                            IncludeInSearch = appRecipes.IncludeInSearch,
                            Secuencia = appRecipes.Secuencia,
                            AfectaCosto = appRecipes.AfectaCosto,
                            VariablesSearchText = appRecipes.VariablesSearchText,
                            TruncarEntero = appRecipes.TruncarEntero,
                            EsVariableDeEntrada = appRecipes.EsVariableDeEntrada,
                            RetornarElMayor=appRecipes.RetornarElMayor,
                             RetornarElMenor=appRecipes.RetornarElMenor,
                        };
                        int? appVariableId = appRecipes.AppVariableId;
                        /*int num = 92;
                        if (appVariableId.GetValueOrDefault() == num & appVariableId.HasValue)
                            entity.Quantity = new Decimal?(cantidad);*/
                        if (appRecipes.Code == "%CONDICIONDEPAGO")
                        {
                            entity.Quantity = porcentajeCondicioPago;
                        }
                        if (appRecipes.Code == "CANTIDAD") entity.Quantity = new Decimal?(cantidad);
                        if (appRecipes.Code== "MEDIDABASICA")  entity.Quantity = new Decimal?(medidaBasicaCm);
                        if(appRecipes.Code== "MEDIDAOPUESTA")  entity.Quantity = new Decimal?(medidaOpuestaCm);
                        if (appRecipes.Code == "MedidaBasicaCM") entity.Quantity = new Decimal?(medidaBasicaCm);
                        if (appRecipes.Code == "MedidaOpuestaCM") entity.Quantity = new Decimal?(medidaOpuestaCm);
                        if (appRecipes.Code == "DESARROLLO")
                        {
                           
                            var desaarrolloObj = await _unitOfWork.AppDesarrolloEtiquetasPrimeRepository.GetByMedidaBasica(medidaBasicaCm);
                            if (desaarrolloObj != null)
                            {
                            
                                entity.Quantity = desaarrolloObj.Desarrollo;
                            }
                            else
                            {
                                entity.Quantity = 0;

                            }
                          
                        }
                        if(appRecipes.Code.Trim()== "ORDEN ANTERIOR")
                        {
                            if (ordenAnterior == 0) { entity.Quantity = 1; } else { entity.Quantity = 0; }

                        }

                         

                        

                        await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.Add(entity);
                       
                    }
                    await this._unitOfWork.SaveChangesAsync();
                }
            }
                    int num1 = result;
            recipes = (List<AppRecipes>)null;
            return num1;
        }


      
        public async Task<string> GetValueFormula(
          int calculoId,
          string formula,
          string codeProduct,
          string codeRecipe)
        {
            string newFormula = "";
            List<string> listString = this._helperService.GetListString(formula, "[", "]");
            newFormula = formula;
            foreach (string item in listString)
            {
                List<AppRecipesByAppDetailQuotes> codeVariableCode = await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetListRecipesByProductCodeVariableCode(calculoId, codeProduct, item);
                if (codeVariableCode != null && codeVariableCode.Count > 0)
                {
                    if (codeVariableCode.FirstOrDefault<AppRecipesByAppDetailQuotes>().AfectaCosto.Value)
                    {
                        Decimal? nullable = codeVariableCode.Select<AppRecipesByAppDetailQuotes, Decimal?>((Func<AppRecipesByAppDetailQuotes, Decimal?>)(c => c.TotalCost)).Sum();
                        newFormula = newFormula.Replace("[" + item + "]", nullable.ToString());
                    }
                    else
                    {
                       
                        Decimal? quantity = codeVariableCode.Where(x => x.Quantity != null).FirstOrDefault<AppRecipesByAppDetailQuotes>().Quantity;
                        newFormula = newFormula.Replace("[" + item + "]", quantity.ToString());
                    }
                }
                else
                {
                    AppConfigApp byKey = await this._unitOfWork.AppConfigAppRepository.GetByKey(item);
                    if (byKey != null)
                    {
                        newFormula = newFormula.Replace("[" + item + "]", byKey.Valor.ToString());
                    }
                    else
                    {
                        newFormula = newFormula.Replace("[" + item + "]", "0");
                    }
                       
                }
            }
            newFormula = newFormula.Replace(",", ".");
            string valueFormula = newFormula;
            newFormula = (string)null;
            return valueFormula;
        }

        public async Task ValidaFormula(AppRecipes recipe)
        {
            string newFormula = "";
            string mensaje = "";
            List<string> listString = this._helperService.GetListString(recipe.Formula, "[", "]");

            foreach (string item in listString)
            {
                var recetaVariable = await _unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode((int)recipe.AppproductsId, item);
                if (recetaVariable != null)
                {
                    if (recetaVariable.OrderCalculate >= recipe.OrderCalculate)
                    {
                        mensaje = item + " con orden de calculo" + recetaVariable.OrderCalculate + " >= " + recipe.OrderCalculate + " " + recipe.Description;
                    }
                }

            }

            var recipeFind = await _unitOfWork.AppRecipesRepository.GetById(recipe.Id);
            if (recipeFind != null)
            {
                recipeFind.MensajeValidacionFormula = mensaje;
                _unitOfWork.AppRecipesRepository.Update(recipeFind);
                await _unitOfWork.SaveChangesAsync();

            }

        }



        public async Task CalulateRecipeByCalculoId(int calculoId)
        {

            try
            {
                List<AppRecipesByAppDetailQuotes> allByCalculoId = await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetAllByCalculoId(calculoId);
                if (allByCalculoId == null)
                    return;
                var firstCalculo = allByCalculoId.FirstOrDefault();
                var product = await _unitOfWork.AppProductsRepository.GetById((int)firstCalculo.AppproductsId);

                foreach (AppRecipesByAppDetailQuotes recipe in allByCalculoId.Where<AppRecipesByAppDetailQuotes>((Func<AppRecipesByAppDetailQuotes, bool>)(x => x.Formula.Length > 0)).OrderBy<AppRecipesByAppDetailQuotes, Decimal?>((Func<AppRecipesByAppDetailQuotes, Decimal?>)(x => x.OrderCalculate)).ToList<AppRecipesByAppDetailQuotes>())
                {
                    Decimal formula = await this.CalculateFormula(recipe,product);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           


        }
        public async Task CalulateRecipeByCalculoIdInMemory(int calculoId)
        {
            List<AppRecipesByAppDetailQuotes> allByCalculoId = await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.GetAllByCalculoId(calculoId);
            if (allByCalculoId == null)
                return;
            var firstCalculo = allByCalculoId.FirstOrDefault();
            var product = await _unitOfWork.AppProductsRepository.GetById((int)firstCalculo.AppproductsId);

            var calculo = await this.CalculateFormulaInMemory(allByCalculoId, product);
            
            List<AppRecipesByAppDetailQuotes> newListCalculo = new List<AppRecipesByAppDetailQuotes>();


            foreach (var item in calculo)
            {
                AppRecipesByAppDetailQuotes newItem = new AppRecipesByAppDetailQuotes();

                newItem.CalculoId = item.CalculoId;
                newItem.AppproductsId = item.AppproductsId;
                newItem.AppVariableId = item.AppVariableId;
                newItem.Description = item.Description;
                newItem.AppIngredientsId = item.AppIngredientsId;
                newItem.Quantity = item.Quantity;
                newItem.TotalCost = item.TotalCost;
                newItem.Formula = item.Formula;
                newItem.FormulaValue = item.FormulaValue;
                newItem.SumValue = item.SumValue;
                newItem.OrderCalculate = item.OrderCalculate;
                newItem.Code = item.Code;
                newItem.IncludeInSearch = item.IncludeInSearch;
                newItem.Secuencia = item.Secuencia;
                newItem.AfectaCosto = item.AfectaCosto;
                newItem.VariablesSearchText = item.VariablesSearchText;
                newItem.TruncarEntero = item.TruncarEntero;
                newItem.EsVariableDeEntrada = item.EsVariableDeEntrada;
                newItem.RetornarElMayor = item.RetornarElMayor;
                newItem.RetornarElMenor = item.RetornarElMenor;
             
                 newListCalculo.Add(newItem);
            }

            //TODO
            //Eliminar y agregar el calculo
            this._unitOfWork.AppRecipesByAppDetailQuotesRepository.DeleteRange(allByCalculoId);
            await this._unitOfWork.SaveChangesAsync();


            await this._unitOfWork.AppRecipesByAppDetailQuotesRepository.AddRange(newListCalculo);
            await this._unitOfWork.SaveChangesAsync();

        }

        public async Task<Decimal> CalculateFormula(AppRecipesByAppDetailQuotes recipe, AppProducts producto)
        {
            try
            {


                Decimal result = 0M;
                if (recipe == null)
                {
                    result = 0M;
                    return result;
                }

                string valueFormula = await this.GetValueFormula(recipe.CalculoId, recipe.Formula, producto.Code, recipe.Code);
                object obj = new DataTable().Compute(valueFormula, "");
                obj.ToString();
                result = Convert.ToDecimal(obj.ToString());
                if (recipe.AfectaCosto.Value)
                {
                    recipe.TotalCost = new Decimal?(result);
                }
                else
                {
                    bool? truncarEntero = recipe.TruncarEntero;
                    bool flag = true;
                    recipe.Quantity = !(truncarEntero.GetValueOrDefault() == flag & truncarEntero.HasValue) ? new Decimal?(result) : new Decimal?(Decimal.Truncate(result));
                }
                recipe.FormulaValue = valueFormula;
                AppRecipesByAppDetailQuotes byAppDetailQuotes = await this.Update(recipe);
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return 0M;
            }
        }

        //Calculo en Memoria
        public async Task<List<AppRecipesByAppDetailQuotes>> CalculateFormulaInMemory(List<AppRecipesByAppDetailQuotes> recipes, AppProducts producto)
        {
            try
            {

                List<AppRecipesByAppDetailQuotes> originalRecipes = new List<AppRecipesByAppDetailQuotes>();
                originalRecipes = recipes;

                Decimal result = 0M;
                if (recipes.Count == 0)
                {
                    result = 0M;
                    return originalRecipes;
                }
                var recipesEvaluar = recipes.Where(x => x.Formula.Length > 0).OrderBy(x => x.OrderCalculate).ToList();
                if (recipesEvaluar != null && recipesEvaluar.Count <= 0)
                {
                    return originalRecipes;
                }
                   
                foreach (var recipe in recipesEvaluar)
                {
                 
                    try
                    {
                    string valueFormula = await this.GetValueFormulaInMemory(recipe.CalculoId, recipe.Formula, producto.Code, recipe.Code, originalRecipes);

                    try
                    {
                        if (valueFormula.Contains("/0"))
                        {
                            var detente = "a";
                        }
                        object obj = new DataTable().Compute(valueFormula, "");
                        obj.ToString();
                        result = Convert.ToDecimal(obj.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                      
                  
                  
                    if (recipe.AfectaCosto.Value)
                    {
                        recipe.TotalCost = new Decimal?(result);
                    }
                    else
                    {
                        bool? truncarEntero = recipe.TruncarEntero;
                        bool flag = true;
                      
                       var valorTruncado= !(truncarEntero.GetValueOrDefault() == flag & truncarEntero.HasValue) ? new Decimal?(result) : new Decimal?(Decimal.Truncate(result));
                       if (valorTruncado == 0)
                       {
                           recipe.Quantity = 1;
                       }
                       else
                       {
                           recipe.Quantity = valorTruncado;
                       }
    

                    }
                    recipe.FormulaValue = valueFormula;

                    AppRecipesByAppDetailQuotes mewRecipe = new AppRecipesByAppDetailQuotes();
                    mewRecipe = recipe;
                    originalRecipes.Add(mewRecipe);
                    var vrified = originalRecipes.Where(x => x.Code == recipe.Code).ToList();
                    originalRecipes.Remove(recipe);
                    }
                    catch (Exception ex)
                    {
                        var m = ex.Message;
                        result = 0;
                    }
                }

                return originalRecipes;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        public async Task<string> GetValueFormulaInMemory(
         int calculoId,
         string formula,
         string codeProduct,
         string codeRecipe,
         List<AppRecipesByAppDetailQuotes> recipes)
        {
            string newFormula = "";
           
            List<string> listString = this._helperService.GetListString(formula, "[", "]");
            newFormula = formula;
            foreach (string item in listString)
            {

              
                List<AppRecipesByAppDetailQuotes> codeVariableCode = recipes.Where(x=>x.Code.Trim()==item.Trim()).ToList();
                if (codeVariableCode != null && codeVariableCode.Count > 0)
                {
                    if (codeVariableCode.FirstOrDefault<AppRecipesByAppDetailQuotes>().AfectaCosto.Value)
                    {
                        Decimal? nullable = codeVariableCode.Select<AppRecipesByAppDetailQuotes, Decimal?>((Func<AppRecipesByAppDetailQuotes, Decimal?>)(c => c.TotalCost)).Sum();
                        newFormula = newFormula.Replace("[" + item + "]", nullable.ToString());
                    }
                    else
                    {
                        Decimal? quantity = codeVariableCode.Where(x=> x.Quantity!=null).FirstOrDefault<AppRecipesByAppDetailQuotes>().Quantity;
                        newFormula = newFormula.Replace("[" + item + "]", quantity.ToString());
                    }
                }
                else
                {
                    AppConfigApp byKey = await this._unitOfWork.AppConfigAppRepository.GetByKey(item);
                    if (byKey != null)
                    {
                        newFormula = newFormula.Replace("[" + item + "]", byKey.Valor.ToString());
                    }
                    else
                    {
                        newFormula = newFormula.Replace("[" + item+ "]","0");
                    }
                       
                }
            }
            newFormula = newFormula.Replace(",", ".");
            string valueFormula = newFormula;
            newFormula = (string)null;
            return valueFormula;
        }


        public async Task<int> NextId()
        {
            int result = 0;
            AppConfigApp byKey = await this._unitOfWork.AppConfigAppRepository.GetByKey("ULTIMOCALCULO");
            if (byKey == null)
            {
                result = 1;
                await this._unitOfWork.AppConfigAppRepository.Add(new AppConfigApp()
                {
                    Clave = "ULTIMOCALCULO",
                    Valor = "1"
                });
                await this._unitOfWork.SaveChangesAsync();
            }
            else
            {
                result = Convert.ToInt32(byKey.Valor) + 1;
                byKey.Valor = result.ToString();
                this._unitOfWork.AppConfigAppRepository.Update(byKey);
                await this._unitOfWork.SaveChangesAsync();
            }
            return result;
        }





    }
}
