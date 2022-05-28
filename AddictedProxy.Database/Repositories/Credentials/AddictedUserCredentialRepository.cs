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
        try
        {
            var min = await _context.AddictedUserCreds.MinAsync(credentials => credentials.Usage, token);
            return await _context.AddictedUserCreds.Where(credentials => credentials.Usage <= min).FirstOrDefaultAsync(token);
        }
        catch (InvalidOperationException e) when (e.Message == "Sequence contains no elements.")
        {
            return null;
        }
    }

    public async Task<AddictedUserCredentials?> GetLeastUsedCredDownloadAsync(CancellationToken token)
    {
        try
        {
            var min = await _context.AddictedUserCreds.MinAsync(credentials => credentials.DownloadUsage, token);
            return await _context.AddictedUserCreds.Where(credentials => credentials.DownloadUsage <= min && credentials.DownloadExceededDate == null).FirstOrDefaultAsync(token);
        }
        catch (InvalidOperationException e) when (e.Message == "Sequence contains no elements.")
        {
            return null;
        }
    }

    /// <summary>
    /// Get all the credentials
    /// </summary>
    public IAsyncEnumerable<AddictedUserCredentials> GetAllCredentialsAsync()
    {
        return _context.AddictedUserCreds.ToAsyncEnumerable();
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _context.SaveChangesAsync(token);
    }
}