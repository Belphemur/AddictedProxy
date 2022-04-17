﻿using KeyedSemaphores;

namespace Locking;

public static class Lock<T>
{
    private class Container : ILockContainer
    {
        private readonly IKeyedSemaphore _semaphore;

        public Container(IKeyedSemaphore semaphore)
        {
            _semaphore = semaphore;
        }

        /// <summary>
        /// Wait for getting the lock
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="token"></param>
        /// <returns>True if got the lock before <see cref="timeSpan"/> expires</returns>
        public Task<bool> WaitAsync(TimeSpan timeSpan, CancellationToken token)
        {
            return _semaphore.WaitAsync(timeSpan, token);
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }

    /// <summary>
    /// Get a named lock
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ILockContainer GetNamedLock(string name)
    {
        return new Container(KeyedSemaphore.GetOrCreate(name));
    }

    private static string GetLockKey(string key)
    {
        return $"{typeof(T).Name}_{key}";
    }
}