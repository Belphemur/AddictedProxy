using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Migration;

[Table("OneTimeMigrationRelease")]
[Index(nameof(MigrationType), nameof(State), IsUnique = true)]
public class OneTimeMigrationRelease
{
    public enum MigrationState
    {
        Success,
        Running,
        Fail
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string MigrationType { get; set; }

    public MigrationState State { get; set; }

    public DateTime RanAt { get; set; }
}