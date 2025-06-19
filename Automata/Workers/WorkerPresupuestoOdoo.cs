using AppService.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Automata.Workers
{
    public class WorkerPresupuestoOdoo : BackgroundService
    {
        private readonly ICotizacionService _cotizacionService;
        private readonly IAppDocumentosFiscalesService _service;
        private readonly IAppCostosEmbarqueService _appCostosEmbaqueService;
        private readonly IEmaiService _emaiService;

        public WorkerPresupuestoOdoo(ICotizacionService cotizacionService, 
                                     IAppDocumentosFiscalesService service,
                                     IAppCostosEmbarqueService appCostosEmbaqueService,
                                     IEmaiService emaiService)
        {
            _cotizacionService = cotizacionService;
            _service = service;
            _appCostosEmbaqueService = appCostosEmbaqueService;
            _emaiService = emaiService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
              /* int millisecondsDelay = 30 * 60000; //Convertimos 30 minutos a Milisegundos
                Console.WriteLine("Ejecutando worker cada 30 minuto");
                var mensaje = "Iniciando Generacion de costos de embarque: " + DateTime.Now;

                Console.WriteLine(mensaje);
             

                await _appCostosEmbaqueService.CrearCostosDelMes(2023,9);
                Console.WriteLine("Culminado Generacion de costos de embarque: " + DateTime.Now);
                await Task.Delay(millisecondsDelay);*/


                /*Console.WriteLine("Ejecutando worker cada 7 minuto");
                var mensaje = "Iniciando envio de Documentos Fiscales a oficina Digital: " + DateTime.Now;

                Console.WriteLine(mensaje);
                var src = "";

                var response = await _service.ReadLineByLineTextPdf(src);
                Console.WriteLine("Culminado envio de documentos fiscales a oficina digital: " + DateTime.Now);
                await Task.Delay(420000);*/


                
                Console.WriteLine("Ejecutando worker cada 5 minuto");
                var mensaje = "Iniciando envio de Emails: " + DateTime.Now;

                Console.WriteLine(mensaje);
                var src = "";

                 await _emaiService.Send();
                Console.WriteLine("Culminado envio de Emails: " + DateTime.Now);
                await Task.Delay(300000);
                
                
                

                /*Console.WriteLine("Ejecutando worker cada 10 minuto");
                var mensaje = "Iniciando envio de Clientes a Odoo: " + DateTime.Now;
                Console.WriteLine(mensaje);
                await _cotizacionService.ActualizarClientes();
                Console.WriteLine("Culminado envio de Clientes a Odoo: " + DateTime.Now) ;
                await Task.Delay(600000);*/


                /*Console.WriteLine("Ejecutando worker cada 10 minuto");

                var mensaje = "Iniciando envio de cotizaciones a Odoo: " + DateTime.Now;

                Console.WriteLine(mensaje);

                await _cotizacionService.UpdateCotizacionesToOdoo();
                mensaje = "Culminado envio de cotizaciones a Odoo: " + DateTime.Now;
               Console.WriteLine(mensaje);
               await Task.Delay(600000);*/



               /*Console.WriteLine("Ejecutando worker cada 7 Segundos");
                var mensaje = "Iniciando integracion de cotizaciones: " + DateTime.Now;

               Console.WriteLine(mensaje);
               var src = "";

               await _cotizacionService.IntegrarCotizaciones();
               Console.WriteLine("Culminado integracion de cotizaciones: " + DateTime.Now); 
               await Task.Delay(5000);*/
               






            }
        }



    }
}
