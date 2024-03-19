using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;

namespace AppService.Core.Services;

public class VCotizacionesPorAprobarService:IVCotizacionesPorAprobarService
{
    private readonly IUnitOfWork _unitOfWork;
    public VCotizacionesPorAprobarService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<List<V_CotizacionesPorAprobar>> GetAllBySearchText(AppGeneralQuotesQueryFilter filter)
    {
       var cotizacionesPorAprobar= await _unitOfWork.VCotizacionesPorAprobarRepository.GetAllBySearchText(filter);
       return cotizacionesPorAprobar;
    }
}