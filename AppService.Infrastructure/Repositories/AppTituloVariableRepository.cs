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
    public class AppTituloVariableRepository: IAppTituloVariableRepository
    {
        private readonly RRDContext _context;

        public AppTituloVariableRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<List<AppTituloVariable>> GetAll()
        {

            return await _context.AppTituloVariables.ToListAsync();

        }
        public async Task<List<AppTituloVariable>> GetAllBySubCategoria(int subcategoria)
        {



            List<AppTituloVariable> result = new List<AppTituloVariable>();


            try
            {

                result = await _context.AppTituloVariables.Where(x => x.IdSubCategoria== subcategoria).ToListAsync();

              


                return result;

            }
            catch (Exception e)
            {
                var a = e.InnerException.Message;
                return result;
            }


        }

   

        public async Task<AppTituloVariable> GetById(int id)
        {
            return await _context.AppTituloVariables.FindAsync(id);
        }
      
        
        public async Task Add(AppTituloVariable entity)
        {
            await _context.AppTituloVariables.AddAsync(entity);


        }



        public void Update(AppTituloVariable entity)
        {
            _context.AppTituloVariables.Update(entity);

        }

        public async Task Delete(int id)
        {
            AppTituloVariable entity = await GetById(id);
            _context.AppTituloVariables.Remove(entity);

        }

    }
}
