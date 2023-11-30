#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Migration;
using AddictedProxy.Database.Model.Shared;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Context;

public class EntityContext : DbContext
{
    public EntityContext(DbContextOptions<EntityContext> options) : base(options)
    {
    }

    internal EntityContext() : this(new DbContextOptions<EntityContext>())
    {
    }

    public DbSet<TvShow> TvShows { get; set; } = null!;
    public DbSet<Subtitle> Subtitles { get; set; } = null!;
    public DbSet<Episode> Episodes { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;
    public DbSet<AddictedUserCredentials> AddictedUserCreds { get; set; } = null!;

    public DbSet<OneTimeMigrationRelease> OneTimeMigrationRelease { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }
}