#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Context;

public class EntityContext : DbContext
{
    public EntityContext(DbContextOptions options) : base(options)
    {
        var folder = Environment.GetEnvironmentVariable("DB_PATH") ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        DbPath = Path.Join(folder, "addicted.db");
    }

    internal EntityContext() : this(new DbContextOptions<EntityContext>())
    {
    }

    public DbSet<TvShow> TvShows { get; set; } = null!;
    public DbSet<Subtitle> Subtitles { get; set; } = null!;
    public DbSet<Episode> Episodes { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;

    public DbSet<AddictedUserCredentials> AddictedUserCreds { get; set; } = null!;

    private string DbPath { get; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath};Cache=Shared");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TvShow>()
                    .Property(c => c.Name)
                    .UseCollation("NOCASE");
    }
}