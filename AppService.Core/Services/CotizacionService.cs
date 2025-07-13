﻿using AppService.Core.CustomEntities;
using AppService.Core.DataContratosStock;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Odoo.Clientes.OdooClients;
using AppService.Core.DTOs.Odoo.Cotizaciones.Enviar;
using AppService.Core.Entities;
using AppService.Core.EntitiesClientes;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;
using AppService.Core.Utility;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using static AppService.Core.DTOs.Odoo.Cotizaciones.Enviar.OdooCotizacionEnviar;
using static AppService.Core.DTOs.Odoo.Cotizaciones.Enviar.OdooDetailCotizacionDelete;
using static AppService.Core.DTOs.Odoo.Cotizaciones.Enviar.OdooGeneralCotizacionDelete;
using ArgClass = AppService.Core.DTOs.Odoo.Cotizaciones.Enviar.OdooCotizacionEnviar.ArgClass;

namespace AppService.Core.Services
{
    public class CotizacionService : ICotizacionService
    {
        private const decimal ValorCero = 0.0000M;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;
        private readonly IMapper _mapper;
        private readonly IAppUnitsService _appUnitsService;
        private readonly IMtrTipoMonedaService _mtrTipoMonedaService;
        private readonly IAprobacionesServices _aprobacionesServices;
        private readonly IOdooClient _odooClient;
        private readonly IAppProductsService _appProductsService;
        private readonly IMtrContactosService _mtrContactosService;
        private readonly IAppRecipesByAppDetailQuotesService _appRecipesByAppDetailQuotesService;
     
        private readonly IConnectionMultiplexer _connectionMultiplexer;


