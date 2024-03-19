
using AppService.Core.Interfaces;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.DataMooreve;

namespace AppService.Infrastructure.Repositories
{
    public class VCotizacionesPorAprobarRepository : IVCotizacionesPorAprobarRepository
    {

        private readonly MooreveContext _context;

        public VCotizacionesPorAprobarRepository(MooreveContext context)
        {
            _context = context;
        }


      
        
        public async Task<List<V_CotizacionesPorAprobar>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter)
        {

            if (filter.PageNumber == 0) filter.PageNumber = 1;
            if (filter.PageSize == 0) filter.PageSize = 100;
            
            List<V_CotizacionesPorAprobar> result = new List<V_CotizacionesPorAprobar>();
            try
            {
                if (filter.SearchText != null && filter.SearchText.Length > 0)
                {
                    var allData =  await _context.V_CotizacionesPorAprobar.OrderBy(x => x.Fecha).ToListAsync();
                    
                    
                    result = allData
              
                        .Where(x =>  x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .OrderBy(x => x.Fecha).Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToList();
                }
                else
                {
                    result =  await _context.V_CotizacionesPorAprobar.OrderBy(x => x.Fecha).ToListAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         
         
            
            return result;
        }
      
       
     
    }
}
