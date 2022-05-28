#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Services.Credentials;

#endregion

namespace AddictedProxy.Model.Crendentials;

public class CredsContainerNormalUsage : ICredsContainer
{
    private readonly AddictedUserCredentials _addictedUserCredentials;
    private readonly bool _forDownloadOperation;
    private readonly ICredentialsService _credentialsService;

    private bool _isCredentialsCalled;

    public CredsContainerNormalUsage(ICredentialsService credentialsService, AddictedUserCredentials addictedUserCredentials, bool forDownloadOperation)
    {
        _addictedUserCredentials = addictedUserCredentials;
        _forDownloadOperation = forDownloadOperation;
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

        await _credentialsService.UpdateUsageCredentialsAsync(AddictedUserCredentials, _forDownloadOperation, default);
    }
}