using Microsoft.EntityFrameworkCore.Design;

namespace AddictedProxy.Database.Context.Factory;

public class EntityContextFactory : IDesignTimeDbContextFactory<EntityContext>
{
    public EntityContext CreateDbContext(string[] args) => new ();
}