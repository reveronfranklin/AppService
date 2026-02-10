using AppService.Core.EntitiesPlanta;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataPlanta;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Csmy021Repository : ICsmy021Repository
    {



        private readonly PlantaContext _context;

        public Csmy021Repository(PlantaContext context)
        {
            _context = context;
        }


        public async Task<List<Csmy021>> GetByOrdenAsync(long orden)
        {
            return await _context.Csmy021.Where(x => x.Orden == orden).ToListAsync();
        }
        public async Task<Csmy021> GetByOrdenParteAsync(long orden,int parte)
        {
            return await _context.Csmy021.Where(x => x.Orden == orden && x.NoPartePapel ==parte).FirstOrDefaultAsync();
        }


        public async Task UpdateMedidasPapel(Csmy021 csmy021)
        {
            FormattableString xqueryDiario = $"";
    
            xqueryDiario =
                $"update Csmy021 set COD_PAPEL={csmy021.CodPapel},MEDIDA_PAPEL={csmy021.MedidaPapel},PESO_PAPEL={csmy021.PesoPapel} where Orden={csmy021.Orden} and NO_PARTE_PAPEL={csmy021.NoPartePapel}";


            try
            {

                _context.Database.ExecuteSqlInterpolated(xqueryDiario);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(xqueryDiario);
                throw;
            }
        }
       




    }


}

