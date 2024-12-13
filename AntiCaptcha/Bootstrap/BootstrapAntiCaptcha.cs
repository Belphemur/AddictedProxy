using AntiCaptcha.Model.Config;
using AntiCaptcha.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AntiCaptcha.Bootstrap;

public class BootstrapAntiCaptcha : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AntiCaptchaConfig>(configuration.GetSection("AntiCaptcha"));
        services.AddHttpClient<IAntiCaptchaClient, AntiCaptchaClient>(client => client.BaseAddress = new Uri("https://api.anti-captcha.com/"))
            .SetHandlerLifetime(TimeSpan.FromHours(2))
            .AddStandardHedgingHandler();
    }
}