using AsyncKeyedLock;

namespace Locking
{
    public static class LockOptions
    {
        private readonly static AsyncKeyedLockOptions defaultOptions = new()
        {
            PoolSize = 20,
            PoolInitialFill = 1
        };

        public static AsyncKeyedLockOptions Default => defaultOptions;
    }
}
