using System.IO.Compression;
using System.Text.RegularExpressions;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;

namespace AddictedProxy.Services.Provider.SeasonPack;

public partial class SeasonPackCatalogService : ISeasonPackCatalogService
{
    private readonly ISeasonPackEntryRepository _entryRepository;
    private readonly ILogger<SeasonPackCatalogService> _logger;



    public SeasonPackCatalogService(ISeasonPackEntryRepository entryRepository, ILogger<SeasonPackCatalogService> logger)
    {
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public async Task CatalogAndPersistAsync(SeasonPackSubtitle seasonPack, byte[] zipBlob, CancellationToken token)
    {
        var entries = ParseZipEntries(seasonPack.Id, zipBlob);

        if (entries.Count == 0)
        {
            _logger.LogWarning("No SRT entries found in season pack {SeasonPackId} ({Filename})", seasonPack.Id, seasonPack.Filename);
            return;
        }

        await _entryRepository.BulkUpsertAsync(entries, token);
        _logger.LogInformation("Cataloged {Count} entries for season pack {SeasonPackId}", entries.Count, seasonPack.Id);
    }

    public Task<bool> IsCatalogedAsync(long seasonPackSubtitleId, CancellationToken token)
    {
        return _entryRepository.HasEntriesAsync(seasonPackSubtitleId, token);
    }

    /// <summary>
    /// Parse a ZIP blob and return <see cref="SeasonPackEntry"/> records for each SRT entry.
    /// </summary>
    internal static List<SeasonPackEntry> ParseZipEntries(long seasonPackSubtitleId, byte[] zipBlob)
    {
        var entries = new List<SeasonPackEntry>();

        using var stream = new MemoryStream(zipBlob);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            var fileName = entry.FullName;
            var match = EpisodeNumberRegex().Match(fileName);
            if (!match.Success)
            {
                continue;
            }

            var episodeNumber = int.Parse(match.Groups[1].Value);
            var (episodeTitle, releaseGroup) = ParseTitleAndReleaseGroups(fileName, match);

            entries.Add(new SeasonPackEntry
            {
                SeasonPackSubtitleId = seasonPackSubtitleId,
                EpisodeNumber = episodeNumber,
                FileName = fileName,
                EpisodeTitle = episodeTitle,
                ReleaseGroup = releaseGroup
            });
        }

        return entries;
    }

    /// <summary>
    /// Extract the episode title and release groups from the filename text after the SxxExx token.
    /// </summary>
    private static (string? EpisodeTitle, string? ReleaseGroup) ParseTitleAndReleaseGroups(string fileName, Match episodeMatch)
    {
        // Get the text after SxxExx
        var afterEpisode = fileName[(episodeMatch.Index + episodeMatch.Length)..];

        // Remove file extension
        var extIndex = afterEpisode.LastIndexOf('.');
        if (extIndex > 0)
        {
            // Check if the extension is a known media extension (not part of the name)
            var ext = afterEpisode[(extIndex + 1)..];
            if (ext.Equals("srt", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals("sub", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals("ass", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals("ssa", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals("idx", StringComparison.OrdinalIgnoreCase))
            {
                afterEpisode = afterEpisode[..extIndex];
            }
        }

        // Find release markers using pattern matching against token boundaries
        var markerMatches = ReleaseMarkerPattern().Matches(afterEpisode);
        var matchedMarkers = new List<string>();
        var firstMarkerIndex = afterEpisode.Length;

        foreach (Match markerMatch in markerMatches)
        {
            matchedMarkers.Add(markerMatch.Value);
            if (markerMatch.Index < firstMarkerIndex)
            {
                firstMarkerIndex = markerMatch.Index;
            }
        }

        // Episode title is text between SxxExx and first release marker
        string? episodeTitle = null;
        if (firstMarkerIndex > 0)
        {
            var titlePart = afterEpisode[..firstMarkerIndex]
                .Replace('.', ' ')
                .Replace('_', ' ')
                .Trim();

            // Remove leading dot/separator
            titlePart = titlePart.TrimStart('.', '-', ' ');

            if (!string.IsNullOrWhiteSpace(titlePart))
            {
                episodeTitle = titlePart;
            }
        }

        var releaseGroup = matchedMarkers.Count > 0 ? string.Join(", ", matchedMarkers) : null;

        return (episodeTitle, releaseGroup);
    }

    [GeneratedRegex(@"S\d{2}E(\d{2,3})", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodeNumberRegex();

    /// <summary>
    /// Matches common release markers in scene filenames using structural patterns
    /// (e.g. \d{3,4}p for any resolution) so new variants are caught automatically.
    /// Each marker must be a complete dot/dash/space/underscore-delimited token.
    /// </summary>
    [GeneratedRegex(@"(?<=[.\-_ ]|^)(\d{3,4}p|\dK|WEB[-.]?DL|WEB[-.]?Rip|Blu[-.]?Ray|BDRip|HDTV|DVDRip|HDRip|PDTV|SDTV|x26[45]|H\.?26[45]|HEVC|XviD|AV1|VP\d|PROPER|REPACK|INTERNAL|REMUX)(?=[.\-_ ]|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseMarkerPattern();
}
