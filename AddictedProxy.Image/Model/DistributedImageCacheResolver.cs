using Microsoft.Extensions.Caching.Distributed;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Resolvers;

namespace AddictedProxy.Image.Model;

public class DistributedImageCacheResolver : IImageCacheResolver
{
    private readonly IDistributedCache _distributedCache;
    private readonly string _imageKey;
    private readonly ImageCacheMetadata _metadata;

    public DistributedImageCacheResolver(IDistributedCache distributedCache, string imageKey,  ImageCacheMetadata metadata)
    {
        _distributedCache = distributedCache;
        _imageKey = imageKey;
        _metadata = metadata;
    }

    public Task<ImageCacheMetadata> GetMetaDataAsync()
    {
        return Task.FromResult(_metadata);
    }

    public async Task<Stream> OpenReadAsync()
    {
        var imageBytes = await _distributedCache.GetAsync(_imageKey);
        return new MemoryStream(imageBytes!);
    }
}