        public CotizacionService(
          IUnitOfWork unitOfWork,
          IOptions<PaginationOptions> options,
          IMapper mapper,
          IAppUnitsService appUnitsService,
          IMtrTipoMonedaService mtrTipoMonedaService,
          IAprobacionesServices aprobacionesServices,
          IOdooClient odooClient,
          IAppProductsService appProductsService,
          IMtrContactosService mtrContactosService,
          IAppRecipesByAppDetailQuotesService appRecipesByAppDetailQuotesService,
       
          IConnectionMultiplexer connectionMultiplexer
       
        )
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _mapper = mapper;
            _appUnitsService = appUnitsService;
            _mtrTipoMonedaService = mtrTipoMonedaService;
            _aprobacionesServices = aprobacionesServices;
            _odooClient = odooClient;
            _appProductsService = appProductsService;
            _mtrContactosService = mtrContactosService;
            _appRecipesByAppDetailQuotesService = appRecipesByAppDetailQuotesService;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task AddRedis(string key, string value)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, value,TimeSpan.FromHours(2));
        }
        public void DeleteRedis(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            db.KeyDelete(key);
        }
        public async Task<string> GetRedis(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            //db.KeyDelete("ListProducts");
            return await db.StringGetAsync(key);
        }
        
        public async Task<List<Wsmy501>> GetAll() => await this._unitOfWork.CotizacionRepository.GetAll();

        public async Task<Wsmy501> GetById(int id) => await this._unitOfWork.CotizacionRepository.GetById(id);

        public async Task<Wsmy501> GetByCotizacion(string cotizacion) => await this._unitOfWork.CotizacionRepository.GetByCotizacion(cotizacion);
        public async Task<Wsmy501> GetByCotizacionAsNoTracking(string cotizacion) => await this._unitOfWork.CotizacionRepository.GetByCotizacionAsNoTracking(cotizacion);

        public async Task<bool> CotizacionTieneOrden(string cotizacion) => await this._unitOfWork.PropuestaRepository.CotizacionTieneOrden(cotizacion);

        public async Task<Wsmy501> Insert(Wsmy501 cotizacion)
        {
            await this._unitOfWork.CotizacionRepository.Add(cotizacion);
            await this._unitOfWork.SaveChangesAsync();
            return cotizacion;
        }

        public async Task<ApiResponse<CotizacionGetDto>> CreateCotizacion(
          CotizacionCreateDto dto)
        {
            CotizacionGetDto data = new CotizacionGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<CotizacionGetDto> response = new ApiResponse<CotizacionGetDto>(data);
            try
            {
                Wsmy501 source = await this.Insert(this._mapper.Map<Wsmy501>((object)dto));
                response.Data = this._mapper.Map<CotizacionGetDto>((object)source);
                response.Meta = metadata;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Data = (CotizacionGetDto)null;
                response.Meta = metadata;
                return response;
            }
        }

        public async Task<Wsmy501> Update(Wsmy501 cotizacion)
        {
            if (await this.GetByCotizacion(cotizacion.Cotizacion) == null)
                throw new Exception("Documento No existe");


            this._unitOfWork.CotizacionRepository.Update(cotizacion);

            await this._unitOfWork.SaveChangesAsync();
            return await this.GetByCotizacion(cotizacion.Cotizacion);
        }

        public async Task<ApiResponse<CotizacionGetDto>> UpdateCotizacion(
          CotizacionUpdateDto dto)
        {
            CotizacionGetDto resultDto = new CotizacionGetDto();
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<CotizacionGetDto> response = new ApiResponse<CotizacionGetDto>(resultDto);
            try
            {
                Wsmy501 byCotizacion = await this.GetByCotizacion(dto.Cotizacion);
                if (byCotizacion == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cotizacione No existe";
                    response.Data = resultDto;
                    response.Meta = metadata;
                    return response;
                }
                resultDto = this._mapper.Map<CotizacionGetDto>((object)await this.Update(byCotizacion));
                response.Data = resultDto;
                response.Meta = metadata;
                return response;
            }
            catch (Exception ex)
            {
                metadata.IsValid = false;
                metadata.Message = ex.InnerException.Message;
                response.Data = (CotizacionGetDto)null;
                response.Meta = metadata;
                return response;
            }
        }

        public async Task<bool> Delete(string cotizacion)
        {
            await this._unitOfWork.CotizacionRepository.Delete(cotizacion);
            await this._unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<ApiResponse<bool>> DeleteCotizacionOld(
          CotizacionDeleteDtocs dto)
        {
            bool resultDto = true;
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<bool> response = new ApiResponse<bool>(resultDto);
            try
            {
                Wsmy501 byCotizacion = await this.GetByCotizacion(dto.Cotizacion);
                if (byCotizacion == null)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Cotizacion No existe!!!";
                    response.Meta = metadata;
                    response.Data = resultDto;
                    return response;
                }
                resultDto = await this.Delete(byCotizacion.Cotizacion);
                metadata.IsValid = true;
                metadata.Message = "Cotizacion Eliminada Satisfactoriamente!";
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

        public async Task IntegrarCotizaciones()
        {

            
            List<AppGeneralQuotes> pendientesIntegrar = await this._unitOfWork.AppGeneralQuotesRepository.GetByCotizacionesPendientesIntegrar();
            if (pendientesIntegrar == null || pendientesIntegrar.Count <= 0)
                return;
            
            foreach (AppGeneralQuotes item in pendientesIntegrar)
            {
                try
                {
                    Console.WriteLine($"Integrando cotizacion: {item.Cotizacion}");
                    await this.IntegrarCotizacionPorLote(item.Id, true);
                    this._unitOfWork.AppGeneralQuotesRepository.MarcarIntegrado(false, item.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                   
                }
             

            }
        }


        public async Task IntegrarCotizacionesPorMes()
        {

            var pendientesIntegrar = await this._unitOfWork.AppGeneralQuotesRepository.GetListCotizacionesUltimoMes();

            foreach (AppGeneralQuotes item in pendientesIntegrar)
            {
                var detailQuotes = await _unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(item.Id);
                if (detailQuotes.Count > 0)
                {
                    foreach (AppDetailQuotes itemDetail in detailQuotes)
                    {

                        var prod = await _unitOfWork.AppProductsRepository.GetById(itemDetail.IdProducto);

                        if (itemDetail.Precio > 0M)
                        {


                            Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(itemDetail.Cotizacion, prod.ExternalCode.Trim());
                            if (renglon != null)
                            {
                                Wsmy515 propuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(itemDetail.Cotizacion, renglon.Renglon, 1);
                                if (propuesta != null)
                                {
                                    propuesta.UsdListaCpj = itemDetail.UnitPriceBaseProduction;
                                    propuesta.UsdListaCpjminimo = itemDetail.UnitPriceBaseProduction;
                                    propuesta.TotalUsdListaCpj = itemDetail.UnitPriceBaseProduction * itemDetail.Cantidad;
                                    propuesta.TotalUsdListaCpjminimo = (itemDetail.UnitPriceBaseProduction * itemDetail.Cantidad);
                                    this._unitOfWork.PropuestaRepository.Update(propuesta);
                                    await this._unitOfWork.SaveChangesAsync();

                                }

                            }


                        }
                    }

                }

            }
        }


        public async Task IntegrarCotizacionPorLote(int generalQuotesId, bool actualizarDetalle)
        {
          
            AppGeneralQuotes generalQuotes = await this._unitOfWork.AppGeneralQuotesRepository.GetById(generalQuotesId);
            if (generalQuotes == null)
            {
                generalQuotes = (AppGeneralQuotes)null;
            }
            else
            {
                await this.UpdateGeneralCotizacion(generalQuotes);
                if (!(generalQuotes.AppDetailQuotes.Count > 0 & actualizarDetalle))
                {
                    generalQuotes = (AppGeneralQuotes)null;
                }
                else
                {
                    foreach (AppDetailQuotes item in (IEnumerable<AppDetailQuotes>)generalQuotes.AppDetailQuotes)
                    {

                        var prod = await _unitOfWork.AppProductsRepository.GetById(item.IdProducto);

                        if (item.Precio > 0M)
                        {
                            await this.UpdateDetailCotizacion(item);
                            if (item.SolicitarPrecio.Value)
                            {
                                Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(item.Cotizacion, prod.ExternalCode.Trim());

                                if (renglon != null)
                                {
                                    
                                    var solicitudAprobacion = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(renglon.Cotizacion, renglon.Renglon, 1);
                                    if (solicitudAprobacion == null)
                                    {
                                        ApiResponse<Wsmy639> aprobacion = await this._aprobacionesServices.CreateAprobacion(item.Cotizacion, renglon.Renglon, 1, item.UserUpdate);
                                        if (aprobacion.Data != null)
                                        {
                                            await this.UpdateSolicitudPropuesta(renglon.Cotizacion, renglon.Renglon, 1, (int)aprobacion.Data.IdSolicitud);
                                            ApiResponse<Wsmy647> apiResponse = await this._aprobacionesServices.ActivarWORKFLOW(item.Cotizacion, renglon.Renglon, 1, item.UserUpdate, item);
                                        }
                                    }
                                  
                                    renglon = (Wsmy502)null;
                                }

                            
                            }
                        }

                    }
                   
                    generalQuotes = (AppGeneralQuotes)null;
                }




            }
        }

        
        
        public async Task IntegrarCotizacion(int generalQuotesId, bool actualizarDetalle,bool integrar=true)
        {
           var configIntegrar =await  _unitOfWork.AppConfigAppRepository.GetByKey("INTEGRAR");
            if (configIntegrar != null)
            {
                if (!configIntegrar.Valor.IsNullOrEmpty())
                {
                    integrar = true;
                }
            }
           
            if(integrar==false) return;
            AppGeneralQuotes generalQuotes = await this._unitOfWork.AppGeneralQuotesRepository.GetById(generalQuotesId);
            if (generalQuotes == null)
            {
                generalQuotes = (AppGeneralQuotes)null;
            }
            else
            {
                await this.UpdateGeneralCotizacion(generalQuotes);
                if (!(generalQuotes.AppDetailQuotes.Count > 0 & actualizarDetalle))
                {
                    generalQuotes = (AppGeneralQuotes)null;
                }
                else
                {
                    foreach (AppDetailQuotes item in (IEnumerable<AppDetailQuotes>)generalQuotes.AppDetailQuotes)
                    {

                        var prod = await _unitOfWork.AppProductsRepository.GetById(item.IdProducto);

                        if (item.Precio > 0M)
                        {
                            await this.UpdateDetailCotizacion(item);
                            if (item.SolicitarPrecio.Value)
                            {
                                Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(item.Cotizacion, prod.ExternalCode.Trim());

                                if (renglon != null)
                                {
                                    
                                    var solicitudAprobacion = await this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(renglon.Cotizacion, renglon.Renglon, 1);
                                    if (solicitudAprobacion == null)
                                    {
                                        ApiResponse<Wsmy639> aprobacion = await this._aprobacionesServices.CreateAprobacion(item.Cotizacion, renglon.Renglon, 1, item.UserUpdate);
                                        if (aprobacion.Data != null)
                                        {
                                            await this.UpdateSolicitudPropuesta(renglon.Cotizacion, renglon.Renglon, 1, (int)aprobacion.Data.IdSolicitud);
                                            ApiResponse<Wsmy647> apiResponse = await this._aprobacionesServices.ActivarWORKFLOW(item.Cotizacion, renglon.Renglon, 1, item.UserUpdate, item);
                                        }
                                    }
                                  
                                    renglon = (Wsmy502)null;
                                }

                            
                            }
                        }

                    }
                   
                    generalQuotes = (AppGeneralQuotes)null;
                }




            }
        }

        public async Task UpdateGeneralCotizacion(AppGeneralQuotes generalQuotes)
        {
            try
            {
     Wsmy501 cotizacion = await this._unitOfWork.CotizacionRepository.GetByCotizacion(generalQuotes.Cotizacion);
            MtrCliente cliente;
            if (cotizacion != null)
            {
                cotizacion.CodCliente = generalQuotes.IdCliente;
                cotizacion.Cod_ClienteExterno = generalQuotes.IdCliente;
                cotizacion.CodVendedor = generalQuotes.IdVendedor;
                cotizacion.Prospecto = !(cotizacion.CodCliente == "000000") ? "" : "X";
                cotizacion.FlagValidado = new bool?(true);
                cotizacion.Fecha = new DateTime?(generalQuotes.Fecha);
                cotizacion.Observaciones = generalQuotes.Observaciones;
                DateTime fechaCaducidad = generalQuotes.FechaCaducidad;
                cotizacion.FechaCaducidad = new DateTime?(generalQuotes.FechaCaducidad);
                cotizacion.FechaPostergada = generalQuotes.FechaPostergada;
                cotizacion.Estatus = new int?(generalQuotes.IdEstatus);
                cotizacion.Observaciones = generalQuotes.Observaciones;
                cliente = this._unitOfWork.MtrClienteRepository.GetById(generalQuotes.IdCliente);
                cotizacion.EmailCliente = cliente.EMailClient.Trim();
                cotizacion.Condicion = new int?((int)generalQuotes.IdCondPago);
                cotizacion.ObservacionPostergar = generalQuotes.ObservacionPostergar;
                cotizacion.UsuarioActualiza = generalQuotes.UsuarioActualiza;
                //cotizacion.FechaActualiza = new DateTime?(generalQuotes.FechaActualiza);
                cotizacion.FechaActualiza = DateTime.Now;
                cotizacion.IdContacto = new int?((int)generalQuotes.IdContacto);
                MtrContactos byId1 = await this._unitOfWork.MtrContactosRepository.GetById((long)cotizacion.IdContacto.Value);
                cotizacion.NombreContacto = byId1.Nombre;
                cotizacion.TelefonoContacto = byId1.Telefono1;
                cotizacion.EmailContacto = byId1.Email;
                cotizacion.IdDireccion = new Decimal?(generalQuotes.IdDireccionFacturar);
                cotizacion.IdTipoTransaccion = "TRA";
                MtrDirecciones direccionFacturar = await this._unitOfWork.MtrDireccionesRepository.GetById(generalQuotes.IdDireccionFacturar);
                if (direccionFacturar != null)
                {
                    cotizacion.DireccionFacturar = direccionFacturar.Direccion.Trim() + direccionFacturar.Direccion1.Trim() + direccionFacturar.Direccion2.Trim();
                    cotizacion.EstadoFacturar = direccionFacturar.Estado.Trim();
                    cotizacion.MunicipioFacturar = direccionFacturar.Municipio.Trim();
                }
             
                MtrDirecciones direccionEntregar = await this._unitOfWork.MtrDireccionesRepository.GetById(generalQuotes.IdDireccionEntregar);
                if (direccionEntregar != null)
                {
                    cotizacion.DireccionEntregar= direccionEntregar.Direccion.Trim() + direccionEntregar.Direccion1.Trim() + direccionEntregar.Direccion2.Trim();
                    cotizacion.EstadoEntregar = direccionEntregar.Estado.Trim();
                    cotizacion.MunicipioEntregar = direccionEntregar.Municipio.Trim();
                }

                if (cliente.Codigo.Trim() == "000000")
                {
                    cotizacion.RazonSocial = generalQuotes.RazonSocial.Trim();
                    cotizacion.Rif = generalQuotes.Rif;
                    cotizacion.DireccionEntregar = generalQuotes.Direccion.Trim();
                    cotizacion.RifEntregar = generalQuotes.Rif;
                }
                else
                {
                    cotizacion.RazonSocial = cliente.Nombre;
                    cotizacion.Rif = cliente.NoRegTribut;
                    cotizacion.DireccionEntregar = direccionEntregar.Direccion.Trim() + direccionEntregar.Direccion1.Trim() + direccionEntregar.Direccion2.Trim();
                    cotizacion.RifEntregar = direccionEntregar.Rif;
                }
                cotizacion.OrdenCompra = generalQuotes.OrdenCompra;
                cotizacion.FlagActualizado = "X";
                cotizacion.IdDireccionEntregar = new Decimal?(generalQuotes.IdDireccionEntregar);
                long? idMtrTipoMoneda = generalQuotes.IdMtrTipoMoneda;
                long num = 1;
                cotizacion.NegocioEnUsd = !(idMtrTipoMoneda.GetValueOrDefault() == num & idMtrTipoMoneda.HasValue) ? new bool?(false) : new bool?(true);
                cotizacion.FijarPrecioBs = generalQuotes.FijarPrecioBs;
                cotizacion.OrigenExterno = new bool?(true);
                this._unitOfWork.CotizacionRepository.Update(cotizacion);
                await this._unitOfWork.SaveChangesAsync();
                cliente = (MtrCliente)null;
                cotizacion = (Wsmy501)null;
            }
            else
            {
                Wsmy501 cotizacionNew = new Wsmy501()
                {
                    Cotizacion = generalQuotes.Cotizacion,
                    CodCliente = generalQuotes.IdCliente,
                    CodVendedor = generalQuotes.IdVendedor
                };
                cotizacionNew.Cod_ClienteExterno = generalQuotes.IdCliente;
                cotizacionNew.Prospecto = !(cotizacionNew.CodCliente.Trim() == "000000") ? "" : "X";
                cotizacionNew.FlagValidado = new bool?(true);
                cotizacionNew.Fecha = new DateTime?(generalQuotes.Fecha);
                cotizacionNew.Observaciones = generalQuotes.Observaciones;
                DateTime fechaCaducidad = generalQuotes.FechaCaducidad;
                cotizacionNew.FechaCaducidad = new DateTime?(generalQuotes.FechaCaducidad);
                cotizacionNew.FechaCaducidad = new DateTime?(generalQuotes.FechaCaducidad);
                cotizacionNew.FechaPostergada = generalQuotes.FechaPostergada;
                cotizacionNew.Estatus = new int?(generalQuotes.IdEstatus);
                cotizacionNew.Observaciones = generalQuotes.Observaciones;
                cliente = this._unitOfWork.MtrClienteRepository.GetById(generalQuotes.IdCliente);
                cliente.EMailClient ??= "";
                cotizacionNew.EmailCliente = cliente.EMailClient.Trim();
                cotizacionNew.Condicion = new int?((int)generalQuotes.IdCondPago);
                cotizacionNew.IdContacto = new int?((int)generalQuotes.IdContacto);
                cotizacionNew.ObservacionPostergar = generalQuotes.ObservacionPostergar;
                cotizacionNew.UsuarioActualiza = generalQuotes.UsuarioActualiza;
                //cotizacionNew.FechaActualiza = new DateTime?(generalQuotes.FechaActualiza);
                cotizacionNew.FechaActualiza = DateTime.Now;
                cotizacionNew.IdTipoTransaccion = "TRA";
                MtrContactos byId4 = await this._unitOfWork.MtrContactosRepository.GetById((long)cotizacionNew.IdContacto.Value);
                cotizacionNew.NombreContacto = byId4.Nombre;
                cotizacionNew.TelefonoContacto = byId4.Telefono1;
                cotizacionNew.IdDireccion = new Decimal?(generalQuotes.IdDireccionFacturar);
                MtrDirecciones direccionFacturar = await this._unitOfWork.MtrDireccionesRepository.GetById(generalQuotes.IdDireccionFacturar);
                if (direccionFacturar != null)
                {
                    cotizacionNew.DireccionFacturar = direccionFacturar.Direccion.Trim() + direccionFacturar.Direccion1.Trim() + direccionFacturar.Direccion2.Trim();
                    cotizacionNew.EstadoFacturar = direccionFacturar.Estado.Trim();
                    cotizacionNew.MunicipioFacturar = direccionFacturar.Municipio.Trim();
                }
             
                MtrDirecciones direccionEntregar = await this._unitOfWork.MtrDireccionesRepository.GetById(generalQuotes.IdDireccionEntregar);
                if (direccionEntregar != null)
                {
                    cotizacionNew.DireccionEntregar= direccionEntregar.Direccion.Trim() + direccionEntregar.Direccion1.Trim() + direccionEntregar.Direccion2.Trim();
                    cotizacionNew.EstadoEntregar = direccionEntregar.Estado.Trim();
                    cotizacionNew.MunicipioEntregar = direccionEntregar.Municipio.Trim();
                }
             
              
             
                
                if (cotizacionNew.CodCliente == "000000")
                {
                    cotizacionNew.RazonSocial = generalQuotes.RazonSocial;
                    cotizacionNew.Rif = generalQuotes.Rif;
                    cotizacionNew.DireccionEntregar = generalQuotes.Direccion;
                    cotizacionNew.RifEntregar = generalQuotes.Rif;
                }
                else
                {
                    cotizacionNew.RazonSocial = cliente.Nombre.Trim();
                    cotizacionNew.Rif = cliente.NoRegTribut;
                    cotizacionNew.DireccionEntregar = direccionEntregar.Direccion.Trim() + direccionEntregar.Direccion1.Trim() + direccionEntregar.Direccion2.Trim();
                    cotizacionNew.RifEntregar = direccionEntregar.Rif;
                }
                cotizacionNew.OrdenCompra = generalQuotes.OrdenCompra;
                cotizacionNew.FlagActualizado = "X";
                cotizacionNew.IdDireccionEntregar = new Decimal?(generalQuotes.IdDireccionEntregar);
                long? idMtrTipoMoneda = generalQuotes.IdMtrTipoMoneda;
                long num = 1;
                cotizacionNew.NegocioEnUsd = !(idMtrTipoMoneda.GetValueOrDefault() == num & idMtrTipoMoneda.HasValue) ? new bool?(false) : new bool?(true);
                cotizacionNew.FijarPrecioBs = generalQuotes.FijarPrecioBs;
                cotizacionNew.OrigenExterno = new bool?(true);
                await this._unitOfWork.CotizacionRepository.Add(cotizacionNew);
                await this._unitOfWork.SaveChangesAsync();
                cotizacionNew = (Wsmy501)null;
                cliente = (MtrCliente)null;
                cotizacion = (Wsmy501)null;
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
       
        }

        public async Task UpdateDetailCotizacion(AppDetailQuotes appDetailQuotes)
        {
            AppProducts appProducts = await this._unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
            if (appProducts != null)
            {

                var civy004 = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(appProducts.ExternalCode);
                if (civy004 != null)
                {
                    appProducts.Inventariable = true;
                }
                else
                {
                    appProducts.Inventariable = false;
                }

            }
            Wsmy406 aplicacionProducto = await this._unitOfWork.Wsmy406Repository.GetByProduct(appProducts.ExternalCode.Trim());
            var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(appProducts.ExternalCode.Trim());
            Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(appDetailQuotes.Cotizacion, appProducts.ExternalCode.Trim());
            if (renglon != null)
            {
                if (aplicacionProducto != null)
                {
                    renglon.SubCategoria = aplicacionProducto.CodAplicacion;
                }
                else
                {
                    renglon.SubCategoria = (int)csmy036.Idsubcategoria;
                }

                renglon.Descripcion = appDetailQuotes.NombreComercialProducto;
                renglon.Estatus = new int?(appDetailQuotes.IdEstatus);
                renglon.RazonGanadaPerdida = appDetailQuotes.RazonGanadaPerdida;
                renglon.Competidor = appDetailQuotes.Competidor;
                var unidad = await this._unitOfWork.AppUnitsRepository.GetById(appDetailQuotes.IdUnidad);
                renglon.UnidadCotizacion = unidad.Description2;

                renglon.Observaciones = appDetailQuotes.Observaciones;
                renglon.TiempoEntrega = new int?(appDetailQuotes.DiasEntrega);
                renglon.Probabilidad = new int?(50);
                renglon.FechaRegistro = new DateTime?(appDetailQuotes.AppGeneralQuotes.Fecha);
                renglon.TotalRenglon = new Decimal?(appDetailQuotes.Total);
                renglon.TotalRenglonUsd = new Decimal?(appDetailQuotes.TotalUsd);
                renglon.FlagEstimada = true;
                if ((await this._unitOfWork.AppStatusQuoteRepository.GetById(appDetailQuotes.IdEstatus)).FlagGanada == "X")
                {
                    renglon.TotalGanado = new Decimal?(appDetailQuotes.Total);
                    renglon.TotalGanadoUsd = new Decimal?(appDetailQuotes.TotalUsd);
                    renglon.EstatusPlanta = "V";
                }
                if ((await this._unitOfWork.AppStatusQuoteRepository.GetById(appDetailQuotes.IdEstatus)).PrimeraEstacion == "X")
                {
                    renglon.TotalGanado = 0;
                    renglon.TotalGanadoUsd = 0;
                    renglon.EstatusPlanta = "";
                }
                AppService.Core.EntitiesMaestros.Csmy036 byCode = await this._unitOfWork.Csmy036Repository.GetByCode(renglon.CodProducto);
                if (byCode != null)
                    renglon.SubCategoria = new int?((int)byCode.Idsubcategoria);
                await this._unitOfWork.RenglonRepository.Update(renglon);
                await this._unitOfWork.SaveChangesAsync();
                await this.UpdatePropuestaCotizacion(appDetailQuotes, renglon.Renglon);

            }
            else
            {
                Wsmy502 renglonNew = new Wsmy502();
                renglonNew.Cotizacion = appDetailQuotes.Cotizacion;
                renglonNew.Renglon = await this.getNextRenglon(appDetailQuotes.Cotizacion);
                if (appDetailQuotes.IdProductoNavigation != null)
                {
                    renglonNew.CodProducto = appDetailQuotes.IdProductoNavigation.ExternalCode.Trim();
                }
                else
                {
                    var prod = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
                    if (prod != null)
                    {
                        renglonNew.CodProducto = prod.ExternalCode;
                    }
                }
                if (aplicacionProducto != null)
                {
                    renglonNew.SubCategoria = aplicacionProducto.CodAplicacion;
                }
                else
                {
                    renglonNew.SubCategoria = (int)csmy036.Idsubcategoria;
                }


                renglonNew.Estatus = new int?(appDetailQuotes.IdEstatus);
                renglonNew.RazonGanadaPerdida = appDetailQuotes.RazonGanadaPerdida;
                renglonNew.Competidor = appDetailQuotes.Competidor;

                var unidad = await this._unitOfWork.AppUnitsRepository.GetById(appDetailQuotes.IdUnidad);
                renglonNew.UnidadCotizacion = unidad.Description2;

                renglonNew.Observaciones = appDetailQuotes.Observaciones;
                renglonNew.TiempoEntrega = new int?(appDetailQuotes.DiasEntrega);
                renglonNew.Probabilidad = new int?(50);
                renglonNew.FechaRegistro = new DateTime?(appDetailQuotes.AppGeneralQuotes.Fecha);
                renglonNew.TotalRenglon = new Decimal?(appDetailQuotes.Total);
                renglonNew.TotalRenglonUsd = new Decimal?(appDetailQuotes.TotalUsd);
                renglonNew.FlagEstimada = true;
                if ((await this._unitOfWork.AppStatusQuoteRepository.GetById(appDetailQuotes.IdEstatus)).FlagGanada == "X")
                {
                    renglonNew.TotalGanado = new Decimal?(appDetailQuotes.Total);
                    renglonNew.TotalGanadoUsd = new Decimal?(appDetailQuotes.TotalUsd);
                }
                if ((await this._unitOfWork.AppStatusQuoteRepository.GetById(appDetailQuotes.IdEstatus)).PrimeraEstacion == "X")
                {
                    renglonNew.TotalGanado = 0;
                    renglonNew.TotalGanadoUsd = 0;
                    renglonNew.EstatusPlanta = "";
                }
                
                AppService.Core.EntitiesMaestros.Csmy036 byCode = await this._unitOfWork.Csmy036Repository.GetByCode(renglonNew.CodProducto);
                if (byCode != null)
                    renglonNew.SubCategoria = new int?((int)byCode.Idsubcategoria);
                await this._unitOfWork.RenglonRepository.Add(renglonNew);
                var resSave = await this._unitOfWork.SaveChangesAsync();

                await this.UpdatePropuestaCotizacion(appDetailQuotes, renglonNew.Renglon);
                if (appDetailQuotes.SolicitarPrecio.Value && this._aprobacionesServices.GetByCotizacionRenglonPrpopuesta(renglonNew.Cotizacion, renglonNew.Renglon, 1) == null)
                {
                    if ((await this._aprobacionesServices.CreateAprobacion(renglonNew.Cotizacion, renglonNew.Renglon, 1, appDetailQuotes.UserCreate)).Meta.IsValid)
                    {
                        ApiResponse<Wsmy647> apiResponse = await this._aprobacionesServices.ActivarWORKFLOW(renglonNew.Cotizacion, renglonNew.Renglon, 1, appDetailQuotes.UserCreate, appDetailQuotes);
                    }
                }

                renglonNew = (Wsmy502)null;
                appProducts = (AppProducts)null;
                renglon = (Wsmy502)null;
            }
        }
        /*public async Task<Decimal> GetFleteByIdDireccionEntrega(Decimal id)
        {
            MtrDirecciones byId = await this._unitOfWork.MtrDireccionesRepository.GetById(id);
            Decimal direccionEntrega;
            if (byId != null)
            {
                var byEstadoMunicipio = await this._unitOfWork.Winy243Repository.GetByEstadoMunicipio(byId.Estado, byId.Municipio);
                direccionEntrega = byEstadoMunicipio == null ? 0M : byEstadoMunicipio.PorcFlete.Value;
            }
            else
                direccionEntrega = 0M;
            return direccionEntrega;
        }
        
        public async Task<Decimal> GetFleteByIdMunicipio(decimal idMunicipio)
        {
            Decimal direccionEntrega=0;
            var byEstadoMunicipio = await this._unitOfWork.Winy243Repository.GetFleteByIdMunicipo(idMunicipio);
            direccionEntrega = byEstadoMunicipio == null ? 0M : byEstadoMunicipio;
          
            return direccionEntrega;
        }*/


        public async Task UpdatePropuestaCotizacion(AppDetailQuotes appDetailQuotes, int renglon)
        {

            var appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);
            decimal porcFlete;
            if (appDetailQuotes.PorcFlete == null) appDetailQuotes.PorcFlete = 0;
            porcFlete = (decimal)appDetailQuotes.PorcFlete;
           
     
            

            bool ganandoProducto = false;
            ValoresCotizacionDto valoresCotizacion = await this.ValoresCotizacion(appDetailQuotes);

            var prod = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
            int cantTintas = 0;
            List<AppRecipes> productIdVariableCode1 = await this._unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(appDetailQuotes.IdProducto, "CANT_TINTAS");
            if (productIdVariableCode1 != null && productIdVariableCode1.Count > 0)
                cantTintas = Convert.ToInt32((object)productIdVariableCode1.FirstOrDefault<AppRecipes>().Quantity);
            int cantPartes = 0;
            List<AppRecipes> productIdVariableCode2 = await this._unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(appDetailQuotes.IdProducto, "CANT_PARTES");
            if (productIdVariableCode2 != null && productIdVariableCode2.Count > 0)
                cantPartes = Convert.ToInt32((object)productIdVariableCode2.FirstOrDefault<AppRecipes>().Quantity);

            decimal cantFormasRollo = 0;
            List<AppRecipes> productIdVariableCodeFormasRollo = await this._unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(appDetailQuotes.IdProducto, "CANTFORMAS");
            if (productIdVariableCodeFormasRollo != null && productIdVariableCodeFormasRollo.Count > 0)
                cantFormasRollo = Convert.ToInt32((object)productIdVariableCodeFormasRollo.FirstOrDefault<AppRecipes>().Quantity);

            Wsmy515 propuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(appDetailQuotes.Cotizacion, renglon, 1);
            if (propuesta != null)
            {

                var civy004 = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(prod.ExternalCode);
                if (civy004 != null)
                {
                    propuesta.Cantidad = appDetailQuotes.CantidadSolicitada;
                }
                else
                {
                    propuesta.Cantidad = new Decimal?((Decimal)valoresCotizacion.CantMill.Value);
                }


                propuesta.CantFormas = valoresCotizacion.CantFormas;
                propuesta.CantMill = valoresCotizacion.CantMill;
                propuesta.PrecioUnitario = valoresCotizacion.PrecioUnitario;
                propuesta.TotalPropuesta = valoresCotizacion.TotalPropuesta;
                propuesta.PrecioUnitarioUsd = valoresCotizacion.PrecioUnitarioUsd;
                propuesta.TotalPropuestaUsd = valoresCotizacion.TotalPropuestaUsd;
                propuesta.PrecioUnitarioExterno = valoresCotizacion.PrecioUnitarioExterno;
                propuesta.TotalPropuestaExterno = valoresCotizacion.TotalPropuestaExterno;
                propuesta.PrecioUnitarioUsdExterno = valoresCotizacion.PrecioUnitarioUsdExterno;
                propuesta.TotalPropuestaUsdExterno = valoresCotizacion.TotalPropuestaUsdExterno;
                propuesta.CantidadExterno = valoresCotizacion.CantidadExterno;
                propuesta.IdUnidadExterna = valoresCotizacion.IdUnidadExterna;
                propuesta.CodeUnidadExterna = valoresCotizacion.CodeUnidadExterna;
                propuesta.DescripcionUnidadExterna = valoresCotizacion.DescripcionUnidadExterna;
                propuesta.ValorConvertidoExterno = valoresCotizacion.ValorConvertidoExterno;
                propuesta.CantidadPorUnidadProduccion = valoresCotizacion.CantidadPorUnidadProduccion;
                propuesta.CantidadSolicitadaExterna = valoresCotizacion.CantidadSolicitadaExterna;
                propuesta.IdUnidadPrecio = valoresCotizacion.IdUnidadPrecio;
                propuesta.CodeUnidaPrecio = valoresCotizacion.CodeUnidaPrecio;
                propuesta.CantTintasExterna = cantTintas;
                propuesta.CantPartesExterna = cantPartes;
                int? estatus = propuesta.Estatus;
                int num = 2;
                if (appDetailQuotes.IdEstatus == 5)
                    ganandoProducto = true;
                propuesta.Estatus = new int?(appDetailQuotes.IdEstatus);
                propuesta.Observaciones = appDetailQuotes.Observaciones;
                if (valoresCotizacion.CantidadXCaja > 0)
                {
                    propuesta.CantXCaja = valoresCotizacion.CantidadXCaja;
                }

                //Si es rollo asigna la cantidad de formas por Rollo
                if (cantFormasRollo > 0) {
                    propuesta.CantXCaja = cantFormasRollo;
                }

                AppService.Core.EntitiesMaestros.Csmy036 byCode = await this._unitOfWork.Csmy036Repository.GetByCode(prod.ExternalCode.Trim());
                if (byCode != null)
                {
                    propuesta.UnidadCosteo = byCode.UnidadCosteo;

                }

                propuesta.CotizacionRenglonPropuesta = propuesta.Cotizacion + propuesta.Renglon.ToString() + propuesta.Propuesta.ToString();

                var flete = (appDetailQuotes.UnitPriceBaseProduction * porcFlete) /100;
                var fleteMaximo = (appDetailQuotes.UnitPriceBaseProductionMaximo  * porcFlete) /100;
                
                propuesta.UsdListaCpj = appDetailQuotes.UnitPriceBaseProductionMaximo + fleteMaximo;
                propuesta.UsdListaCpjminimo = appDetailQuotes.UnitPriceBaseProduction + flete;

                propuesta.TotalUsdListaCpj = (appDetailQuotes.UnitPriceBaseProductionMaximo + fleteMaximo)* appDetailQuotes.Cantidad;
                propuesta.TotalUsdListaCpjminimo = ((appDetailQuotes.UnitPriceBaseProduction + flete)* appDetailQuotes.Cantidad);


                AppService.Core.EntitiesMC.TPaTasaReferencial tasaByFecha = await this._unitOfWork.TPaTasaReferencialRepository.GetTasaByFecha(DateTime.Now);
                Wsmy515 wsmy515_1 = propuesta;
                Decimal unitPrice = prod.UnitPrice;
                Decimal? tasa = tasaByFecha.Tasa;
                Decimal? nullable1 = tasa.HasValue ? new Decimal?(unitPrice * tasa.GetValueOrDefault()) : new Decimal?();
                wsmy515_1.BsListaCpjminimo = nullable1;
                Wsmy515 wsmy515_2 = propuesta;
                Decimal? bsListaCpjminimo = propuesta.BsListaCpjminimo;
                Decimal cantidad = appDetailQuotes.Cantidad;
                Decimal? nullable2 = bsListaCpjminimo.HasValue ? new Decimal?(bsListaCpjminimo.GetValueOrDefault() * cantidad) : new Decimal?();
                wsmy515_2.TotalBsListaCpjminimo = nullable2;
                propuesta.IdPresentacion = (await this._unitOfWork.RenglonRepository.GetByCotizacionRenglon(propuesta.Cotizacion, propuesta.Renglon)).UnidadCotizacion;
                propuesta.Cajas = propuesta.Cantidad;
                propuesta.PorMc = new Decimal?(0M);
                propuesta.BsMc = new Decimal?(0M);
                propuesta.ImpresionTiro = "";
                propuesta.ImpresionRetiro = "";
                propuesta.Medidas = new int?(0);
                propuesta.Papel = "";
                propuesta.Partes = new int?(0);
                propuesta.Respaldo = "";
                propuesta.MedidaTexto = "";
                propuesta.LoteAct = new Decimal?(0M);
                propuesta.RecalcularMargen = "";
                if (propuesta.Estatus >= 5)
                {
                    propuesta.FlagAprobado = true;
                }
                else
                {
                    propuesta.FlagAprobado = false;
                }
                if (propuesta.OdooId == null)
                {
                    propuesta.OdooId = 0;
                }
                if ((await this._unitOfWork.AppStatusQuoteRepository.GetById(appDetailQuotes.IdEstatus)).PrimeraEstacion == "X")
                {
                    propuesta.EstatusPlanta = "";
                }


                this._unitOfWork.PropuestaRepository.Update(propuesta);
                await this._unitOfWork.SaveChangesAsync();
                short tipoOrden = 1;
                var wpry229 =
                    await _unitOfWork.Wpry229Repository.GetByCotizacionRenglonPropuesta(propuesta.Cotizacion,
                        propuesta.Renglon, propuesta.Propuesta);
                if (wpry229 != null)
                {
                    tipoOrden = (short)wpry229.TipoOrden;
                }
                if (appDetailQuotes.OrdenAnterior == null) appDetailQuotes.OrdenAnterior = 0;
                if (!prod.Inventariable)
                {
                    if (appDetailQuotes.OrdenAnterior > 0 && tipoOrden==1)
                    {
                        await CopiarDatosOrdenAnterior((long)appDetailQuotes.OrdenAnterior, propuesta.Cotizacion, propuesta.Renglon, propuesta.Propuesta, appDetailQuotes);
                    }
                    else
                    {
                        
                        await this.UpdateWpry229(appDetailQuotes, propuesta.Renglon);
                        await this.UpdateWpry240(appDetailQuotes, propuesta.Renglon);
                        
                      
                        // await this.UpdateWpry241(appDetailQuotes, propuestaNew.Renglon);
                    }


                }
                if (!ganandoProducto)
                {
                    valoresCotizacion = (ValoresCotizacionDto)null;
                    propuesta = (Wsmy515)null;
                }
                else
                {

                    //Eliminamos el detalle de productos en Ceos
                    await this._unitOfWork.DatosProductosRepository.DeleteByCotizacion(appDetailQuotes.Cotizacion);
                    await this._unitOfWork.SaveChangesAsync();

                    var details = await _unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appDetailQuotes.AppGeneralQuotesId);
                    if (details != null)
                    {
                        foreach (var item in details.Where(x => x.IdEstatus == 5).ToList())
                        {
                            var prodDetail = await _unitOfWork.AppProductsRepository.GetById(item.IdProducto);
                            await this.PasarDatosACeos(item.Cotizacion, prodDetail.ExternalCode.Trim());
                        }
                    }

                    valoresCotizacion = (ValoresCotizacionDto)null;
                    propuesta = (Wsmy515)null;
                }

            }
            else
            {
                Wsmy515 propuestaNew = new Wsmy515();
                propuestaNew.Cotizacion = appDetailQuotes.Cotizacion;
                propuestaNew.Renglon = renglon;
                propuestaNew.Propuesta = 1;

                var civy004 = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(prod.ExternalCode);
                if (civy004 != null)
                {
                    propuestaNew.Cantidad = appDetailQuotes.CantidadSolicitada;
                }
                else
                {
                    propuestaNew.Cantidad = new Decimal?((Decimal)valoresCotizacion.CantMill.Value);
                }




                propuestaNew.CantFormas = valoresCotizacion.CantFormas;
                propuestaNew.CantMill = valoresCotizacion.CantMill;
                propuestaNew.PrecioUnitario = valoresCotizacion.PrecioUnitario;
                propuestaNew.TotalPropuesta = valoresCotizacion.TotalPropuesta;
                propuestaNew.PrecioUnitarioUsd = valoresCotizacion.PrecioUnitarioUsd;
                propuestaNew.TotalPropuestaUsd = valoresCotizacion.TotalPropuestaUsd;
                propuestaNew.PrecioUnitarioExterno = valoresCotizacion.PrecioUnitarioExterno;
                propuestaNew.TotalPropuestaExterno = valoresCotizacion.TotalPropuestaExterno;
                propuestaNew.PrecioUnitarioUsdExterno = valoresCotizacion.PrecioUnitarioUsdExterno;
                propuestaNew.TotalPropuestaUsdExterno = valoresCotizacion.TotalPropuestaUsdExterno;
                propuestaNew.CantidadExterno = valoresCotizacion.CantidadExterno;
                propuestaNew.IdUnidadExterna = valoresCotizacion.IdUnidadExterna;
                propuestaNew.CodeUnidadExterna = valoresCotizacion.CodeUnidadExterna;
                propuestaNew.DescripcionUnidadExterna = valoresCotizacion.DescripcionUnidadExterna;
                propuestaNew.ValorConvertidoExterno = valoresCotizacion.ValorConvertidoExterno;
                propuestaNew.CantidadPorUnidadProduccion = valoresCotizacion.CantidadPorUnidadProduccion;
                propuestaNew.CantidadSolicitadaExterna = valoresCotizacion.CantidadSolicitadaExterna;
                propuestaNew.IdUnidadPrecio = valoresCotizacion.IdUnidadPrecio;
                propuestaNew.CodeUnidaPrecio = valoresCotizacion.CodeUnidaPrecio;
                propuestaNew.CantTintasExterna = cantTintas;
                propuestaNew.CantPartesExterna = cantPartes;
                propuestaNew.Estatus = new int?(appDetailQuotes.IdEstatus);
                propuestaNew.Observaciones = appDetailQuotes.Observaciones;
                propuestaNew.CantXCaja = valoresCotizacion.CantidadXCaja;
                //Si es rollo asigna la cantidad de formas por Rollo
                if (cantFormasRollo > 0) {
                    propuestaNew.CantXCaja = cantFormasRollo;
                }

                AppService.Core.EntitiesMaestros.Csmy036 byCode = await this._unitOfWork.Csmy036Repository.GetByCode(prod.ExternalCode.Trim());
                if (byCode != null)
                {
                    propuestaNew.UnidadCosteo = byCode.UnidadCosteo;

                }

                propuestaNew.CotizacionRenglonPropuesta = propuestaNew.Cotizacion + propuestaNew.Renglon.ToString() + propuestaNew.Propuesta.ToString();

                var flete = (appDetailQuotes.UnitPriceBaseProduction * porcFlete) / 100;
                var fleteMaximo = (appDetailQuotes.UnitPriceBaseProductionMaximo * porcFlete) / 100;
                propuestaNew.UsdListaCpj = appDetailQuotes.UnitPriceBaseProductionMaximo + fleteMaximo;
                propuestaNew.UsdListaCpjminimo = appDetailQuotes.UnitPriceBaseProduction + flete;

                propuestaNew.TotalUsdListaCpj = ((appDetailQuotes.UnitPriceBaseProductionMaximo + fleteMaximo) * appDetailQuotes.Cantidad);
                propuestaNew.TotalUsdListaCpjminimo = ((appDetailQuotes.UnitPriceBaseProduction + flete) * appDetailQuotes.Cantidad);

                AppService.Core.EntitiesMC.TPaTasaReferencial tasaByFecha = await this._unitOfWork.TPaTasaReferencialRepository.GetTasaByFecha(DateTime.Now);
                Wsmy515 wsmy515_3 = propuestaNew;
                Decimal unitPrice = prod.UnitPrice;
                Decimal? tasa = tasaByFecha.Tasa;
                Decimal? nullable3 = tasa.HasValue ? new Decimal?(unitPrice * tasa.GetValueOrDefault()) : new Decimal?();
                wsmy515_3.BsListaCpjminimo = nullable3;
                Wsmy515 wsmy515_4 = propuestaNew;
                Decimal? bsListaCpjminimo = propuestaNew.BsListaCpjminimo;
                Decimal cantidad = appDetailQuotes.Cantidad;
                Decimal? nullable4 = bsListaCpjminimo.HasValue ? new Decimal?(bsListaCpjminimo.GetValueOrDefault() * cantidad) : new Decimal?();
                wsmy515_4.TotalBsListaCpjminimo = nullable4;
                var wsmy502 = await this._unitOfWork.RenglonRepository.GetByCotizacionRenglon(propuestaNew.Cotizacion, propuestaNew.Renglon);
                if (wsmy502 != null)
                {
                    propuestaNew.IdPresentacion = wsmy502.UnidadCotizacion;
                }
                if ((await this._unitOfWork.AppStatusQuoteRepository.GetById(appDetailQuotes.IdEstatus)).PrimeraEstacion == "X")
                {
                    propuestaNew.EstatusPlanta = "";
                }
                propuestaNew.Cajas = propuestaNew.Cantidad;
                propuestaNew.PorMc = new Decimal?(0M);
                propuestaNew.BsMc = new Decimal?(0M);
                propuestaNew.ImpresionTiro = "";
                propuestaNew.ImpresionRetiro = "";
                propuestaNew.Medidas = new int?(0);
                propuestaNew.Papel = "";
                propuestaNew.Partes = new int?(0);
                propuestaNew.Respaldo = "";
                propuestaNew.MedidaTexto = "";
                propuestaNew.LoteAct = new Decimal?(0M);
                propuestaNew.RecalcularMargen = "";

                propuestaNew.OdooId = 0;

                await _unitOfWork.PropuestaRepository.Add(propuestaNew);
                await _unitOfWork.SaveChangesAsync();
                
                if (!prod.Inventariable)
                {
                    short tipoOrden = 1;
                    var wpry229 =
                        await _unitOfWork.Wpry229Repository.GetByCotizacionRenglonPropuesta(propuestaNew.Cotizacion,
                            propuestaNew.Renglon, propuestaNew.Propuesta);
                    if (wpry229 != null)
                    {
                        tipoOrden = (short)wpry229.TipoOrden;
                    }
                    if (appDetailQuotes.OrdenAnterior == null) appDetailQuotes.OrdenAnterior = 0;
                    if (appDetailQuotes.OrdenAnterior > 0 && tipoOrden==1)
                    {
                        await CopiarDatosOrdenAnterior((long)appDetailQuotes.OrdenAnterior, propuestaNew.Cotizacion, propuestaNew.Renglon, propuestaNew.Propuesta, appDetailQuotes);
                    }
                    else
                    {
                        await this.UpdateWpry229(appDetailQuotes, propuestaNew.Renglon);
                        await this.UpdateWpry240(appDetailQuotes, propuestaNew.Renglon);
                       
                    }

                    if (prod.AppSubCategoryId == 1)
                    {
                        _unitOfWork.Wpry241Repository.CreateTintaEtiquetaDigital(appDetailQuotes.Cotizacion);
                    }
                   

                }



                propuestaNew = (Wsmy515)null;
                valoresCotizacion = (ValoresCotizacionDto)null;
                propuesta = (Wsmy515)null;
            }




        }

        public async Task UpdateWpry229(AppDetailQuotes appDetailQuotes, int renglon)
        {
            var prod = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
            ValoresCotizacionDto valoresCotizacion = await this.ValoresCotizacion(appDetailQuotes);
            Wsmy515 propuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(appDetailQuotes.Cotizacion, renglon, 1);
            if (propuesta == null)
            {
                valoresCotizacion = (ValoresCotizacionDto)null;
                propuesta = (Wsmy515)null;
            }
            else
            {
                //Wsmy406 aplicacionProducto = await this._unitOfWork.Wsmy406Repository.GetByProduct(prod.ExternalCode.Trim());

                Wpry229 renglonPropuesta = await this._unitOfWork.Wpry229Repository.GetByCotizacionRenglonPropuesta(propuesta.Cotizacion, propuesta.Renglon, propuesta.Propuesta);
                if (renglonPropuesta != null)
                {
                    renglonPropuesta.CantidadProducto = (Decimal)valoresCotizacion.CantMill.Value;
                    renglonPropuesta.DescripcionSolicitud = appDetailQuotes.NombreComercialProducto;
                    renglonPropuesta.IdTipoProducto = (short)prod.CodAplicacion;
                    Wpry229 wpry229_1 = renglonPropuesta;
                    Decimal? precioUnitario = valoresCotizacion.PrecioUnitario;
                    Decimal num1 = precioUnitario.Value;
                    wpry229_1.ValorVenta = num1;
                    Wpry229 wpry229_2 = renglonPropuesta;
                    precioUnitario = valoresCotizacion.PrecioUnitario;
                    Decimal num2 = precioUnitario.Value;
                    wpry229_2.ValorLista = num2;
                    renglonPropuesta.ValorVentaUsd = valoresCotizacion.PrecioUnitarioUsd;
                    renglonPropuesta.Observaciones = appDetailQuotes.Observaciones;
                    renglonPropuesta.IdTipoCantidad = (byte)1;

                    this._unitOfWork.Wpry229Repository.Update(renglonPropuesta);
                    await this._unitOfWork.SaveChangesAsync();
                }
                else
                {
                    Wpry229 entity = new Wpry229();
                    entity.Cotizacion = propuesta.Cotizacion;
                    entity.Renglon = propuesta.Renglon;
                    entity.Propuesta = propuesta.Propuesta;
                    entity.CantidadProducto = (Decimal)valoresCotizacion.CantMill.Value;
                    entity.DescripcionSolicitud = appDetailQuotes.NombreComercialProducto;
                    entity.IdTipoProducto = (short)prod.CodAplicacion;;
                    Wpry229 wpry229_3 = entity;
                    Decimal? precioUnitario = valoresCotizacion.PrecioUnitario;
                    Decimal num4 = precioUnitario.Value;
                    wpry229_3.ValorVenta = num4;
                    Wpry229 wpry229_4 = entity;
                    precioUnitario = valoresCotizacion.PrecioUnitario;
                    Decimal num5 = precioUnitario.Value;
                    wpry229_4.ValorLista = num5;
                    entity.ValorVentaUsd = valoresCotizacion.PrecioUnitarioUsd;
                    entity.Observaciones = appDetailQuotes.Observaciones;
                    entity.IdTipoCantidad = (byte)1;
                    entity.OrdenAnterior = valoresCotizacion.OrdenAnterior;
                    long? ordenAnterior = valoresCotizacion.OrdenAnterior;
                    entity.TipoOrden = 3;
                    await this._unitOfWork.Wpry229Repository.Add(entity);
                    await this._unitOfWork.SaveChangesAsync();
                }
              
                valoresCotizacion = (ValoresCotizacionDto)null;
                propuesta = (Wsmy515)null;
            }
        }

        public async Task<Decimal> GetMedida(
          AppDetailQuotes appDetailQuotes,
          string variableMedida)
        {
            Decimal result = 0M;
            Decimal? nullable = appDetailQuotes.MedidaBasica;
            Decimal num1 = 0M;
            if (nullable.GetValueOrDefault() > num1 & nullable.HasValue && variableMedida == "MEDIDABASICA")
            {
                nullable = appDetailQuotes.MedidaBasica;
                result = nullable.Value;
                return result;
            }
            nullable = appDetailQuotes.MedidaOpuesta;
            Decimal num2 = 0M;
            if (nullable.GetValueOrDefault() > num2 & nullable.HasValue && variableMedida == "MEDIDAOPUESTA")
            {
                nullable = appDetailQuotes.MedidaOpuesta;
                result = nullable.Value;
                return result;
            }
            AppRecipes recipesByProductIdCode1 = await this._unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode(appDetailQuotes.IdProducto, "MEDIDABASICA");
            if (recipesByProductIdCode1 != null && variableMedida == "MEDIDABASICA")
            {
                nullable = recipesByProductIdCode1.Quantity;
                result = nullable.Value;
                return result;
            }
            AppRecipes recipesByProductIdCode2 = await this._unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode(appDetailQuotes.IdProducto, "MEDIDAOPUESTA");
            if (recipesByProductIdCode2 == null || !(variableMedida == "MEDIDAOPUESTA"))
                return result;
            nullable = recipesByProductIdCode2.Quantity;
            result = nullable.Value;
            return result;
        }

        public async Task<string> GetMedidaString(AppDetailQuotes appDetailQuotes, string variableMedida)
        {
            string result = string.Empty;


            if (variableMedida == "MEDIDABASICA")
            {
                AppRecipes recipes = await this._unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode(appDetailQuotes.IdProducto, "MEDIDABASICA");
                if (recipes != null)
                {
                    result = recipes.DescriptionSearch;


                }
            }
            if (variableMedida == "MEDIDAOPUESTA")
            {
                AppRecipes recipes = await this._unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode(appDetailQuotes.IdProducto, "MEDIDAOPUESTA");
                if (recipes != null)
                {
                    result = recipes.DescriptionSearch;


                }
            }




            return result;
        }

        public async Task<decimal> GetMedidaDecimal(AppDetailQuotes appDetailQuotes, string variableMedida)
        {
            decimal result = 0;


            if (variableMedida == "MEDIDABASICA")
            {
                AppRecipes recipes = await this._unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode(appDetailQuotes.IdProducto, "MEDIDABASICA");
                if (recipes != null)
                {
                    result = (decimal)recipes.Quantity;


                }
            }

            if (variableMedida == "MEDIDAOPUESTA")
            {
                AppRecipes recipes = await this._unitOfWork.AppRecipesRepository.GetRecipesByProductIdCode(appDetailQuotes.IdProducto, "MEDIDAOPUESTA");
                if (recipes != null)
                {
                    result = (decimal)recipes.Quantity;


                }

            }


            return result;
        }

        public async Task UpdateWsmy582(string producto,string medidaString,string humanRepresentation,decimal medidaDecimal)
        {
            var wsmy582 = await this._unitOfWork.WSMY582Repository.GetByProductoMedidaMascara(producto, medidaString);
            if ( wsmy582== null)
            {
                await this._unitOfWork.WSMY582Repository.Add(new Wsmy582()
                {
                    Producto = producto,
                    MedidaMascara = medidaString,
                    MedidaFraccion = humanRepresentation,
                    Frecuencia = new Decimal?((Decimal)1),
                    MedidaDecimal = medidaDecimal
                });
                await this._unitOfWork.SaveChangesAsync();
            }
        }
        public async Task UpdateWsmy583(string producto,string medidaString,string humanRepresentation,decimal medidaDecimal)
        {
            var wsmy583 = await this._unitOfWork.WSMY583Repository.GetByProductoMedidaMascara(producto, medidaString);
            if ( wsmy583== null)
            {
                await this._unitOfWork.WSMY583Repository.Add(new Wsmy583()
                {
                    Producto = producto,
                    MedidaMascara = medidaString,
                    MedidaFraccion = humanRepresentation,
                    Frecuencia = new Decimal?((Decimal)1),
                    MedidaDecimal = medidaDecimal
                });
                await this._unitOfWork.SaveChangesAsync();
            }
        }
        public static string ValueFormated(string medidaString)
        {
            medidaString = medidaString.Replace("/", "0");
            medidaString = medidaString.Replace(" ", "0");
            medidaString = medidaString.PadRight(5, '0');
            return medidaString;
                
        }  
        public async Task UpdateWpry240(AppDetailQuotes appDetailQuotes, int renglon)
        {

            var papelesActuales = await _unitOfWork.Wpry240Repository.GetByCotizacion(appDetailQuotes.Cotizacion);
            if (papelesActuales != null && papelesActuales.Count()>0)
            {
                return;
            }
            
            var prod = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);
            double factorPulgada = 2.54;
            bool? requiereDatosEntrada = prod.RequiereDatosEntrada;
            bool flag = false;
            if (requiereDatosEntrada.GetValueOrDefault() == flag & requiereDatosEntrada.HasValue)
                factorPulgada = 1.0;
            Wsmy515 propuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(appDetailQuotes.Cotizacion, renglon, 1);
            if (propuesta == null)
            {
                propuesta = (Wsmy515)null;
            }
            else
            {
                string code = "PAPEL";
                List<AppRecipes> productIdVariableCode = await this._unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(appDetailQuotes.IdProducto, code);
                if (productIdVariableCode == null)
                {
                    propuesta = (Wsmy515)null;
                }
                else
                {
                    foreach (AppRecipes itemReceta in productIdVariableCode)
                    {
                        IAppIngredientsRepository ingredientsRepository = this._unitOfWork.AppIngredientsRepository;
                        int? nullable = itemReceta.AppIngredientsId;
                        int id = nullable.Value;
                        AppIngredients ingrediente = await ingredientsRepository.GetById(id);
                        nullable = itemReceta.Secuencia;
                        int parte = nullable.Value;
                        var variableParte = await _unitOfWork.AppConfigAppRepository.GetByKey(itemReceta.Code);
                        if (variableParte != null)
                        {
                            parte = int.Parse(variableParte.Valor);
                        }
                        await this._unitOfWork.Wpry240Repository.Delete(propuesta.Cotizacion, propuesta.Renglon, propuesta.Propuesta, parte);
                        await this._unitOfWork.SaveChangesAsync();

                        Wpry240 wpry240new = new Wpry240();
                        wpry240new.Cotizacion = propuesta.Cotizacion;
                        wpry240new.Renglon = propuesta.Renglon;
                        wpry240new.Propuesta = propuesta.Propuesta;
                        wpry240new.IdParte = parte;
                        wpry240new.IdPapel = ingrediente.Code.ToString().Trim();
                        wpry240new.Cantidad = (decimal)propuesta.Cantidad;
                        wpry240new.FechaRegistro = DateTime.Now;
                        wpry240new.TipoPapel = wpry240new.IdPapel.Substring(0, 3);
                        wpry240new.Gramaje = wpry240new.IdPapel.Substring(3, 3);
                        wpry240new.MedidaBasicaFraccionPulgada = appDetailQuotes.MedidaBasicaFraccionPulgada;
                        wpry240new.MedidaOpuestaFraccionPulgada = appDetailQuotes.MedidaOpuestaFraccionPulgada;

                        string medidaString = String.Empty;
                        decimal medidaDecimal = 0;
                        string humanRepresentation = String.Empty;
                            

                           //var  medidaConvertida = ConvertirMedidas.ConvertirCmAPulgadas(basica);
                            
                            Fractional.Fractional medidaConvertida = new Fractional.Fractional(Math.Truncate(await this.GetMedida(appDetailQuotes, "MEDIDABASICA") / (Decimal)factorPulgada * 100M) / 100M, false);
                          
                            if (appDetailQuotes.MedidaBasica > 0)
                            {
                               var  medidaConvertidaCm = ConvertirMedidas.ConvertirCmAPulgadas((double)appDetailQuotes.MedidaBasica,true);
                                medidaString = medidaConvertidaCm.ValueFormat;
                                medidaDecimal = (decimal)medidaConvertidaCm.Value;
                                humanRepresentation = medidaConvertidaCm.HumanRepresentation;
                                wpry240new.MedidaBase = Convert.ToInt32(medidaString);
                                wpry240new.MedidaBasicaFraccionPulgada = medidaString;
                            }
                            else
                            {
                                medidaString = await this.GetMedidaString(appDetailQuotes, "MEDIDABASICA");
                                medidaDecimal = await this.GetMedidaDecimal(appDetailQuotes, "MEDIDABASICA");
                                humanRepresentation = medidaString;
                                var valorFormateado= ValueFormated(medidaString);
                                wpry240new.MedidaBase =Convert.ToInt32(valorFormateado);
                                wpry240new.MedidaBasicaFraccionPulgada = medidaString;
                            }
                            await UpdateWsmy582(prod.ExternalCode.Trim(),medidaString,humanRepresentation,medidaDecimal);
                            medidaString = "";
                            medidaDecimal = 0;
                            humanRepresentation = "";
                          
                             if (appDetailQuotes.MedidaOpuesta > 0)
                            {
                                var  medidaConvertidaCm = ConvertirMedidas.ConvertirCmAPulgadas((double)appDetailQuotes.MedidaOpuesta,true);

                                medidaString = medidaConvertidaCm.ValueFormat;
                                medidaDecimal = (decimal)medidaConvertidaCm.Value;
                                humanRepresentation = medidaConvertidaCm.HumanRepresentation;
                                wpry240new.MedidaOpuesta = Convert.ToInt32(medidaString);
                                wpry240new.MedidaOpuestaFraccionPulgada = medidaString;
                            }
                            else
                            {
                               // var medida = Math.Truncate(await this.GetMedida(appDetailQuotes, "MEDIDAOPUESTA") / (Decimal)factorPulgada * 100M) / 100M;

                                medidaString = await this.GetMedidaString(appDetailQuotes, "MEDIDAOPUESTA");
                                medidaDecimal = await this.GetMedidaDecimal(appDetailQuotes, "MEDIDAOPUESTA");
                                humanRepresentation = medidaString;
                                var valorFormateado= ValueFormated(medidaString);
                                wpry240new.MedidaOpuesta =Convert.ToInt32(valorFormateado);
                                wpry240new.MedidaOpuestaFraccionPulgada = medidaString;

                            }
                             
                            await UpdateWsmy583(prod.ExternalCode.Trim(),medidaString,humanRepresentation,medidaDecimal);


                            wpry240new.LargoCm = (decimal)wpry240new.MedidaBase;
                            wpry240new.AnchoCm = (decimal)wpry240new.MedidaOpuesta;
                            wpry240new.IdConstruccion = 3;
                            
                        

                         
                            await this._unitOfWork.Wpry240Repository.Add(wpry240new);
                            await this._unitOfWork.SaveChangesAsync();
                            wpry240new = (Wpry240)null;
                  
                   
                    }
                    propuesta = (Wsmy515)null;
                }
            }



            var papeles = await _unitOfWork.Wpry240Repository.GetPapeles(appDetailQuotes.Cotizacion);
            var detailFind = await _unitOfWork.AppDetailQuotesRepository.GetById(appDetailQuotes.Id);
            if (detailFind != null)
            {
                detailFind.Papeles = papeles;
                _unitOfWork.AppDetailQuotesRepository.Update(detailFind);
                await this._unitOfWork.SaveChangesAsync();
            }



        }

        public async Task UpdateWpry241(AppDetailQuotes appDetailQuotes, int renglon)
        {
            Wsmy515 propuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(appDetailQuotes.Cotizacion, renglon, 1);
            if (propuesta == null)
            {
                propuesta = (Wsmy515)null;
            }
            else
            {
                string code = "TINTA";
                List<AppRecipes> productIdVariableCode = await this._unitOfWork.AppRecipesRepository.GetListRecipesByProductIdVariableCode(appDetailQuotes.IdProducto, code);
                if (productIdVariableCode == null)
                {
                    propuesta = (Wsmy515)null;
                }
                else
                {
                    foreach (AppRecipes itemReceta in productIdVariableCode.Where<AppRecipes>((Func<AppRecipes, bool>)(r =>
                    {
                        int? secuencia = r.Secuencia;
                        int num = 0;
                        return secuencia.GetValueOrDefault() > num & secuencia.HasValue;
                    })).ToList<AppRecipes>())
                    {
                        AppIngredients byId = await this._unitOfWork.AppIngredientsRepository.GetById(itemReceta.AppIngredientsId.Value);
                        int parte = itemReceta.Secuencia.Value;
                        string idTinta = byId.Code.Trim();
                        List<Wpry241> propuestaParteTinta = await this._unitOfWork.Wpry241Repository.GetByCotizacionRenglonPropuestaParteTinta(propuesta.Cotizacion, propuesta.Renglon, propuesta.Propuesta, parte, idTinta);
                        if (propuestaParteTinta != null)
                        {
                            foreach (Wpry241 wpry241 in propuestaParteTinta)
                            {
                                await this._unitOfWork.Wpry241Repository.Delete(wpry241.Recnum);
                                await this._unitOfWork.SaveChangesAsync();
                            }
                        }
                        await this._unitOfWork.Wpry241Repository.Add(new Wpry241()
                        {
                            Cotizacion = propuesta.Cotizacion,
                            Renglon = propuesta.Renglon,
                            Propuesta = propuesta.Propuesta,
                            IdParte = parte,
                            IdUbicacion = (short)1,
                            IdTinta = idTinta,
                            FechaRegistro = DateTime.Now
                        });
                        await this._unitOfWork.SaveChangesAsync();
                        idTinta = (string)null;
                    }
                    propuesta = (Wsmy515)null;
                }
            }


            var tintas = await _unitOfWork.Wpry241Repository.GetTintas(appDetailQuotes.Cotizacion);
            var detailFind = await _unitOfWork.AppDetailQuotesRepository.GetById(appDetailQuotes.Id);
            if (detailFind != null)
            {
                detailFind.Tintas = tintas.Trim();
                _unitOfWork.AppDetailQuotesRepository.Update(detailFind);
                await this._unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> getNextRenglon(string cotizacion)
        {
            int result = 1;
            List<Wsmy502> byCotizacion = await this._unitOfWork.RenglonRepository.GetByCotizacion(cotizacion);
            if (byCotizacion.Count >= 0)
                result = byCotizacion.Count + 1;
            return result;
        }

        public async Task<ValoresCotizacionDto> ValoresCotizacion(AppDetailQuotes appDetailQuotes)
        {
            ValoresCotizacionDto result = new ValoresCotizacionDto();


            var generalQuote = await _unitOfWork.AppGeneralQuotesRepository.GetById(appDetailQuotes.AppGeneralQuotesId);


            short unidadCosteo = 0;
            var prod = await _unitOfWork.AppProductsRepository.GetById(appDetailQuotes.IdProducto);



            var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(prod.ExternalCode);
            if (csmy036 != null)
            {
                unidadCosteo = (short)csmy036.UnidadCosteo;
            }


            //Actualizacion de variables externas en ecotizador
            result.PrecioUnitarioExterno = appDetailQuotes.Precio;
            result.TotalPropuestaExterno = appDetailQuotes.Total;
            result.PrecioUnitarioUsdExterno = appDetailQuotes.PrecioUsd;
            result.TotalPropuestaUsdExterno = appDetailQuotes.TotalUsd;
            result.CantidadExterno = appDetailQuotes.Cantidad;
            result.IdUnidadExterna = appDetailQuotes.IdUnidad;
            var unidad = await _unitOfWork.AppUnitsRepository.GetById(appDetailQuotes.IdUnidad);
            result.CodeUnidadExterna = unidad.Code;

            result.DescripcionUnidadExterna = unidad.Description2;
            result.ValorConvertidoExterno = appDetailQuotes.ValorConvertido;
            result.CantidadPorUnidadProduccion = appDetailQuotes.CantidadPorUnidadProduccion;
            result.CantidadSolicitadaExterna = appDetailQuotes.CantidadSolicitada;
            if (generalQuote.OrdenAnterior != null)
            {
                result.OrdenAnterior = generalQuote.OrdenAnterior;
            }
            else
            {
                result.OrdenAnterior = 0;
            }


            result.IdUnidadPrecio = prod.ProductionUnitId;
            var unidaProduccion = await _unitOfWork.AppUnitsRepository.GetById((int)prod.ProductionUnitId);
            result.CodeUnidaPrecio = unidaProduccion.Code;
            var productoConversion =
                await _unitOfWork.AppProductConversionRepository.GetAllByProduct(appDetailQuotes.IdProducto);
            if (productoConversion.Count > 0)
            {
                var firstConversion = productoConversion.Where(x => x.AppUnitsIdAlternativa == appDetailQuotes.IdUnidad)
                    .FirstOrDefault();
                if (firstConversion != null)
                {
                    unidaProduccion = await _unitOfWork.AppUnitsRepository.GetById((int)firstConversion.AppUnitsIdBase);
                    result.CodeUnidaPrecio = unidaProduccion.Code;
                }
            }
           



            /*if (prod.ProductionUnitId == appDetailQuotes.IdUnidad)
            {
                if (appDetailQuotes.ValorConvertido>0)
                {
                    var valor = (1 / appDetailQuotes.ValorConvertido) * appDetailQuotes.Cantidad;
                    result.Cantidad = Math.Truncate((decimal)valor);
                }
                else
                {
                    result.Cantidad = appDetailQuotes.CantidadSolicitada;
                }
              

            }
            else
            {
                result.Cantidad = appDetailQuotes.CantidadSolicitada;
            }*/
            result.Cantidad = appDetailQuotes.CantidadSolicitada;
            result.CantFormas = result.Cantidad;
            result.CantMill = (float)((decimal)result.Cantidad / (decimal)unidadCosteo);


            var formasEnCalculo = await _unitOfWork.AppRecipesByAppDetailQuotesRepository.GetListRecipesByProductCodeVariableCodeHistorico((int)appDetailQuotes.CalculoId, prod.Code, "TOTALFORMAS");
            if (formasEnCalculo != null && formasEnCalculo.Count > 0)
            {
                result.CantFormas = formasEnCalculo.FirstOrDefault().Quantity;
                result.CantMill = (float)((decimal)result.CantFormas / (decimal)unidadCosteo);
            }
              

            decimal precioUsdUnidadSolicitada = 0;
            decimal precioUnidadSolicitada = 0;

            decimal precioUsdMillarSolicitada = 0;
            decimal precioMillarSolicitada = 0;


            precioUsdUnidadSolicitada = appDetailQuotes.TotalUsd / (decimal)result.Cantidad;
            precioUsdMillarSolicitada = appDetailQuotes.TotalUsd / (decimal)result.CantMill;
            result.PrecioUnitarioUsd = precioUsdMillarSolicitada;
            result.TotalPropuestaUsd = appDetailQuotes.TotalUsd;


            precioUnidadSolicitada = appDetailQuotes.Total / (decimal)result.Cantidad;
            precioMillarSolicitada = appDetailQuotes.Total/ (decimal)result.CantMill; ;
            result.PrecioUnitario = precioMillarSolicitada;
            result.TotalPropuesta = appDetailQuotes.Total;

            var civy004 = await _unitOfWork.Csmy036Repository.GetCivy004ByCode(prod.ExternalCode);
            if (civy004 != null)
            {
                result.Cantidad = appDetailQuotes.CantidadSolicitada;
                result.CantidadXCaja = civy004.CantidadXCaja;
                result.CantMill = ((float)((decimal)result.Cantidad * civy004.CantidadXCaja) / (float)(decimal)unidadCosteo);
                result.CantFormas = result.Cantidad * result.CantidadXCaja;
                result.PrecioUnitarioUsd = appDetailQuotes.PrecioUsd;
                result.TotalPropuestaUsd = appDetailQuotes.TotalUsd;
                result.PrecioUnitario = appDetailQuotes.Precio;
                result.TotalPropuesta = appDetailQuotes.Total;
            }
            else
            {
                result.CantidadXCaja = 0;
            }

            return result;
        }


        public async Task<bool> ExisteWpry229ByCotizacion(string cotizacion) => await this._unitOfWork.Wpry229Repository.GetByCotizacion(cotizacion) != null;

        public async Task DeleteWpry229ByCotizacion(AppDetailQuotes appDetailQuotes)
        {
            List<Wpry229> byCotizacion = await this._unitOfWork.Wpry229Repository.GetByCotizacion(appDetailQuotes.Cotizacion);
            if (byCotizacion == null)
                return;
       
            
            
            foreach (Wpry229 wpry229 in byCotizacion)
            {
              
                await this._unitOfWork.Wpry229Repository.Delete(wpry229.Cotizacion,wpry229.Renglon,wpry229.Propuesta);
                await this._unitOfWork.SaveChangesAsync();
            }
        }
      

        public async Task DeleteWpry240ByCotizacion(AppDetailQuotes appDetailQuotes)
        {
            List<Wpry240> byCotizacion = await this._unitOfWork.Wpry240Repository.GetByCotizacion(appDetailQuotes.Cotizacion);
            if (byCotizacion == null)
                return;
            foreach (Wpry240 wpry240 in byCotizacion)
            {
                await this._unitOfWork.Wpry240Repository.Delete(wpry240.Cotizacion, wpry240.Renglon, wpry240.Propuesta, wpry240.IdParte);
                await this._unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteWpry241ByCotizacion(AppDetailQuotes appDetailQuotes)
        {
            List<Wpry241> byCotizacion = await this._unitOfWork.Wpry241Repository.GetByCotizacion(appDetailQuotes.Cotizacion);
            if (byCotizacion == null)
                return;
            foreach (Wpry241 wpry241 in byCotizacion)
            {
                await this._unitOfWork.Wpry241Repository.Delete(wpry241.Recnum);
                await this._unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteWpry251ByCotizacion(AppDetailQuotes appDetailQuotes)
        {

             var wpry251 = await this.GetByCotizacion(appDetailQuotes.Cotizacion);
             if (wpry251 != null)
             {
                 _unitOfWork.Wpry251Repository.DeleteByCotizacion(appDetailQuotes.Cotizacion);

             }
            
          
        }


        public async Task DeletePropuestaCotizacion(AppDetailQuotes appDetailQuotes, int renglon)
        {
            if (await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(appDetailQuotes.Cotizacion, renglon, 1) == null)
                return;
            await this._unitOfWork.PropuestaRepository.Delete(appDetailQuotes.Cotizacion, renglon, 1);
            await this._unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRenglonCotizacion(AppDetailQuotes appDetailQuotes, int renglon)
        {
            if (await this._unitOfWork.RenglonRepository.GetByCotizacionRenglon(appDetailQuotes.Cotizacion, renglon) == null)
                return;

            //Eliminamos el detalle de productos en Ceos
            await this._unitOfWork.DatosProductosRepository.DeleteByCotizacion(appDetailQuotes.Cotizacion);
            await this._unitOfWork.SaveChangesAsync();

            await this._unitOfWork.RenglonRepository.Delete(appDetailQuotes.Cotizacion, renglon);
            await this._unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCotizacion(AppGeneralQuotes appGeneralQuotes)
        {
            if (await this._unitOfWork.CotizacionRepository.GetByCotizacion(appGeneralQuotes.Cotizacion) == null)
                return;
            List<AppDetailQuotes> appGeneralQuotesId = await this._unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(appGeneralQuotes.Id);
            if (appGeneralQuotesId != null && appGeneralQuotesId.Count > 0)
            {
                foreach (AppDetailQuotes item in appGeneralQuotesId)
                {
                    Wsmy502 renglonObj = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(item.Cotizacion, item.IdProductoNavigation.ExternalCode.Trim());
                    if (renglonObj != null)
                    {
                        await this.DeleteWpry229ByCotizacion(item);
                        await this.DeleteWpry240ByCotizacion(item);
                        await this.DeleteWpry241ByCotizacion(item);
                        await this.DeleteWpry251ByCotizacion(item);

                        await this.DeletePropuestaCotizacion(item, renglonObj.Renglon);
                        await this.DeleteRenglonCotizacion(item, renglonObj.Renglon);
                    }
                    renglonObj = (Wsmy502)null;
                }
            }
            await this._unitOfWork.CotizacionRepository.Delete(appGeneralQuotes.Cotizacion);
            await this._unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCotizacionRenglon(AppDetailQuotes appDetailQuotes)
        {
            
            
            await _unitOfWork.PropuestaRepository.DeleteCotizacion(appDetailQuotes.Cotizacion);
            
           /* List<Wsmy502> renglonObj = await this._unitOfWork.RenglonRepository.GetByCotizacion(appDetailQuotes.Cotizacion);
            if (renglonObj != null)
            {
                foreach (var item in renglonObj)
                {
                    await this.DeleteWpry229ByCotizacion(appDetailQuotes);
                    await this.DeleteWpry240ByCotizacion(appDetailQuotes);
                    await this.DeleteWpry241ByCotizacion(appDetailQuotes);
                    await this.DeleteWpry251ByCotizacion(appDetailQuotes);
                    await this.DeletePropuestaCotizacion(appDetailQuotes, item.Renglon);
                    await this.DeleteRenglonCotizacion(appDetailQuotes, item.Renglon);
                }
            }*/
        }

        public async Task updateMedidas()
        {
            List<Wsmy582> all1 = await this._unitOfWork.WSMY582Repository.GetAll();
            if (all1 != null)
            {
                foreach (Wsmy582 entity in all1)
                {
                    Fractional.Fractional fractional = new Fractional.Fractional(entity.MedidaFraccion);
                    entity.MedidaDecimal = new Decimal?(fractional.Value);
                    this._unitOfWork.WSMY582Repository.Update(entity);
                    await this._unitOfWork.SaveChangesAsync();
                }
            }
            List<Wsmy583> all2 = await this._unitOfWork.WSMY583Repository.GetAll();
            if (all2 == null)
                return;
            foreach (Wsmy583 entity in all2)
            {
                Fractional.Fractional fractional = new Fractional.Fractional(entity.MedidaFraccion);
                entity.MedidaDecimal = new Decimal?(fractional.Value);
                this._unitOfWork.WSMY583Repository.Update(entity);
                await this._unitOfWork.SaveChangesAsync();
            }
        }

        public async Task PasarDatosACeos(string cotizacion, string producto)
        {
            try
            {




                AppService.Core.EntitiesMaestros.Civy004 civy004 = await this._unitOfWork.Csmy036Repository.GetCivy004ByCode(producto);
                if (civy004 == null)
                    return;
                Wsmy501 Wsmy501 = await this.GetByCotizacion(cotizacion);
                if (Wsmy501 == null)
                    return;
                var datosCliente = await this._unitOfWork.DatosClienteRepository.GetByCotizacion(cotizacion);
                if ( datosCliente== null)
                {
                    AppService.Core.DataContratosStock.DatosCliente entity = new AppService.Core.DataContratosStock.DatosCliente();
                    entity.NumCot = this._unitOfWork.DatosClienteRepository.NextNumCot(Wsmy501.CodVendedor);
                    entity.FecGrab = new DateTime?(DateTime.Now);
                    AppService.Core.DataContratosStock.DatosCliente datosCliente1 = entity;
                    string[] strArray = new string[5]
                    {
                        DateTime.Now.Year.ToString(),
                        "/",
                        null,
                        null,
                        null
                    };
                    DateTime now = DateTime.Now;
                    strArray[2] = now.Month.ToString();
                    strArray[3] = "/";
                    now = DateTime.Now;
                    strArray[4] = now.Day.ToString();
                    string str = string.Concat(strArray);
                    datosCliente1.FecOrd = str;
                    entity.CodVend = Wsmy501.CodVendedor;
                    entity.CodigoClient = Wsmy501.CodCliente;
                    entity.NomCliente = Wsmy501.RazonSocial.Trim();
                    entity.RifFact = Wsmy501.Rif;
                    entity.RifEnt = Wsmy501.RifEntregar;
                    entity.IdDireccion = Wsmy501.IdDireccion;
                    entity.IdDireccionEntregar = Wsmy501.IdDireccionEntregar;
                    entity.EstadoFacturar = Wsmy501.EstadoFacturar;
                    entity.EstadoEntregar = Wsmy501.EstadoEntregar;
                    entity.MunicipioEntregar = Wsmy501.MunicipioEntregar;
                    entity.MunicipioFacturar = Wsmy501.MunicipioFacturar;
                    entity.Contacto = Wsmy501.NombreContacto;
                    entity.NombFact = Wsmy501.RazonSocial.Trim();
                    entity.NomEnt = Wsmy501.RazonSocial.Trim();
                    if (Wsmy501.CodCliente == "000000")
                    {
                        entity.ClienteNuevo = "X";
                        MtrVendedor byId = this._unitOfWork.MtrVendedorRepository.GetById(Wsmy501.CodVendedor);
                        entity.Oficina = byId.Oficina.ToString().Trim();
                    }
                    else
                    {
                        entity.ClienteNuevo = "";
                        MtrCliente byId = this._unitOfWork.MtrClienteRepository.GetById(Wsmy501.CodCliente);
                        entity.Oficina = byId.OficinaVenta.Trim();
                    }

                    AppService.Core.DataContratosStock.DatosCliente datosCliente2 = entity;
                    int? nullable1 = Wsmy501.IdContacto;
                    Decimal? nullable2 = nullable1.HasValue ? new Decimal?((Decimal)nullable1.GetValueOrDefault()) : new Decimal?();
                    datosCliente2.IdContactos = nullable2;
                    entity.CotizacionOriginal = cotizacion;
                    entity.CotizacionGeneral = cotizacion;
                    AppService.Core.DataContratosStock.DatosCliente datosCliente3 = entity;
                    nullable1 = Wsmy501.Condicion;
                    Decimal? nullable3 = nullable1.HasValue ? new Decimal?((Decimal)nullable1.GetValueOrDefault()) : new Decimal?();
                    datosCliente3.DescTipoPago = nullable3;
                    entity.Estado = "V";
                    entity.ObservCliente = Wsmy501.Observaciones;
                    entity.Email = Wsmy501.EmailCliente.Trim();
                    await this._unitOfWork.DatosClienteRepository.Add(entity);
                    await this._unitOfWork.SaveChangesAsync();
                }
                else
                {
                    datosCliente.CodVend = Wsmy501.CodVendedor;
                    datosCliente.CodigoClient = Wsmy501.CodCliente;
                    datosCliente.NomCliente = Wsmy501.RazonSocial.Trim();
                    datosCliente.RifFact = Wsmy501.Rif;
                    datosCliente.RifEnt = Wsmy501.RifEntregar;
                    datosCliente.IdDireccion = Wsmy501.IdDireccion;
                    datosCliente.IdDireccionEntregar = Wsmy501.IdDireccionEntregar;
                    datosCliente.Contacto = Wsmy501.NombreContacto;
                    datosCliente.NombFact = Wsmy501.RazonSocial.Trim();
                    datosCliente.NomEnt = Wsmy501.RazonSocial.Trim();
                    datosCliente.EstadoFacturar = Wsmy501.EstadoFacturar;
                    datosCliente.EstadoEntregar = Wsmy501.EstadoEntregar;
                    datosCliente.MunicipioEntregar = Wsmy501.MunicipioEntregar;
                    datosCliente.MunicipioFacturar = Wsmy501.MunicipioFacturar;
                    if (Wsmy501.CodCliente == "000000")
                    {
                        datosCliente.ClienteNuevo = "X";
                        MtrVendedor byId = this._unitOfWork.MtrVendedorRepository.GetById(Wsmy501.CodVendedor);
                        datosCliente.Oficina = byId.Oficina.ToString().Trim();
                    }
                    else
                    {
                        datosCliente.ClienteNuevo = "";
                        MtrCliente byId = this._unitOfWork.MtrClienteRepository.GetById(Wsmy501.CodCliente);
                        datosCliente.Oficina = byId.OficinaVenta.Trim();
                    }
                    this._unitOfWork.DatosClienteRepository.Update(datosCliente);
                    await this._unitOfWork.SaveChangesAsync();
                }
                
                AppService.Core.DataContratosStock.DatosCliente datosClienteFind = await this._unitOfWork.DatosClienteRepository.GetByCotizacion(cotizacion);
                if (datosClienteFind != null)
                {
                    Wsmy502 renglon = await this._unitOfWork.RenglonRepository.GetByCotizacionProducto(cotizacion, producto);
                    Wsmy515 propuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(cotizacion, renglon.Renglon, 1);
                    if (await this._unitOfWork.DatosProductosRepository.GetByNumCotProducto(datosClienteFind.NumCot, producto) == null)
                    {
                        AppService.Core.EntitiesMaestros.Csmy036 byCode = await this._unitOfWork.Csmy036Repository.GetByCode(producto);
                        DatosProductos entity = new DatosProductos();
                        entity.NumCot = datosClienteFind.NumCot;
                        entity.Codigo = renglon.CodProducto;
                        entity.Descripcion = renglon.Descripcion;
                        entity.CantXCaja = new Decimal?((Decimal)civy004.CantidadXCaja);
                        entity.CantMill = new float?((float)(propuesta.Cantidad.Value * (Decimal)civy004.CantidadXCaja / (Decimal)byCode.UnidadCosteo.Value));
                        entity.Cajas = propuesta.Cantidad;
                        entity.PorcComRegulada = propuesta.PorcComRegulada;
                        entity.PorcRespSocial = propuesta.PorcRespSocial;
                        DatosProductos datosProductos1 = entity;
                        Decimal cantidadXcaja = (Decimal)civy004.CantidadXCaja;
                        Decimal? nullable4 = propuesta.Cantidad;
                        Decimal? nullable5 = nullable4.HasValue ? new Decimal?(cantidadXcaja * nullable4.GetValueOrDefault()) : new Decimal?();
                        datosProductos1.CantFormas = nullable5;
                        entity.VentaTot = propuesta.TotalPropuesta;
                        DatosProductos datosProductos2 = entity;
                        nullable4 = entity.VentaTot;
                        Decimal num = (Decimal)entity.CantMill.Value;
                        Decimal? nullable6 = nullable4.HasValue ? new Decimal?(nullable4.GetValueOrDefault() / num) : new Decimal?();
                        datosProductos2.VentaUnit = nullable6;
                        entity.ValorLista = entity.VentaUnit;
                        entity.ListaTot = entity.VentaTot;
                        entity.Concesion = new Decimal?(0M);
                        entity.Estado = "V";
                        DatosProductos datosProductos3 = entity;
                        short? unidadCosteo = byCode.UnidadCosteo;
                        Decimal? nullable7;
                        if (!unidadCosteo.HasValue)
                        {
                            nullable4 = new Decimal?();
                            nullable7 = nullable4;
                        }
                        else
                            nullable7 = new Decimal?((Decimal)unidadCosteo.GetValueOrDefault());
                        datosProductos3.UnidadCosteo = nullable7;
                        entity.CotizacionGeneral = cotizacion;
                        entity.Propuesta = new int?(1);
                        entity.Reglon = new int?(renglon.Renglon);
                        DateTime now = DateTime.Now;
                        entity.FechaReorden = now.AddDays(30);
                        entity.Remision = "";
                        entity.ValListaDol = 0;
                        entity.TotListDol = 0;
                        entity.TotVentDol = 0;
                        entity.PrecDol = 0;

                        await this._unitOfWork.DatosProductosRepository.Add(entity);
                        await this._unitOfWork.SaveChangesAsync();
                    }
                    renglon = (Wsmy502)null;
                    propuesta = (Wsmy515)null;
                }
                civy004 = (AppService.Core.EntitiesMaestros.Civy004)null;
                Wsmy501 = (Wsmy501)null;
                datosClienteFind = (AppService.Core.DataContratosStock.DatosCliente)null;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                throw;
            }
        }

        public async Task UpdateSolicitudPropuesta(
          string cotizacion,
          int renglon,
          int propuesta,
          int idSolicitud)
        {
            Wsmy515 renglonPropuesta = await this._unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(cotizacion, renglon, propuesta);
            if (renglonPropuesta == null)
                return;
            renglonPropuesta.IdSolicitud = idSolicitud.ToString();
            this._unitOfWork.PropuestaRepository.Update(renglonPropuesta);
            await this._unitOfWork.SaveChangesAsync();
        }


        public async Task CopiarDatosOrdenAnterior(long orden, string cotizacion, int renglon, int propuesta, AppDetailQuotes appDetailQuotes)
        {
            short defaultTipoOrden = 1;
            var tipoOrden = await _unitOfWork.AppConfigAppRepository.GetByKey("DefaultTipoOrden");
            if (tipoOrden != null)
            {
                defaultTipoOrden = short.Parse(tipoOrden.Valor);
            }


            var cpry012 = await _unitOfWork.Cpry012Repository.GetByOrdenAsync(orden);
            if (cpry012 != null)
            {
                ValoresCotizacionDto valoresCotizacion = await this.ValoresCotizacion(appDetailQuotes);
              
                var aplicacion = await _unitOfWork.Wsmy406Repository.GetById((short)cpry012.CodAplicacion);
                var wpry229FindData = await _unitOfWork.Wpry229Repository.GetByCotizacionRenglonPropuesta(cotizacion, renglon, propuesta);
                if (wpry229FindData != null)
                {
                    defaultTipoOrden=(short)wpry229FindData.TipoOrden;
                }
                
                await   _unitOfWork.Wpry229Repository.Delete(cotizacion, renglon, propuesta);
                await this._unitOfWork.SaveChangesAsync();
                
                var wpry229 = await _unitOfWork.Wpry229Repository.GetByCotizacionRenglonPropuesta(cotizacion, renglon, propuesta);
                if (wpry229 == null)
                {
                    Wpry229 wpry229New = new Wpry229();
                    wpry229New.IdTipoProducto = aplicacion.CodAplicacion;
                    wpry229New.DescripcionSolicitud = cpry012.NombreProducto.Trim();
                    wpry229New.Cotizacion = cotizacion;
                    wpry229New.Renglon = renglon;
                    wpry229New.Propuesta = propuesta;
                    wpry229New.CantidadProducto = appDetailQuotes.Cantidad;
                    wpry229New.IdTipoCantidad = 1;
                    wpry229New.ValorVenta = (decimal)valoresCotizacion.PrecioUnitario;
                    wpry229New.ValorVentaUsd = valoresCotizacion.PrecioUnitarioUsd;
                    wpry229New.Observaciones = appDetailQuotes.Observaciones;
                    if(cpry012.InstFacturar==null) cpry012.InstFacturar= "";
                    wpry229New.Instrucciones = cpry012.InstFacturar.Trim();
                    wpry229New.OrdenAnterior = cpry012.Orden;
                    wpry229New.FlagFiscal = cpry012.Fiscal;
                    wpry229New.FechaEntrega = DateTime.Now;

                    wpry229New.TipoOrden = defaultTipoOrden;

                    await _unitOfWork.Wpry229Repository.Add(wpry229New);
                    await this._unitOfWork.SaveChangesAsync();

                    //AGREGAMOS LAS ESPECIFICACIONES ADICIONALES GENERALES
                    //BuscaValoresAdocionales(Orden)
                    await DeleteWpry251ByCotizacion(appDetailQuotes);
                    await BuscacaValoresAdicionales(cpry012, cotizacion, renglon, propuesta);
                    var csmy021 = await _unitOfWork.Csmy021Repository.GetByOrdenAsync(orden);
                    if (csmy021.Count > 0)
                    {
                        foreach (var item in csmy021)
                        {
                            var wpry240 = await _unitOfWork.Wpry240Repository.GetByCotizacionRenglonPropuestaParte(cotizacion, renglon, propuesta, item.NoPartePapel);
                            if (wpry240 == null)
                            {
                                Wpry240 wpry240New = new Wpry240();
                                wpry240New.Cotizacion = cotizacion;
                                wpry240New.Renglon = renglon;
                                wpry240New.Propuesta = propuesta;
                                wpry240New.IdParte = item.NoPartePapel;
                                wpry240New.IdPapel = item.CodPapel;
                                wpry240New.IdConstruccion = 3;
                                wpry240New.LargoCm = item.MedidaPapel;
                                wpry240New.AnchoCm = (decimal)cpry012.MedidaBase;
                                wpry240New.Cantidad = appDetailQuotes.Cantidad;
                                wpry240New.MedidaBase = cpry012.MedidaBase;
                                wpry240New.MedidaOpuesta = item.MedidaPapel;

                                wpry240New.TipoPapel = wpry240New.IdPapel.Substring(0, 3);
                                wpry240New.Gramaje = wpry240New.IdPapel.Substring(3, 3);

                                var wpry229Find = await _unitOfWork.Wpry229Repository.GetByCotizacionRenglonPropuesta(cotizacion, renglon, propuesta);
                                if (wpry229Find != null)
                                {
                                    wpry229Find.TalonExtra = item.TalonExtra;
                                    _unitOfWork.Wpry229Repository.Update(wpry229Find);
                                }
                                if (!item.FraseMarginal.IsNullOrEmpty())
                                {
                                    wpry240New.FrasesMarginales = item.FraseMarginal;

                                }
                                else
                                {
                                    var wsmy369 = await _unitOfWork.Wsmy369Repository.GetByOrdenParte(orden, item.Parte);
                                    if (wsmy369 != null)
                                    {
                                        wpry240New.FrasesMarginales = wsmy369.FraseMarg;
                                    }
                                }
                                await _unitOfWork.Wpry240Repository.Add(wpry240New);
                                await this._unitOfWork.SaveChangesAsync();
                            }

                            //Agregar tintas por parte
                            var idFrente = await _unitOfWork.Wpry243Repository.BuscarIdFrente();

                            var idRespaldo = await _unitOfWork.Wpry243Repository.BuscarIdRespaldo();
                            //ArrayList TintasFrente = new ArrayList();

                            string[] tintasFrente = { item.PrimTintaFte, item.SegTintaFrent, item.TercTintaFren, item.CuarTintaFren, item.QuinTintaFren, item.SexTintaFren };
                            string[] tintasRespaldo = { item.PrimTintaResp, item.SegTintaResp, item.TercTintaResp, item.CuarTintaResp, item.QuinTintaResp, item.SexTintaResp };



                            for (int i = 0; i <= 5; i++)
                            {
                                var idTintaFrente = tintasFrente[i];
                                if (!idTintaFrente.ToString().IsNullOrEmpty())
                                {
                                    var wpry241FindFrente = await _unitOfWork.Wpry241Repository.GetByCotizacionRenglonPropuestaParteTinta(cotizacion, renglon, propuesta, item.NoPartePapel, idTintaFrente);
                                    var wpry240Frente = wpry241FindFrente.Where(x => x.IdUbicacion == idFrente).FirstOrDefault();
                                    if (wpry240Frente==null)
                                    {
                                        Wpry241 wpry241New = new Wpry241();
                                        wpry241New.Cotizacion = cotizacion;
                                        wpry241New.Renglon = renglon;
                                        wpry241New.Propuesta = propuesta;
                                        wpry241New.IdParte = item.NoPartePapel;
                                        wpry241New.IdUbicacion = idFrente;
                                        wpry241New.IdTinta = idTintaFrente;
                                        await _unitOfWork.Wpry241Repository.Add(wpry241New);
                                        await this._unitOfWork.SaveChangesAsync();
                                    }
                                   


                                }

                                var idTintarespaldo = tintasRespaldo[i];
                                if (!idTintarespaldo.ToString().IsNullOrEmpty())
                                {
                                    var wpry241FindRespaldo = await _unitOfWork.Wpry241Repository.GetByCotizacionRenglonPropuestaParteTinta(cotizacion, renglon, propuesta, item.NoPartePapel, idTintarespaldo);
                                    var wpry240Respaldo = wpry241FindRespaldo.Where(x => x.IdUbicacion == idRespaldo).FirstOrDefault();
                                    if (wpry240Respaldo==null)
                                    {
                                        Wpry241 wpry241New = new Wpry241();
                                        wpry241New.Cotizacion = cotizacion;
                                        wpry241New.Renglon = renglon;
                                        wpry241New.Propuesta = propuesta;
                                        wpry241New.IdParte = item.NoPartePapel;
                                        wpry241New.IdUbicacion = idRespaldo;
                                        wpry241New.IdTinta = idTintarespaldo;
                                        await _unitOfWork.Wpry241Repository.Add(wpry241New);
                                        await this._unitOfWork.SaveChangesAsync();
                                    }


                                }

                            }//RRECORRE ARREGLO DE TINTAS FRENTE Y RESPALDO


                        }//RECORRIDO DE PARTES EN CSMY021

                    }



                }
              

            }

        }


        public async Task BuscacaValoresAdicionales(AppService.Core.EntitiesPlanta.Cpry012 cpry012, string cotizacion, int renglon, int propuesta)
        {

            var propuestaOrdenAnterior = await _unitOfWork.PropuestaRepository.GetByOrden(cpry012.Orden);
            if (propuestaOrdenAnterior != null)
            {
             
                
                
                var wpry251List = await _unitOfWork.Wpry251Repository.GetByCotizacionRenglonPropuesta(propuestaOrdenAnterior.Cotizacion, propuestaOrdenAnterior.Renglon, propuestaOrdenAnterior.Propuesta);
                if (wpry251List.Count > 0)
                {
                    
                    _unitOfWork.Wpry251Repository.CopiarOrdenAnteriorCotizacion(cotizacion,renglon,propuesta,propuestaOrdenAnterior.Cotizacion, propuestaOrdenAnterior.Renglon, propuestaOrdenAnterior.Propuesta);
                    /*foreach (var item in wpry251List)
                    {
                        Wpry251 wpry251New = new Wpry251();
                        wpry251New.Cotizacion = cotizacion;
                        wpry251New.Renglon = renglon;
                        wpry251New.Propuesta = propuesta;
                        wpry251New.IdVariable = item.IdVariable;
                        wpry251New.IdParte = item.IdParte;
                        wpry251New.Valor = item.Valor;
                        wpry251New.FechaRegistro = DateTime.Now;
                        await _unitOfWork.Wpry251Repository.Add(wpry251New);
                        await _unitOfWork.SaveChangesAsync();


                    }*/

                }

            }



        }


        //######################################################
        public async Task DeleteCotizacionesDetailToOdoo(List<int> mooreIds)
        {



            foreach (var item in mooreIds)
            {



                try
                {



                    var odooProduct = await GetOdooDeleteDetail(item);
                    //JsonSerializerOptions options = new JsonSerializerOptions
                    //{
                    //    IgnoreNullValues = true,
                    //    WriteIndented = true,
                    //    PropertyNameCaseInsensitive = true
                    //};
                    //string json = JsonSerializer.Serialize(odooProduct, options);

                    //StringContent data = new StringContent(json, Encoding.UTF8, "application/json");


                    string json1 = JsonConvert.SerializeObject(odooProduct);
                    StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");
                    var result = await _odooClient.Post(data);
                    try
                    {
                        OdooResultCotizacion respuesta = new OdooResultCotizacion();
                        respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<OdooResultCotizacion>(result.Message);

                        if (respuesta == null || respuesta.result == null || !respuesta.result.success)
                        {
                            var mensajeError = "";
                            if (respuesta == null || respuesta.result == null)
                            {
                                mensajeError = result.Message;
                            }
                            else
                            {
                                if (!respuesta.result.success)
                                {
                                    var errorDetail = respuesta.result.data.FirstOrDefault();
                                    mensajeError = errorDetail.message;
                                }
                            }




                        }




                    }
                    catch (Exception e)
                    {
                        var mensajeError = "";

                    }



                }
                catch (Exception e)
                {

                    var msg = e.InnerException.Message + "" + item;
                    return;
                }



            }


            //var myclass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result.Message);
            // Respuesta respuesta = new Respuesta();
            // respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(result.Message);


        }

        public async Task<OdooClients> GetOdooDeleteDetail(int mooreId)
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
            arr.Add("sale_order_line_delete");

            var listOfStrings = new List<string>();
            string[] arrayOfStrings = listOfStrings.ToArray();
            arr.Add(arrayOfStrings);

            ArrayList arrCliente = new ArrayList();

            ArgClassDetailDelete result = new ArgClassDetailDelete();
            result.MooreId = mooreId;

            arrCliente.Add(result);
            arr.Add(arrCliente);


            OdooClients odooClients = new OdooClients();
            odooClients.jsonrpc = "2.0";
            odooClients.method = "call";
            odooClients.Params = new DTOs.Odoo.Clientes.OdooClients.Params
            {
                Service = "object",
                Method = "execute",
                Args = arr

            };

            return odooClients;

        }

        public async Task DeleteCotizacionesToOdoo(List<string> cotizaciones)
        {



            foreach (var item in cotizaciones)
            {



                try
                {



                    var odooProduct = await GetOdooCotizacionDeleteGeneral(item);
                    //JsonSerializerOptions options = new JsonSerializerOptions
                    //{
                    //    IgnoreNullValues = true,
                    //    WriteIndented = true,
                    //    PropertyNameCaseInsensitive = true
                    //};
                    //string json = JsonSerializer.Serialize(odooProduct, options);

                    //StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

                    string json1 = JsonConvert.SerializeObject(odooProduct);
                    StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");

                    var result = await _odooClient.Post(data);
                    try
                    {
                        OdooResultCotizacion respuesta = new OdooResultCotizacion();
                        respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<OdooResultCotizacion>(result.Message);

                        if (respuesta == null || respuesta.result == null || !respuesta.result.success)
                        {
                            var mensajeError = "";
                            if (respuesta == null || respuesta.result == null)
                            {
                                mensajeError = result.Message;
                            }
                            else
                            {
                                if (!respuesta.result.success)
                                {
                                    var errorDetail = respuesta.result.data.FirstOrDefault();
                                    mensajeError = errorDetail.message;
                                }
                            }




                        }




                    }
                    catch (Exception e)
                    {
                        var mensajeError = "";

                    }



                }
                catch (Exception e)
                {

                    var msg = e.InnerException.Message + "" + item;
                    return;
                }



            }


            //var myclass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result.Message);
            // Respuesta respuesta = new Respuesta();
            // respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(result.Message);


        }


        public async Task<OdooClients> GetOdooCotizacionDeleteGeneral(string cotizacion)
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
            arr.Add("sale_order_delete");

            var listOfStrings = new List<string>();
            string[] arrayOfStrings = listOfStrings.ToArray();
            arr.Add(arrayOfStrings);

            ArrayList arrCliente = new ArrayList();

            ArgClassGeneralDelete result = new ArgClassGeneralDelete();
            result.idCotizacion = cotizacion;

            arrCliente.Add(result);
            arr.Add(arrCliente);


            OdooClients odooClients = new OdooClients();
            odooClients.jsonrpc = "2.0";
            odooClients.method = "call";
            odooClients.Params = new DTOs.Odoo.Clientes.OdooClients.Params
            {
                Service = "object",
                Method = "execute",
                Args = arr

            };

            return odooClients;

        }

        public async Task<OdooClients> GetOdooCotizacion(string cotizacion)
        {

            /*
            OdooServerProd
            OdooServerDev
            OdooTokenProd
            OdooTokenDev*/

            AppConfigApp server = new AppConfigApp();
            AppConfigApp token = new AppConfigApp();

            var ambiente = Ambiente.ValorAmbiente();
            if (ambiente == 1)
            {
                server = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooServerProd");
                token = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooTokenProd");
            }
            else
            {
                server = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooServerDev");
                token = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooTokenDev");
            }


            ArrayList arr = new ArrayList();
            arr.Add(server.Valor.Trim());
            arr.Add(2);
            arr.Add(token.Valor.Trim());
            arr.Add("integration.model");
            arr.Add("sale_order_create_update");

            var listOfStrings = new List<string>();
            string[] arrayOfStrings = listOfStrings.ToArray();
            arr.Add(arrayOfStrings);

            ArrayList arrCliente = new ArrayList();
            var detailContacto = await GetOdooInfoCotizacion(cotizacion);
            arrCliente.Add(detailContacto);
            arr.Add(arrCliente);


            OdooClients odooClients = new OdooClients();
            odooClients.jsonrpc = "2.0";
            odooClients.method = "call";
            odooClients.Params = new DTOs.Odoo.Clientes.OdooClients.Params
            {
                Service = "object",
                Method = "execute",
                Args = arr

            };

            return odooClients;

        }

        public async Task ActualizarClientes()
        {

            try
            {

               //await UpdateCotizacionesToOdoo();


                var diasAcualizaPresupuesto = 1;
                var appConfig = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooDiasActualizaClientes");
                if (appConfig != null)
                {
                    diasAcualizaPresupuesto = int.Parse(appConfig.Valor);
                }
                //diasAcualizaPresupuesto = 50;
                var clientes = await _unitOfWork.MtrClienteRepository.GetAllDAyUpdate(diasAcualizaPresupuesto);

               clientes = clientes.Where(x => x.Codigo.Trim() == "744952").ToList();    
               Console.WriteLine($"Se actualizaran {clientes.Count()} Clientes");
                await _mtrContactosService.UpdateClientesToOdoo(clientes);
            }
            catch (Exception ex)
            {
                var msg=ex.Message;
            }
          



        }

        public async Task ReasignaClienteContacto(long idContacto)
        {

            var contacto = await _unitOfWork.MtrContactosRepository.GetById(idContacto);
            if (contacto != null)
            {
                var contactoEsProspecto = EsProspecto.ClienteEsProspecto(contacto.IdCliente);
                if (contactoEsProspecto)
                {

                    var cotizacion = await _unitOfWork.CotizacionRepository.GetByRif(contacto.Rif);
                    if (cotizacion != null)
                    {
                        var clienteCotizacionEsProspecto = EsProspecto.ClienteEsProspecto(cotizacion.CodCliente);
                        if (!clienteCotizacionEsProspecto)
                        {
                            contacto.IdCliente = cotizacion.CodCliente;
                            _unitOfWork.MtrContactosRepository.Update(contacto);
                            await _unitOfWork.SaveChangesAsync();
                            var wsmy265 = await _unitOfWork.Wsmy265Repository.GetById(idContacto);
                            if (wsmy265 != null)
                            {
                                wsmy265.Cliente = Convert.ToDecimal(cotizacion.CodCliente);
                                _unitOfWork.Wsmy265Repository.Update(wsmy265);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            var appGeneralQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetByCotizacions(cotizacion.Cotizacion);
                            if (appGeneralQuotes != null)
                            {
                                appGeneralQuotes.IdCliente = cotizacion.CodCliente;
                                _unitOfWork.AppGeneralQuotesRepository.Update(appGeneralQuotes);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }


                }
            }


        }



        public async Task UpdateCotizacionesToOdoo()
        {


            var diasAcualizaPresupuesto = 1;
            var appConfig = await _unitOfWork.AppConfigAppRepository.GetByKey("OdooDiasActualizarPresupuesto");

            if (appConfig != null)
            {
                diasAcualizaPresupuesto = int.Parse(appConfig.Valor);
            }

            //diasAcualizaPresupuesto = 30;
            var cotizaciones = await _unitOfWork.CotizacionRepository.GetListCotizaciones(diasAcualizaPresupuesto);

            
            //#####ACTUALIZACION DE CLIENTE PROSPECTO
            MtrClienteQueryFilter filter = new MtrClienteQueryFilter();
            //filter.Codigo = "000000";
            //filter.Usuario = "RR105841";
            //var mtrClientesProspecto = await _unitOfWork.MtrClienteRepository.ListCliente(filter);
            //await _mtrContactosService.UpdateClientesToOdoo(mtrClientesProspecto);

            var cont = 0;
            foreach (var item in cotizaciones.Where(x=>x.Trim()=="AY02202405018").ToList())
            {
                cont++;
                Console.WriteLine("Actualizando: " + cont.ToString() + " de: " + cotizaciones.Count().ToString());

                try
                {
                    if (item.Trim()== "BM12202404016") {
                        var DETENTE = "";
                    }

                    filter.Codigo = await GetClienteCotizacion(item);
                    filter.Usuario = "RR105841";

                    var mtrClientes = await _unitOfWork.MtrClienteRepository.ListCliente(filter);

                    //await _mtrContactosService.UpdateClientesToOdoo(mtrClientes);
                    var cotizacion = await _unitOfWork.CotizacionRepository.GetByCotizacion(item);
                    if (cotizacion != null)
                    {
                        await ReasignaClienteContacto((long)cotizacion.IdContacto);


                        /*var contacto = await _unitOfWork.MtrContactosRepository.GetById((long)cotizacion.IdContacto);
                        if (contacto != null)
                        {
                            List<MtrContactos> listMtrContactos = new List<MtrContactos>();
                            listMtrContactos.Add(contacto);
                            await _mtrContactosService.UpdateContactosToOdooByListMtrContacto(listMtrContactos);
                        }*/

                    }

                    
                    var odooProduct = await GetOdooCotizacion(item);


                    string json1 = JsonConvert.SerializeObject(odooProduct);
                    StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");


                    Console.WriteLine("iniciando envio de cotizacion: " + item);
                    var result = await _odooClient.Post(data);
                    Console.WriteLine("finalizando envio de cotizacion: " + item);
                    try
                    {
                        OdooResultCotizacion respuesta = new OdooResultCotizacion();
                        respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<OdooResultCotizacion>(result.Message);

                        if (respuesta == null || respuesta.result == null || !respuesta.result.success)
                        {
                            var mensajeError = "";
                            if (respuesta == null || respuesta.result == null)
                            {
                                mensajeError = result.Message;
                            }
                            else
                            {
                                if (!respuesta.result.success)
                                {
                                    var errorDetail = respuesta.result.data.FirstOrDefault();
                                    if (errorDetail.message != null)
                                    {
                                        mensajeError = errorDetail.message;
                                    }
                                    else
                                    {
                                        mensajeError = result.Message;
                                    }

                                }
                            }
                            _unitOfWork.CotizacionRepository.MarcarResultOdoo(item,mensajeError, 1);

                          

                        }
                        else
                        {
                            _unitOfWork.CotizacionRepository.MarcarResultOdoo(item,"", 0);
                            


                        }



                    }
                    catch (Exception e)
                    {
                        var mensaje = "";
                        if (e.InnerException != null)
                        {
                            mensaje= e.InnerException.Message;
                        }
                        {
                            mensaje = "Error no identificad, se requiere evaluacion detallada";
                        }
                        
                        
                        _unitOfWork.CotizacionRepository.MarcarResultOdoo(item,mensaje, 1);
                      
                    }



                }
                catch (Exception e)
                {


                    return;
                }



            }


            //var myclass = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result.Message);
            // Respuesta respuesta = new Respuesta();
            // respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(result.Message);


        }



        public async Task UpdateCotizacionToOdoo(string cotizacion)
        {



            //#####ACTUALIZACION DE CLIENTE PROSPECTO
            MtrClienteQueryFilter filter = new MtrClienteQueryFilter();
            filter.Codigo = "000000";
            filter.Usuario = "RR105841";






            try
            {
                filter.Codigo = await GetClienteCotizacion(cotizacion);


                filter.Usuario = "RR105841";


                var mtrClientes = await _unitOfWork.MtrClienteRepository.ListCliente(filter);

                await _mtrContactosService.UpdateClientesToOdoo(mtrClientes);



                var odooProduct = await GetOdooCotizacion(cotizacion);


                string json1 = JsonConvert.SerializeObject(odooProduct);
                StringContent data = new StringContent(json1, Encoding.UTF8, "application/json");



                var result = await _odooClient.Post(data);
                try
                {
                    OdooResultCotizacion respuesta = new OdooResultCotizacion();
                    respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<OdooResultCotizacion>(result.Message);

                    if (respuesta == null || respuesta.result == null || !respuesta.result.success)
                    {
                        var mensajeError = "";
                        if (respuesta == null || respuesta.result == null)
                        {
                            mensajeError = result.Message;
                        }
                        else
                        {
                            if (!respuesta.result.success)
                            {
                                var errorDetail = respuesta.result.data.FirstOrDefault();
                                mensajeError = errorDetail.message;
                            }
                        }


                   

                    }
                  



                }
                catch (Exception e)
                {
                    return;
                   
                }



            }
            catch (Exception e)
            {


                return;
            }





        }



      

        public async Task<string> GetClienteCotizacion(string cotizacion)
        {
            string result = "";

            AppGeneralQuotes generalQuotes = await this._unitOfWork.AppGeneralQuotesRepository.GetByCotizacions(cotizacion);
            if (generalQuotes != null)
            {
                result = generalQuotes.IdCliente;
            }
            else
            {

                Wsmy501 Wsmy501 = await this.GetByCotizacion(cotizacion);
                if (Wsmy501 != null)
                {
                    result = Wsmy501.CodCliente;
                }
                else
                {
                    result = "";
                }

            }

            return result;
        }



        private async Task<ArgClass> GetOdooInfoCotizacion(string cotizacion)
        {
            ArgClass result = new ArgClass();

            AppGeneralQuotes generalQuotes = await this._unitOfWork.AppGeneralQuotesRepository.GetByCotizacions(cotizacion);
            if (generalQuotes != null)
            {
                result = await GetByGeneradorPresupuesto(generalQuotes);
            }
            else
            {

                Wsmy501 Wsmy501 = await this.GetByCotizacion(cotizacion);
                if (Wsmy501 != null)
                {
                    result = await GetByCotizadorPlus(Wsmy501);
                }
                else
                {
                    result = null;
                }

            }

            return result;
        }

        private async Task<ArgClass> GetByGeneradorPresupuesto(AppGeneralQuotes generalQuotes)
        {

            ArgClass result = new ArgClass();

            result.idCotizacion = generalQuotes.Cotizacion;
            var estatus = await _unitOfWork.AppStatusQuoteRepository.GetById(generalQuotes.IdEstatus);
            if (estatus != null) result.state = estatus.OdooStatus;

            var contacto = await _unitOfWork.MtrContactosRepository.GetById((long)generalQuotes.IdContacto);
            //var contactoCot = contacto.Where(x => x.IdContacto == wsmy501.IdContacto).FirstOrDefault();
            if (contacto != null)
            {
                result.idContacto = (int)generalQuotes.IdContacto;
                result.IdClienteOdoo = (int)contacto.IdClienteOdoo;
                result.IdContactoOdoo = (int)contacto.IdContactoOdoo;

            }

            //result.idContacto = (int)generalQuotes.IdContacto;
            result.idCliente = generalQuotes.IdCliente.Trim();
            if (result.idCliente.Trim() == "000000")
            {
                var direcciones = await _unitOfWork.MtrDireccionesRepository.GetByIdCliente(result.idCliente.Trim());
                if (direcciones.Count > 0)
                {
                    var direccion = direcciones.FirstOrDefault();
                    result.idDireccionEntregar = direccion.Id.ToString();
                    result.idDireccionFactrurar = direccion.Id.ToString();
                    result.rif = generalQuotes.Rif;
                    result.RazonSocial = generalQuotes.RazonSocial;
                    result.direccion = generalQuotes.Direccion;
                    result.idMunicipio = generalQuotes.IdMunicipio.ToString();

                }
            }
            else
            {
                result.idDireccionEntregar = generalQuotes.IdDireccionEntregar.ToString();
                result.idDireccionFactrurar = generalQuotes.IdDireccionFacturar.ToString();
                result.rif = "";
                result.RazonSocial = "";
                result.direccion = "";
                result.idMunicipio = "";
            }

            result.idCondPago = generalQuotes.IdCondPago.ToString();
            result.idMtrTipoMoneda = generalQuotes.IdMtrTipoMoneda.ToString();
            var tipoMoneda = await _unitOfWork.MtrTipoMonedaRepository.GetById((long)generalQuotes.IdMtrTipoMoneda);
            if (tipoMoneda != null)
            {
                result.idMtrTipoMoneda = tipoMoneda.OdooId;

                result.MonedaFacturar = tipoMoneda.OdooId;
            }
            //"fecha": "2022-07-17 12:30:30"
            result.fecha = FechaString(generalQuotes.Fecha);

            result.ordenCompra = generalQuotes.OrdenCompra.ToString();
            result.observaciones = generalQuotes.Observaciones;
            var vendedor = _unitOfWork.MtrVendedorRepository.GetById(generalQuotes.IdVendedor);
            if (vendedor != null)
            {
                var oficina = await _unitOfWork.MtrOficinaRepository.GetById(vendedor.Oficina);
                if (oficina != null)
                {
                        result.idEquipoVentas = oficina.OdooId;
                    result.idComercial = oficina.IdComercianteOdoo;
                }

                if (vendedor.IdUsuarioOdoo != null && vendedor.IdUsuarioOdoo > 0 && vendedor.Activo == "X")
                {
                    result.idComercial = (int)vendedor.IdUsuarioOdoo;
                }
                var ambiente = Ambiente.ValorAmbiente();
                if (ambiente == 2)
                {
                    result.idComercial = 2;
                    result.idEquipoVentas = 5;
                }

            }




            result.OrderLines = await GetDetailByGeneradorPresupuesto(generalQuotes);
            return result;
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

            result = año + "-" + mes + "-" + dia + " " + hora + ":" + minuto + ":" + segundo;

            return result;
        }

        private async Task<List<OrderLines>> GetDetailByGeneradorPresupuesto(AppGeneralQuotes generalQuotes)
        {
            List<OrderLines> result = new List<OrderLines>();

            var appdetails = await _unitOfWork.AppDetailQuotesRepository.GetByAppGeneralQuotesId(generalQuotes.Id);
            foreach (var item in appdetails)
            {
                OrderLines resultItem = new OrderLines();
                var producto = await _unitOfWork.AppProductsRepository.GetById(item.IdProducto);


                var wsmy502 = await _unitOfWork.RenglonRepository.GetByCotizacionProducto(generalQuotes.Cotizacion, producto.ExternalCode);
                /*if (wsmy502 == null)
                {
                    //DeleteCotizacion(generalQuotes);
                    await DeleteCotizacionRenglon(item);
                    await IntegrarCotizacion(generalQuotes.Id, true);
                    wsmy502 = await _unitOfWork.RenglonRepository.GetByCotizacionProducto(generalQuotes.Cotizacion, producto.ExternalCode);
                }*/


                var wsmy515 = await _unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(wsmy502.Cotizacion, wsmy502.Renglon, 1);
               /* if (wsmy515 == null)
                {
                    await DeleteCotizacionRenglon(item);
                    await IntegrarCotizacion(generalQuotes.Id, true);
                    wsmy515 = await _unitOfWork.PropuestaRepository.GetByCotizacionRenglonPropuesta(wsmy502.Cotizacion, wsmy502.Renglon, 1);
                }*/

                resultItem.MooreId = (int)wsmy515.Id;
                if (item.OdooId == null)
                {
                    item.OdooId = 0;

                }
                resultItem.OdooId = (int)item.OdooId;
                resultItem.idProducto = item.IdProducto;
                resultItem.idVariante = 0;
                resultItem.nombreComercialProducto = item.NombreComercialProducto.Trim();
                resultItem.cantidadSolicitada = (decimal)item.CantidadSolicitada;

                appRecipesByAppDetailQuotesQueryFilter price = new appRecipesByAppDetailQuotesQueryFilter();
                price.AppProuctId = item.IdProducto;
                price.Cantidad = (int)item.CantidadSolicitada;

                if (item.MedidaBasica == null) item.MedidaBasica = ValorCero;
                if (item.MedidaOpuesta == null) item.MedidaOpuesta = ValorCero;
                price.Ancho = (decimal)item.MedidaBasica;
                price.Largo = (decimal)item.MedidaOpuesta;
                if (generalQuotes.IdCliente == "000000")
                {
                    price.IdMunicipio = (decimal)generalQuotes.IdMunicipio;
                }
                else
                {
                    var direccionEntregar = await _unitOfWork.MtrDireccionesRepository.GetById(generalQuotes.IdDireccionEntregar);
                    var municipio = await _unitOfWork.Winy243Repository.GetByCodigoMunicipio(direccionEntregar.Municipio);
                    if (municipio != null)
                    {
                        price.IdMunicipio = municipio.Recnum;
                    }
                    else
                    {
                        price.IdMunicipio = 1;

                    }

                }



               /* var appPriceGetDto = await _appRecipesByAppDetailQuotesService.GetPrice(price);
                if (resultItem.cantidadConvertida == 0 || resultItem.cantidadConvertida == null)
                {
                    if (appPriceGetDto.Data == null)
                    {
                        resultItem.cantidadConvertida = (int)item.CantidadSolicitada;
                    }
                    else
                    {
                        resultItem.cantidadConvertida = (decimal)appPriceGetDto.Data.CantidadConvertida;

                    }

                }*/

                resultItem.cantidadConvertida = Convert.ToDecimal(string.Format("{0:F4}", item.Cantidad));

                DateTime fecha = DateTime.Now;
                var tasa = await _unitOfWork.TPaTasaReferencialRepository.GetTasaByFecha(fecha);

                if (generalQuotes.IdMtrTipoMoneda == 2)
                {
                    resultItem.PrecioUnitVendedor = item.PrecioUsd;
                    resultItem.Total = item.TotalUsd;
                    resultItem.precioLista = item.PrecioUsd;
                    resultItem.precioLista = item.PrecioUsd;
                 

                }
                else
                {
                    resultItem.PrecioUnitVendedor = item.Precio;
                    resultItem.Total = item.Total;
                    resultItem.precioLista = item.Precio;

                }

                if (resultItem.precioLista == 0)
                {
                    resultItem.precioLista = resultItem.PrecioUnitVendedor;
                }


                resultItem.medidaBasica = (decimal)item.MedidaBasica;
                resultItem.medidaOpuesta = (decimal)item.MedidaOpuesta;


                result.Add(resultItem);

            }

            return result;
        }

        private async Task<ArgClass> GetByCotizadorPlus(Wsmy501 wsmy501)
        {
            ArgClass result = new ArgClass();

            result.idCotizacion = wsmy501.Cotizacion;
            var estatus = await _unitOfWork.AppStatusQuoteRepository.GetById((int)wsmy501.Estatus);
            if (estatus != null) result.state = estatus.OdooStatus;

            var contacto = await _unitOfWork.MtrContactosRepository.GetById((long)wsmy501.IdContacto);
            //var contactoCot = contacto.Where(x => x.IdContacto == wsmy501.IdContacto).FirstOrDefault();
            if (contacto != null)
            {
                result.idContacto = (int)wsmy501.IdContacto;
                result.IdClienteOdoo = (int)contacto.IdClienteOdoo;
                result.IdContactoOdoo = (int)contacto.IdContactoOdoo;

            }


            result.idCliente = wsmy501.CodCliente.Trim();
            if (result.idCliente.Trim() == "000000")
            {
                var direcciones = await _unitOfWork.MtrDireccionesRepository.GetByIdCliente(result.idCliente.Trim());
                if (direcciones.Count > 0)
                {
                    var direccion = direcciones.FirstOrDefault();
                    result.idDireccionEntregar = direccion.Id.ToString();
                    result.idDireccionFactrurar = direccion.Id.ToString();
                    result.rif = wsmy501.Rif;
                    result.RazonSocial = wsmy501.RazonSocial;
                    var direccionCotizacion = await _unitOfWork.MtrDireccionesRepository.GetById((decimal)wsmy501.IdDireccionEntregar);
                    if (direccionCotizacion != null)
                    {
                        result.direccion = wsmy501.DireccionFacturar;
                        var municipio = await _unitOfWork.Winy243Repository.GetByCodigoMunicipio(direccionCotizacion.Municipio);
                        if (municipio != null)
                        {
                            result.idMunicipio = municipio.Recnum.ToString();
                        }


                    }


                }
            }
            else
            {
                result.idDireccionEntregar = wsmy501.IdDireccionEntregar.ToString();
                result.idDireccionFactrurar = wsmy501.IdDireccion.ToString();
            }

            result.idCondPago = wsmy501.Condicion.ToString();

            var tipoMoneda = await _unitOfWork.MtrTipoMonedaRepository.GetById(2);
            if (tipoMoneda != null)
            {
                result.idMtrTipoMoneda = tipoMoneda.OdooId;

                result.MonedaFacturar = tipoMoneda.OdooId;
            }
            //"fecha": "2022-07-17 12:30:30"
            DateTime Fecha = (DateTime)wsmy501.Fecha;
            result.fecha = FechaString(Fecha);
            result.ordenCompra = wsmy501.OrdenCompra.ToString();
            result.observaciones = wsmy501.Observaciones;
            var vendedor = _unitOfWork.MtrVendedorRepository.GetById(wsmy501.CodVendedor);
            if (vendedor != null)
            {
                var oficina = await _unitOfWork.MtrOficinaRepository.GetById(vendedor.Oficina);
                if (oficina != null)
                {
                    result.idEquipoVentas = oficina.OdooId;
                    result.idComercial = oficina.IdComercianteOdoo;
                }

                if (vendedor.IdUsuarioOdoo != null && vendedor.IdUsuarioOdoo > 0 && vendedor.Activo == "X")
                {
                    result.idComercial = (int)vendedor.IdUsuarioOdoo;
                }

                var ambiente = Ambiente.ValorAmbiente();
                if (ambiente == 2)
                {
                    result.idComercial = 2;
                    result.idEquipoVentas = 5;
                }

            }



            result.OrderLines = await GetDetailByCotizadorPlus(wsmy501);
            return result;
        }

        private async Task<List<OrderLines>> GetDetailByCotizadorPlus(Wsmy501 wsmy501)
        {
            List<OrderLines> result = new List<OrderLines>();

            var appdetails = await _unitOfWork.PropuestaRepository.GetAllByCotizacion(wsmy501.Cotizacion);
            foreach (var item in appdetails)
            {
                var renglon = await _unitOfWork.RenglonRepository.GetByCotizacionRenglon(item.Cotizacion, item.Renglon);

                OrderLines resultItem = new OrderLines();
                resultItem.MooreId = (int)item.Id;
                if (item.OdooId == null)
                {
                    item.OdooId = 0;

                }
                resultItem.OdooId = (int)item.OdooId;

                var appProduct = await _unitOfWork.AppProductsRepository.GetByCode(renglon.CodProducto);
                if (appProduct != null)
                {
                    resultItem.idProducto = appProduct.Id;
                }
                else
                {
                    var csmy036 = await _unitOfWork.Csmy036Repository.GetByCode(renglon.CodProducto);

                    AppProductsCreateDto newProduct = new AppProductsCreateDto();


                    //var firstProduct = await _unitOfWork.AppProductsRepository.GetFirstByExternalCode(renglon.CodProducto);

                    newProduct.Code = renglon.CodProducto;
                    newProduct.ExternalCode = renglon.CodProducto;
                    newProduct.Description1 = renglon.CodProducto + " " + csmy036.Nombre.Trim();
                    newProduct.Description2 = "";
                    newProduct.PrymaryMtrMonedaId = 2;
                    newProduct.SecundaryMtrMonedaId = 1;
                    newProduct.AppUnitsId = 4;
                    newProduct.ProductionUnitId = 4;
                    newProduct.UnitPrice = 99999999999;
                    newProduct.TipoCalculo = 2;
                    newProduct.AppSubCategoryId = 8;
                    newProduct.UsuarioConectado = "RR105841";
                    newProduct.UrlImage = "";



                    var response = await _appProductsService.InsertProducts(newProduct);
                    //var product = await _unitOfWork.AppProductsRepository.GetByCode(renglon.CodProducto);
                    resultItem.idProducto = response.Data.Id;
                }



                resultItem.idVariante = 0;
                resultItem.nombreComercialProducto = renglon.Descripcion;
                resultItem.cantidadSolicitada = (decimal)item.Cantidad;
                resultItem.cantidadConvertida = Convert.ToDecimal(string.Format("{0:F4}", item.CantMill));

                DateTime fecha = DateTime.Now;
                var tasa = await _unitOfWork.TPaTasaReferencialRepository.GetTasaByFecha(fecha);
                var IdMtrTipoMoneda = 2;
                if (IdMtrTipoMoneda == 2)
                {
                    resultItem.PrecioUnitVendedor = (decimal)item.PrecioUnitarioUsd;
                    resultItem.Total = (decimal)item.TotalPropuestaUsd;
                    resultItem.precioLista = (decimal)item.PrecioUnitarioUsd;
                }
                else
                {
                    resultItem.PrecioUnitVendedor = (decimal)item.PrecioUnitario;
                    resultItem.Total = (decimal)item.TotalPropuesta;
                    resultItem.precioLista = (decimal)item.UsdListaCpjminimo / (decimal)tasa.Tasa;

                }

                if (resultItem.precioLista == 0)
                {
                    resultItem.precioLista = resultItem.PrecioUnitVendedor;
                }

                resultItem.medidaBasica = (decimal)ValorCero;
                resultItem.medidaOpuesta = (decimal)ValorCero;


                result.Add(resultItem);

            }

            return result;
        }

    }
}
