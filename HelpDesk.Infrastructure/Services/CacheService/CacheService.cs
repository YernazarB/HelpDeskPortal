using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace HelpDesk.Infrastructure.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache) 
        {
            _cache = cache;
        }

        private const int CacheLifeTimeInMinutes = 5;
        private static DistributedCacheEntryOptions _cacheOptions;
        private static DistributedCacheEntryOptions CacheEntryOptions => _cacheOptions ??
            (_cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheLifeTimeInMinutes) });

        public async Task<T> GetData<T>(string key, CancellationToken token) where T : class
        {
            var valueString = await _cache.GetStringAsync(key, token);
            if (valueString == null)
            {
                return null;
            }

            var value = JsonConvert.DeserializeObject<T>(valueString);
            return value;
        }

        public async Task RemoveData(string key, CancellationToken token)
        {
            await _cache.RemoveAsync(key, token);
        }

        public async Task SetData<T>(string key, T value, CancellationToken token) where T : class
        {
            var valueString = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, valueString, CacheEntryOptions, token);
        }
    }
}
