using AddictedProxy.Database.Model.Credentials;

namespace AddictedProxy.Database.Repositories.Credentials;

public interface IAddictedUserCredentialRepository
{
    Task<AddictedUserCredentials?> GetLeastUsedCredAsync(CancellationToken token);
    Task UpsertUserCredentialsAsync(AddictedUserCredentials credentials, CancellationToken token);
}