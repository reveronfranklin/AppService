using System;
using AppService.Core.Entities;
using System.Threading.Tasks;
using AppService.Infrastructure.Data;
using System.Linq;
using AppService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppService.Infrastructure.Repositories
{
	public class OfdAdjuntoCriterioRepository : IOfdAdjuntoCriterioRepository
    {
	
        private readonly RRDContext _context;

        public OfdAdjuntoCriterioRepository(RRDContext context)
        {
            _context = context;
        }

       
        public async Task<OfdAdjuntoCriterio> Add(OfdAdjuntoCriterio entity)
        {
            await _context.OfdAdjuntoCriterio.AddAsync(entity);

            return entity;
        }

        public async Task<OfdAdjuntoCriterio> GetById(long idAdjunto)
        {
            return await _context.OfdAdjuntoCriterio.Where(x => x.IdAdjuntoCriterio == idAdjunto).FirstOrDefaultAsync();

        }

        public async Task<bool> DeleteGyIdAdjunto(long idAdjunto)
        {

            var criterio = await _context.OfdAdjuntoCriterio.Where(x => x.IdAdjunto == idAdjunto).ToListAsync();
            if (criterio.Count > 0)
            {
                _context.OfdAdjuntoCriterio.RemoveRange(criterio);
                return true;
            }
            else
            {
                return false;
            }
           


        }



    }
}

