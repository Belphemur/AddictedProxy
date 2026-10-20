#region

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Model;
using AddictedProxy.Upstream.Service.Exception;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;

#endregion

namespace AddictedProxy.Upstream.Service;

public partial class Parser
{
    private readonly IHtmlParser _parser;
    private readonly ILogger<Parser> _logger;
    private readonly ICultureParser _cultureParser;

    [GeneratedRegex("(?<completion>\\d+\\.?\\d+)%", RegexOptions.Compiled)]
    private static partial Regex CompletionRegex();

    [GeneratedRegex("(?<usage>\\d{1,2}) of (?<total>\\d{1,2})", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex DownloadUsageRegex();

    public Parser(IHtmlParser parser, ILogger<Parser> logger, ICultureParser cultureParser)
    {
        _parser = parser;
        _logger = logger;
        _cultureParser = cultureParser;
    }

    /// <summary>
    /// Parse the download usage of the profile page
    /// </summary>
    /// <param name="html"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<DownloadUsage> GetDownloadUsageAsync(Stream html, CancellationToken token)
    {
        var document = await _parser.ParseDocumentAsync(html, token);
        var downloads = document.QuerySelector<IHtmlAnchorElement>(".tabel a[href=\"mydownloads.php\"]");
        var text = downloads?.Text();
        if (text == null)
        {
            throw new NothingToParseException("Couldn't find HTML DOM for the download usage");
        }

        var match = DownloadUsageRegex().Match(text);
        if (!match.Success)
        {
            throw new NothingToParseException($"Couldn't parse the download usage:{text}");
        }

        return new DownloadUsage(int.Parse(match.Groups["usage"].Value), int.Parse(match.Groups["total"].Value));
    }

    public async IAsyncEnumerable<TvShow> GetShowsAsync(Stream html, [EnumeratorCancellation] CancellationToken token)
    {
        var document = await _parser.ParseDocumentAsync(html, token);
        var selectShow = document.QuerySelector<IHtmlSelectElement>("#qsShow");

        if (selectShow == null)
        {
            throw new NothingToParseException("No shows found", null);
        }

        foreach (var option in selectShow.Options)
        {
            var id = int.Parse(option.Value);
            if (id == 0)
            {
                continue;
            }

            yield return new TvShow
            {
                ExternalId = id,
                Name = option.Text,
                LastUpdated = DateTime.UtcNow,
                Discovered = DateTime.UtcNow
            };
        }
    }

    public async IAsyncEnumerable<int> GetSeasonsAsync(Stream html, [EnumeratorCancellation] CancellationToken token)
    {
        var document = await _parser.ParseDocumentAsync(html, token);
        var selectSeason = document.QuerySelector<IHtmlSelectElement>("#qsiSeason");
        if (selectSeason == null)
        {
            throw new NothingToParseException("No season found", null);
        }

        if (selectSeason.Options.Length == 1 && !int.TryParse(selectSeason.Options[0].Value, out _))
        {
            throw new NothingToParseException("No season found", null);
        }

        foreach (var option in selectSeason.Options)
        {
            if (option.Text.ToLowerInvariant() == "season")
            {
                continue;
            }

            yield return int.Parse(option.Value);
        }
    }

    public async IAsyncEnumerable<Episode> GetSeasonSubtitlesAsync(Stream html, [EnumeratorCancellation] CancellationToken token)
    {
        var document = await _parser.ParseDocumentAsync(html, token);
        var table = document.QuerySelector("#season")?.QuerySelector<IHtmlTableElement>("table");

        var subtitlesRows = new List<SubtitleRow>();

        if (table == null)
        {
            throw new NothingToParseException("Can't find episode table");
        }

        foreach (var row in table.Rows.Skip(1))
        {
            if (row.Cells.Length <= 2) continue;
            var subtitleRow = ParseSubtitleRow(row);
            if (subtitleRow == null)
            {
                _logger.LogWarning("Couldn't parse subtitle row (missing download link or unexpected column count), skipping it");
                continue;
            }

            subtitlesRows.Add(subtitleRow);
        }

        if (subtitlesRows.Count == 0)
        {
            _logger.LogWarning("Couldn't find subtitles");
            yield break;
        }

        var episodeGroups = subtitlesRows.GroupBy(r => (r.Season, r.Number));

        foreach (var episodeGroup in episodeGroups.Where(groups => groups.Any()))
        {
            var episode = new Episode
            {
                Title = episodeGroup.First().Title,
                Number = episodeGroup.First().Number,
                Season = episodeGroup.First().Season,
                Discovered = DateTime.UtcNow,
                ExternalIds =
                [
                    new EpisodeExternalId
                    {
                        Source = DataSource.Addic7ed,
                        ExternalId = episodeGroup.First().EpisodeId.ToString()
                    }
                ]
            };

            var subtitles = new List<Subtitle>();
            foreach (var subtitleRow in episodeGroup)
            {
                var languageIsoCode = (await _cultureParser.FromStringAsync(subtitleRow.Language, token))?.Name;
                subtitles.Add(new Subtitle
                {
                    Scene = NormalizeScene(subtitleRow.Scene),
                    Corrected = subtitleRow.Corrected,
                    DownloadUri = subtitleRow.DownloadUri,
                    Qualities = subtitleRow.HD ? VideoQuality.Q720P | VideoQuality.Q1080P : VideoQuality.None,
                    HearingImpaired = subtitleRow.HearingImpaired,
                    Language = subtitleRow.Language,
                    Completed = subtitleRow.Completed,
                    CompletionPct = subtitleRow.CompletionPercentage,
                    Discovered = DateTime.UtcNow,
                    Episode = episode,
                    Version = subtitleRow.Version,
                    LanguageIsoCode = languageIsoCode,
                    Source = DataSource.Addic7ed,
                    ExternalId = subtitleRow.DownloadUri.ToString()
                });
            }
            episode.Subtitles = subtitles;
            yield return episode;
        }
    }

    /// <summary>
    /// Parse one row of the season subtitles table.
    /// The table always renders the same 10 columns; returns null when the row
    /// doesn't match that layout (logged and skipped by the caller).
    /// </summary>
    private SubtitleRow? ParseSubtitleRow(IHtmlTableRowElement row)
    {
        var cells = row.Cells;
        if (cells.Length < 10)
        {
            return null;
        }

        var state = cells[5].TextContent;
        var match = CompletionRegex().Match(state);
        //Consider the subtitle completed if there isn't a percentage
        var completed = !match.Success;
        var completionPercentage = completed ? 100 : double.Parse(match.Groups["completion"].Value);

        var downloadUri = cells[9].FirstElementChild?.GetAttribute("href");
        if (downloadUri == null)
        {
            return null;
        }

        var splitOnSlash = downloadUri
            .Replace("/updated/", "")
            .Replace("/original/", "")
            .Split('/');

        return new SubtitleRow
        {
            Season = int.Parse(cells[0].TextContent),
            Number = int.Parse(cells[1].TextContent),
            Title = cells[2].TextContent,
            Language = cells[3].TextContent.Trim(),
            Scene = cells[4].TextContent,
            Completed = completed,
            CompletionPercentage = completionPercentage,
            HearingImpaired = cells[6].TextContent.Length > 0,
            Corrected = cells[7].TextContent.Length > 0,
            HD = cells[8].TextContent.Length > 0,
            DownloadUri = new Uri(downloadUri, UriKind.Relative),
            EpisodeId = int.Parse(splitOnSlash[1]),
            Version = int.Parse(splitOnSlash[^1])
        };
    }

    /// <summary>
    /// Addic7ed joins multiple release groups with '+' in the version column
    /// (e.g. "NTb+playWEB+Kitsune"). Splits them into the comma-separated format
    /// used by the SuperSubtitles source.
    /// </summary>
    private static string NormalizeScene(string scene)
    {
        var groups = scene.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return groups.Length > 0 ? string.Join(", ", groups) : scene.Trim();
    }
}