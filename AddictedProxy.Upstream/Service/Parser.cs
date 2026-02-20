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

        if (selectSeason.Options.Length == 1)
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
        IHtmlTableElement table;
        try
        {
            table = document.QuerySelector("#season").QuerySelector<IHtmlTableElement>("table");
        }
        catch (NullReferenceException e)
        {
            throw new NothingToParseException("Problem while parsing episodes", e);
        }

        var subtitlesRows = new List<SubtitleRow>();

        if (table == null)
        {
            throw new NothingToParseException("Can't find episode table");
        }

        foreach (var row in table.Rows.Skip(1))
        {
            if (row.Cells.Length <= 2) continue;
            var subtitleRow = new SubtitleRow();

            for (var i = 0; i < row.Cells.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        subtitleRow.Season = int.Parse(row.Cells[i].TextContent);
                        break;
                    case 1:
                        subtitleRow.Number = int.Parse(row.Cells[i].TextContent);
                        break;
                    case 2:
                        subtitleRow.Title = row.Cells[i].TextContent;
                        break;
                    case 3:
                        subtitleRow.Language = row.Cells[i].TextContent.Trim();
                        break;
                    case 4:
                        subtitleRow.Scene = row.Cells[i].TextContent;
                        break;
                    case 5:
                        var state = row.Cells[i].TextContent;
                        var match = CompletionRegex().Match(state);
                        //Consider the subtitle completed if there isn't a percentage
                        if (!match.Success)
                        {
                            subtitleRow.Completed = true;
                            subtitleRow.CompletionPercentage = 100;
                            break;
                        }

                        subtitleRow.CompletionPercentage = double.Parse(match.Groups["completion"].Value);

                        break;
                    case 6:
                        subtitleRow.HearingImpaired = row.Cells[i].TextContent.Length > 0;
                        break;
                    case 7:
                        subtitleRow.Corrected = row.Cells[i].TextContent.Length > 0;
                        break;
                    case 8:
                        subtitleRow.HD = row.Cells[i].TextContent.Length > 0;
                        break;
                    case 9:
                        var downloadUri = row.Cells[i].FirstElementChild.Attributes["href"].Value;
                        var splitOnSlash = downloadUri
                            .Replace("/updated/", "")
                            .Replace("/original/", "")
                            .Split('/');
                        subtitleRow.EpisodeId = int.Parse(splitOnSlash[1]);
                        subtitleRow.Version = int.Parse(splitOnSlash[^1]);
                        subtitleRow.DownloadUri = new Uri(downloadUri, UriKind.Relative);
                        break;
                }
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
                    Scene = subtitleRow.Scene.Trim(),
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
}