using System.Reflection;
using Hangfire.Server;

namespace AddictedProxy.OneTimeMigration.Model;

[AttributeUsage(AttributeTargets.Class)]
public class MigrationDateAttribute : Attribute
{
    public readonly string Date;

    public MigrationDateAttribute(int year, int month, int day)
    {
        Date = $"{year}-{month}-{day}";
    }
}
public interface IMigration
{
    public string MigrationType
    {
        get
        {
            var type = GetType();
            var name = type.Name;
            var date = type.GetCustomAttribute<MigrationDateAttribute>()!.Date;
            return $"{date}_{name}";
        }
    }

    /// <summary>
    /// Execute the migration
    /// </summary>
    /// <param name="context">Hangfire perform context for console output</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task ExecuteAsync(PerformContext context, CancellationToken token);
}