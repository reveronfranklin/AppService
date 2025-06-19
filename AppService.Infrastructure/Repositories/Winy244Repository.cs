
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.DataFacturacion;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Winy244Repository : IWiny244Repository
    {

        private readonly MooreveContext _context;

        public Winy244Repository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<Winy244> GetUltimoPeriodo()
        {
           var winy244= await _context.Winy244.OrderByDescending(x=>x.AñoContable).ThenByDescending(x=>x.MesContable).FirstOrDefaultAsync();
            return winy244;
        }

 
  

    }
}
