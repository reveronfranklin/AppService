
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;


using AppService.Core.Interfaces;

using System;

using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Precios;
using ResultConversionUnidadesMetrosCuadrados = AppService.Core.DTOs.Precios.ResultConversionUnidadesMetrosCuadrados;

namespace AppService.Core.Services
{
    public class AppGetPriceService : IAppGetPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
      
        private readonly IAppProductsService _appProductsService;
        private readonly IAppRecipesByAppDetailQuotesService _appRecipesByAppDetailQuotesService;


        private readonly PaginationOptions _paginationOptions;

        public AppGetPriceService(
          IUnitOfWork unitOfWork,
          IAppProductsService appProductsService,
          IAppRecipesByAppDetailQuotesService appRecipesByAppDetailQuotesService
       
         )
        {
            this._unitOfWork = unitOfWork;
            this._appProductsService = appProductsService;
            _appRecipesByAppDetailQuotesService = appRecipesByAppDetailQuotesService;
        }

        public async Task<decimal> GetPorcFlete(int idMunicipio,int idProducto )
        {
            decimal result = 0;
            var municipio = await _unitOfWork.Winy243Repository.GetFleteByIdMunicipo(idMunicipio);
            if (municipio != null)
            {
                result = municipio;
            }
            var producto= await _unitOfWork.AppProductsRepository.GetById(idProducto);
            if (producto != null)
            {
                if (producto.PorcFlete > 0) {
                    result = producto.PorcFlete;
                }
            }

            return result;
        }
        public async Task<OptionsPreciosProductosResponse> GetPrecioProductoCantidad(int idMunicipio,decimal cantidad,int idProducto ){ 
            
                
                
                var resultProducto = await _appProductsService.GetProduct(idProducto);
                var producto = resultProducto.Data;
                var precio = producto.AppPriceDto.Where(x=>  cantidad >=x.Desde && cantidad<=x.Hasta).FirstOrDefault();
                OptionsPreciosProductosResponse response = new OptionsPreciosProductosResponse();
                decimal porcFlete = 0;
                decimal flete = 0;
                decimal fleteMaximo = 0;
                porcFlete= await GetPorcFlete(idMunicipio,idProducto);

                if (precio!=null)
                {
                    flete = (precio.Precio * porcFlete) / 100;
                    fleteMaximo = (precio.PrecioMaximo * porcFlete) / 100;
                
                }
                response.Producto = producto;
                response.Cantidad=cantidad;
                response.PrecioMinimo = precio.Precio;
                response.PrecioMaximo = precio.PrecioMaximo;
                response.CantidadConvertida=cantidad;
                response.Flete = flete;
                response.FleteMaximo = fleteMaximo;
                response.IdCalculo = 0;
                return response;
                
            
        }


        public ResultConversionUnidadesMetrosCuadrados CalculaConversion(decimal cantidadSolicitada,decimal medidaBasica,decimal medidaOpuesta)
        {
            
            ResultConversionUnidadesMetrosCuadrados response = new ResultConversionUnidadesMetrosCuadrados();

            decimal adicionalProduccion = (decimal)0.635;
            decimal adicionalProduccionOpuesta = (decimal)1.27;
            decimal medidaOpuestaRollo = (decimal)22.2758;
            decimal medidaBasicaRollo = (decimal)448.9176;
            var conversion = new ConversionUnidadesMetrosCuadrados(adicionalProduccion,adicionalProduccionOpuesta, medidaOpuestaRollo, medidaBasicaRollo);

            conversion.cantidadBase = cantidadSolicitada;
            conversion.medidaBasica = medidaBasica;
            conversion.medidaOpuesta = medidaOpuesta;
            var result = conversion.getCantidadPorUnidad();
            var area = conversion.area;
            response.ResulCantidad = (double)result;
            response.Area = (double)area;
             
            return response;
            
        }



