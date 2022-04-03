#region

using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Services.Credentials.Bootstrap;

public class BootstrapCredentials : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ICredentialsService, CredentialsService>();
    }
}