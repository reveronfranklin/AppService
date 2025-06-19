using System;
using System.Threading.Tasks;
using AppService.Core.EntitiesFacturacion;

namespace AppService.Core.Interfaces
{
	public interface ICiny056Repository
	{

        Task<Ciny056> GetByNumero(int factura);


    }
}

