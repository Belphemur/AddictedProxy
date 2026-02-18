namespace AddictedProxy.Services.Provider.SuperSubtitles.Config;

/// <summary>
/// Configuration for the SuperSubtitles bulk import migration.
/// Bound from the "SuperSubtitles:Import" configuration section.
/// </summary>
public class SuperSubtitlesImportConfig
{
    public const string SectionName = "SuperSubtitles:Import";

    /// <summary>Number of shows to request per gRPC batch call.</summary>
    public int BatchSize { get; set; } = 60;

    /// <summary>Minimum delay in seconds between batch calls (random delay will be between Min and Max).</summary>
    public int MinDelaySeconds { get; set; } = 10;

    /// <summary>Maximum delay in seconds between batch calls (random delay will be between Min and Max).</summary>
    public int MaxDelaySeconds { get; set; } = 30;
}
