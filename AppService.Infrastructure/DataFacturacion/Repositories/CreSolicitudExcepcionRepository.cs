using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class CreSolicitudExcepcionRepository: ICreSolicitudExcepcionRepository
    {
        private readonly RRDContext _context;

        public CreSolicitudExcepcionRepository(RRDContext context)
        {
            _context = context;
        }



        public async Task<CreSolicitudExcepcion> GetById(decimal solicitudCredito)
        {
            return await _context.CreSolicitudExcepcion.Where(x => x.SolicitudDeCredito == solicitudCredito)
                .FirstOrDefaultAsync();
        }







    }
}
