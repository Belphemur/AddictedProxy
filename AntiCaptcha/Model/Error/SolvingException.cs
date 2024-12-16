namespace AntiCaptcha.Model.Error;

public class SolvingException : Exception
{
    public SolvingException(string message, Exception e) : base(message, e)
    {
    }
    public SolvingException(string message) : base(message)
    {
    }
}