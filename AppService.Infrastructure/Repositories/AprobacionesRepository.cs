using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataMooreve;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class AprobacionesRepository : IAprobacionesRepository
    {

        private readonly MooreveContext _context;

        public AprobacionesRepository(MooreveContext context)
        {
            _context = context;
        }




        public async Task<List<Wsmy639>> GetAll()
        {

            return await _context.Wsmy639.ToListAsync();

        }




        public async Task<Wsmy639> GetById(long id)
        {
            return await _context.Wsmy639.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Wsmy639> GetByCotizacionRenglonPropuesta(string cotizacion, int renglon, int propuesta)
        {
            return await _context.Wsmy639.Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }


        public async Task<Wsmy639> CreaAprobacion(string cotizacion, int renglon, int propuesta, string usuarioConectado)
        {

            if (usuarioConectado == null)
            {
                usuarioConectado = "RR105841";

            }
            var wsmy501 = await _context.Wsmy501.Where(x => x.Cotizacion == cotizacion).FirstOrDefaultAsync();
            if (wsmy501 != null)
            {
                usuarioConectado = wsmy501.CodVendedor;
            }


            var cotizacionP = new SqlParameter("@Cotizacion", cotizacion);
            var renglonP = new SqlParameter("@Renglon", renglon);
            var propuestaP = new SqlParameter("@Propuesta", propuesta);
            var usuarioConectadoP = new SqlParameter("@UsuarioConectado", usuarioConectado);
            try
            {
                //var result = await _context.Wsmy639.FromSqlRaw<Wsmy639>("exec AppCreaWsmy639 @Cotizacion,@Renglon,@Propuesta,@UsuarioConectado", cotizacionP, renglonP, propuestaP, usuarioConectadoP).ToListAsync();
                //var aprobacion = result.FirstOrDefault();
                //return aprobacion;
                
                
                FormattableString xqueryDiario = $"exec AppCreaWsmy639 {cotizacion},{renglon},{propuesta},{usuarioConectado}";
                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

                var result = await _context.Wsmy639
                    .Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta)
                    .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                
                return result;
                
            }
            catch (Exception ex)
            {
                var mess = ex.InnerException.Message;

                throw;
            }




        }

        public async Task<Wsmy639> CreaAprobacionAprobada(string cotizacion, int renglon, int propuesta, string usuarioConectado)
        {

            var cotizacionP = new SqlParameter("@Cotizacion", cotizacion);
            var renglonP = new SqlParameter("@Renglon", renglon);
            var propuestaP = new SqlParameter("@Propuesta", propuesta);
            var usuarioConectadoP = new SqlParameter("@UsuarioConectado", usuarioConectado);
            try
            {
                //var result = await _context.da("exec AppCreaWsmy639Aprobado @Cotizacion,@Renglon,@Propuesta,@UsuarioConectado", cotizacionP, renglonP, propuestaP, usuarioConectadoP).ToListAsync();
                FormattableString xqueryDiario = $"exec AppCreaWsmy639Aprobado {cotizacion},{renglon},{propuesta},{usuarioConectado}";
                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

                var result = await _context.Wsmy639
                    .Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta)
                    .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                
                return result;
            }
            catch (Exception ex)
            {
                var mess = ex.InnerException.Message;

                throw;
            }




        }


        public async Task Add(Wsmy639 entity)
        {


            try
            {
                if (entity.Id == 0)
                {
                    entity.Id = await NextId();
                }

                await _context.Wsmy639.AddAsync(entity);
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;

            }



        }



        public void Update(Wsmy639 entity)
        {
            _context.Wsmy639.Update(entity).Property(x => x.RecNum).IsModified = false; ;

        }

        public async Task Delete(long id)
        {
            Wsmy639 entity = await GetById(id);
            if (entity == null) return;
            var latestStation = await _context.Wsmy647.Where(x => x.IdCalculo == id)
                .OrderByDescending(x => x.FechaEntrada).ThenByDescending(x => x.Recnum).FirstOrDefaultAsync();
            // REQ-132: returning to draft must retain a rejection for the consultant.
            // A new cycle is created only by the explicit resubmission use case.
            if (entity.IdEstatus == "RECH" ||
                (latestStation != null && latestStation.IdEstatus == "RECH" && latestStation.FechaSalida.HasValue)) return;
            _context.Wsmy639.Remove(entity);

        }

        public async Task<long> NextId()
        {
            var next = new SqlParameter("@NextId", System.Data.SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output };
            var message = new SqlParameter("@Message", System.Data.SqlDbType.NVarChar, 4000) { Direction = System.Data.ParameterDirection.Output };
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.PriceApproval_AllocateRequestId @NextId OUTPUT,@Message OUTPUT", next, message);
            if (message.Value?.ToString() != "Success" || next.Value == DBNull.Value)
                throw new InvalidOperationException(message.Value?.ToString() ?? "No fue posible reservar identidad de solicitud.");
            return Convert.ToInt64(next.Value);


        }


    }
}
