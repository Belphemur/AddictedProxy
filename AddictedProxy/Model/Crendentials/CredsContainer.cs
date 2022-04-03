#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Services.Credentials;

#endregion

namespace AddictedProxy.Model.Crendentials;

public class CredsContainer : IAsyncDisposable
{
    private readonly ICredentialsService _credentialsService;
    private readonly AddictedUserCredentials _addictedUserCredentials;

    private bool _isCredentialsCalled = false;

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