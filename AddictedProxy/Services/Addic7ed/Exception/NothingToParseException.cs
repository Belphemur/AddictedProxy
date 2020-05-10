using JetBrains.Annotations;

namespace AddictedProxy.Services.Addic7ed.Exception
{
    public class NothingToParseException : System.Exception
    {
        public NothingToParseException([CanBeNull] string? message, [CanBeNull] System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}