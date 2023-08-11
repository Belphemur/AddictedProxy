using SixLabors.ImageSharp.Web.Resolvers;
using TvMovieDatabaseClient.Service;
using TvMovieDatabaseClient.Service.Model;

namespace AddictedProxy.Image.Model;

public class TmdbImageResolver : IImageResolver
{
    private readonly TmdbImageMetadata _imageMetadata;
    private readonly ITMDBClient _tmdbClient;

    public TmdbImageResolver(TmdbImageMetadata imageMetadata, ITMDBClient tmdbClient)
    {
        _imageMetadata = imageMetadata;
        _tmdbClient = tmdbClient;
    }

    public Task<ImageMetadata> GetMetaDataAsync()
    {
        return Task.FromResult(new ImageMetadata(_imageMetadata.LastModified, TimeSpan.FromDays(90), _imageMetadata.ContentLength));
    }

    public async Task<Stream> OpenReadAsync()
    {
        var image = await _tmdbClient.GetImageAsync(_imageMetadata.ImagePath, default);
        return image!.Value.ImageStream;
    }
}