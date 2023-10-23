using AppService.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Automata.Workers
{
    public class WorkerPresupuestoOdoo : BackgroundService
    {
        private readonly ICotizacionService _cotizacionService;
        private readonly IAppDocumentosFiscalesService _service;
        private readonly IAppCostosEmbarqueService _appCostosEmbaqueService;
        public WorkerPresupuestoOdoo(ICotizacionService cotizacionService, 
                                     IAppDocumentosFiscalesService service,
                                     IAppCostosEmbarqueService appCostosEmbaqueService)
        {
            _cotizacionService = cotizacionService;
            _service = service;
            _appCostosEmbaqueService = appCostosEmbaqueService;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               int millisecondsDelay = 30 * 60000; //Convertimos 30 minutos a Milisegundos
                Console.WriteLine("Ejecutando worker cada 30 minuto");
                var mensaje = "Iniciando Generacion de costos de embarque: " + DateTime.Now;

                Console.WriteLine(mensaje);
             

                await _appCostosEmbaqueService.CrearCostosDelMes(2023,9);
                Console.WriteLine("Culminado Generacion de costos de embarque: " + DateTime.Now);
                await Task.Delay(millisecondsDelay);


              /*Console.WriteLine("Ejecutando worker cada 1 minuto");
                var mensaje = "Iniciando envio de Documentos Fiscales a oficina Digital: " + DateTime.Now;

                Console.WriteLine(mensaje);
                var src = "";

                var response = await _service.ReadLineByLineTextPdf(src);
              Console.WriteLine("Culminado envio de documentos fiscales a oficina digital: " + DateTime.Now);
                await Task.Delay(60000);*/



               /* Console.WriteLine("Ejecutando worker cada 10 minuto");
                var mensaje = "Iniciando envio de Clientes a Odoo: " + DateTime.Now;

                Console.WriteLine(mensaje);
                await _cotizacionService.ActualizarClientes();
                Console.WriteLine("Culminado envio de Clientes a Odoo: " + DateTime.Now) ;
                await Task.Delay(600000);*/


                /* Console.WriteLine("Ejecutando worker cada 10 minuto");

                 var mensaje = "Iniciando envio de cotizaciones a Odoo: " + DateTime.Now;

                 Console.WriteLine(mensaje);

                 await _cotizacionService.UpdateCotizacionesToOdoo();
                 mensaje = "Culminado envio de cotizaciones a Odoo: " + DateTime.Now;
                Console.WriteLine(mensaje);
                await Task.Delay(600000);*/









            }
        }



    }
}
