#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Services.Credentials;

#endregion

namespace AddictedProxy.Model.Crendentials;

public class CredsContainer : IAsyncDisposable
{
    private readonly ICredentialsService _credentialsService;

    public CredsContainer(ICredentialsService credentialsService, AddictedUserCredentials addictedUserCredentials)
    {
        AddictedUserCredentials = addictedUserCredentials;
        _credentialsService = credentialsService;
    }

    public AddictedUserCredentials AddictedUserCredentials { get; }

    public async ValueTask DisposeAsync()
    {
        await _credentialsService.UpdateUsageCredentialsAsync(AddictedUserCredentials, default);
    }
}