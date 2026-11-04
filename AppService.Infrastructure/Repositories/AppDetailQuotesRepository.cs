using Microsoft.Extensions.Configuration;
using System;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace AppService.Infrastructure.Repositories
{
    public class AppDetailQuotesRepository : IAppDetailQuotesRepository
    {

        private readonly RRDContext _context;
        private readonly bool _quoteModel120GuardsEnabled;

        public AppDetailQuotesRepository(RRDContext context)
            : this(context, null)
        {
        }

        public AppDetailQuotesRepository(RRDContext context, IConfiguration configuration)
        {
            _context = context;
            _quoteModel120GuardsEnabled = configuration?.GetValue<bool>("QuoteModel120:CompatibilityGuardsEnabled") ?? false;
        }


        private IQueryable<AppGeneralQuotes> LegacyGeneralQuotes => !_quoteModel120GuardsEnabled
            ? _context.AppGeneralQuotes
            : _context.AppGeneralQuotes.Where(g => _context.LegacyQuoteScope().Any(s => s.Id == g.Id));
        private IQueryable<AppDetailQuotes> LegacyDetailQuotes => !_quoteModel120GuardsEnabled
            ? _context.AppDetailQuotes
            : _context.AppDetailQuotes.Where(d => _context.LegacyQuoteScope().Any(s => s.Id == d.AppGeneralQuotesId && s.Cotizacion == d.Cotizacion));

        public async Task<List<AppDetailQuotes>> GetAll()
        {

            return await LegacyDetailQuotes.ToListAsync();

        }

        public async Task<string> ValidateCanWin(int appDetailQuotesId)
        {
            DbConnection connection = _context.Database.GetDbConnection();
            bool closeConnection = connection.State != ConnectionState.Open;

            try
            {
                if (closeConnection)
                {
                    await connection.OpenAsync();
                }

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.AppDetailQuotes_ValidateCanWin";
                    command.CommandType = CommandType.StoredProcedure;

                    DbParameter detailId = command.CreateParameter();
                    detailId.ParameterName = "@AppDetailQuotesId";
                    detailId.DbType = DbType.Int32;
                    detailId.Value = appDetailQuotesId;
                    command.Parameters.Add(detailId);

                    DbParameter canWin = command.CreateParameter();
                    canWin.ParameterName = "@CanWin";
                    canWin.DbType = DbType.Boolean;
                    canWin.Direction = ParameterDirection.Output;
                    command.Parameters.Add(canWin);

                    DbParameter blockReason = command.CreateParameter();
                    blockReason.ParameterName = "@BlockReason";
                    blockReason.DbType = DbType.String;
                    blockReason.Size = 30;
                    blockReason.Direction = ParameterDirection.Output;
                    command.Parameters.Add(blockReason);

                    DbParameter message = command.CreateParameter();
                    message.ParameterName = "@Message";
                    message.DbType = DbType.String;
                    message.Size = 4000;
                    message.Direction = ParameterDirection.Output;
                    command.Parameters.Add(message);

                    await command.ExecuteNonQueryAsync();

                    bool isValid = canWin.Value != DBNull.Value && Convert.ToBoolean(canWin.Value);
                    return isValid ? string.Empty : Convert.ToString(message.Value);
                }
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }
        }
        
        public async Task<List<AppDetailQuotes>> GetByYearMonth(int year, int month)
        {

            return await LegacyDetailQuotes
                .Where(x=>x.FechaRegistro.Year == year && x.FechaRegistro.Month == month)
                .ToListAsync();

        }


        public async Task<List<AppDetailQuotes>> GetByAppGeneralQuotesId(int appGeneralQuotesId)
        {
            return await LegacyDetailQuotes.Where(x => x.AppGeneralQuotesId == appGeneralQuotesId).Include(x => x.AppGeneralQuotes).Include(x => x.IdEstatusNavigation).Include(x => x.IdProductoNavigation).ToListAsync();
        }
        public async Task<List<AppDetailQuotes>> GeSimpletByAppGeneralQuotesId(int appGeneralQuotesId)
        {
            return await LegacyDetailQuotes.Where(x => x.AppGeneralQuotesId == appGeneralQuotesId).ToListAsync();
        }

        public async Task<bool> ExisteEnEspera(int appGeneralQuotesId)
        {

            bool result = false;
            var detail = await LegacyDetailQuotes.Where(x => x.AppGeneralQuotesId == appGeneralQuotesId).ToListAsync();
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
            var detail = await LegacyDetailQuotes.Where(x => x.IdEstatus<5 && x.IdProductoNavigation.AppSubCategoryId == subCategory).ToListAsync();

            return detail;

        }


        public async Task<AppDetailQuotes> GetById(int id)
        {

            var detail = await LegacyDetailQuotes.Include(x => x.IdProductoNavigation).Include(x => x.IdEstatusNavigation).Include(x => x.AppGeneralQuotes).Where(x => x.Id == id).FirstOrDefaultAsync();
            return detail;

        }

        public async Task<List<AppDetailQuotes>> GetByQuotesCotizacion(string cotizacion)
        {
            var detail = await LegacyDetailQuotes.Where(x => x.Cotizacion == cotizacion).ToListAsync();

            return detail;

        }
        public async Task<AppDetailQuotes> GetByQuetesProduct(string cotizacion, int idProduct)
        {
            var detail = await LegacyDetailQuotes.Where(x => x.Cotizacion == cotizacion && x.IdProducto == idProduct).FirstOrDefaultAsync();

            return detail;

        }
        public async Task<AppDetailQuotes> GetByGeneralQuotesExternalcode(int idGeneralQuotes, string externalCode)
        {
            var detail = await LegacyDetailQuotes.Where(x => x.AppGeneralQuotesId == idGeneralQuotes && x.IdProductoNavigation.ExternalCode == externalCode).FirstOrDefaultAsync();

            return detail;

        }
        public async Task<bool> ProductExistInQuotesDetail(int idProduct)
        {
            // Product integrity spans every quote model, without loading/tracking120 entities.
            return await _context.AppDetailQuotes.AnyAsync(x => x.IdProducto == idProduct);
        }


        public async Task Add(AppDetailQuotes entity)
        {
            await LegacyQuoteModel120Guard.CheckAsync(_context, _quoteModel120GuardsEnabled, generalId: entity.AppGeneralQuotesId, quotation: entity.Cotizacion);

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
            await LegacyQuoteModel120Guard.CheckAsync(_context, _quoteModel120GuardsEnabled, detailId: appDetailQuotesId);
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
            await LegacyQuoteModel120Guard.CheckAsync(_context, _quoteModel120GuardsEnabled, detailId: appDetailQuotesId);
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
            LegacyQuoteModel120Guard.Check(_context, _quoteModel120GuardsEnabled, detailId: appDetailsId);
            var precioMinimoString = precioMinimo.ToString();
            precioMinimoString = precioMinimoString.Replace(",", ".");
            var precioMaximoString = precioMaximo.ToString();
            precioMaximoString = precioMaximoString.Replace(",", ".");
            
            FormattableString queryGeneral = $"update AppDetailQuotes set UnitPriceBaseProduction= {precioMinimoString} ,UnitPriceBaseProductionMaximo={precioMaximoString},CalculoId={calculoId},SolicitarPrecio={solicitarPrecio} where id={appDetailsId}";

            var resultGeneral = _context.Database.ExecuteSqlInterpolated(queryGeneral);
     
            
       
            

        }
        public void UpdateCondicionPago(int appGeneralQuotesId,short condicionPago)
        {
            LegacyQuoteModel120Guard.Check(_context, _quoteModel120GuardsEnabled, generalId: appGeneralQuotesId);

            try
            {
                FormattableString queryGeneral = $"UPDATE AppGeneralQuotes SET IdCondPago ={condicionPago} ,integrarCotizacion= 1 WHERE ID={appGeneralQuotesId}";

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
            LegacyQuoteModel120Guard.Check(_context, _quoteModel120GuardsEnabled, generalId: entity.AppGeneralQuotesId, detailId: entity.Id, quotation: entity.Cotizacion);
            FormattableString xqueryDiario = $"UPDATE AppGeneralQuotes SET IntegrarCotizacion = 1 WHERE ID={entity.AppGeneralQuotesId}";

            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            _context.AppDetailQuotes.Update(entity);

        }

        public async Task Delete(int id)
        {
            await LegacyQuoteModel120Guard.CheckAsync(_context, _quoteModel120GuardsEnabled, detailId: id);
            
          

            AppDetailQuotes entity = await GetById(id);
            _context.AppDetailQuotes.Remove(entity);
            
            FormattableString xqueryDiario = $"UPDATE AppGeneralQuotes SET IntegrarCotizacion = 1 WHERE ID={entity.AppGeneralQuotesId}";

            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

        }


    }
}
