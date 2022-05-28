using AddictedProxy.Database.Model.Credentials;

namespace AddictedProxy.Model.Crendentials;

public interface ICredsContainer : IAsyncDisposable
{
    AddictedUserCredentials AddictedUserCredentials { get; }

    /// <summary>
    /// Tag the credential as unfit to download subtitle
    /// </summary>
    void TagAsDownloadExceeded();
}