        public async Task<OptionsPreciosProductosResponse> RecalculoPrecioPorProductoCantidadLargoAncho( OptionsPreciosProductos options)
        {
            OptionsPreciosProductosResponse response = new OptionsPreciosProductosResponse();

            decimal cantidadConvertida = 0;
            decimal precioMinimo = 0;
            decimal precioMaximo = 0;
            int idCalculo = 0;
            decimal flete = 0;
            decimal fleteMaximo = 0;
            decimal porcFlete = 0;
            appRecipesByAppDetailQuotesQueryFilter filter = new appRecipesByAppDetailQuotesQueryFilter();

            filter.IdMunicipio = options.Municipio.Recnum;
            filter.AppProuctId = options.Producto.Id;
            filter.Cantidad= (int)options.Cantidad;
            filter.Largo = options.MedidaBasica;
            filter.Ancho = options.MedidaOpuesta;
            filter.UnidadId = options.IdUnidad;
            filter.CondicionDePago = options.CondicionPago;
        
            
            var calculo = await _appRecipesByAppDetailQuotesService.GetPrice(filter);
            if (calculo.Meta.IsValid == true)
            {
                precioMinimo = calculo.Data.Precio;
                precioMaximo = calculo.Data.PrecioMaximo;
                cantidadConvertida = (decimal)calculo.Data.CantidadConvertida;
                idCalculo = (int)calculo.Data.CalculoId;
                flete = calculo.Data.Flete;
                porcFlete = await GetPorcFlete((int)options.Municipio.Recnum, options.Producto.Id);

                flete = (precioMinimo * porcFlete) / 100;
                fleteMaximo = (precioMaximo * porcFlete) / 100;
            }
            response.Producto=options.Producto;
            response.Cantidad = options.Cantidad;
            response.PrecioMinimo = precioMinimo;
            response.PrecioMaximo = precioMaximo;
            response.CantidadConvertida = cantidadConvertida;
            response.IdCalculo = idCalculo;
            response.Flete = flete;
            response.FleteMaximo = fleteMaximo;
            
            return response;
            
        }

        public async Task<OptionsPreciosProductosResponse> RecalculoRequiereEntradaLargoAncho(OptionsPreciosProductos options)
        {
            
            OptionsPreciosProductosResponse response = new OptionsPreciosProductosResponse();

            double cantidadPorUnidadProduccion = 0;
            decimal cantidadConvertida = 0;
            int idCalculo = 0;
            decimal flete = 0;
            decimal fleteMaximo = 0;
            decimal porcFlete = 0;
            
            if (options.Producto.TipoCalculo == 1)
            {
                var calculoConversion = CalculaConversion(
                    options.Cantidad,
                    options.MedidaBasica, //medidaBasica,
                    options.MedidaOpuesta //medidaOpuesta
                );

                //let cantidadPorUnidad= calculoConversion.resulCantidad;
                cantidadPorUnidadProduccion = calculoConversion.ResulCantidad;
            }

            cantidadConvertida = (decimal)options.Cantidad / (decimal)cantidadPorUnidadProduccion;
            var precio =
                await GetPrecioProductoCantidad((int)options.Municipio.Recnum, cantidadConvertida, options.Producto.Id);

            porcFlete = await GetPorcFlete((int)options.Municipio.Recnum, options.Producto.Id);
            flete = (precio.PrecioMinimo * porcFlete) / 100;
            fleteMaximo = (precio.PrecioMaximo * porcFlete) / 100;
            response.Producto = options.Producto;
            response.Cantidad = options.Cantidad;
            response.PrecioMinimo = precio.PrecioMinimo;
            response.PrecioMaximo = precio.PrecioMaximo;
            response.CantidadConvertida = cantidadConvertida;
            response.IdCalculo = precio.IdCalculo;
            response.Flete =flete;
            response.FleteMaximo = fleteMaximo;
            
            return response;

        }


