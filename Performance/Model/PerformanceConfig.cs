namespace Performance.Model;

public class PerformanceConfig
{
    public enum BackendType
    {
        OpenTelemetry,
        Sentry,
        None
    }
    public double SampleRate { get; init; }
    // Only required when the backend is OpenTelemetry; defaulted so other backends can bind without it.
    public string Endpoint { get; init; } = string.Empty;
    public BackendType Type { get; init; }
    
    public bool SendLogs { get; init; }
}