namespace AddictedProxy.Upstream.Service.Exception;

public class DownloadLimitExceededException : System.Exception
{
    public DownloadLimitExceededException(string? message, System.Exception? innerException = null) : base(message, innerException)
    {
    }
}