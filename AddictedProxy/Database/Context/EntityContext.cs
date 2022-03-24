using AddictedProxy.Model.Shows;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Context;

public class EntityContext : DbContext
{
    public EntityContext([NotNull] DbContextOptions options) : base(options)
    {
        var folder = Environment.SpecialFolder.ApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "addicted.db");
    }

    public DbSet<TvShow> TvShows { get; set; } = null!;
    public DbSet<Subtitle> Subtitles { get; set; } = null!;
    public DbSet<Episode> Episodes { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;

    private string DbPath { get; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Console.Out.WriteLine($"Set DB path to {DbPath}");
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TvShow>().Property(c => c.Name)
            .UseCollation("NOCASE");
    }
}