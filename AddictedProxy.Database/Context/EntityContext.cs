#region

using System.Data;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Context;

public class EntityContext : DbContext
{
    /// <summary>
    /// Only needed for MariaDB/MYSQL
    /// </summary>
    private readonly string? _connectionString;

    public EntityContext(DbContextOptions options) : base(options)
    {
        _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
        if (_connectionString == null)
        {
            throw new ArgumentException("Need to have a DB_CONNECTION Env variable to connect to Mysql/MariaDB");
        }
    }

    internal EntityContext() : this(new DbContextOptions<EntityContext>())
    {
    }

    public DbSet<TvShow> TvShows { get; set; } = null!;
    public DbSet<Subtitle> Subtitles { get; set; } = null!;
    public DbSet<Episode> Episodes { get; set; } = null!;
    public DbSet<Season> Seasons { get; set; } = null!;
    public DbSet<AddictedUserCredentials> AddictedUserCreds { get; set; } = null!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}