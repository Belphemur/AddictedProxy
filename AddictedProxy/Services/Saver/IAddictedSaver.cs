namespace AddictedProxy.Services.Saver;

public interface IAddictedSaver
{
    Task RefreshShows(CancellationToken token);
}