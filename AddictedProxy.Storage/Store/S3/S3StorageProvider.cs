using System.Net;
using AddictedProxy.Storage.Compressor;
using AddictedProxy.Storage.Store.S3.Bootstrap.EnvVar;
using Amazon.S3;
using Amazon.S3.Model;

namespace AddictedProxy.Storage.Store.S3;

public class S3StorageProvider : IStorageProvider
{
    private readonly ICompressor _compressor;
    private readonly S3Config _s3Config;
    private readonly AmazonS3Client _awsS3Client;

    public S3StorageProvider(ICompressor compressor, S3Config s3Config)
    {
        _compressor = compressor;
        _s3Config = s3Config;
        var config = new AmazonS3Config
        {
            ServiceURL = s3Config.Gateway
        };
        _awsS3Client = new AmazonS3Client(s3Config.AccessKey, s3Config.SecretKey, config);
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

    public async Task<bool> StoreAsync(string filename, Stream inputStream, CancellationToken cancellationToken)
    {
        var result = await _awsS3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _s3Config.Bucket,
            Key = GetFileName(filename),
            InputStream = await _compressor.CompressAsync(inputStream, cancellationToken)
        }, cancellationToken);

        return result?.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _awsS3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _s3Config.Bucket,
                Key = GetFileName(filename)
            }, cancellationToken);

            if (result?.HttpStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await _compressor.DecompressAsync(result.ResponseStream, cancellationToken);
        }
        catch (AmazonS3Exception e) when (e.Message == "The specified key does not exist.")
        {
            return null;
        }
    }
}