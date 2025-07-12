using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Wpry254Repository : IWpry254Repository
    {


        private readonly MooreveContext _context;

        public Wpry254Repository(MooreveContext context)
        {
            _context = context;
        }


      
     

      
        public async Task<Wpry254> GetByCotizacionNombreArchivo(string cotizacion,string nombreArchivo)
        {

            var result = await _context.Wpry254.Where(x => x.Cotizacion == cotizacion && x.NombreArchivo==nombreArchivo).FirstOrDefaultAsync();
            return result;

        }


        public async Task DeleteByFileName(string fileName)
        {


            try
            {
                
                
                FormattableString xqueryDiario = $"delete from mooreve.dbo.wpry254 where  nombrearchivo={fileName}";
                var resultDiario =  _context.Database.ExecuteSqlInterpolated(xqueryDiario);
              
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                throw;
            }
        }
        public async Task Add(Wpry254 entity)
        {


            try
            {
              
                await _context.Wpry254.AddAsync(entity);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                throw;
            }
        }



        public void Update(Wpry251 entity)
        {
            _context.Wpry251.Update(entity);

        }


        public void Delete(Wpry251 entity)
        {
            _context.Wpry251.Remove(entity);

        }

        public void DeleteRange(List<Wpry251> entities)
        {
            _context.Wpry251.RemoveRange(entities);

        }
    }
}
