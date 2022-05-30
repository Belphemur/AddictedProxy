#region

using System.Data;
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
        Database.GetDbConnection().StateChange += (_, args) =>
        {
            if (args.CurrentState != ConnectionState.Open)
            {
                return;
            }

            //We're using litestream to do backup of the sqlite database.
            //They advise to have the busy_timeout set to 5 seconds. To be sure, I'm putting it to 7.5.
            //https://litestream.io/tips/#busy-timeout
            Database.ExecuteSqlRaw("PRAGMA busy_timeout = 7500;");
        };
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