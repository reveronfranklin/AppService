﻿using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;

namespace AppService.Infrastructure.Repositories
{
    public class AppGeneralQuotesRepository : IAppGeneralQuotesRepository
    {

        private readonly RRDContext _context;
        private readonly MooreveContext _mooreveContext;

        public AppGeneralQuotesRepository(RRDContext context, MooreveContext mooreveContext)
        {
            _context = context;
            _mooreveContext = mooreveContext;
        }


        public async Task UpdatSearchText(string usuario)
        {

            FormattableString query = $"";
            query =
                $"update AppGeneralQuotes set Supervisor=(select supervisor from MtrVendedor where Codigo=AppGeneralQuotes.IdVendedor) where Supervisor is Null";
            
            bool esVendedor = false;
            MtrVendedor vendedor = await _context.MtrVendedor.Where(x => x.Codigo == usuario).FirstOrDefaultAsync();
            if (vendedor != null)
            {
                esVendedor = true;

            }

            FormattableString xqueryDiario = $"";
            if (esVendedor)
            {
               xqueryDiario  =
                    $"update AppGeneralQuotes set Fecha=CreatedAt,SearchText=Cotizacion + '-' + IdVendedor + '-' + (select top 1 NOMBRE from MtrVendedor where CODIGO=IdVendedor) + '-' + IdCliente + '-' + rtrim(ltrim(RazonSocial)) + '-' + Rif + '-' + (select top 1 ClaseCss from AppStatusQuote where AppStatusQuote.id= AppGeneralQuotes.IdEstatus) from AppGeneralQuotes WHERE IdVendedor={usuario} and DATEDIFF(DAY,CreatedAt,GETDATE())<=45";
               
            }
            else
            {
              xqueryDiario  =
                    $"update AppGeneralQuotes set Fecha=CreatedAt,SearchText=Cotizacion + '-' + IdVendedor + '-' + (select top 1 NOMBRE from MtrVendedor where CODIGO=IdVendedor) + '-' + IdCliente + '-' + rtrim(ltrim(RazonSocial)) + '-' + Rif + '-' + (select top 1 ClaseCss from AppStatusQuote where AppStatusQuote.id= AppGeneralQuotes.IdEstatus) from AppGeneralQuotes WHERE DATEDIFF(DAY,CreatedAt,GETDATE())<=45";
              
            }

            try
            {
                _context.Database.ExecuteSqlInterpolated(query);
                _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(xqueryDiario);
                throw;
            }
           
        
        }
        
        
        public void UpdatSearchTextById(int id)
        {

         
            FormattableString xqueryDiario = $"";
            xqueryDiario  =
                $"update AppGeneralQuotes set Fecha=CreatedAt,SearchText=Cotizacion + '-' + IdVendedor + '-' + (select top 1 NOMBRE from MtrVendedor where CODIGO=IdVendedor) + '-' + IdCliente + '-' + rtrim(ltrim(RazonSocial)) + '-' + Rif + '-' + (select top 1 ClaseCss from AppStatusQuote where AppStatusQuote.id= AppGeneralQuotes.IdEstatus) from AppGeneralQuotes WHERE id ={id}";


            try
            {
        
                _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(xqueryDiario);
                throw;
            }
           
        
        }
        
        
        public async Task<List<AppGeneralQuotes>> GetAll(AppGeneralQuotesQueryFilter filter)
        {

           // var query = "exec AppReparaStatusEnEsperaCliente";


            //var result1 = _context.AppGeneralQuotes.FromSqlRaw(query);

           // await UpdatSearchText(filter.UsuarioConectado);
            DateTime fechaDesde;
            DateTime fechaHasta;

            if (filter.FechaDesde == null)
            {
                fechaDesde = DateTime.Now.AddDays(-30);
                fechaHasta = DateTime.Now;
            }
            else
            {
                fechaDesde = Convert.ToDateTime(filter.FechaDesde);
                fechaHasta = Convert.ToDateTime(filter.FechaHasta);
            }



            fechaHasta = fechaHasta.AddHours(24);

            //var fechaDesde = System.Data.Entity.DbFunctions.TruncateTime(Convert.ToDateTime(filter.FechaDesde));
            //var fechaHasta = System.Data.Entity.DbFunctions.TruncateTime(Convert.ToDateTime(filter.FechaHasta));
            //DateTime.Now.ToShortDateString();

            bool esVendedor = false;
            bool esSupervisor = false;
            MtrVendedor vendedor = await _context.MtrVendedor.Where(x => x.Codigo == filter.UsuarioConectado).FirstOrDefaultAsync();
            if (vendedor != null)
            {
                esVendedor = true;
                if (vendedor.Supervisor == "")
                {
                    esSupervisor = true;
                    esVendedor = false;
                    
                }

            }
            
            

            List<AppGeneralQuotes> result = new List<AppGeneralQuotes>();


            try
            {

                if (esSupervisor)
                {
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                             .Include(x => x.IdClienteNavigation)
                             .Include(x => x.IdVendedorNavigation)
                             .Include(x => x.IdContactoNavigation)
                             .Include(x => x.IdEstatusNavigation)
                             .Include(x => x.IdMtrTipoMonedaNavigation)
                             .Where(x => (x.Supervisor == filter.UsuarioConectado.ToString() || x.IdVendedor== filter.UsuarioConectado) && x.CreatedAt >= fechaDesde && x.CreatedAt <= fechaHasta && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                             .OrderByDescending(x => x.CreatedAt).Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .ToListAsync();
                        return result;

                    }
                    else if (filter.Cotizacion != null && filter.Cotizacion.Length > 0)
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                             .Include(x => x.IdClienteNavigation)
                             .Include(x => x.IdVendedorNavigation)
                             .Include(x => x.IdContactoNavigation)
                             .Include(x => x.IdEstatusNavigation)
                             .Include(x => x.IdMtrTipoMonedaNavigation)
                             .Where(x => (x.Supervisor == filter.UsuarioConectado.ToString() || x.IdVendedor== filter.UsuarioConectado)  && x.Cotizacion.Trim() == filter.Cotizacion.Trim()).OrderByDescending(x => x.Fecha)
                             .Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .ToListAsync();
                        return result;

                    }
                    else
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                            .Include(x => x.IdClienteNavigation)
                            .Include(x => x.IdVendedorNavigation)
                            .Include(x => x.IdContactoNavigation)
                            .Include(x => x.IdEstatusNavigation)
                            .Include(x => x.IdMtrTipoMonedaNavigation)
                            .Where(x => (x.Supervisor == filter.UsuarioConectado.ToString() || x.IdVendedor== filter.UsuarioConectado)  && x.CreatedAt >= fechaDesde && x.CreatedAt <= fechaHasta)
                            .OrderByDescending(x => x.CreatedAt).Skip((filter.PageNumber - 1) * filter.PageSize)
                            .Take(filter.PageSize)
                            .ToListAsync();
                        return result;

                    }
                }


