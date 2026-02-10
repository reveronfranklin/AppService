using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace AppService.Infrastructure.Repositories
{
    public class AppGeneralQuotesActionSheetRepository:IAppGeneralQuotesActionSheetRepository
    {


        private readonly RRDContext _context;

        public AppGeneralQuotesActionSheetRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<AppGeneralQuotesActionSheet> GetByCotizacion(string cotizacion)
        {

            try
            {
                  var actionSheet = await _context.AppGeneralQuotesActionSheet.Where(x => x.Cotizacion == cotizacion).FirstOrDefaultAsync();

            return actionSheet;
            }
            catch (System.Exception ex)
            {
                
                 Console.WriteLine(ex.Message);
                return null;
            }
          

        }

        public async Task<AppGeneralQuotesActionSheet> CreateBk(string cotizacion)
        {

            try
            {
                  FormattableString xqueryDiario =  $"exec rrd.dbo.AppUpdateAppGeneralQuotesActionSheet '{cotizacion}'";
                    var resultDiario =_context.Database.ExecuteSqlInterpolated(xqueryDiario);
                    
                    var result= await GetByCotizacion(cotizacion);

                    return result;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        
            
        }

        public async Task<AppGeneralQuotesActionSheet> Create(string cotizacion)
{
    // Usamos el using de System.FormattableString para la interpolación segura, 
    // aunque la sintaxis $"" lo maneja implícitamente en el argumento del método.
    
    // NOTA: 'rrd.dbo.AppUpdateAppGeneralQuotesActionSheet' parece modificar datos, 
    // por lo que 'ExecuteSqlInterpolatedAsync' es correcto.

    try
    {
        // 1. Usar EXEC explícito para mayor claridad.
        // 2. Usar await y el método asíncrono 'ExecuteSqlInterpolatedAsync'.
        // 3. Usar ConfigureAwait(false) si este método no necesita retornar al contexto de sincronización.

        // El SP requiere el prefijo EXEC
        FormattableString xqueryDiario =  $"EXEC rrd.dbo.AppUpdateAppGeneralQuotesActionSheet {cotizacion}";
        
        // Ejecutar el SP de forma asíncrona
        var resultDiario = await _context.Database
                                         .ExecuteSqlInterpolatedAsync(xqueryDiario)
                                         .ConfigureAwait(false); // Recomendado para servicios

        // Obtener el resultado final de forma asíncrona
        var result = await GetByCotizacion(cotizacion).ConfigureAwait(false);

        return result;
    }
    catch (System.Exception ex)
    {
        // Es mejor registrar el error que solo imprimirlo en consola.
        Console.WriteLine($"Error al ejecutar el SP para cotización {cotizacion}: {ex.Message}");
        // Devolver una excepción si es un servicio API, o null si es la convención.
        return null;
    }
}
   
  






    }
}
