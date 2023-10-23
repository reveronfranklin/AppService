using System;
using AppService.Core.EntitiesSap;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
	public interface ISapEntregasFiscalesRepository
	{

        Task<Z91EntfiscDoc> GetByDocumento(string documento);

    }
}

