using System.Threading.RateLimiting;

namespace AddictedProxy.Model.RateLimiting;

public class RateLimitingConfig
{
    /// <summary>
    ///   The token bucket rate limiter options
    /// </summary>
    public required TokenBucketRateLimiterOptions Token { get; set; }

    /// <summary>
    /// Enable or disable the rate limiting
    /// </summary>
    public bool Enabled { get; set; }
}