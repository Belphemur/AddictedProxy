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
    public string Endpoint { get; init; }
    public BackendType Type { get; init; }
    
    public bool SendLogs { get; init; }
}