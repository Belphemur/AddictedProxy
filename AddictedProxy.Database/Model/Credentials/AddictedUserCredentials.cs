using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Credentials;

[Table("AddictedUserCredentials")]
[Index(nameof(AddictedUserCredentials.Cookie), IsUnique = true)]
public class AddictedUserCredentials
{
    public int Id { get; set; }
    public string Cookie { get; set; }
    public int Usage { get; set; }
    public DateTime? LastUsage { get; set; }
}