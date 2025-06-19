using AppService.Core.EntitiesMooreve;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
   public interface IWsmy686Repository
    {


        Task<List<Wsmy686>> GetAll();
        Task<Wsmy686> GetById(decimal id);
        Task Add(Wsmy686 entity);
        void Update(Wsmy686 entity);
        Task Delete(decimal id);



    }
}
