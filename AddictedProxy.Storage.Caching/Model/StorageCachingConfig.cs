using AddictedProxy.Caching.Model;

namespace AddictedProxy.Storage.Caching.Model;

public class StorageCachingConfig : CachingConfig
{
    public enum Type
    {
        Distributed,
        Sqlite
    }

    public Type StorageType { get; set; }
    
    public string? CachePath { get; set; }
}