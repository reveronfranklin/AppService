using System;
using System.Data;
using System.Threading.Tasks;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace AppService.Infrastructure.Repositories
{
    internal static class LegacyQuoteModel120Maintenance
    {
        internal static async Task RefreshSearchAsync(RRDContext context, string user)
        {
            if (user != null && user.Length > 50) throw new InvalidOperationException("Usuario no valido.");
            var connection = context.Database.GetDbConnection();
            var close = connection.State != ConnectionState.Open;
            try
            {
                if (close) await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "dbo.AppQuote120_LegacyRefreshSearch";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 30;
                command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();
                var actor = command.CreateParameter();actor.ParameterName="@Usuario";actor.DbType=DbType.String;actor.Size=50;actor.Value=(object)user??DBNull.Value;command.Parameters.Add(actor);
                var message = command.CreateParameter();message.ParameterName="@Message";message.DbType=DbType.String;message.Size=4000;message.Direction=ParameterDirection.Output;command.Parameters.Add(message);
                await command.ExecuteNonQueryAsync();
                var result=message.Value as string;
                if(result!="Success")throw new InvalidOperationException(string.IsNullOrEmpty(result)?"No se pudo actualizar la busqueda legacy.":result);
            }
            finally { if(close && connection.State==ConnectionState.Open)await connection.CloseAsync(); }
        }
    }
}
