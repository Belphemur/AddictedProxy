#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Services.Credentials;

#endregion

namespace AddictedProxy.Model.Crendentials;

public class CredsContainer : IAsyncDisposable
{
    private readonly AddictedUserCredentials _addictedUserCredentials;
    private readonly ICredentialsService _credentialsService;

    private bool _isCredentialsCalled;

    public CredsContainer(ICredentialsService credentialsService, AddictedUserCredentials addictedUserCredentials)
    {
        _addictedUserCredentials = addictedUserCredentials;
        _credentialsService = credentialsService;
    }

    public AddictedUserCredentials AddictedUserCredentials
    {
        get
        {
            _isCredentialsCalled = true;
            return _addictedUserCredentials;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isCredentialsCalled)
        {
            return;
        }

        await _credentialsService.UpdateUsageCredentialsAsync(AddictedUserCredentials, default);
    }
}