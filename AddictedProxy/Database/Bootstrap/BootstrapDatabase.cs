using AddictedProxy.Database.Context;
using AddictedProxy.Database.Repositories;
using InversionOfControl.Model;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Extensions;

namespace AddictedProxy.Database.Bootstrap;

public class BootstrapDatabase : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<EntityContext>();
        EntityFrameworkManager.ContextFactory = context => new EntityContext(new DbContextOptions<EntityContext>());
        EFCoreConfig.AddLicense(Environment.GetEnvironmentVariable("EFCORE_LICENSE"), Environment.GetEnvironmentVariable("EFCORE_KEY"));
        services.AddScoped<ITvShowRepository, TvShowRepository>();
        
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<ISubtitleRepository, SubtitleRepository>();
    }
}