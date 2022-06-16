#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace AddictedProxy.Database.Repositories.Credentials;

public class AddictedUserCredentialRepository : IAddictedUserCredentialRepository
{
    private readonly EntityContext _context;
    private readonly ILogger<AddictedUserCredentialRepository> _logger;

    public AddictedUserCredentialRepository(EntityContext context, ILogger<AddictedUserCredentialRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AddictedUserCredentials?> GetLeastUsedCredQueryingAsync(CancellationToken token)
    {
        try
        {
            //Casting as nullable type to handle the case where the sequence contains no elements, it will return null instead of throw.
            //keeping the catch in case of
            var min = await _context.AddictedUserCreds.MinAsync(credentials => (int?)credentials.Usage, token);
            if (min == null)
            {
                _logger.LogWarning("Couldn't find credential to use for querying");
                return null;
            }
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
            //Casting as nullable type to handle the case where the sequence contains no elements, it will return null instead of throw.
            //keeping the catch in case of
            var min = await _context.AddictedUserCreds.Where(credentials => credentials.DownloadExceededDate == null).MinAsync(credentials => (int?)credentials.DownloadUsage, token);
            if (min == null)
            {
                _logger.LogWarning("Couldn't find credential to use for download");
                return null;
            }
            return await _context.AddictedUserCreds.Where(credentials => credentials.DownloadUsage <= min && credentials.DownloadExceededDate == null).FirstOrDefaultAsync(token);
        }
        catch (InvalidOperationException e) when (e.Message == "Sequence contains no elements.")
        {
            return null;
        }
    }

    /// <summary>
    /// Get credentials that have reach their download limits
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<AddictedUserCredentials> GetDownloadExceededCredentialsAsync()
    {
        return _context.AddictedUserCreds.Where(credentials => credentials.DownloadExceededDate != null).ToAsyncEnumerable();
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
        await _context.BulkSaveChangesAsync(token);
    }
}