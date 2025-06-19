using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.DTOs.PowerBi;
using AppService.Core.EntitiesPowerBI;
using AppService.Core.Interfaces.PowerBi;
using AppService.Infrastructure.DataPowerBI;

namespace AppService.Infrastructure.Repositories
{
    public class PowerBiOrdenesRepository: IPowerBiOrdenesRepository
    {
        private readonly POWERBIContext _context;

        public PowerBiOrdenesRepository(POWERBIContext context)
        {
            _context = context;
        }

        public async Task<List<Ordene>> GetAll(string usuario,bool esVendedor,int year)
        {

            List<Ordene> result = new List<Ordene>();
            if (esVendedor)
            {
                result=await _context.Ordenes.Where(x=>x.CodigoVendedor==usuario && x.AloContable>=year).ToListAsync();
            }
            else
            {
                result = await _context.Ordenes
                    .Where(x=> x.AloContable>=year).ToListAsync();
               
            }

            return result;

        }
        
        
        public async Task<List<VVenta>> GetAllVentas(decimal year)
        {

            List<VVenta> result = new List<VVenta>();
          
                result=await _context.VVentas.Where(x=>x.AloContable>=(decimal)year).ToListAsync();
           

            return result;

        }
        
        public async Task<List<AppService.Core.EntitiesPowerBI.Ventas>> GetAllVentasReducido(decimal year)
        {

            List<AppService.Core.EntitiesPowerBI.Ventas> result = new List<AppService.Core.EntitiesPowerBI.Ventas>();
          
            result=await _context.Ventas.Where(x=>x.AloContable>=(decimal)year).ToListAsync();
           

            return result;

        }
       
     




    }
}
