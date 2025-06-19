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

namespace AppService.Infrastructure.Repositories
{
    public class AppReporteCotizacionEtiquetasPrimeRepository : IAppReporteCotizacionEtiquetasPrimeRepository
    {
        private readonly RRDContext _context;

        public AppReporteCotizacionEtiquetasPrimeRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<List<AppReporteCotizacionEtiquetasPrime>> GetAll()
        {

            return await _context.AppReporteCotizacionEtiquetasPrimes.ToListAsync();

        }
     

     

        public async Task<AppReporteCotizacionEtiquetasPrime> GetById(int id)
        {
            return await _context.AppReporteCotizacionEtiquetasPrimes.FindAsync(id);
        }

        public async Task<AppReporteCotizacionEtiquetasPrime> GetByCotizacion(string cotizacion)
        {
            return await _context.AppReporteCotizacionEtiquetasPrimes.Where(x=> x.Cotizacion == cotizacion).FirstOrDefaultAsync();
        }
        
        public async Task Add(AppReporteCotizacionEtiquetasPrime entity)
        {
            await _context.AppReporteCotizacionEtiquetasPrimes.AddAsync(entity);


        }



        public void Update(AppReporteCotizacionEtiquetasPrime entity)
        {
            _context.AppReporteCotizacionEtiquetasPrimes.Update(entity);

        }

        public async Task Delete(int id)
        {
            AppReporteCotizacionEtiquetasPrime entity = await GetById(id);
            _context.AppReporteCotizacionEtiquetasPrimes.Remove(entity);

        }

    }
}
