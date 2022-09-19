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
                .Expire(TimeSpan.FromHours(6))
                .With(context => context.HttpContext.Response.StatusCode == 200);
        });
        
        options.AddPolicy(nameof(PolicyEnum.Shows), builder =>
        {
            builder
                .Tag(nameof(PolicyEnum.Shows))
                .Cache()
                .Expire(TimeSpan.FromHours(2))
                .With(context => context.HttpContext.Response.StatusCode == 200);
        });
        
        options.AddPolicy(nameof(PolicyEnum.Download), builder =>
        {
            builder
                .Tag(nameof(PolicyEnum.Download))
                .Cache()
                .Expire(TimeSpan.FromDays(7))
                .With(context => context.HttpContext.Response.StatusCode == 200);
        });
        options.AddBasePolicy(builder => builder.Cache().With(context => context.HttpContext.Response.StatusCode == 404).Expire(TimeSpan.FromDays(0.5)));
        options.AddBasePolicy(builder => builder.NoCache().With(context => context.HttpContext.Response is { StatusCode: > 299 and not 404 }));
        options.AddBasePolicy(builder => builder.Cache().Expire(TimeSpan.FromMinutes(1)));

        return options;
    }
}