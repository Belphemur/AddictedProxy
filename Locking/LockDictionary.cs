using System.Collections.Concurrent;

namespace Locking;

internal sealed class LockDictionary : ConcurrentDictionary<string, SemaphoreSlim>
{
    private static readonly Lazy<LockDictionary> lazy = new(Create);
    internal static LockDictionary Instance => lazy.Value;

    private LockDictionary(ConcurrentDictionary<string, SemaphoreSlim> dictionary) : base(dictionary)
    {
    }

    private static LockDictionary Create()
    {
        return new LockDictionary(new ConcurrentDictionary<string, SemaphoreSlim>());
    }
}