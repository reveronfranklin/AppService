
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;

namespace AppService.Infrastructure.Repositories.Comisiones
{
    public class PcProcesoRepository: IPcProcesoRepository
    {
        private readonly MooreveContext _context;


        public PcProcesoRepository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<List<PcProceso>> GetAll()
        {

            return await _context.PcProceso.ToListAsync();

        }

      
        public async Task<PcProceso> GetById(long id)
        {

            try
            {
                return await _context.PcProceso.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                return null;
            }
            return await _context.PcProceso.FindAsync(id);
        }
        
        public async Task Add(PcProceso entity)
        {
            await _context.PcProceso.AddAsync(entity);


        }

        public void Update(PcProceso entity)
        {
            _context.PcProceso.Update(entity);

        }

        public async Task Delete(long id)
        {
            PcProceso entity = await GetById(id);
            _context.PcProceso.Remove(entity);

        }
        
      
    }
}
