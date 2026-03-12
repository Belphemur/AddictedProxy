using System.IO.Compression;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.SeasonPack;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AddictedProxy.Services.Tests.Provider.SeasonPack;

[TestFixture]
public class SeasonPackCatalogServiceTests
{
    private ISeasonPackEntryRepository _entryRepository = null!;
    private ILogger<SeasonPackCatalogService> _logger = null!;
    private SeasonPackCatalogService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _entryRepository = Substitute.For<ISeasonPackEntryRepository>();
        _logger = Substitute.For<ILogger<SeasonPackCatalogService>>();
        _sut = new SeasonPackCatalogService(_entryRepository, _logger);
    }

    private static byte[] CreateZipBlob(params string[] fileNames)
    {
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var name in fileNames)
            {
                var entry = archive.CreateEntry(name);
                using var writer = new StreamWriter(entry.Open());
                writer.Write("dummy content");
            }
        }

        return ms.ToArray();
    }

    [Test]
    public async Task CatalogAndPersistAsync_ValidZip_CallsBulkUpsert()
    {
        var seasonPack = new SeasonPackSubtitle { Id = 42, Filename = "test.zip" };
        var blob = CreateZipBlob("Show.S01E01.srt", "Show.S01E02.srt");

        await _sut.CatalogAndPersistAsync(seasonPack, blob, CancellationToken.None);

        await _entryRepository.Received(1).BulkUpsertAsync(
            Arg.Is<IEnumerable<SeasonPackEntry>>(entries => entries.Count() == 2),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task CatalogAndPersistAsync_EmptyZip_DoesNotCallBulkUpsert()
    {
        var seasonPack = new SeasonPackSubtitle { Id = 42, Filename = "test.zip" };
        var blob = CreateZipBlob("readme.txt");

        await _sut.CatalogAndPersistAsync(seasonPack, blob, CancellationToken.None);

        await _entryRepository.DidNotReceive().BulkUpsertAsync(
            Arg.Any<IEnumerable<SeasonPackEntry>>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task IsCatalogedAsync_DelegatesToRepository()
    {
        _entryRepository.HasEntriesAsync(42, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.IsCatalogedAsync(42, CancellationToken.None);

        result.Should().BeTrue();
        await _entryRepository.Received(1).HasEntriesAsync(42, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task IsCatalogedAsync_ReturnsFalse_WhenNoEntries()
    {
        _entryRepository.HasEntriesAsync(42, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.IsCatalogedAsync(42, CancellationToken.None);

        result.Should().BeFalse();
    }
}
