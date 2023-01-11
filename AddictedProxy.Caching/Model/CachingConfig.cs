namespace AddictedProxy.Caching.Model;

public abstract class CachingConfig
{
    public TimeSpan Sliding { get; init; }
    public TimeSpan Absolute { get; init; }
    
}