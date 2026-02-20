#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Credentials.Job;
using Hangfire;

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

    /// <summary>
    /// Tag the credential as unfit to download subtitle
    /// </summary>
    public void TagAsDownloadExceeded()
    {
        _addictedUserCredentials.DownloadExceededDate = DateTime.UtcNow;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isCredentialsCalled)
        {
            return;
        }

        await _credentialsService.UpdateUsageCredentialsAsync(AddictedUserCredentials, _forDownloadOperation, default);
        if (_addictedUserCredentials.DownloadExceededDate != null)
        {
            BackgroundJob.Enqueue<ResetDownloadCredJob>(job => job.CheckAndResetCredAsync(_addictedUserCredentials.Id, null, default));
        }
    }
}