#region

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Model.Credentials;

[Table("AddictedUserCredentials")]
[Index(nameof(Cookie), IsUnique = true)]
public class AddictedUserCredentials
{
    public int Id { get; set; }
    public string Cookie { get; set; }
    public int Usage { get; set; }
    public DateTime? LastUsage { get; set; }
}