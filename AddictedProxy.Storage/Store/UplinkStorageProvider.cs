#region

using AddictedProxy.Storage.Compressor;
using AddictedProxy.Storage.Store.Boostrap.EnvVar;
using uplink.NET.Exceptions;
using uplink.NET.Models;
using uplink.NET.Services;

#endregion

namespace AddictedProxy.Storage.Store;

public class UplinkStorageProvider : IStorageProvider
{
    private readonly BucketService _bucketService;
    private readonly ICompressor _compressor;
    private readonly ObjectService _objectService;
    private readonly UplinkSettings _settings;

    public UplinkStorageProvider(UplinkSettings settings, ICompressor compressor)
    {
        _settings = settings;
        _compressor = compressor;
        _objectService = new ObjectService(new Access(settings.AccessGrant));
        _bucketService = new BucketService(new Access(settings.AccessGrant));
    }

    /// <summary>
    /// Store a file
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="inputStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> StoreAsync(string filename, Stream inputStream, CancellationToken cancellationToken)
    {
        var bucket = await _bucketService.GetBucketAsync(_settings.Bucket);
        await using var compressedStream = await _compressor.CompressAsync(inputStream, cancellationToken);
        var uploadOperation = await _objectService.UploadObjectAsync(bucket, GetFileName(filename), new UploadOptions(), compressedStream, false);

        await using var ctr = cancellationToken.Register(_ => { uploadOperation.Cancel(); }, null);

        await uploadOperation.StartUploadAsync();
        return !(uploadOperation.Cancelled || uploadOperation.Failed);
    }

    /// <summary>
    /// Download the file as a stream
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken)
    {
        var bucket = await _bucketService.GetBucketAsync(_settings.Bucket);
        try
        {
            var downloadOperation = await _objectService.DownloadObjectAsync(bucket, GetFileName(filename), new DownloadOptions(), false);
            await using var ctr = cancellationToken.Register(_ => { downloadOperation.Cancel(); }, null);
            await downloadOperation.StartDownloadAsync();
            if (downloadOperation.Failed || downloadOperation.Cancelled)
            {
                return null;
            }

            var memoryStream = new MemoryStream(downloadOperation.DownloadedBytes);
            return await _compressor.DecompressAsync(memoryStream, cancellationToken);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Get the full file name in the storage
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private string GetFileName(string file)
    {
        return $"{file}{_compressor.Extension}";
    }
}