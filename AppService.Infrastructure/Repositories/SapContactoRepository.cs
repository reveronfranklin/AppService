using AppService.Core.EntitiesSap;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataSap;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class SapEntregasFiscalesRepository : ISapEntregasFiscalesRepository
    {

        private readonly SapContext _context;

        public SapEntregasFiscalesRepository(SapContext context)
        {
            _context = context;
        }

       


        public async Task<Z91EntfiscDoc> GetByDocumento(string documento)
        {
            return await _context.Z91EntfiscDoc.Where(x => x.Vbeln == documento).FirstOrDefaultAsync();
        }

      






    }
}
