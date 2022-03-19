using AddictedProxy.Model.Shows;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database;

public class EntityContext : DbContext
{
    public EntityContext([NotNull] DbContextOptions options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "addicted.db");
    }

    public DbSet<TvShow> TvShows { get; set; }
    public DbSet<Subtitle> Subtitles { get; set; }
    public DbSet<Episode> Episodes { get; set; }

    public string DbPath { get; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}