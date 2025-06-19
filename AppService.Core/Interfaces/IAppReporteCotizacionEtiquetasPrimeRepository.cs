using System;
using AppService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
	public interface IAppReporteCotizacionEtiquetasPrimeRepository
	{

        Task<List<AppReporteCotizacionEtiquetasPrime>> GetAll();
        Task<AppReporteCotizacionEtiquetasPrime> GetById(int id);
        Task<AppReporteCotizacionEtiquetasPrime> GetByCotizacion(string cotizacion);
        Task Add(AppReporteCotizacionEtiquetasPrime entity);
        void Update(AppReporteCotizacionEtiquetasPrime entity);
        Task Delete(int id);


    }
}

