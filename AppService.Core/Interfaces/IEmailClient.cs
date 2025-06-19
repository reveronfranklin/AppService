using AppService.Core.CustomEntities;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IEmailClient
    {
        Task<Metadata> Post(StringContent data);
    }
}
