using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.EntitiesMaestros;

namespace AppService.Core.Interfaces;

public interface IEmailRepository
{
    Task<List<Email>> GetAll();
    Task<List<Email>> GetAllByIdFile(string idFile);
    Task<Email> GetById(decimal id);
    Task Delete(decimal id);
    Task LimpiaEmailInvalidos();
}