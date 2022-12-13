namespace AddictedProxy.Upstream.Model;

public record struct DownloadUsage(int Used, int TotalAvailable)
{
    public int Remaining => TotalAvailable - Used;
}