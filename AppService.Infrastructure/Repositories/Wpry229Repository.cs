using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Wpry229Repository: IWpry229Repository
    {

        private readonly MooreveContext _context;

        public Wpry229Repository(MooreveContext context)
        {
            _context = context;
        }


        public async Task<List<Wpry229>> GetAll()
        {

            return await _context.Wpry229.ToListAsync();

        }

        public async Task<Wpry229> GetById(int id)
        {

            return await _context.Wpry229.FindAsync(id);
        }

        public async Task<Wpry229> GetByCotizacionRenglonPropuesta(string cotizacion ,int renglon, int propuesta)
        {

            return await _context.Wpry229.Where(x=>x.Cotizacion== cotizacion && x.Renglon==renglon && x.Propuesta==propuesta).FirstOrDefaultAsync();
        }

        public async Task<Wpry229> DeleteByCotizacionRenglonPropuesta(string cotizacion ,int renglon, int propuesta)
        {

            return await _context.Wpry229.Where(x=>x.Cotizacion== cotizacion && x.Renglon==renglon && x.Propuesta==propuesta).FirstOrDefaultAsync();
        }
        
        public async Task<List<Wpry229>> GetByCotizacion(string cotizacion)
        {

            return await _context.Wpry229.Where(x => x.Cotizacion == cotizacion ).ToListAsync();
        }

        public async Task Add(Wpry229 entity)
        {
            try
            {
                await _context.Wpry229.AddAsync(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            


        }



        public void Update(Wpry229 entity)
        {
            try
            {
                _context.Wpry229.Update(entity);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task Delete(string cotizacion ,int renglon, int propuesta)
        {
            try
            {
                Wpry229 entity = await GetByCotizacionRenglonPropuesta( cotizacion , renglon,  propuesta);
                if (entity != null)
                {
                    _context.Wpry229.Remove(entity);
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         

        }


       


    }
}
