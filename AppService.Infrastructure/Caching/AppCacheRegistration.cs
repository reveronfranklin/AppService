using System;
using System.Threading.Tasks;
using AppService.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AppService.Infrastructure.Caching
{
    public static class AppCacheRegistration
    {
        public static IServiceCollection AddAppCache(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["AppCache:Provider"] ?? "Memory";
            if (provider.Equals("Memory", StringComparison.OrdinalIgnoreCase))
            {
                var sizeMb = configuration.GetValue<long?>("AppCache:MemoryLimitMB") ?? 128;
                if (sizeMb <= 0 || sizeMb > long.MaxValue / (1024 * 1024))
                    throw new InvalidOperationException("AppCache:MemoryLimitMB debe ser positivo y valido.");
                services.AddDistributedMemoryCache(options => options.SizeLimit = sizeMb * 1024 * 1024);
                services.AddSingleton<IAppCache, MemoryAppCache>();
            }
            else if (provider.Equals("Redis", StringComparison.OrdinalIgnoreCase))
            {
                var connection = configuration.GetConnectionString("redisConnection");
                if (string.IsNullOrWhiteSpace(connection))
                    throw new InvalidOperationException("Configure ConnectionStrings:redisConnection para AppCache:Provider=Redis.");
                services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connection));
                services.AddSingleton<IAppCache, RedisAppCache>();
            }
            else throw new InvalidOperationException("AppCache:Provider debe ser Memory o Redis.");
            return services;
        }
    }

    // MemoryDistributedCache owns its memory store; its size is measured in payload bytes.
    public sealed class MemoryAppCache : IAppCache
    {
        private readonly IDistributedCache cache;
        public MemoryAppCache(IDistributedCache cache) { this.cache = cache; }
        public Task<string> GetAsync(string key) => cache.GetStringAsync(key);
        public Task SetAsync(string key, string value, TimeSpan lifetime) => cache.SetStringAsync(key, value,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = lifetime });
        public void Remove(string key) => cache.Remove(key);
    }

    // Keep the existing Redis string format and keys to allow rollback without migration.
    public sealed class RedisAppCache : IAppCache
    {
        private readonly IConnectionMultiplexer connection;
        public RedisAppCache(IConnectionMultiplexer connection) { this.connection = connection; }
        public async Task<string> GetAsync(string key) => await connection.GetDatabase().StringGetAsync(key);
        public async Task SetAsync(string key, string value, TimeSpan lifetime) =>
            await connection.GetDatabase().StringSetAsync(key, value, lifetime);
        public void Remove(string key) => connection.GetDatabase().KeyDelete(key);
    }
}
