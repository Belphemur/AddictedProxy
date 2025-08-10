using AddictedProxy.Image.Model;
using AddictedProxy.Storage.Caching.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using TvMovieDatabaseClient.Service;
using TvMovieDatabaseClient.Service.Model;
using AddictedProxy.Caching.Extensions;
using Performance.Service;
using SixLabors.ImageSharp.Web;
using DistributedCacheExtensions = AddictedProxy.Caching.Extensions.DistributedCacheExtensions;

namespace AddictedProxy.Image.Logic;

public class TmdbImageProvider : IImageProvider
{
    private const string TmdbImagePrefix = "/tmdb/image/";
    private readonly FormatUtilities _formatUtilities;
    private readonly ITMDBClient _tmdbClient;
    private readonly IDistributedCache _distributedCache;
    private readonly ICachedStorageProvider _cachedStorageProvider;
    private readonly IPerformanceTracker _performanceTracker;

    public TmdbImageProvider(FormatUtilities formatUtilities,
        ITMDBClient tmdbClient,
        IDistributedCache distributedCache, 
        ICachedStorageProvider cachedStorageProvider,
        IPerformanceTracker performanceTracker)
    {
        _formatUtilities = formatUtilities;
        _tmdbClient = tmdbClient;
        _distributedCache = distributedCache;
        _cachedStorageProvider = cachedStorageProvider;
        _performanceTracker = performanceTracker;
    }

    public bool IsValidRequest(HttpContext context)
    {
        return _formatUtilities.TryGetExtensionFromUri(context.Request.GetDisplayUrl(), out _);
    }

    public async Task<IImageResolver?> GetAsync(HttpContext context)
    {
        var imagePath = context.Request.Path.Value![TmdbImagePrefix.Length..];
        using var span = _performanceTracker.BeginNestedSpan("tmdb-image-provider", $"Get image from TMDB: {imagePath}");
        if (string.IsNullOrEmpty(imagePath))
        {
            return null;
        }

        var metadata = await _distributedCache.GetSertAsync<TmdbImageMetadata?>($"tmdb-metadata-{imagePath}", async () =>
        {
            TmdbImage? image;
            try
            {
                image = await _tmdbClient.GetImageAsync(imagePath, default);
                if (image == null)
                    return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }

            await _cachedStorageProvider.GetSertAsync("tmdb",  context.Request.Path, _ => Task.FromResult(image.Value.ImageStream)!, default);
    

            return new DistributedCacheExtensions.CacheData<TmdbImageMetadata?>(image.Value.Metadata, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
            });
        });

        if (metadata == null)
            return null;

        return new TmdbImageResolver( context.Request.Path, new ImageMetadata(metadata.LastModified, TimeSpan.FromDays(365), metadata.ContentLength), _tmdbClient, _cachedStorageProvider);
    }

    public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;
    public Func<HttpContext, bool> Match { get; set; } = context => context.Request.Path.Value?.StartsWith(TmdbImagePrefix) ?? false;
}