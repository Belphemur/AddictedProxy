#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Credentials;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Repositories.Credentials;

public class AddictedUserCredentialRepository : IAddictedUserCredentialRepository
{
    private readonly EntityContext _context;

    public AddictedUserCredentialRepository(EntityContext context)
    {
        _context = context;
    }

    public async Task<AddictedUserCredentials?> GetLeastUsedCredAsync(CancellationToken token)
    {
        var min = await _context.AddictedUserCreds.MinAsync(credentials => credentials.Usage, token);
        return await _context.AddictedUserCreds.Where(credentials => credentials.Usage <= min).FirstOrDefaultAsync(token);
    }

    public async Task UpsertUserCredentialsAsync(AddictedUserCredentials credentials, CancellationToken token)
    {
        _context.AddictedUserCreds.Update(credentials);
        await _context.SaveChangesAsync(token);
    }
}