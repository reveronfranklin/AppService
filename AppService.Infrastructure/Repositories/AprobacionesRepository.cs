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
            return await _context.Wsmy639.Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta).FirstOrDefaultAsync();
        }
        public async Task<Wsmy639> GetByCotizacionProducto(string cotizacion, string producto)
        {
            return await _context.Wsmy639.Where(x => x.Cotizacion == cotizacion && x.IdProducto == producto ).FirstOrDefaultAsync();
        }
        
        public async Task<Wsmy639> CreaAprobacion(string cotizacion, int renglon, int propuesta, string usuarioConectado,string mensajeSolicitarPrecio)
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
                
                
                FormattableString xqueryDiario = $"exec AppCreaWsmy639 {cotizacion},{renglon},{propuesta},{usuarioConectado},{mensajeSolicitarPrecio}";
                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

                var result = await _context.Wsmy639
                    .Where(x => x.Cotizacion == cotizacion && x.Renglon == renglon && x.Propuesta == propuesta)
                    .FirstOrDefaultAsync();
                
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
                    .FirstOrDefaultAsync();
                
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
            _context.Wsmy639.Remove(entity);

        }
        public async Task DeleteWorkFlow(long id)
        {
            try
            {
                   FormattableString xqueryDiario = $"DELETE FROM WSMY647 WHERE IdCalculo = {id}";
                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            }
            catch (System.Exception e)
            {
                
               Console.WriteLine(e.Message);
            }

         


        }

        public async Task<long> NextId()
        {
            var ultimo = await _context.Wsmy639.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return ultimo.Id + 1;


        }


    }
}