                if (esVendedor)
                {
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                             .Include(x => x.IdClienteNavigation)
                             .Include(x => x.IdVendedorNavigation)
                             .Include(x => x.IdContactoNavigation)
                             .Include(x => x.IdEstatusNavigation)
                             .Include(x => x.IdMtrTipoMonedaNavigation)
                             .Where(x => x.CreatedAt >= fechaDesde && x.CreatedAt <= fechaHasta && x.IdVendedor == filter.UsuarioConectado.ToString()  && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                             .OrderByDescending(x => x.CreatedAt).Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .ToListAsync();

                    }
                    else if (filter.Cotizacion != null && filter.Cotizacion.Length > 0)
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                             .Include(x => x.IdClienteNavigation)
                             .Include(x => x.IdVendedorNavigation)
                             .Include(x => x.IdContactoNavigation)
                             .Include(x => x.IdEstatusNavigation)
                             .Include(x => x.IdMtrTipoMonedaNavigation)
                             .Where(x => x.IdVendedor == filter.UsuarioConectado.ToString() && x.Cotizacion.Trim() == filter.Cotizacion.Trim()).OrderByDescending(x => x.Fecha)
                             .Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .ToListAsync();

                    }
                    else
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                            .Include(x => x.IdClienteNavigation)
                            .Include(x => x.IdVendedorNavigation)
                            .Include(x => x.IdContactoNavigation)
                            .Include(x => x.IdEstatusNavigation)
                            .Include(x => x.IdMtrTipoMonedaNavigation)
                            .Where(x =>  x.CreatedAt >= fechaDesde && x.CreatedAt <= fechaHasta && x.IdVendedor == filter.UsuarioConectado.ToString() )
                            .OrderByDescending(x => x.CreatedAt).Skip((filter.PageNumber - 1) * filter.PageSize)
                            .Take(filter.PageSize)
                            .ToListAsync();

                    }
                }
                else
                {
                    if (filter.SearchText != null && filter.SearchText.Length > 0)
                    {
                        result = await _context.AppGeneralQuotes
                                     .AsNoTracking()
                                    .Include(x => x.IdClienteNavigation)
                                    .Include(x => x.IdVendedorNavigation)
                                    .Include(x => x.IdContactoNavigation)
                                    .Include(x => x.IdEstatusNavigation)
                                    .Include(x => x.IdMtrTipoMonedaNavigation)
                                    .Where(x => x.CreatedAt >= fechaDesde && x.CreatedAt <= fechaHasta && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())).OrderByDescending(x => x.CreatedAt)
                                    .Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                                    .ToListAsync();

                    }
                    else if (filter.Cotizacion != null && filter.Cotizacion.Length > 0)
                    {
                        result = await _context.AppGeneralQuotes
                            .AsNoTracking()
                             .Include(x => x.IdClienteNavigation)
                             .Include(x => x.IdVendedorNavigation)
                             .Include(x => x.IdContactoNavigation)
                             .Include(x => x.IdEstatusNavigation)
                             .Include(x => x.IdMtrTipoMonedaNavigation)
                             .Where(x => x.Cotizacion.Trim() == filter.Cotizacion.Trim()).OrderByDescending(x => x.CreatedAt)
                             .Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .ToListAsync();

                    }
                    else
                    {
                        result = await _context.AppGeneralQuotes
                             .AsNoTracking()
                             .Include(x => x.IdClienteNavigation)
                             .Include(x => x.IdVendedorNavigation)
                             .Include(x => x.IdContactoNavigation)
                             .Include(x => x.IdEstatusNavigation)
                             .Include(x => x.IdMtrTipoMonedaNavigation)
                             .Where(x => x.CreatedAt >= fechaDesde && x.CreatedAt <= fechaHasta)
                             .OrderByDescending(x => x.CreatedAt)
                             .Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .ToListAsync();

                    }
                }

                return result;

            }
            catch (Exception e)
            {
                var a = e.InnerException.Message;
                return result;
            }



        }

        public async Task<AppGeneralQuotes> GetById(int id)
        {

            AppGeneralQuotes result = new AppGeneralQuotes();
            try
            {
                result = await _context.AppGeneralQuotes.AsNoTracking().Include(x => x.IdEstatusNavigation)
                                                         .Include(x => x.AppDetailQuotes)
                                                         .Include(x => x.IdClienteNavigation)
                                                         .Where(x => x.Id == id).FirstOrDefaultAsync();

                return result;

            }
            catch (Exception e)
            {
                var mesg = e.InnerException.Message;

                return null;
            }




        }

        public async Task<AppGeneralQuotes> GetByIdForUpdate(int id)
        {

            AppGeneralQuotes result = new AppGeneralQuotes();
            try
            {
                result = await _context.AppGeneralQuotes.Where(x => x.Id == id).FirstOrDefaultAsync();

                return result;

            }
            catch (Exception e)
            {
                var mesg = e.InnerException.Message;

                return null;
            }




        }

        public async Task<List<string>> GetListCotizaciones()
        {



            var query = "exec AppReparaStatusEnEsperaCliente";


            var result1 = _context.AppGeneralQuotes.FromSqlRaw(query);

            List<string> result = new List<string>();
            try
            {
                DateTime fechaDesde;
                DateTime fechaHasta;


                fechaDesde = DateTime.Now.AddDays(-3);
                fechaHasta = DateTime.Now;

                result = await _context.AppGeneralQuotes.Where(x => x.FechaActualiza >= fechaDesde && x.AppDetailQuotes.Count > 0).Select(p => p.Cotizacion).ToListAsync();


                return result;

            }
            catch (Exception e)
            {
                var mesg = e.InnerException.Message;

                return null;
            }




        }


        public async Task<List<AppGeneralQuotes>> GetListCotizacionesUltimoMes()
        {

            var query = "exec AppReparaStatusEnEsperaCliente";


            var result1 = _context.AppGeneralQuotes.FromSqlRaw(query);
            List<AppGeneralQuotes> result = new List<AppGeneralQuotes>();
            try
            {
                DateTime fechaDesde;
                DateTime fechaHasta;


                fechaDesde = DateTime.Now.AddDays(-30);
                fechaHasta = DateTime.Now;

                result = await _context.AppGeneralQuotes.Where(x => x.Fecha >= fechaDesde && x.AppDetailQuotes.Count > 0).ToListAsync();


                return result;

            }
            catch (Exception e)
            {
                var mesg = e.InnerException.Message;

                return null;
            }




        }



        public async Task<List<AppGeneralQuotes>> GetByCotizacionesPendientesIntegrar()
        {
            var result = await _context.AppGeneralQuotes.AsNoTracking().Where(x => x.IntegrarCotizacion == true).ToListAsync();
            return result;
        }

        public async Task<AppGeneralQuotes> GetByCotizacions(string cotizacion)
        {
            return await _context.AppGeneralQuotes.AsNoTracking().Where(x => x.Cotizacion == cotizacion).FirstOrDefaultAsync();
        }

        public async Task Add(AppGeneralQuotes entity)
        {
            entity.IntegrarCotizacion = true;
            await _context.AppGeneralQuotes.AddAsync(entity);
             UpdatSearchTextById(entity.Id);

        }



        public void Update(AppGeneralQuotes entity)
        {
            entity.IntegrarCotizacion = true;
            _context.AppGeneralQuotes.Update(entity);
            UpdatSearchTextById(entity.Id);

        }

        public async Task Delete(int id)
        {
            AppGeneralQuotes entity = await GetByIdForUpdate(id);
            _context.AppGeneralQuotes.Remove(entity);

        }

        public void MarcarIntegrado(bool marca,int id)
        {
            int integrar = 0;
            if (marca == false)
            {
                integrar = 0;

            }
            else
            {
                integrar = 1;
            }
            FormattableString xqueryDiario = $"UPDATE AppGeneralQuotes SET IntegrarCotizacion = {integrar} WHERE ID={id}";

            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);}

        public string ProximaCotizacion(string Cod_Vendedor)
        {

            try
            {
                string ProximaCotizacion = "";
                string vendededor = Cod_Vendedor.ToUpper();
                string año = DateTime.Now.Year.ToString();
                string mes = DateTime.Now.Month.ToString().PadRight(2, '0');

                ProximaCotizacion = $"{vendededor}{año}{mes}";
                //var query = $"SELECT Max(Proximo) as Proximo From (select isnull(cast(max(substring(cotizacion,11,3))  as integer),0) as Proximo from  wsmy501 where substring(cotizacion,1,10) = '{ProximaCotizacion}' UNION ALL select isnull(cast(max(substring(cotizacion,11,3))  as integer),0) as Proximo from  wsmy501Log where substring(cotizacion,1,10) = '{ProximaCotizacion}') as Detalle";

                var query = $"SELECT Max(Proximo) as Proximo From (select isnull(cast(max(substring(cotizacion,11,3))  as integer),0) as Proximo from  AppGeneralQuotes where substring(cotizacion,1,10) = '{ProximaCotizacion}' UNION ALL select isnull(cast(max(substring(cotizacion,11,3))  as integer),0) as Proximo from  AppGeneralQuotes where substring(cotizacion,1,10) = '{ProximaCotizacion}') as Detalle";


                var result = _context.AppGeneralQuotes.FromSqlRaw(query).Select(b => new
                {
                    Proximo = b.Proximo
                }).FirstOrDefault();



                var siguiente = result.Proximo + 1;
                var proximoString = siguiente.ToString();
                proximoString = proximoString.ToString().PadLeft(3, '0');
                ProximaCotizacion = ProximaCotizacion + proximoString;

                return ProximaCotizacion;
            }
            catch (Exception e)
            {
                string ProximaCotizacion = "";

                ProximaCotizacion = e.Message;
                return ProximaCotizacion;

            }


            /* Dim Dt New ClsDB.clsDB
        Cnn.Conectar(ClsDB.clsDB.BasesDeDatos.Mooreve, My.Settings.Servidor, ClsDB.clsDB.Seguridad.SQL, Var_UsuarioConectar, Var_ClaveConectar)
        New SqlCommand("SELECT Max(Proximo) as Proximo From (select isnull(cast(max(substring(cotizacion,11,3))  as integer),0) as Proximo from  wsmy501 where substring(cotizacion,1,10) = '" & ProximaCotizacion & "' UNION ALL select isnull(cast(max(substring(cotizacion,11,3))  as integer),0) as Proximo from  wsmy501Log where substring(cotizacion,1,10) = '" & ProximaCotizacion & "') as Detalle", Cnn.Conexion)
            Dim Da SqlDataAdapter(Cmd)
            Da.Fill(Dt)
        Catch ex             Me.Var_Excepcion = ex.Message
        Dt.Rows.Count > 0 Then
                Proximo = Format(Dt.Rows(0).Item(0) + 1, "000")
                ProximaCotizacion = ProximaCotizacion + Proximo
            Dt = Nothing
        Cnn.Desconectar()
        Cnn = Nothing*/
        }



        public async Task<bool> PermiteAdicionarDetalle(int idGeneralQuote)
        {

            bool result = new bool();

            try
            {
                var detail = await _context.AppDetailQuotes.Where(x => x.AppGeneralQuotesId == idGeneralQuote).ToListAsync();
                if (detail == null || detail.Count == 0)
                {
                    result = true;
                }
                else
                {
                    foreach (var item in detail)
                    {

                        var appProducts = await _context.AppProducts.Where(x => x.Id == item.IdProducto).FirstOrDefaultAsync();
                        if (appProducts != null)
                        {
                            if (appProducts.AceptaMultiplesItem == true)
                            {
                                result = true;
                            }
                            else
                            {
                                return false;
                            }
                        }

                    }
                }


                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }

         
        }

        public async Task<bool> AppDeleteSolcitudCreditoCotizacion
            (string cotizacion )
        {
          

            FormattableString xqueryDiarioPasePlanta = $"exec AppDeleteSolcitudCreditoCotizacion {cotizacion}";
            var resultPasePlanta = _context.Database.ExecuteSqlInterpolated(xqueryDiarioPasePlanta);
         
            return true;

        
            return true;
        }
        public async Task<int> VerificarStatus(int idGeneralQuote)
        {

            int result = 0;

            var generalQuotes = await GetById(idGeneralQuote);
            if (generalQuotes != null)
            {

                var cotizacion = await _mooreveContext.Wsmy501.Where(x => x.Cotizacion == generalQuotes.Cotizacion).FirstOrDefaultAsync();

                if (cotizacion != null)
                {
                    if (generalQuotes.IdEstatus > cotizacion.Estatus)
                    {

                        generalQuotes.IdEstatus = (int)cotizacion.Estatus;
                        result = generalQuotes.IdEstatus;
                        Update(generalQuotes);


                        var detailQuotes = await _context.AppDetailQuotes.Where(x => x.AppGeneralQuotesId == idGeneralQuote).ToListAsync();
                        if (detailQuotes.Count > 0)
                        {

                            foreach (var item in detailQuotes)
                            {
                                item.IdEstatus = generalQuotes.IdEstatus;
                                _context.AppDetailQuotes.Update(item);
                            }

                        }

                    }
                    else
                    {
                        result = generalQuotes.IdEstatus;
                    }

                }


            }






            return generalQuotes.IdEstatus;


        }
    }
}
