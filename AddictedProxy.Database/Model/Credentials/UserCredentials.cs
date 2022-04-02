using System.ComponentModel.DataAnnotations.Schema;

namespace AddictedProxy.Database.Model.Credentials;

[Table("AddictedUserCredentials")]
public record UserCredentials(int Id, string Cookie);