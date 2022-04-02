using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Model.Credentials;

[Table("AddictedUserCredentials")]
[Index(nameof(UserCredentials.Cookie), IsUnique = true)]
public record UserCredentials(int Id, string Cookie, int Usage = 0, DateTime? LastUsage = null);