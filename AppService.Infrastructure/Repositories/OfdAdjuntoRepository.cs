using System;
using AppService.Core.Entities;
using System.Threading.Tasks;
using AppService.Infrastructure.Data;
using System.Linq;
using AppService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppService.Infrastructure.Repositories
{
	public class OfdAdjuntoRepository: IOfdAdjuntoRepository
    {
	
        private readonly RRDContext _context;

        public OfdAdjuntoRepository(RRDContext context)
        {
            _context = context;
        }

       
        public async Task<OfdAdjunto> Add(OfdAdjunto entity)
        {
          await _context.OfdAdjunto.AddAsync(entity);
           
            return entity;
        }

        public void ActualizaOfdAdjuntoEnWpry254 (long idAdjunto )
        {
          

       
            FormattableString xqueryDiario = $"execute rrd.dbo.[PaActualizaOfdAdjuntoEnWpry254] {idAdjunto}";
            var resultDiario =  _context.Database.ExecuteSqlInterpolated(xqueryDiario);
    
        }
        
        public async Task<OfdAdjunto> GetById(long idAdjunto)
        {
            return await _context.OfdAdjunto.Where(x => x.IdAdjunto == idAdjunto).FirstOrDefaultAsync();

        }
        public async Task<bool> Delete(long idAdjunto)
        {
            var entity = await GetById(idAdjunto);
            if(entity!= null)
            {
                _context.OfdAdjunto.Remove(entity);
                return true;
            }
            else
            {
                return false;

            }
        }
        public async Task<OfdAdjunto> GetByFileName(string fileName)
        {
            OfdAdjunto result = new OfdAdjunto();
            try
            {
                result= await _context.OfdAdjunto.Where(x=> x.NombreArchivo==fileName && x.IdTipoDocumento>0).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
            
        }
        public async Task DeleteByFileName(string fileName)
        {
            var listaArchivos= await _context.OfdAdjunto.Where(x=> x.NombreArchivo==fileName).ToListAsync();
            if (listaArchivos.Count() > 0)
            {
                _context.OfdAdjunto.RemoveRange(listaArchivos);
            }
            
        
        }


    }
}

