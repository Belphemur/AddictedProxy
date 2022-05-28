using AddictedProxy.Database.Model.Credentials;

namespace AddictedProxy.Model.Crendentials;

public interface ICredsContainer : IAsyncDisposable
{
    AddictedUserCredentials AddictedUserCredentials { get; }
}