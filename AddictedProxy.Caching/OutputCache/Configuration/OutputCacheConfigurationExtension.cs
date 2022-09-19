using Microsoft.AspNetCore.OutputCaching;

namespace AddictedProxy.Caching.OutputCache.Configuration;

internal static class OutputCacheConfigurationExtension
{
    /// <summary>
    /// Add our own policies for the output caching
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static OutputCacheOptions AddOwnPolicies(this OutputCacheOptions options)
    {
        options.AddPolicy(nameof(PolicyEnum.Stats), builder =>
        {
            builder
                .Tag(nameof(PolicyEnum.Stats))
                .Cache()
                .Expire(TimeSpan.FromHours(6));
        });
        
        options.AddPolicy(nameof(PolicyEnum.Shows), builder =>
        {
            builder
                .Tag(nameof(PolicyEnum.Shows))
                .Cache()
                .Expire(TimeSpan.FromHours(2));
        });
        
        options.AddPolicy(nameof(PolicyEnum.Download), builder =>
        {
            builder
                .Tag(nameof(PolicyEnum.Download))
                .Cache()
                .Expire(TimeSpan.FromDays(2));
        });
        options.AddBasePolicy(builder => builder.Cache().Expire(TimeSpan.FromMinutes(1)));

        return options;
    }
}