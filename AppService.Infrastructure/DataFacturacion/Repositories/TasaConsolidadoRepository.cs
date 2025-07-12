using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.EntitiesMooreve;
using AppService.Infrastructure.DataMooreve;

namespace AppService.Infrastructure.Repositories
{
    public class TasaConsolidadoRepository: ITasaConsolidadoRepository
    {
        private readonly MooreveContext _context;

        public TasaConsolidadoRepository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<List<TasaConsolidada>> GetAll()
        {

            return await _context.TasaConsolidada.OrderByDescending(x=>x.Fecha)
                .ToListAsync();

        }

        public async Task<string> UpdateTasaConsolidada(TasasConsolidadasUpdateDto tasas)
        {
            try
            {
                var tasaBcv = tasas.TasaBcv.ToString(CultureInfo.InvariantCulture);
                var tasaParalelo = tasas.TasaParalelo.ToString(CultureInfo.InvariantCulture);
                var tasaPromedio = tasas.TasaPromedio.ToString(CultureInfo.InvariantCulture);
                
                FormattableString xqueryDiarioPasePlanta = $"exec mooreve.dbo.App_Update_Tasas {tasas.Day},{tasas.Month},{tasas.Year},{tasaBcv},{tasaParalelo},{tasaPromedio}";
                await _context.Database.ExecuteSqlInterpolatedAsync(xqueryDiarioPasePlanta);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               return e.Message;
            }
            
        
        }
        
        public async Task<string> DeleteTasaConsolidada(TasasConsolidadasUpdateDto tasas)
        {
            try
            {
             
                
                FormattableString xqueryDiarioPasePlanta = $"exec mooreve.dbo.App_Delete_Tasas {tasas.Day},{tasas.Month},{tasas.Year}";
                await _context.Database.ExecuteSqlInterpolatedAsync(xqueryDiarioPasePlanta);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
            
        
        }


    }
}
