using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMaestros;
using AppService.Core.Responses;

namespace AppService.Core.Interfaces;

public interface IEmaiService
{
    Task<ApiResponse<List<Email>>> GetAll();
    Task Send();
}