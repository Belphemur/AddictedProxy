namespace InversionOfControl.Service.EnvironmentVariable.Exception;

public class EnvironmentVariableException : System.Exception
{
    public string? Key { get; }
    public EnvironmentVariableException(string? key, string message, System.Exception? innerException = null) : base(message, innerException)
    {
        Key = key;
    }
}