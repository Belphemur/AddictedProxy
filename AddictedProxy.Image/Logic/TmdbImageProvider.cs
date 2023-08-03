using AddictedProxy.Image.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Image.Logic;

public class TmdbImageProvider : IImageProvider
{
    private const string TmdbImagePrefix = "/tmdb/image/";
    private readonly FormatUtilities _formatUtilities;
    private readonly ITMDBClient _tmdbClient;

    public TmdbImageProvider(FormatUtilities formatUtilities, ITMDBClient tmdbClient)
    {
        _formatUtilities = formatUtilities;
        _tmdbClient = tmdbClient;
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
            var metadata = await _tmdbClient.GetImageMetadataAsync(imagePath, context.RequestAborted);
            if (metadata == null) return null;
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