using System;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IAppCache
    {
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value, TimeSpan lifetime);
        void Remove(string key);
    }
}
