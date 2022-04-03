#region

using JetBrains.Annotations;

#endregion

namespace AddictedProxy.Services.Addic7ed.Exception;

public class DownloadLimitExceededException : System.Exception
{
    public DownloadLimitExceededException([CanBeNull] string? message, [CanBeNull] System.Exception? innerException = null) : base(message, innerException)
    {
    }
}