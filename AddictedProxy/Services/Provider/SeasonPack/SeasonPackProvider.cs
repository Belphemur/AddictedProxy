using System.IO.Compression;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Culture.Service;
using AddictedProxy.Storage.Extensions;
using AddictedProxy.Storage.Store;
using Grpc.Core;
using Hangfire;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Net.Http.Headers;
using SuperSubtitleClient.Service;

namespace AddictedProxy.Services.Provider.SeasonPack;

public class SeasonPackProvider : ISeasonPackProvider
{
    private readonly ISeasonPackSubtitleRepository _seasonPackSubtitleRepository;
    private readonly IStorageProvider _storageProvider;
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly ISeasonPackEntryRepository _entryRepository;
    private readonly ICultureParser _cultureParser;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<SeasonPackProvider> _logger;

    public SeasonPackProvider(
        ISeasonPackSubtitleRepository seasonPackSubtitleRepository,
        IStorageProvider storageProvider,
        ISuperSubtitlesClient superSubtitlesClient,
        ISeasonPackEntryRepository entryRepository,
        ICultureParser cultureParser,
        IBackgroundJobClient backgroundJobClient,
        ILogger<SeasonPackProvider> logger)
    {
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _storageProvider = storageProvider;
        _superSubtitlesClient = superSubtitlesClient;
        _entryRepository = entryRepository;
        _cultureParser = cultureParser;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _seasonPackSubtitleRepository.GetByUniqueIdAsync(uniqueId, token);
    }

