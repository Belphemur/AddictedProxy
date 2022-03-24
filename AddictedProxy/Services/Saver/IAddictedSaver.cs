namespace AddictedProxy.Services.Saver;

public interface IAddictedSaver
{
    Task RefreshShowsAsync(CancellationToken token);
}