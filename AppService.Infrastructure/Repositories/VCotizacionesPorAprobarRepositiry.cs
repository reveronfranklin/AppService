
using AppService.Core.Interfaces;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs.Aprobaciones;
using AppService.Core.Entities;
using AppService.Core.EntitiesMooreve;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;
using AppService.Infrastructure.Data;
using AppService.Infrastructure.DataMooreve;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.SqlClient;

namespace AppService.Infrastructure.Repositories
{
    public class VCotizacionesPorAprobarRepository : IVCotizacionesPorAprobarRepository
    {

        private readonly MooreveContext _context;
        private readonly RRDContext _rrdContext;

        public VCotizacionesPorAprobarRepository(MooreveContext context, RRDContext rrdContext)
        {
            _context = context;
            _rrdContext = rrdContext;
        }


        public async Task<bool> UpdateCotizacion(ActualizarCotizacion dto)
        {
            var cotizacion = await GetByCotizacion(dto.Cotizacion);
            if (cotizacion != null)
            {
                var cliente = await _rrdContext.MtrCliente.Where(x => x.Codigo == dto.IdCliente).FirstOrDefaultAsync();
                cotizacion.TasaExcepcion = dto.TasaExcepcion;
                if (dto.TasaExcepcion == 0)
                {
                    dto.FechaPago = null;
                }

                cotizacion.FechaPago = dto.FechaPago;
                cotizacion.ImprimirFacturaEnUsd = dto.ImprimirFacturaEnUSD;
                if (cliente.Codigo != "000000")
                {
                    cotizacion.CodCliente = dto.IdCliente;
                    cotizacion.RazonSocial = cliente.Nombre.Trim();
                }

                Update(cotizacion);



                var generalQuotes = await GetGeneralQuotesByCotizacion(dto.Cotizacion);
                if (generalQuotes != null)
                {
                    if (cliente.Codigo != "000000")
                    {
                        generalQuotes.IdCliente = dto.IdCliente;
                        generalQuotes.RazonSocial = cliente.Nombre.Trim();
                        UpdateGeneral(generalQuotes);
                    }


                }

                await _context.SaveChangesAsync();
            }

            return true;
        }

  
        
        public async Task<ApiResponse<string>> VerificarCredito(string cotizacion,string usuarioConectado)
        {

            ApiResponse<string> response = new ApiResponse<string>("");
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            try
            {
                var cotizacionObj = await GetByCotizacion(cotizacion);
                if (cotizacionObj != null)
                {
                  
                    FormattableString xqueryDiario = $"exec [rrd].[dbo].PaCreVerificarCredito '{cotizacion}','{usuarioConectado}'";

                    var result = await _rrdContext.Database.ExecuteSqlInterpolatedAsync(xqueryDiario);

                    
                  
                    
                    var mensaje = "Credito Verificado";

                

                    response.Data = mensaje;
                    metadata.Message = mensaje;
                    metadata.success = true;
                    metadata.IsValid = true;
                    response.Meta = metadata;
                    return response;

                }
                else
                {
                    response.Data = "No existe Cotizacion";
                    metadata.Message = "No existe Cotizacion";
                    metadata.success = false;
                    metadata.IsValid = false;
                    response.Meta = metadata;

                    return response;
                }

               
            }
            catch (Exception e)
            {
                response.Data = "";
                metadata.Message = e.Message;
                metadata.success = false;
                metadata.IsValid = false;
                response.Meta = metadata;

                return response;
            }



        }
        
