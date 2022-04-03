using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Credentials;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories.Credentials;

public class AddictedUserCredentialRepository : IAddictedUserCredentialRepository
{
    private readonly EntityContext _context;

    public AddictedUserCredentialRepository(EntityContext context)
    {
        _context = context;
    }

    public Task<AddictedUserCredentials?> GetLeastUsedCredAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var result = _context.AddictedUserCreds.AsSingleQuery()
                             .MinBy(credentials => credentials.Usage);
        return Task.FromResult(result);
    }

    public async Task UpsertUserCredentialsAsync(AddictedUserCredentials credentials, CancellationToken token)
    {
        await _context.AddictedUserCreds.AddAsync(credentials, token);
        await _context.SaveChangesAsync(token);
    }
}