using System.Globalization;
using System.Reflection;

namespace AddictedProxy.OneTimeMigration.Model;

[AttributeUsage(AttributeTargets.Class)]
public class MigrationDateAttribute : Attribute
{
    public string Date => _migrationDate.ToString(CultureInfo.InvariantCulture);
    private readonly DateTime _migrationDate;

    public MigrationDateAttribute(int year, int month, int day)
    {
        _migrationDate = new DateTime(year, month, day);
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
    /// <param name="token"></param>
    /// <returns></returns>
    public Task ExecuteAsync(CancellationToken token);
}