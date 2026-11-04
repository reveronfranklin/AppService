using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppService.Infrastructure.Repositories
{
    // Enable only after the F01 policy SP is installed. A missing policy fails
    // before repository writes; the client cannot override the model decision.
    internal static class LegacyQuoteModel120Guard
    {
        private static DbCommand Command(RRDContext context, int? generalId, int? detailId, string quotation)
        {
            if (quotation != null && quotation.Trim().Length > 13)
                throw new InvalidOperationException("Identidad de cotizacion no valida.");
            var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.AppQuote120_CheckLegacyAccess";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 30;
            command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();
            Add(command, "@AppGeneralQuotesId", DbType.Int32, generalId > 0 ? (object)generalId.Value : DBNull.Value);
            Add(command, "@AppDetailQuotesId", DbType.Int32, detailId > 0 ? (object)detailId.Value : DBNull.Value);
            Add(command, "@Cotizacion", DbType.String, string.IsNullOrWhiteSpace(quotation) ? DBNull.Value : (object)quotation.Trim(), 13);
            var message = Add(command, "@Message", DbType.String, DBNull.Value, 4000);
            message.Direction = ParameterDirection.Output;
            return command;
        }
        private static DbParameter Add(DbCommand command, string name, DbType type, object value, int size = 0)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name; parameter.DbType = type; parameter.Value = value;
            if (size > 0) parameter.Size = size;
            command.Parameters.Add(parameter); return parameter;
        }
        private static void Validate(DbCommand command)
        {
            var message = command.Parameters["@Message"].Value as string;
            if (message != "Success")
                throw new InvalidOperationException(string.IsNullOrEmpty(message)
                    ? "No se pudo validar el modelo de cotizacion." : message);
        }
        internal static void Check(RRDContext context, bool enabled, int? generalId = null, int? detailId = null, string quotation = null)
        {
            if (!enabled) return;
            var connection = context.Database.GetDbConnection();
            var close = connection.State != ConnectionState.Open;
            try
            {
                if (close) connection.Open();
                using var command = Command(context, generalId, detailId, quotation);
                command.ExecuteNonQuery(); Validate(command);
            }
            finally { if (close && connection.State == ConnectionState.Open) connection.Close(); }
        }
        internal static async Task CheckAsync(RRDContext context, bool enabled, int? generalId = null, int? detailId = null, string quotation = null)
        {
            if (!enabled) return;
            var connection = context.Database.GetDbConnection();
            var close = connection.State != ConnectionState.Open;
            try
            {
                if (close) await connection.OpenAsync();
                using var command = Command(context, generalId, detailId, quotation);
                await command.ExecuteNonQueryAsync(); Validate(command);
            }
            finally { if (close && connection.State == ConnectionState.Open) await connection.CloseAsync(); }
        }
        internal static async Task CheckExternalAsync(RRDContext context, bool enabled, string quotation)
        {
            if (!enabled) return;
            if (string.IsNullOrWhiteSpace(quotation) || quotation.Trim().Length > 13)
                throw new InvalidOperationException("Identidad de cotizacion externa no valida.");
            var connection = context.Database.GetDbConnection();
            var close = connection.State != ConnectionState.Open;
            try
            {
                if (close) await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "dbo.AppQuote120_CheckExternalLegacyAccess";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                Add(command, "@Cotizacion", DbType.String, quotation.Trim(), 4000);
                var message = Add(command, "@Message", DbType.String, DBNull.Value, 4000);
                message.Direction = ParameterDirection.Output;
                await command.ExecuteNonQueryAsync(); Validate(command);
            }
            finally { if (close && connection.State == ConnectionState.Open) await connection.CloseAsync(); }
        }

    }
}
