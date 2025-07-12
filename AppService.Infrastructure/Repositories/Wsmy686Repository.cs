using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Wsmy686Repository: IWsmy686Repository
    {

        private readonly MooreveContext _context;

        public Wsmy686Repository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<List<Wsmy686>> GetAll()
        {

            return await _context.Wsmy686.ToListAsync();

        }
    
        public async Task<Wsmy686> GetById(decimal id) { 
  
            return await _context.Wsmy686.FindAsync(id);
        }

        public async Task Add(Wsmy686 entity)
        {
            await _context.Wsmy686.AddAsync(entity);


        }



        public void Update(Wsmy686 entity)
        {
            _context.Wsmy686.Update(entity);

        }

        public async Task Delete(decimal id)
        {
            Wsmy686 entity = await GetById(id);
            _context.Wsmy686.Remove(entity);

        }


    }
}
