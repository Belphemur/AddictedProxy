namespace AddictedProxy.Upstream.Model;

/// <summary>
/// A subtitle row scraped from the Addic7ed season page.
/// The season table always renders the same columns, so every field is always populated.
/// </summary>
internal class SubtitleRow
{
    public required int Season { get; init; }
    public required int Number { get; init; }
    public required string Title { get; init; }
    public required string Language { get; init; }
    public required string Scene { get; init; }
    public required bool Completed { get; init; }

    public required double CompletionPercentage { get; init; }
    public required bool HearingImpaired { get; init; }
    public required bool Corrected { get; init; }
    public required bool HD { get; init; }
    public required Uri DownloadUri { get; init; }

    public required int EpisodeId { get; init; }
    public required int Version { get; init; }
}
