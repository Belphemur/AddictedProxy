using System;

namespace AddictedProxy.Services.Caching
{
    public interface ICachingService
    {
        /// <summary>
        /// Get or set in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objBuilder"></param>
        /// <param name="expiry">Does this entry expire ?</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSet<T>(string key, Func<T> objBuilder, TimeSpan? expiry);
    }
}