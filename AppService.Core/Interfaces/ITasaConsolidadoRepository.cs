using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces;

public interface ITasaConsolidadoRepository
{
    Task<List<TasaConsolidada>> GetAll();
    Task<string> UpdateTasaConsolidada(TasasConsolidadasUpdateDto tasas);
    Task<string> DeleteTasaConsolidada(TasasConsolidadasUpdateDto tasas);
}