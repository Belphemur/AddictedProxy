namespace Performance.Model;

public class PerformanceConfig
{
    public enum BackendType
    {
        OpenTelemetry,
        Sentry
    }
    public double SampleRate { get; init; }
    public string Endpoint { get; init; }
    public BackendType Type { get; init; }
    
    public bool SendLogs { get; init; }
}