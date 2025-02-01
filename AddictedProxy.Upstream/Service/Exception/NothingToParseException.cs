namespace AddictedProxy.Upstream.Service.Exception;

public class NothingToParseException : System.Exception
{
    public NothingToParseException(string? message, System.Exception? innerException = null) : base(message, innerException)
    {
    }
}