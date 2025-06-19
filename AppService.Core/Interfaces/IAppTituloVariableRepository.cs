using System;
using AppService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
  

    public interface IAppTituloVariableRepository
	{

        Task<List<AppTituloVariable>> GetAll();
        Task<List<AppTituloVariable>> GetAllBySubCategoria(int subcategoria);
        Task<AppTituloVariable> GetById(int id);
        Task Add(AppTituloVariable entity);
        void Update(AppTituloVariable entity);
        Task Delete(int id);


    }
}

