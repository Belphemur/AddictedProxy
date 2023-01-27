#region

using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Services.Credentials.Bootstrap;

public class BootstrapCredentials : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICredentialsService, CredentialsService>();
    }
}