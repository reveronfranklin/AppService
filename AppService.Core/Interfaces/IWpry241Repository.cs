﻿using AppService.Core.EntitiesMooreve;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IWpry241Repository
    {


        Task<List<Wpry241>> GetAll();
        Task<bool> CotizacionTieneTintas(string cotizacion);

        Task<List<Wpry241>> GetByCotizacionRenglonPropuestaParteTinta(string cotizacion, int renglon, int propuesta, int parte, string idTinta);
        Task<List<Wpry241>> GetByCotizacionRenglonPropuestaParte(string cotizacion, int renglon, int propuesta, int parte);
        Task Add(Wpry241 entity);

        void Update(Wpry241 entity);

        Task Delete(int id);
        Task DeleteRange(List<Wpry241> entities);

        Task<List<Wpry241>> GetByCotizacion(string cotizacion);
        Task<string> GetTintas(string cotizacion);
        void CreateTintaEtiquetaDigital(string cotizacion);
    }
}
