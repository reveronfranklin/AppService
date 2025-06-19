

using AppService.Core.EntitiesFacturacion;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataFacturacion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Ciny057Repository : ICiny057Repository
    {

        private readonly FacturacionContext _context;

        public Ciny057Repository(FacturacionContext context)
        {
            _context = context;
        }


        public async Task<Ciny057> GetByNumero(int factura)
        {
           
            try
            {
               var result = _context.Ciny057.Where(x => x.Factura == factura).FirstOrDefaultAsync().GetAwaiter().GetResult();
                //var result = await _context.Ciny057.Where(x => x.Factura == factura).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }



        }

        public async Task<List<Ciny057>> GetByPeriodo(int año, int mes)
        {

            try
            {
                var result = await _context.Ciny057.Where(x => x.Año == año && x.Mes==mes).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }



        }
        public async Task<List<Cary029>> GetNotasCreditoDetalleByPeriodo(int año, int mes)
        {

            try
            {
                var result = await _context.Cary029.Where(x => x.Año == año && x.Mes == mes).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }



        }

        public async Task<Cary028> GetNotaCreditoByNumero(int nota)
        {

            try
            {
                var result = await _context.Cary028.Where(x => x.NotaCredito == nota).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }



        }

        public async Task<Ciny208> GetNotaEntregaByDocumentoSap(decimal documentoSap)
        {

            try
            {
                var result = await _context.Ciny208.Where(x => x.DocumentoSap == documentoSap).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }



        }

        public async Task<Ciny208> GetNotaEntregaByRemision(int remision)
        {

            try
            {
                var result = await _context.Ciny208.Where(x => x.Remision == remision).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }



        }

    
    }
}
