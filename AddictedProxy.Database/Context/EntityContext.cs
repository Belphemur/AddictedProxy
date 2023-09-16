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

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
                                    .Where(x => x is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified })
                                    .Select(entry => (entry.State, Entity: (BaseEntity)entry.Entity));

        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.CreatedAt = now;
            }

            entity.Entity.UpdatedAt = now;
        }
    }
}