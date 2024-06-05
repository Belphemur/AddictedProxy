using AddictedProxy.Caching.Extensions;
using AddictedProxy.Storage.Caching.Service;
using Microsoft.Extensions.Caching.Distributed;
using SixLabors.ImageSharp.Web.Resolvers;
using TvMovieDatabaseClient.Service;
using TvMovieDatabaseClient.Service.Model;
using DistributedCacheExtensions = AddictedProxy.Caching.Extensions.DistributedCacheExtensions;

namespace AddictedProxy.Image.Model;

public class TmdbImageResolver : IImageResolver
{
    private readonly string _imagePath;
    private readonly ImageMetadata _imageMetadata;
    private readonly ITMDBClient _tmdbClient;
    private readonly ICachedStorageProvider _cacheStorageProvider;

    public TmdbImageResolver(string imagePath, ImageMetadata imageMetadata, ITMDBClient tmdbClient, ICachedStorageProvider cacheStorageProvider)
    {
        _imagePath = imagePath;
        _imageMetadata = imageMetadata;
        _tmdbClient = tmdbClient;
        _cacheStorageProvider = cacheStorageProvider;
    }

    public Task<ImageMetadata> GetMetaDataAsync()
    {
        return Task.FromResult(_imageMetadata);
    }

    public async Task<Stream> OpenReadAsync()
    {
        return (await _cacheStorageProvider.GetSertAsync("tmdb", _imagePath, async ct =>
        {
            var image = await _tmdbClient.GetImageAsync(_imagePath, ct);
            return image?.ImageStream;
        }, default))!;
    }
}