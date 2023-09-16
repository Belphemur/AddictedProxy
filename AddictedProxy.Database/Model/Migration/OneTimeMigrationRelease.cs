using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using AddictedProxy.Database.Model.Shared;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Migration;

[Table("OneTimeMigrationRelease")]
[Index(nameof(MigrationType), nameof(State), IsUnique = true)]
public class OneTimeMigrationRelease : BaseEntity
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
}