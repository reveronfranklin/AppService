using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.AppCalculadora;
using AppService.Core.Entities;

namespace AppService.Core.Interfaces;

public interface IAppCalculadoraRepository
{
    Task<List<AppCalculadora>> GetByVendedor(AppCalculadoraFilterDto filter);
    Task<string> Create(AppCalculadoraUpdateDto dto);
    Task<string> Update(AppCalculadoraUpdateDto dto);

    Task<string> Delete(int id);
    Task<AppCalculadora> GetById(int id);
}