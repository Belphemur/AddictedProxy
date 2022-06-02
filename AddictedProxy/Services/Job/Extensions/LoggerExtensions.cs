using JetBrains.Annotations;
using Job.Scheduler.Job.Exception;

namespace AddictedProxy.Services.Job.Extensions;

public static class LoggerExtensions
{
    /// <summary>
    /// Log a job exception
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
    public static void LogJobException(this ILogger logger, JobException exception, [StructuredMessageTemplate] string? message, params object?[] args)
    {
        logger.LogInformation("Job exception: {Message}", exception.Message);
        logger.Log(LogLevel.Error, exception.InnerException, message, args);
    }
}