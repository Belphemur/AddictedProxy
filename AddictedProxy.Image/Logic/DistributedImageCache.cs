using AddictedProxy.Caching.Extensions;
using AddictedProxy.Image.Model;
using Microsoft.Extensions.Caching.Distributed;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Resolvers;

namespace AddictedProxy.Image.Logic;

public class DistributedImageCache : IImageCache
{
    private readonly IDistributedCache _distributedCache;

    public DistributedImageCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    private static string GetImageKey(string key)
    {
        return $"image.cache.{key}.content";
    }

    private static string GetMetadataKey(string key)
    {
        return $"image.cache.{key}.metadata";
    }


    public async Task<IImageCacheResolver?> GetAsync(string key)
    {
        var metadataDictionary = await _distributedCache.GetAsync<Dictionary<string, string>>(GetMetadataKey(key));
        if (metadataDictionary == null) return null;

        return new DistributedImageCacheResolver(_distributedCache, GetImageKey(key), ImageCacheMetadata.FromDictionary(metadataDictionary));
    }

    private async Task<byte[]> ReadFully(Stream input)
    {
        using var ms = new MemoryStream();
        await input.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task SetAsync(string key, Stream stream, ImageCacheMetadata metadata)
    {
        await _distributedCache.SetAsync(GetImageKey(key), await ReadFully(stream), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromDays(30)
        });
        await _distributedCache.SetAsync(GetMetadataKey(key), metadata.ToDictionary(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromDays(30)
        });
    }
}