        public async Task<ApiResponse<string>> CerrarExcepcion(ActualizarCotizacion dto)
        {

            ApiResponse<string> response = new ApiResponse<string>("");
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            try
            {
                var cotizacion = await GetByCotizacion(dto.Cotizacion);
                if (cotizacion != null)
                {
                  
                    FormattableString xqueryDiario = $"exec [rrd].[dbo].Pa_CerrarSolicitudExcepcion {dto.SolicitudDeCredito}";

                    var result = await _rrdContext.Database.ExecuteSqlInterpolatedAsync(xqueryDiario);

                    
                    var mensaje = "Excepcion Cerrada Satisfactoriamente";

                

                    response.Data = mensaje;
                    metadata.Message = mensaje;
                    metadata.success = true;
                    metadata.IsValid = true;
                    response.Meta = metadata;
                    return response;

                }
                else
                {
                    response.Data = "No existe Cotizacion";
                    metadata.Message = "No existe Cotizacion";
                    metadata.success = false;
                    metadata.IsValid = false;
                    response.Meta = metadata;

                    return response;
                }

               
            }
            catch (Exception e)
            {
                response.Data = "";
                metadata.Message = e.Message;
                metadata.success = false;
                metadata.IsValid = false;
                response.Meta = metadata;

                return response;
            }



        }
    


        
        public async Task<ApiResponse<string>> EtlPasePlanta(ActualizarCotizacion dto)
        {

            ApiResponse<string> response = new ApiResponse<string>("");
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            try
            {
                var cotizacion = await GetByCotizacion(dto.Cotizacion);
                if (cotizacion != null)
                {

                   // FormattableString xqueryDiario = $"exec [rrd].[dbo].[PA_AprobarCreditoManualPrueba] {dto.Cotizacion},{dto.UsuarioConectado}";

                    //var result = await _rrdContext.Database.ExecuteSqlInterpolatedAsync(xqueryDiario);

                    /*var solicitudCredito = await _rrdContext.CreSolicitudDeCredito
                        .Where(x => cotizacion.Cotizacion == dto.Cotizacion && x.Aprobada==true).FirstOrDefaultAsync();
                    if (solicitudCredito == null)
                    {
                        response.Data = "Requiere Aprobacion de Credito";
                        metadata.Message = "Requiere Aprobacion de Credito";
                        metadata.success = false;
                        metadata.IsValid = false;
                        response.Meta = metadata;
                        return response;
                    }*/
                    var cotizacionLarga = $"{dto.Cotizacion}{dto.Renglon}{dto.Propuesta}";
                    var parametroMensaje = new SqlParameter("@mensaje", SqlDbType.NVarChar,400);
                    parametroMensaje.Direction = ParameterDirection.Output;
                    FormattableString xqueryDiarioPasePlanta =
                        $"exec ETL_PasePlantaPRUEBA @COTIZACIONRENGLONPROPUESTA={cotizacionLarga},@usuarioConectado={dto.UsuarioConectado},@mensaje={parametroMensaje} OUTPUT";
                    await _context.Database.ExecuteSqlInterpolatedAsync(xqueryDiarioPasePlanta);
                    var mensaje = (string)parametroMensaje.Value;

                    /*var propuesta = await _context.Wsmy515.Where(x =>
                            x.Cotizacion == dto.Cotizacion && x.Renglon == dto.Renglon && x.Propuesta == dto.Propuesta)
                        .FirstOrDefaultAsync();
                    if (propuesta != null)
                    {
                        mensaje = $"{mensaje}-{propuesta.Orden}";
                    }*/

                    response.Data = mensaje;
                    metadata.Message = mensaje;
                    metadata.success = true;
                    metadata.IsValid = true;
                    response.Meta = metadata;
                    return response;

                }
                else
                {
                    response.Data = "No existe Cotizacion";
                    metadata.Message = "No existe Cotizacion";
                    metadata.success = false;
                    metadata.IsValid = false;
                    response.Meta = metadata;

                    return response;
                }

               
            }
            catch (Exception e)
            {
                response.Data = "";
                metadata.Message = e.Message;
                metadata.success = false;
                metadata.IsValid = false;
                response.Meta = metadata;

                return response;
            }



        }
    






