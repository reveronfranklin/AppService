using System;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class AppDetailQuotesRepository : IAppDetailQuotesRepository
    {

        private readonly RRDContext _context;

        public AppDetailQuotesRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<List<AppDetailQuotes>> GetAll()
        {

            return await _context.AppDetailQuotes.ToListAsync();

        }
        
        public async Task<List<AppDetailQuotes>> GetByYearMonth(int year, int month)
        {

            return await _context.AppDetailQuotes
                .Where(x=>x.FechaRegistro.Year == year && x.FechaRegistro.Month == month)
                .ToListAsync();

        }


        public async Task<List<AppDetailQuotes>> GetByAppGeneralQuotesId(int appGeneralQuotesId)
        {
            return await _context.AppDetailQuotes.Where(x => x.AppGeneralQuotesId == appGeneralQuotesId).Include(x => x.AppGeneralQuotes).Include(x => x.IdEstatusNavigation).Include(x => x.IdProductoNavigation).ToListAsync();
        }
        public async Task<List<AppDetailQuotes>> GeSimpletByAppGeneralQuotesId(int appGeneralQuotesId)
        {
            return await _context.AppDetailQuotes.Where(x => x.AppGeneralQuotesId == appGeneralQuotesId).ToListAsync();
        }

        public async Task<bool> ExisteEnEspera(int appGeneralQuotesId)
        {

            bool result = false;
            var detail = await _context.AppDetailQuotes.Where(x => x.AppGeneralQuotesId == appGeneralQuotesId).ToListAsync();
            if (detail != null)
            {
                foreach (var item in detail)
                {
                    AppStatusQuote appStatusQuote = await _context.AppStatusQuote.Where(x => x.Id == item.IdEstatus).FirstOrDefaultAsync();
                    if (appStatusQuote.FlagEnEspera == "X")
                    {
                        result = true;
                    }
                }

                return result;
            }
            else
            {
                return false;
            }



        }


        public async Task<List<AppDetailQuotes>> GetBySubCategopry(int subCategory)
        {
            var detail = await _context.AppDetailQuotes.Where(x => x.IdEstatus<5 && x.IdProductoNavigation.AppSubCategoryId == subCategory).ToListAsync();

            return detail;

        }


        public async Task<AppDetailQuotes> GetById(int id)
        {

            var detail = await _context.AppDetailQuotes.Include(x => x.IdProductoNavigation).Include(x => x.IdEstatusNavigation).Include(x => x.AppGeneralQuotes).Where(x => x.Id == id).FirstOrDefaultAsync();
            return detail;

        }

        public async Task<List<AppDetailQuotes>> GetByQuotesCotizacion(string cotizacion)
        {
            var detail = await _context.AppDetailQuotes.Where(x => x.Cotizacion == cotizacion).ToListAsync();

            return detail;

        }
        public async Task<AppDetailQuotes> GetByQuetesProduct(string cotizacion, int idProduct)
        {
            var detail = await _context.AppDetailQuotes.Where(x => x.Cotizacion == cotizacion && x.IdProducto == idProduct).FirstOrDefaultAsync();

            return detail;

        }
        public async Task<AppDetailQuotes> GetByGeneralQuotesExternalcode(int idGeneralQuotes, string externalCode)
        {
            var detail = await _context.AppDetailQuotes.Where(x => x.AppGeneralQuotesId == idGeneralQuotes && x.IdProductoNavigation.ExternalCode == externalCode).FirstOrDefaultAsync();

            return detail;

        }
        public async Task<bool> ProductExistInQuotesDetail(int idProduct)
        {

            var detail = await _context.AppDetailQuotes.Where(x => x.IdProducto == idProduct).FirstOrDefaultAsync();
            if (detail != null)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public async Task Add(AppDetailQuotes entity)
        {

            FormattableString xqueryDiario = $"UPDATE AppGeneralQuotes SET IntegrarCotizacion = 1 WHERE ID={entity.AppGeneralQuotesId}";

            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

            
            var cotizacion = await GetByQuetesProduct(entity.Cotizacion, entity.IdProducto);

            if (cotizacion == null)
            {
                await _context.AppDetailQuotes.AddAsync(entity);
            }



        }

        public async Task UpdateFlete(int appDetailQuotesId,decimal porcFlete,decimal flete)
        {
            try
            {
                string porcFleteString = porcFlete.ToString();
                string fleteString = flete.ToString();
                porcFleteString = porcFleteString.Replace(",", ".");
                fleteString = fleteString.Replace(",", ".");
                FormattableString xqueryDiario = $"UPDATE AppDetailQuotes SET PorcFlete={porcFleteString},Flete={fleteString} WHERE ID={appDetailQuotesId}";

                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
         
            

        }
        
        public async Task UpdatePrecioMinimo(int appDetailQuotesId,decimal precioMinimo,decimal precioMaximo,int calculoId)
        {
            try
            {
                string minimo = precioMinimo.ToString();
                string maximo = precioMaximo.ToString();
                minimo = minimo.Replace(",", ".");
                maximo = maximo.Replace(",", ".");
                var query =$"exec UpdatePrecioListaCotizacion {appDetailQuotesId},{minimo}, {maximo},{calculoId}";
                FormattableString xqueryDiario = $"exec UpdatePrecioListaCotizacion {appDetailQuotesId},{minimo}, {maximo},{calculoId}";

                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
         
            

        }

        
        
        
        public void UpdatePrecios(int appDetailsId,decimal precioMinimo,decimal precioMaximo, int calculoId,int solicitarPrecio)
        {
            var precioMinimoString = precioMinimo.ToString();
            precioMinimoString = precioMinimoString.Replace(",", ".");
            var precioMaximoString = precioMaximo.ToString();
            precioMaximoString = precioMaximoString.Replace(",", ".");
            
            FormattableString queryGeneral = $"update AppDetailQuotes set UnitPriceBaseProduction= {precioMinimoString} ,UnitPriceBaseProductionMaximo={precioMaximoString},CalculoId={calculoId},SolicitarPrecio={solicitarPrecio} where id={appDetailsId}";

            var resultGeneral = _context.Database.ExecuteSqlInterpolated(queryGeneral);
     
            
       
            

        }
        public void UpdateCondicionPago(int appGeneralQuotesId,short condicionPago)
        {

            try
            {
                FormattableString queryGeneral = $"UPDATE AppGeneralQuotes SET IdCondPago ={condicionPago} WHERE ID={appGeneralQuotesId}";

                var resultGeneral = _context.Database.ExecuteSqlInterpolated(queryGeneral);
     
            
                FormattableString queryDetail = $"UPDATE AppDetailQuotes SET IdCondPago ={condicionPago} WHERE AppGeneralQuotesId={appGeneralQuotesId}";

                var resultDetail = _context.Database.ExecuteSqlInterpolated(queryDetail);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
          
            
            

        }

        public void Update(AppDetailQuotes entity)
        {
            FormattableString xqueryDiario = $"UPDATE AppGeneralQuotes SET IntegrarCotizacion = 1 WHERE ID={entity.AppGeneralQuotesId}";

            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            _context.AppDetailQuotes.Update(entity);

        }

        public async Task Delete(int id)
        {
            
          

            AppDetailQuotes entity = await GetById(id);
            _context.AppDetailQuotes.Remove(entity);
            
            FormattableString xqueryDiario = $"UPDATE AppGeneralQuotes SET IntegrarCotizacion = 1 WHERE ID={entity.AppGeneralQuotesId}";

            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

        }


    }
}
