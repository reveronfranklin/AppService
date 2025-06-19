using System;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
	public interface IAppCostosEmbarqueService
	{
        Task CrearCostosDelMes(int ano, int mes, string tipoDocumento = "FA");

    }
}

