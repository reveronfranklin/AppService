using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class MtrCondicionPagoRepository:IMtrCondicionPagoRepository
    {

        private readonly RRDContext _context;

        public MtrCondicionPagoRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<List<MtrCondicionPago>> GetByIds(List<short> ids)
        {
            return await _context.MtrCondicionPago.Where(c => ids.Contains(c.Codigo)).ToListAsync();
        }
        
        public async Task<List<MtrCondicionPago>> GetAll()
        {

            return await _context.MtrCondicionPago.Where(x => x.Inactivo == "").ToListAsync();

        }

        public async Task<MtrCondicionPago> GetById(short idCondicionPago)
        {
            return await _context.MtrCondicionPago.Where(x => x.Codigo == idCondicionPago).FirstOrDefaultAsync();
        }


    }
}
