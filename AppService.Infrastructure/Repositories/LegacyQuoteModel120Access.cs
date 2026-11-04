using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AppService.Infrastructure.DataMooreve;
using System;
using System.Threading.Tasks;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
namespace AppService.Infrastructure.Repositories
{
    public sealed class LegacyQuoteModel120Access : ILegacyQuoteModelAccess
    {
        private readonly RRDContext _context;
        private readonly MooreveContext _mooreve;
        private readonly bool _enabled;
        public LegacyQuoteModel120Access(RRDContext context, IConfiguration configuration)
            : this(context, configuration, null)
        {
        }
        public LegacyQuoteModel120Access(RRDContext context, IConfiguration configuration, MooreveContext mooreve)
        {
            _context = context;
            _mooreve = mooreve;
            _enabled = configuration.GetValue<bool>("QuoteModel120:CompatibilityGuardsEnabled");
        }
        public async Task<string> CheckAsync(int? generalId = null, int? detailId = null, string quotation = null)
        {
            try
            {
                await LegacyQuoteModel120Guard.CheckAsync(_context, _enabled, generalId, detailId, quotation);
                return null;
            }
            catch (InvalidOperationException ex) { return ex.InnerException?.Message ?? ex.Message; }
            // SQL/connection errors propagate to the case use's existing error path;
            // they must never be interpreted as an allowed legacy model.
        }
        public async Task<string> CheckExternalAsync(string quotation)
        {
            try
            {
                await LegacyQuoteModel120Guard.CheckExternalAsync(_context, _enabled, quotation);
                return null;
            }
            catch (InvalidOperationException ex) { return ex.InnerException?.Message ?? ex.Message; }
        }

        public async Task<string> CheckOdooDetailAsync(int mooreId)
        {
            if (!_enabled) return null;
            if (mooreId <= 0) return "Identificador del detalle Odoo no valido.";
            if (_mooreve == null) return "No se pudo validar la identidad del detalle Odoo.";
            var connection = _mooreve.Database.GetDbConnection();
            var close = connection.State != ConnectionState.Open;
            string quotation = null;
            try
            {
                if (close) await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "dbo.AppQuote120_GetOdooDetailIdentity";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Transaction = _mooreve.Database.CurrentTransaction?.GetDbTransaction();
                var id = command.CreateParameter();id.ParameterName="@MooreId";id.DbType=DbType.Int32;id.Value=mooreId;command.Parameters.Add(id);
                var message = command.CreateParameter();message.ParameterName="@Message";message.DbType=DbType.String;message.Size=4000;message.Direction=ParameterDirection.Output;command.Parameters.Add(message);
                var count=0;
                using (var reader=await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        count++;
                        if(count!=1 || Convert.ToDecimal(reader["MooreId"])!=mooreId)
                            return "El identificador del detalle Odoo es ambiguo.";
                        quotation=reader["Cotizacion"] as string;
                    }
                    if(await reader.NextResultAsync()) return "No se pudo validar la identidad del detalle Odoo.";
                }
                var result=message.Value as string;
                if(result!="Success")return string.IsNullOrEmpty(result)?"No se pudo validar la identidad del detalle Odoo.":result;
                if(count!=1 || string.IsNullOrWhiteSpace(quotation))return "No se pudo validar la identidad del detalle Odoo.";
            }
            finally { if(close && connection.State==ConnectionState.Open)await connection.CloseAsync(); }
            return await CheckExternalAsync(quotation);
        }

    }
}
