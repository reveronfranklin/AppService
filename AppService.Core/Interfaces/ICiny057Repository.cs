using AppService.Core.EntitiesFacturacion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface ICiny057Repository
    {

        Task<Ciny057> GetByNumero(int factura);
        Task<Cary028> GetNotaCreditoByNumero(int nota);
        Task<List<Cary029>> GetNotasCreditoDetalleByPeriodo(int año, int mes);
        Task<List<Ciny057>> GetByPeriodo(int año, int mes);
        Task<Ciny208> GetNotaEntregaByDocumentoSap(decimal documentoSap);
        Task<Ciny208> GetNotaEntregaByRemision(int remision);
    }
}
