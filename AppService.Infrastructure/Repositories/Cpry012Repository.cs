using AppService.Core.Entities;
using AppService.Core.EntitiesPlanta;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataPlanta;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Cpry012Repository : ICpry012Repository
    {

        private readonly PlantaContext _context;

        public Cpry012Repository(PlantaContext context)
        {
            _context = context;
        }


        public async Task<Core.EntitiesPlanta.Cpry012> GetByOrdenAsync(long orden)
        {
            return await _context.Cpry012.Where(x => x.Orden == orden).FirstOrDefaultAsync();
        }
      
        public void Update(Core.EntitiesPlanta.Cpry012 entity)
        {
            _context.Cpry012.Update(entity);


        }
        public void UpdateProductoEnOrden(Core.EntitiesPlanta.Cpry012 entity)
        {

            FormattableString xqueryDiario = $"update cpry012 set AppproductsId={entity.AppproductsId} where Orden={entity.Orden}";

            var result = _context.Database.ExecuteSqlInterpolated(xqueryDiario);

         

        }
    }
}
