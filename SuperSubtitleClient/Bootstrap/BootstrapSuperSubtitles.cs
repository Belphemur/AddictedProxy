using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSubtitleClient.Generated;
using SuperSubtitleClient.Model;
using SuperSubtitleClient.Service;

namespace SuperSubtitleClient.Bootstrap;

public class BootstrapSuperSubtitles : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddOptions<SuperSubtitlesConfig>()
            .Bind(configuration.GetSection(SuperSubtitlesConfig.SectionName))
            .ValidateOnStart();

        services.AddGrpcClient<SuperSubtitlesService.SuperSubtitlesServiceClient>((sp, options) =>
            {
                var config = sp.GetRequiredService<IOptions<SuperSubtitlesConfig>>().Value;
                options.Address = config.Address;
            })
            .AddStandardResilienceHandler();

        services.AddSingleton<ISuperSubtitlesClient, SuperSubtitlesClientImpl>();
    }
}
