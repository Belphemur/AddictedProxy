using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Model;
using AddictedProxy.Services.Addic7ed.Exception;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AddictedProxy.Services.Addic7ed
{
    public class Parser
    {
        private readonly IHtmlParser _parser;

        public Parser(IHtmlParser parser)
        {
            _parser = parser;
        }

        public async IAsyncEnumerable<TvShow> GetShowsAsync(Stream html, [EnumeratorCancellation] CancellationToken token)
        {
            var document   = await _parser.ParseDocumentAsync(html, token);
            var selectShow = document.QuerySelector("#qsShow") as IHtmlSelectElement;

            if (selectShow == null)
                throw new NothingToParseException("No shows found", null);

            foreach (var option in selectShow.Options)
            {
                yield return new TvShow
                {
                    Id   = int.Parse(option.Value),
                    Name = option.Text
                };
            }
        }

        public async Task<int> GetNumberOfSeasonsAsync(Stream html, CancellationToken token)
        {
            var document     = await _parser.ParseDocumentAsync(html, token);
            var selectSeason = document.QuerySelector("#qsiSeason") as IHtmlSelectElement;

            return selectSeason?.Options?.Length > 0 ? selectSeason.Options.Length - 1 : throw new NothingToParseException("No season found", null);
        }

        public async IAsyncEnumerable<Episode> GetSeasonSubtitlesAsync(Stream html, [EnumeratorCancellation] CancellationToken token)
        {
            var               document = await _parser.ParseDocumentAsync(html, token);
            IHtmlTableElement table;
            try
            {
                table = document.QuerySelector("#season").QuerySelector("table") as IHtmlTableElement;
            }
            catch (NullReferenceException e)
            {
                throw new NothingToParseException("Problem while parsing episodes", e);
            }

            var subtitlesRows = new List<SubtitleRow>();

            if (table == null)
            {
                throw new NothingToParseException("Can't find episode table", null);
            }

            foreach (var row in table.Rows.Skip(1))
                if (row.Cells.Length > 2)
                {
                    var subtitleRow = new SubtitleRow();

                    for (var i = 0; i < row.Cells.Length; i++)
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
                                subtitleRow.Language = row.Cells[i].TextContent;
                                break;
                            case 4:
                                subtitleRow.Version = row.Cells[i].TextContent;
                                break;
                            case 5:
                                var state = row.Cells[i].TextContent;
                                if (!state.Contains("%") && state.Contains("Completed"))
                                    subtitleRow.Completed = true;
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
                                subtitleRow.EpisodeId =
                                    int.Parse(
                                        downloadUri.Replace("/updated/", "").Replace("/original/", "")
                                                   .Split('/')[1]);
                                subtitleRow.DownloadUri = new Uri(downloadUri.Replace("/updated/", "/download/").Replace("/original/", "/download/"), UriKind.Relative);
                                break;
                        }

                    subtitlesRows.Add(subtitleRow);
                }

            if (!subtitlesRows.Any())
                throw new NothingToParseException("No subtitle", null);

            var episodeGroups = subtitlesRows.GroupBy(r => r.EpisodeId).ToList();
            foreach (var episode in from episodeGroup in episodeGroups
                                    where episodeGroup.Any()
                                    let subtitles = episodeGroup.Select(subtitleRow => new Subtitle
                                    {
                                        Version         = subtitleRow.Version,
                                        Corrected       = subtitleRow.Corrected,
                                        DownloadUri     = subtitleRow.DownloadUri,
                                        HD              = subtitleRow.HD,
                                        HearingImpaired = subtitleRow.HearingImpaired,
                                        Language        = subtitleRow.Language,
                                        Completed       = subtitleRow.Completed
                                    })
                                    select new Episode
                                    {
                                        Title     = episodeGroup.First().Title,
                                        Number    = episodeGroup.First().Number,
                                        Season    = episodeGroup.First().Season,
                                        Id        = episodeGroup.First().EpisodeId,
                                        Subtitles = subtitles.ToArray()
                                    })
                yield return episode;
        }
    }
}