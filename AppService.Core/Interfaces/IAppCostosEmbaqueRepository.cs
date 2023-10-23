using System;
using AppService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
	public interface IAppCostosEmbaqueRepository
	{
        Task<List<AppCostosEmbarque>> GetAll();
        Task<AppCostosEmbarque> GetById(decimal id);
        Task Add(AppCostosEmbarque entity);
        void Update(AppCostosEmbarque entity);
        Task Delete(decimal id);
        Task DeleteRange(int ano, int mes, string tipoDocumento = "FA");
        Task<List<AppCostosEmbarque>> GetAllByDocumento(int documento);

    }
}

