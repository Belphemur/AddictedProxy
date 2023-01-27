using AsyncKeyedLock;
using System.Runtime.CompilerServices;

namespace Locking;

public static class Lock<T>
{
    private static readonly AsyncKeyedLocker<string> AsyncKeyedLocker = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNamedKey(string name)
    {
        return $"{typeof(T).Name}_{name}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<IDisposable> LockAsync(string key, CancellationToken token)
    {
        return AsyncKeyedLocker.LockAsync(key, token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInUse(string key)
    {
        return AsyncKeyedLocker.IsInUse(key);
    }
}