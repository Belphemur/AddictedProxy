using AddictedProxy.Image.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using TvMovieDatabaseClient.Service;
using TvMovieDatabaseClient.Service.Model;

namespace AddictedProxy.Image.Logic;

public class TmdbImageProvider : IImageProvider
{
    private const string TmdbImagePrefix = "/tmdb/image/";
    private readonly FormatUtilities _formatUtilities;
    private readonly ITMDBClient _tmdbClient;
    private readonly IMemoryCache _memoryCache;

    public TmdbImageProvider(FormatUtilities formatUtilities, ITMDBClient tmdbClient, IMemoryCache memoryCache)
    {
        _formatUtilities = formatUtilities;
        _tmdbClient = tmdbClient;
        _memoryCache = memoryCache;
    }

    public bool IsValidRequest(HttpContext context)
    {
        return _formatUtilities.TryGetExtensionFromUri(context.Request.GetDisplayUrl(), out _);
    }

    public async Task<IImageResolver?> GetAsync(HttpContext context)
    {
        var imagePath = context.Request.Path.Value!.Substring(TmdbImagePrefix.Length);
        if (string.IsNullOrEmpty(imagePath))
        {
            return null;
        }

        try
        {
            var metadata = await _memoryCache.GetOrCreateAsync($"tmdb-{imagePath}", async entry =>
            {
                var currentMetadata = await _tmdbClient.GetImageMetadataAsync(imagePath, context.RequestAborted);
                if (currentMetadata == null)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                    return null;
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(14);
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return currentMetadata;
            });

            if (!metadata.HasValue) return null;

            return new TmdbImageResolver(metadata.Value, _tmdbClient);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;
    public Func<HttpContext, bool> Match { get; set; } = context => context.Request.Path.Value?.StartsWith(TmdbImagePrefix) ?? false;
}