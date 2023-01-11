using AddictedProxy.Caching.Model;

namespace AddictedProxy.Storage.Caching.Model;

public record StorageCachingConfig(TimeSpan Sliding, TimeSpan Absolute) : CachingConfig(Sliding, Absolute);