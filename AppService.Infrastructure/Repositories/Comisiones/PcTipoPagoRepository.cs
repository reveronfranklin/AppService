

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using AppService.Infrastructure.DataMooreve;

namespace AppService.Infrastructure.Repositories.Comisiones
{
    public class PcTipoPagoRepository: IPcTipoPagoRepository
    {
        private readonly MooreveContext _context;


        public PcTipoPagoRepository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<List<PCTipoPago>> GetAll()
        {

            return await _context.PCTipoPago.ToListAsync();

        }
        public async Task<List<PCTipoPago>> GetAllManuales()
        {
            try
            {
                var tipos= await _context.PCTipoPago.Where(x=>x.FlagCalcular==false).ToListAsync();
                return tipos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
          
        }

    }
}
