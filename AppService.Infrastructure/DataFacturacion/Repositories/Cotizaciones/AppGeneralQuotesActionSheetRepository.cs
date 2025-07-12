

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Shared;

using AppService.Core.Entities;
using AppService.Core.Interfaces.Cotizaciones;

using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace AppService.Infrastructure.Repositories.Cotizaciones
{
    public class AppGeneralQuotesActionSheetRepository:IAppGeneralQuotesActionSheetRepository
    {
        private readonly RRDContext _context;


        public AppGeneralQuotesActionSheetRepository(RRDContext context)
        {
            _context = context;
        }

       
        public async Task<AppGeneralQuotesActionSheet> GetByCotizacion(string cotizacion)
        {
            
            string query = $"EXEC [RRD].[dbo].[AppUpdateAppGeneralQuotesActionSheet] '{cotizacion}'";

            FormattableString xquery =

                $"EXEC [RRD].[dbo].[AppUpdateAppGeneralQuotesActionSheet] '{cotizacion}'";
             
            await _context.Database.ExecuteSqlInterpolatedAsync(xquery);
            
            var result= await _context.AppGeneralQuotesActionSheet.Where(x=> x.Cotizacion.Trim() == cotizacion.Trim() ).FirstOrDefaultAsync();
            return result;
        }
        
         
     
   
    }
}