        public async Task<OptionsPreciosProductosResponse> RecalculoPrecioPorProductoCantidadLargoRollo(
            OptionsPreciosProductos options)
        {
            
            OptionsPreciosProductosResponse response = new OptionsPreciosProductosResponse();

            decimal cantidadConvertida = 0;
            decimal precioMinimo = 0;
            decimal precioMaximo = 0;
            int idCalculo = 0;
            decimal flete = 0;
            decimal fleteMaximo = 0;
            decimal porcFlete = 0;
            appRecipesByAppDetailQuotesQueryFilter filter = new appRecipesByAppDetailQuotesQueryFilter();

            filter.IdMunicipio = options.Municipio.Recnum;
            filter.AppProuctId = options.Producto.Id;
            filter.Cantidad= (int)options.Cantidad;
            filter.UnidadId = options.IdUnidad;
            filter.CondicionDePago = options.CondicionPago;
        
            
            var calculo = await _appRecipesByAppDetailQuotesService.GetPrice(filter);
            if (calculo.Meta.IsValid == true)
            {
                precioMinimo = calculo.Data.Precio;
                precioMaximo = calculo.Data.PrecioMaximo;
                cantidadConvertida = (decimal)calculo.Data.CantidadConvertida;
                idCalculo = (int)calculo.Data.CalculoId;
                porcFlete = await GetPorcFlete((int)options.Municipio.Recnum, options.Producto.Id);
                flete = (precioMinimo * porcFlete) / 100;
                fleteMaximo = (precioMaximo * porcFlete) / 100;
            }
            
            response.Producto = options.Producto;
            response.Cantidad = options.Cantidad;
            response.PrecioMinimo = precioMinimo;
            response.PrecioMaximo = precioMaximo;
            response.CantidadConvertida = cantidadConvertida;
            response.IdCalculo = idCalculo;
            response.Flete =flete;
            response.FleteMaximo = fleteMaximo;
            return response;

        }



        public async Task<OptionsPreciosProductosResponse> RecalculoPrecioPorProductoCantidad(OptionsPreciosProductos options)
        {
            OptionsPreciosProductosResponse response = new OptionsPreciosProductosResponse();
            decimal cantidadConvertida = 0;
            decimal precioMinimo = 0;
            decimal precioMaximo = 0;
            int idCalculo = 0;
            decimal flete = 0;
            decimal fleteMaximo = 0;
            decimal porcFlete = 0;
            
         
            var calculo = await _appRecipesByAppDetailQuotesService.GetPrecioProductoCantidad(options.Producto.Id, options.Cantidad,options.CondicionPago);
            
            if (calculo != null)
            {
                precioMinimo = calculo.Precio;
                precioMaximo = calculo.PrecioMaximo;
                cantidadConvertida = (decimal)calculo.CantidadConvertida;
                idCalculo = (int)calculo.CalculoId;
                porcFlete = await GetPorcFlete((int)options.Municipio.Recnum, options.Producto.Id);
                flete = (precioMinimo * porcFlete) / 100;
                fleteMaximo = (precioMaximo * porcFlete) / 100;
            }
            response.Producto = options.Producto;
            response.Cantidad = options.Cantidad;
            response.PrecioMinimo = precioMinimo;
            response.PrecioMaximo = precioMaximo;
            response.CantidadConvertida = cantidadConvertida;
            response.IdCalculo = idCalculo;
            response.Flete =flete;
            response.FleteMaximo = fleteMaximo;
            return response;
            
            return response;
        }


