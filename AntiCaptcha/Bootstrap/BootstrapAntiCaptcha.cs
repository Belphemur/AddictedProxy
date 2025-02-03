using AntiCaptcha.Model.Config;
using AntiCaptcha.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;

namespace AntiCaptcha.Bootstrap;

public class BootstrapAntiCaptcha : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<AntiCaptchaConfig>(configuration.GetSection("AntiCaptcha"));
        services.AddHttpClient<IAntiCaptchaClient, AntiCaptchaClient>(client => client.BaseAddress = new Uri("https://api.anti-captcha.com/"))
            .SetHandlerLifetime(TimeSpan.FromHours(2))
            .AddStandardResilienceHandler(options => options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMinutes(1)
            });
        services.AddHostedService<MetricGatherHostedService>();
    }
}