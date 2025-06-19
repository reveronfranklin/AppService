using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.EntitiesMaestros;
using AppService.Infrastructure.DataMaestros;

namespace AppService.Infrastructure.Repositories
{
    public class EmailRepository:IEmailRepository
    {
        private readonly IMaestrosContext _context;

        public EmailRepository(IMaestrosContext context)
        {
            _context = context;
        }

        public async Task<List<Email>> GetAll()
        {

            return await _context.Email.ToListAsync();

        }
        public async Task<List<Email>> GetAllByIdFile(string idFile)
        {

            return await _context.Email.Where(x=>x.IdFile==idFile).ToListAsync();

        }
        
     
     
        public async Task<Email> GetById(decimal id)
        {
            return await _context.Email.Where(x=>x.Id==id).FirstOrDefaultAsync();
        }

        public async Task LimpiaEmailInvalidos()
        {
            FormattableString query = $"exec [Pa_LimpiaEmailInvalidos]";
            try
            {
                _context.Database.ExecuteSqlInterpolated(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(query);
                throw;
            }
        }
        
        public async Task Delete(decimal id)
        {
            
            FormattableString query = $"Pa_CopiaEmailToEmailHistorico {id}";
            try
            {
                _context.Database.ExecuteSqlInterpolated(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(query);
                throw;
            }

          
        }
      
        
     


    }
}