        public async Task<DTOs.Shared.ResultDto<OptionsPreciosProductosResponse>> GetPrice(
            OptionsPreciosProductos options)
        {
            DTOs.Shared.ResultDto<OptionsPreciosProductosResponse> result = new DTOs.Shared.ResultDto<OptionsPreciosProductosResponse>(null);

            OptionsPreciosProductosResponse data = new OptionsPreciosProductosResponse();
            
            try
            {

                decimal cantidadConvertida = 0;
                decimal precioMinimo = 0;
                decimal precioMaximo = 0;
                int idCalculo = 0;
                decimal flete = 0;
                decimal fleteMaximo = 0;
                decimal porcFlete = 0;

                if (options.Cantidad <= 0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Cantidad Invalida";
                    return result;
                }

                if (options.IdUnidad<=0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Unidad Invalida";
                    return result;
                }
                if (options.Producto == null || options.Producto.Id<=0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Producto Invalido";
                    return result;
                }

                if (options.Municipio == null || options.Municipio.Recnum < 0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Municipio Invalido";
                    return result;
                }
                if (options.CondicionPago <= 0)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Municipio Invalido";
                    return result;
                }
                var producto = await _unitOfWork.AppProductsRepository.GetById(options.Producto.Id);
                if (producto == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Producto Invalido";
                    return result;
                }
                
                var municipio = await _unitOfWork.Winy243Repository.GetById(options.Municipio.Recnum);
                if (municipio == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "Municipio Invalido";
                    return result;
                }
                var condicioPago = await _unitOfWork.MtrCondicionPagoRepository.GetById((short)options.CondicionPago);
                if (condicioPago == null)
                {
                    result.Data = null;
                    result.IsValid = false;
                    result.Message = "CondicioPago Invalido";
                    return result;
                }

                switch (producto.TipoCalculo)
                {
                    case 1:
                        var resultEntradaLargoAncho = await RecalculoRequiereEntradaLargoAncho(options);
                        precioMinimo = resultEntradaLargoAncho.PrecioMinimo;
                        precioMaximo = resultEntradaLargoAncho.PrecioMaximo;
                        cantidadConvertida = resultEntradaLargoAncho.CantidadConvertida;
                        flete = resultEntradaLargoAncho.Flete;
                        fleteMaximo = resultEntradaLargoAncho.FleteMaximo;
                        idCalculo=resultEntradaLargoAncho.IdCalculo;
                        
                        break;
                    case 2:
                        var precio = await GetPrecioProductoCantidad((int)options.Municipio.Recnum, options.Cantidad,
                            options.Producto.Id);
                        precioMinimo = precio.PrecioMinimo;
                        precioMaximo = precio.PrecioMaximo;
                        cantidadConvertida = precio.CantidadConvertida;
                        flete = precio.Flete;
                        fleteMaximo = precio.FleteMaximo;
                        idCalculo = precio.IdCalculo;
                        break;
                    case 3:
                        var resultEntradaLargoAncho3 = await RecalculoPrecioPorProductoCantidad(options);
                        precioMinimo = resultEntradaLargoAncho3.PrecioMinimo;
                        precioMaximo = resultEntradaLargoAncho3.PrecioMaximo;
                        cantidadConvertida = resultEntradaLargoAncho3.CantidadConvertida;
                        idCalculo = resultEntradaLargoAncho3.IdCalculo;
                        flete = resultEntradaLargoAncho3.Flete;
                        fleteMaximo = resultEntradaLargoAncho3.FleteMaximo;
                        break;
                    case 4:
                        var resultEntradaLargoAncho4 = await RecalculoPrecioPorProductoCantidadLargoAncho(options);
                        precioMinimo = resultEntradaLargoAncho4.PrecioMinimo;
                        precioMaximo = resultEntradaLargoAncho4.PrecioMaximo;
                        cantidadConvertida = resultEntradaLargoAncho4.CantidadConvertida;
                        idCalculo = resultEntradaLargoAncho4.IdCalculo;
                        flete = resultEntradaLargoAncho4.Flete;
                        fleteMaximo = resultEntradaLargoAncho4.FleteMaximo;
                        break;
                    case 5:
                        var resultEntradaLargoAncho5 = await RecalculoPrecioPorProductoCantidadLargoRollo(options);
                        precioMinimo = resultEntradaLargoAncho5.PrecioMinimo;
                        precioMaximo = resultEntradaLargoAncho5.PrecioMaximo;
                        cantidadConvertida = resultEntradaLargoAncho5.CantidadConvertida;
                        idCalculo = resultEntradaLargoAncho5.IdCalculo;
                        flete = resultEntradaLargoAncho5.Flete;
                        fleteMaximo = resultEntradaLargoAncho5.FleteMaximo;
                        break;
                    case 6:

                        var resultEntradaLargoAncho6 = await RecalculoPrecioPorProductoCantidadLargoAncho(options);
                        precioMinimo = resultEntradaLargoAncho6.PrecioMinimo;
                        precioMaximo = resultEntradaLargoAncho6.PrecioMaximo;
                        cantidadConvertida = resultEntradaLargoAncho6.CantidadConvertida;
                        idCalculo = resultEntradaLargoAncho6.IdCalculo;
                        flete = resultEntradaLargoAncho6.Flete;
                        fleteMaximo = resultEntradaLargoAncho6.FleteMaximo;
                        break;
                }

                data.Producto = options.Producto;
                data.Cantidad = cantidadConvertida;
                data.PrecioMinimo = precioMinimo;
                data.PrecioMaximo = precioMaximo;
                data.CantidadConvertida = cantidadConvertida;
                data.Flete = flete;
                data.FleteMaximo = fleteMaximo;
                data.IdCalculo = idCalculo;
                
                
                result.Data = data;
                result.IsValid = true;
                result.Message = "";

                return result;


            }
            catch (Exception ex)
            {
                result.Data = null;
                result.IsValid = false;
                result.Message = ex.Message;
            
            }
            
            

            return result;

        }
        
        
    }
}
