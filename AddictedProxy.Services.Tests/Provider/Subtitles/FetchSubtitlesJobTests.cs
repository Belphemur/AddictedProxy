using AddictedProxy.Services.Provider.Subtitle.Jobs;
using FluentAssertions;

namespace AddictedProxy.Services.Tests.Provider.Subtitles;

[TestFixture]
public class FetchSubtitlesJobTests
{
    [Test]
    public void SceneMatchesFileName_MatchesGroupAfterFirstComma()
    {
        FetchSubtitlesJob.SceneMatchesFileName("NTb, playWEB, Kitsune", "Show.S01E01.1080p.WEB.playWEB.x264.srt")
                         .Should().BeTrue();
    }

    [Test]
    public void SceneMatchesFileName_MatchesFirstGroup()
    {
        FetchSubtitlesJob.SceneMatchesFileName("NTb, playWEB", "Show.S01E01.1080p.WEB.NTb.x264.srt")
                         .Should().BeTrue();
    }

    [Test]
    public void SceneMatchesFileName_MatchesLegacyPlusSeparatedScene()
    {
        FetchSubtitlesJob.SceneMatchesFileName("NTb+playWEB+Kitsune", "Show.S01E01.1080p.WEB.Kitsune.x264.srt")
                         .Should().BeTrue();
    }

    [Test]
    public void SceneMatchesFileName_MatchesDotAndDashSeparatedTokens()
    {
        FetchSubtitlesJob.SceneMatchesFileName("AMZN.WEB-DL", "Show.S01E01.1080p.AMZN.WEBRip.x264.srt")
                         .Should().BeTrue();
    }

    [Test]
    public void SceneMatchesFileName_IsCaseInsensitive()
    {
        FetchSubtitlesJob.SceneMatchesFileName("NTB, PLAYWEB", "Show.S01E01.1080p.WEB.playweb.x264.srt")
                         .Should().BeTrue();
    }

    [Test]
    public void SceneMatchesFileName_ReturnsFalseWhenNoTokenMatches()
    {
        FetchSubtitlesJob.SceneMatchesFileName("NTb, playWEB", "Show.S01E01.1080p.HDTV.x264.srt")
                         .Should().BeFalse();
    }

    [Test]
    public void SceneMatchesFileName_EmptySceneDoesNotMatchEverything()
    {
        FetchSubtitlesJob.SceneMatchesFileName("", "Show.S01E01.1080p.HDTV.x264.srt")
                         .Should().BeFalse();
    }

    [Test]
    public void SceneMatchesFileName_SeparatorOnlySceneDoesNotMatch()
    {
        FetchSubtitlesJob.SceneMatchesFileName(", ,", "Show.S01E01.1080p.HDTV.x264.srt")
                         .Should().BeFalse();
    }
}
