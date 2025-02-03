using Community.Microsoft.Extensions.Caching.PostgreSql;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Caching.Bootstrap;

public class BootstrapPostgresCaching : IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddDistributedPostgreSqlCache(setup =>
        {
            setup.ConnectionString = configuration.GetConnectionString("Cache");
            setup.SchemaName = "public";
            setup.TableName = "cache";
            setup.DisableRemoveExpired = false;
            // Optional - DisableRemoveExpired default is FALSE
            setup.CreateInfrastructure = true;
            // CreateInfrastructure is optional, default is TRUE
            // This means que every time starts the application the
            // creation of table and database functions will be verified.
            setup.ExpiredItemsDeletionInterval = TimeSpan.FromMinutes(30);
            // ExpiredItemsDeletionInterval is optional
            // This is the periodic interval to scan and delete expired items in the cache. Default is 30 minutes.
            // Minimum allowed is 5 minutes. - If you need less than this please share your use case 😁, just for curiosity...
        });
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return configuration.GetSection("Caching:Provider").Get<string>() == "postgres";
    }
}