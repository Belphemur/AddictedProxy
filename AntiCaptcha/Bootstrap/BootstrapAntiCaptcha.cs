using AntiCaptcha.Model.Config;
using AntiCaptcha.Service;
using InversionOfControl.Model;
using InversionOfControl.Service.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AntiCaptcha.Bootstrap;

public class BootstrapAntiCaptcha : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<AntiCaptchaConfig>(configuration.GetSection("AntiCaptcha"));
        services.AddHttpClient<IAntiCaptchaClient, AntiCaptchaClient>(client => client.BaseAddress = new Uri("https://api.anti-captcha.com/"))
            .SetHandlerLifetime(TimeSpan.FromHours(2))
            .AddSharedResilienceHandler("anti-captcha");
        services.AddHostedService<MetricGatherHostedService>();
    }
}