using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Core.Interfaces.Comisiones;

public interface IPcProcesosService
{
    Task<DTOs.Shared.ResultDto<PcProceso>> EjecutarProceso(EjecutarProcesoDto dto);
    Task<DTOs.Shared.ResultDto<List<PcProceso>>> GetAll();
}