    public Task<SeasonPackEntry?> GetEntryByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _entryRepository.FindByUniqueIdAsync(uniqueId, token);
    }

    public async Task<Results<FileStreamHttpResult, NotFound<string>>> GetSeasonPackZipAsync(Guid uniqueId, CancellationToken token)
    {
        var seasonPack = await _seasonPackSubtitleRepository.GetByUniqueIdAsync(uniqueId, token);
        if (seasonPack == null)
        {
            return TypedResults.NotFound($"Season pack ({uniqueId}) couldn't be found");
        }

        try
        {
            var stream = await GetSeasonPackZipStreamAsync(seasonPack, token);
            return TypedResults.Stream(
                stream,
                contentType: "application/zip",
                fileDownloadName: await BuildSeasonPackZipFileNameAsync(seasonPack, token),
                lastModified: seasonPack.StoredAt,
                entityTag: CreateEntityTag(seasonPack.UniqueId, seasonPack.StoredAt)
            );
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DataLoss)
        {
            return await HandleCorruptSeasonPackAsync(seasonPack, ex, $"Season pack ({uniqueId}) is corrupted and couldn't be downloaded", token);
        }
    }

    private async Task<Stream> GetSeasonPackZipStreamAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        if (seasonPack.StoragePath != null)
        {
            var stream = await _storageProvider.DownloadAsync(seasonPack.StoragePath, token);
            if (stream == null)
            {
                _logger.LogError("Storage returned null for season pack with path [{path}]", seasonPack.StoragePath);
                return await DownloadAndStoreFullZipAsync(seasonPack, token);
            }

            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
            return stream;
        }

        return await DownloadAndStoreFullZipAsync(seasonPack, token);
    }

    public async Task<Results<FileStreamHttpResult, NotFound<string>>> GetEntryFileAsync(Guid seasonPackUniqueId, Guid entryUniqueId, CancellationToken token)
    {
        var seasonPack = await _seasonPackSubtitleRepository.GetByUniqueIdAsync(seasonPackUniqueId, token);
        if (seasonPack == null)
        {
            return TypedResults.NotFound($"Season pack ({seasonPackUniqueId}) couldn't be found");
        }

        var entry = await _entryRepository.FindByUniqueIdAsync(entryUniqueId, token);
        if (entry == null || entry.SeasonPackSubtitleId != seasonPack.Id)
        {
            return TypedResults.NotFound($"Entry ({entryUniqueId}) not found in season pack ({seasonPackUniqueId})");
        }

        try
        {
            var stream = await GetEntryFileStreamAsync(seasonPack, entry, token);
            return TypedResults.Stream(
                stream,
                contentType: "text/srt",
                fileDownloadName: await BuildSeasonPackEpisodeFileNameAsync(seasonPack, entry.EpisodeNumber, token),
                lastModified: seasonPack.StoredAt,
                entityTag: CreateEntityTag(entry.UniqueId, seasonPack.StoredAt)
            );
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DataLoss)
        {
            return await HandleCorruptSeasonPackAsync(seasonPack, ex, $"Season pack ({seasonPackUniqueId}) is corrupted and entry ({entryUniqueId}) couldn't be downloaded", token);
        }
    }

    private async Task<Stream> GetEntryFileStreamAsync(SeasonPackSubtitle seasonPack, SeasonPackEntry entry, CancellationToken token)
    {
        if (seasonPack.StoragePath == null)
        {
            _logger.LogWarning("Season pack {SeasonPackId} has no storage path, falling back to upstream for episode {Episode}", seasonPack.Id, entry.EpisodeNumber);
            return await DownloadEpisodeFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
        }

        var zipStream = await _storageProvider.DownloadAsync(seasonPack.StoragePath, token);
        if (zipStream == null)
        {
            _logger.LogError("Storage returned null for season pack with path [{path}] during entry extraction", seasonPack.StoragePath);
            return await DownloadEpisodeFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
        }

        try
        {
            await using (zipStream)
            {
                await using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
                var zipEntry = archive.GetEntry(entry.FileName);
                if (zipEntry == null)
                {
                    _logger.LogWarning("Catalog entry {FileName} not found in ZIP for season pack {SeasonPackId}, falling back to upstream", entry.FileName, seasonPack.Id);
                    return await DownloadEpisodeFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
                }

                var result = new MemoryStream();
                await using var entryStream = await zipEntry.OpenAsync(token);
                await entryStream.CopyToAsync(result, token);
                result.ResetPosition();

                await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
                return result;
            }
        }
        catch (InvalidDataException ex)
        {
            _logger.LogError(ex, "Corrupt ZIP detected for season pack {SeasonPackId} at {StoragePath}, clearing storage and falling back to upstream", seasonPack.Id, seasonPack.StoragePath);
            seasonPack.StoragePath = null;
            seasonPack.StoredAt = null;
            await _seasonPackSubtitleRepository.SaveChangeAsync(token);
            // DownloadEpisodeFromUpstreamAsync will enqueue a re-store job since StoragePath is now null
            return await DownloadEpisodeFromUpstreamAsync(seasonPack, entry.EpisodeNumber, token);
        }
    }

    public async Task<Results<FileStreamHttpResult, NotFound<string>>> GetEpisodeFromUpstreamAsync(Guid seasonPackUniqueId, int episode, CancellationToken token)
    {
        var seasonPack = await _seasonPackSubtitleRepository.GetByUniqueIdAsync(seasonPackUniqueId, token);
        if (seasonPack == null)
        {
            return TypedResults.NotFound($"Season pack ({seasonPackUniqueId}) couldn't be found");
        }

        try
        {
            var stream = await GetEpisodeFromUpstreamStreamAsync(seasonPack, episode, token);
            return TypedResults.Stream(
                stream,
                contentType: "text/srt",
                fileDownloadName: await BuildSeasonPackEpisodeFileNameAsync(seasonPack, episode, token),
                lastModified: seasonPack.StoredAt,
                entityTag: CreateEntityTag(seasonPack.UniqueId, seasonPack.StoredAt)
            );
        }
        catch (EpisodeNotInSeasonPackException)
        {
            return TypedResults.NotFound($"Episode {episode} not found in season pack ({seasonPackUniqueId})");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DataLoss)
        {
            return await HandleCorruptSeasonPackAsync(seasonPack, ex, $"Season pack ({seasonPackUniqueId}) is corrupted and episode {episode} couldn't be downloaded", token);
        }
    }

    private async Task<Stream> GetEpisodeFromUpstreamStreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        if (seasonPack.RangeStart.HasValue && episode < seasonPack.RangeStart.Value)
        {
            throw new EpisodeNotInSeasonPackException(episode, $"Episode {episode} is before season pack range start ({seasonPack.RangeStart.Value})");
        }

        if (seasonPack.RangeEnd.HasValue && episode > seasonPack.RangeEnd.Value)
        {
            throw new EpisodeNotInSeasonPackException(episode, $"Episode {episode} is after season pack range end ({seasonPack.RangeEnd.Value})");
        }

        return await DownloadEpisodeFromUpstreamAsync(seasonPack, episode, token);
    }

    private async Task<Stream> DownloadEpisodeFromUpstreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token)
    {
        try
        {
            var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: episode, cancellationToken: token);
            await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);

            return new MemoryStream(response.Content.ToByteArray());
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound or StatusCode.FailedPrecondition)
        {
            throw new EpisodeNotInSeasonPackException(episode, e.Status.Detail);
        }
        finally
        {
            if (seasonPack.StoragePath == null)
            {
                _backgroundJobClient.Enqueue<StoreSeasonPackJob>(job => job.DownloadAndStoreAsync(new StoreSeasonPackJob.JobData(seasonPack.UniqueId), null!, default));
            }
        }
    }

    private async Task<Stream> DownloadAndStoreFullZipAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        var response = await _superSubtitlesClient.DownloadSubtitleAsync(seasonPack.ExternalId.ToString(), episode: null, cancellationToken: token);
        var blob = response.Content.ToByteArray();

        if (seasonPack.StoragePath == null)
        {
            _backgroundJobClient.Enqueue<StoreSeasonPackJob>(job => job.StoreAsync(new StoreSeasonPackJob.JobData(seasonPack.UniqueId), blob, null, default));
        }

        await _seasonPackSubtitleRepository.IncrementDownloadCountAsync(seasonPack, token);
        return new MemoryStream(blob);
    }

    private async Task<NotFound<string>> HandleCorruptSeasonPackAsync(SeasonPackSubtitle seasonPack, RpcException exception, string message, CancellationToken token)
    {
        _logger.LogWarning(exception, "Soft-deleting corrupt season pack {SeasonPackUniqueId} after upstream reported DataLoss", seasonPack.UniqueId);
        await _seasonPackSubtitleRepository.SoftDeleteAsync(seasonPack, token);
        return TypedResults.NotFound(message);
    }

    private async Task<string> BuildSeasonPackZipFileNameAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        var languageCode = await GetLanguageCodeAsync(seasonPack, token);
        return $"{seasonPack.TvShow.Name.Replace(" ", ".")}.S{seasonPack.Season:D2}.{languageCode}.zip";
    }

    private async Task<string> BuildSeasonPackEpisodeFileNameAsync(SeasonPackSubtitle seasonPack, int episodeNumber, CancellationToken token)
    {
        var languageCode = await GetLanguageCodeAsync(seasonPack, token);
        return $"{seasonPack.TvShow.Name.Replace(" ", ".")}.S{seasonPack.Season:D2}E{episodeNumber:D2}.{languageCode}.srt";
    }

    private async Task<string> GetLanguageCodeAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        return (await _cultureParser.FromStringAsync(seasonPack.Language, token))?.TwoLetterISOLanguageName.ToLowerInvariant()
               ?? seasonPack.LanguageIsoCode?.ToLowerInvariant()
               ?? "unknown";
    }

    private static EntityTagHeaderValue CreateEntityTag(Guid uniqueId, DateTime? storedAt)
    {
        return new EntityTagHeaderValue('"' + $"{uniqueId}{(storedAt.HasValue ? "-" + storedAt.Value.Ticks : "")}" + '"');
    }
}