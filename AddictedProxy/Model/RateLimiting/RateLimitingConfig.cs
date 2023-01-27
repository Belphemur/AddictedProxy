using System.Threading.RateLimiting;

namespace AddictedProxy.Model.RateLimiting;

public class RateLimitingConfig
{
    public TokenBucketRateLimiterOptions Token { get; set; }
}