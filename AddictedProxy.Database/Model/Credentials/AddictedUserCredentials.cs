using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Credentials;

[Table("AddictedUserCredentials")]
[Index(nameof(AddictedUserCredentials.Cookie), IsUnique = true)]
public record AddictedUserCredentials(int Id, string Cookie, int Usage = 0, DateTime? LastUsage = null);