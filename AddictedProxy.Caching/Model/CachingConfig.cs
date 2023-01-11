namespace AddictedProxy.Caching.Model;

public abstract record CachingConfig(TimeSpan Sliding, TimeSpan Absolute);