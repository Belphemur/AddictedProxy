using AddictedProxy.Storage.Store.S3.Bootstrap.EnvVar;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace AddictedProxy.Storage.Store.S3.Bootstrap;

public class BootstrapS3 : IBootstrap, IBootstrapEnvironmentVariable<S3Config, S3ConfigParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<IStorageProvider, S3StorageProvider>();
        services.AddResiliencePipeline("s3-download", builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = args => ValueTask.FromResult(args.Outcome.Exception is AmazonS3Exception),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(50),
                MaxDelay = TimeSpan.FromMilliseconds(500),
                UseJitter = true
            });
        });
    }

    public EnvVarRegistration<S3Config, S3ConfigParser> EnvVarRegistration { get; } = new("S3_GATEWAY", "S3_ACCESS", "S3_SECRET", "S3_BUCKET");
}