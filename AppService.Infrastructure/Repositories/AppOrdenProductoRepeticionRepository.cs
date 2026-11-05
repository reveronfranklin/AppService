using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class AppOrdenProductoRepeticionRepository : IAppOrdenProductoRepeticionRepository
    {

        private readonly RRDContext _context;

        public AppOrdenProductoRepeticionRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<List<AppOrdenProductoRepeticion>> GetAll()
        {

            return await _context.AppOrdenProductoRepeticion.ToListAsync();

        }
        public async Task<AppOrdenProductoRepeticion> GetByOrden(long orden)
        {
            try
            {
                var repeticiones = await _context.AppOrdenProductoRepeticion.Where(x => x.Orden == orden).FirstOrDefaultAsync();

                return repeticiones;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<(bool PuedeModificar, string Message)> PuedeModificarProductoAsync(string usuarioConectado)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            try
            {
                if (shouldClose)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = "sp_AppOrdenProductoRepeticion_CanUpdateProduct";
                command.CommandType = CommandType.StoredProcedure;

                var usuarioParameter = command.CreateParameter();
                usuarioParameter.ParameterName = "@UsuarioConectado";
                usuarioParameter.DbType = DbType.String;
                usuarioParameter.Size = 50;
                usuarioParameter.Value = usuarioConectado ?? string.Empty;
                command.Parameters.Add(usuarioParameter);

                var puedeModificarParameter = command.CreateParameter();
                puedeModificarParameter.ParameterName = "@PuedeModificar";
                puedeModificarParameter.DbType = DbType.Boolean;
                puedeModificarParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(puedeModificarParameter);

                var messageParameter = command.CreateParameter();
                messageParameter.ParameterName = "@Message";
                messageParameter.DbType = DbType.String;
                messageParameter.Size = 4000;
                messageParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(messageParameter);

                await command.ExecuteNonQueryAsync();

                var puedeModificar = puedeModificarParameter.Value != System.DBNull.Value
                    && System.Convert.ToBoolean(puedeModificarParameter.Value);
                var message = messageParameter.Value == System.DBNull.Value
                    ? string.Empty
                    : messageParameter.Value?.ToString() ?? string.Empty;

                return (puedeModificar, message);
            }
            finally
            {
                if (shouldClose && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        public void Update(AppOrdenProductoRepeticion entity)
        {
            _context.AppOrdenProductoRepeticion.Update(entity);

          

        }

        public async Task<List<AppOrdenProductoRepeticion>> GetByCliente(string idCliente)
        {
            try
            {
                var repeticiones = await _context.AppOrdenProductoRepeticion.Where(x => x.IdCliente == idCliente).ToListAsync();

                return repeticiones;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
        public async Task<List<AppRepeticionClienteProducto>> GetAppRepeticionClienteProductoByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClienteProducto.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
        public async Task<List<AppRepeticionClienteNombreForma>> GetAppRepeticionClienteNombreFormaByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClienteNombreForma.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClienteBasica>> GetAppRepeticionClienteBasicaByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClienteBasica.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClienteOpuesta>> GetAppRepeticionClienteOpuestaByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClienteOpuesta.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClientePartes>> GetAppRepeticionClientePartesByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClientePartes.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
        public async Task<List<AppRepeticionClienteTintas>> GetAppRepeticionClienteTintasByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClienteTintas.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClientePapelPrimeraParte>> GetAppRepeticionClientePapelPrimeraParteByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClientePapelPrimeraParte.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
        public async Task<List<AppRepeticionClientePapelSegundaParte>> GetAppRepeticionClientePapelSegundaParteByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClientePapelSegundaParte.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClientePapelTerceraParte>> GetAppRepeticionClientePapelTerceraParteByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClientePapelTerceraParte.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClientePapelCuartaParte>> GetAppRepeticionClientePapelCuartaParteByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClientePapelCuartaParte.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }

        public async Task<List<AppRepeticionClientePapelQuintaParte>> GetAppRepeticionClientePapelQuintaParteByCliente(string idCliente)
        {
            try
            {
                var result = await _context.AppRepeticionClientePapelQuintaParte.Where(x => x.IdCliente == idCliente).ToListAsync();

                return result;
            }
            catch (System.Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }



    }
}
