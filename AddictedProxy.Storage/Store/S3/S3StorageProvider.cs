using System.Net;
using AddictedProxy.Storage.Store.S3.Bootstrap.EnvVar;
using Amazon.S3;
using Amazon.S3.Model;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace AddictedProxy.Storage.Store.S3;

public class S3StorageProvider : IStorageProvider
{
    private readonly S3Config _s3Config;
    private readonly AmazonS3Client _awsS3Client;
    private readonly AsyncRetryPolicy _asyncRetryPolicy;

    public S3StorageProvider(S3Config s3Config)
    {
        _s3Config = s3Config;
        var config = new AmazonS3Config
        {
            ServiceURL = s3Config.Gateway
        };
        _awsS3Client = new AmazonS3Client(s3Config.AccessKey, s3Config.SecretKey, config);
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(50), retryCount: 3);
        _asyncRetryPolicy = Policy.Handle<AmazonS3Exception>().WaitAndRetryAsync(delay);
    }


    public async Task<bool> StoreAsync(string filename, Stream inputStream, CancellationToken cancellationToken)
    {
        var result = await _awsS3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _s3Config.Bucket,
            Key = filename,
            InputStream = inputStream,
            //For Cloudflare R2
            //see: https://github.com/cloudflare/cloudflare-docs/issues/4683
            //see: https://community.cloudflare.com/t/putobjectasync-not-working-for-r2-with-aws-s3-net-sdk/427335
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        }, cancellationToken);

        return result?.HttpStatusCode == HttpStatusCode.OK;
    }

    /// <summary>
    /// Delete the specific file
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(string filename, CancellationToken cancellationToken) => await _awsS3Client.DeleteObjectAsync(_s3Config.Bucket, filename, cancellationToken);

    public async Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken)
    {
        return await _asyncRetryPolicy.ExecuteAsync(token => DownloadInternalAsync(filename, token), cancellationToken);
    }

    private async Task<Stream?> DownloadInternalAsync(string filename, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _awsS3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = _s3Config.Bucket,
                Key = filename
            }, cancellationToken);

            if (result?.HttpStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return result.ResponseStream;
        }
        catch (AmazonS3Exception e) when (e.Message.StartsWith("The specified key does not exist."))
        {
            return null;
        }
    }
}