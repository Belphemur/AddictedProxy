using System;
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace AddictedProxy.Services.Caching
{
    public class CachingService : ICachingService
    {
        private readonly MemoryCache _memoryCache;

        public CachingService(MemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get or set in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objBuilder"></param>
        /// <param name="expiry">Does this entry expire ?</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSet<T>(string key, Func<T> objBuilder, TimeSpan? expiry)
        {
            var inCache = _memoryCache.Get(key);
            if (inCache != null)
            {
                return (T) inCache;
            }

            var toCache = objBuilder();
            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now + (expiry ?? TimeSpan.Zero),
                RemovedCallback = async arguments =>
                {
                    switch (arguments.CacheItem.Value)
                    {
                        case IDisposable disposable:
                            disposable.Dispose();
                            break;
                        case IAsyncDisposable disposable:
                            await disposable.DisposeAsync();
                            break;
                    }
                }
            };
            var cacheEntry = new CacheItem(key, toCache);
            _memoryCache.Add(cacheEntry, cachePolicy);
            return toCache;
        }
    }
}