    public async Task<Wsmy501> GetByCotizacion(string cotizacion)
        {
            try
            {
                var cotizacionFind = await _context.Wsmy501.Where(x => x.Cotizacion == cotizacion).FirstOrDefaultAsync();

                return cotizacionFind;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
        public async Task<AppGeneralQuotes> GetGeneralQuotesByCotizacion(string cotizacion)
        {
            try
            {
                var cotizacionFind = await _rrdContext.AppGeneralQuotes.Where(x => x.Cotizacion == cotizacion).FirstOrDefaultAsync();

                return cotizacionFind;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
    
        public void UpdateGeneral(AppGeneralQuotes entity)
        {


            entity.FechaActualiza = DateTime.Now;
            _rrdContext.AppGeneralQuotes.Update(entity).Property(x => x.Id).IsModified = false;

        }


        public void Update(Wsmy501 entity)
        {


            entity.FechaActualiza = DateTime.Now;
            _context.Wsmy501.Update(entity).Property(x => x.Id).IsModified = false;

        }
        
        public async Task<List<V_CotizacionesPorAprobar>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter)
        {

            if (filter.PageNumber == 0) filter.PageNumber = 1;
            if (filter.PageSize == 0) filter.PageSize = 100;
            
            DateTime fechaDesde;
            DateTime fechaHasta;

            if (filter.FechaDesde == null)
            {
                fechaDesde = DateTime.Now.AddDays(-30);
                fechaHasta = DateTime.Now;
            }
            else
            {
                fechaDesde = Convert.ToDateTime(filter.FechaDesde).Date;
                fechaHasta = Convert.ToDateTime(filter.FechaHasta).Date;
            }



            fechaHasta = fechaHasta.AddHours(24);
            
            List<V_CotizacionesPorAprobar> result = new List<V_CotizacionesPorAprobar>();
            try
            {


                List<V_CotizacionesPorAprobar> allData = new List<V_CotizacionesPorAprobar>();

             
                if (filter.EstatusPlanta == "E")
                {
                    allData =  await _context.V_CotizacionesPorAprobar
                        .Where(x=>  x.Fecha >= fechaDesde && x.Fecha <= fechaHasta && x.ObservacionesCreditoExcepcion.Length>0  && x.SolicitudCerrada==false)
                        .OrderByDescending(x => x.Fecha)
                        .ToListAsync();
                    
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {

                        result = allData
              
                            .Where(x =>  x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                            .OrderByDescending(x => x.Fecha)
                            .ToList();
                    }

                    else
                    {
                        result = allData;
                    }
                }
                if (filter.EstatusPlanta == "A")
                {
                    allData =  await _context.V_CotizacionesPorAprobar
                        .OrderByDescending(x => x.Fecha)
                        .ToListAsync();
                    
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {

                        result = allData
              
                            .Where(x =>  x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                            .OrderByDescending(x => x.Fecha)
                            .ToList();
                    }   
                    else
                    {
                        result = allData;
                    }

                    /*foreach (var item in result)
                    {
                        await VerificarCredito(item.Cotizacion, filter.UsuarioConectado);
                    }*/
                    
                }
                
                if (String.IsNullOrEmpty(filter.EstatusPlanta))
                {
                    
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {
                        var allDataNew =  await _context.V_CotizacionesPorAprobar
                            .Where(x => x.Fecha >= fechaDesde && x.Fecha <= fechaHasta && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                            .OrderByDescending(x => x.Fecha)
                            .ToListAsync();
                        allData=allDataNew  
                            .Skip((filter.PageNumber - 1) * filter.PageSize)
                            .Take(filter.PageSize)
                           
                            .ToList();
                      
                        result = allData;
                    }
                    else
                    {
                       var  allDataNew  =  await _context.V_CotizacionesPorAprobar
                           .Where(x=> x.Fecha >= fechaDesde && x.Fecha <= fechaHasta) 
                           .OrderByDescending(x => x.Fecha)
                            .ToListAsync();
                       allData=allDataNew  
                           .Skip((filter.PageNumber - 1) * filter.PageSize)
                           .Take(filter.PageSize)
                           
                           .ToList();
                      
                       result = allData;
                    }
                    
                 
                       
                }

               

             
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         
         
            
            return result;
        }
      
         public async Task<List<CotizacionesViewData>> GetAllBySearchTextAll(AppGeneralQuotesQueryFilter filter)
        {

            if (filter.PageNumber == 0) filter.PageNumber = 1;
            if (filter.PageSize == 0) filter.PageSize = 100;
            
            DateTime fechaDesde;
            DateTime fechaHasta;

            if (filter.FechaDesde == null)
            {
                fechaDesde = DateTime.Now.AddDays(-30);
                fechaHasta = DateTime.Now;
            }
            else
            {
                fechaDesde = Convert.ToDateTime(filter.FechaDesde).Date;
                fechaHasta = Convert.ToDateTime(filter.FechaHasta).Date;
            }



            fechaHasta = fechaHasta.AddHours(24);
            
            List<CotizacionesViewData> result = new List<CotizacionesViewData>();
            try
            {


                List<CotizacionesViewData> allData = new List<CotizacionesViewData>();

             
        
                if (String.IsNullOrEmpty(filter.EstatusPlanta))
                {
                    
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {
                        var allDataNew =  await _context.CotizacionesViewData
                            .Where(x => x.Fecha >= fechaDesde && x.Fecha <= fechaHasta && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                            .OrderByDescending(x => x.Fecha)
                            .ToListAsync();
                        allData=allDataNew  
                            .Skip((filter.PageNumber - 1) * filter.PageSize)
                            .Take(filter.PageSize)
                           
                            .ToList();
                      
                        result = allData;
                    }
                    else
                    {
                       var  allDataNew  =  await _context.CotizacionesViewData
                           .Where(x=> x.Fecha >= fechaDesde && x.Fecha <= fechaHasta) 
                           .OrderByDescending(x => x.Fecha)
                            .ToListAsync();
                       allData=allDataNew  
                           .Skip((filter.PageNumber - 1) * filter.PageSize)
                           .Take(filter.PageSize)
                           
                           .ToList();
                      
                       result = allData;
                    }
                    
                 
                       
                }

               

             
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         
         
            
            return result;
        }
      
       
     
    }
}
