#region

using Microsoft.EntityFrameworkCore.Design;

#endregion

namespace AddictedProxy.Database.Context.Factory;

public class EntityContextFactory : IDesignTimeDbContextFactory<EntityContext>
{
    public EntityContext CreateDbContext(string[] args)
    {
        return new();
    }
}