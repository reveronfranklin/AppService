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
using static iText.IO.Util.IntHashtable;

namespace AppService.Infrastructure.Repositories
{
    public class AppCostosEmbaqueRepository: IAppCostosEmbaqueRepository
    {
        private readonly RRDContext _context;

        public AppCostosEmbaqueRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<List<AppCostosEmbarque>> GetAll()
        {

            return await _context.AppCostosEmbarques.ToListAsync();

        }

        public async Task<List<AppCostosEmbarque>> GetAllByDocumento(int documento)
        {

            return await _context.AppCostosEmbarques.Where(x=>x.Documento== documento).ToListAsync();

        }



        public async Task<AppCostosEmbarque> GetById(decimal id)
        {
            return await _context.AppCostosEmbarques.FindAsync(id);
        }
      
        
        public async Task Add(AppCostosEmbarque entity)
        {
            try
            {
                await _context.AppCostosEmbarques.AddAsync(entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
          
          

        }



        public void Update(AppCostosEmbarque entity)
        {
            _context.AppCostosEmbarques.Update(entity);

        }

        public async Task Delete(decimal id)
        {
            AppCostosEmbarque entity = await GetById(id);
            _context.AppCostosEmbarques.Remove(entity);

        }
        public async Task DeleteRange(int ano,int mes,string tipoDocumento="FA")
        {
           
            var embaques = await _context.AppCostosEmbarques.Where(x=>x.Ano==ano && x.Mes==mes && x.TipoDocumento== tipoDocumento).ToListAsync();
            if (embaques.Count > 0)
            {
                _context.AppCostosEmbarques.RemoveRange(embaques);
            }
           

        }

    }
}
