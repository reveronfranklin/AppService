using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Infrastructure.DataMooreve;

namespace AppService.Infrastructure.Repositories
{
    public class AppAdjuntosCotizacionRepository:IAppAdjuntosCotizacionRepository
    {
        private readonly MooreveContext _context;


        public AppAdjuntosCotizacionRepository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<List<AppAdjuntosCotizacion>> GetByCotizacion(string cotizacion,int renglon)
        {
            
            FormattableString xqueryDiario = $"exec App_ViewAdjuntosCotizacion {cotizacion},{renglon}";
            var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            
            var result= await _context.AppAdjuntosCotizacion.Where(x=>x.Cotizacion==cotizacion).ToListAsync();

            return result;

        }
        
    
    


    }
}
