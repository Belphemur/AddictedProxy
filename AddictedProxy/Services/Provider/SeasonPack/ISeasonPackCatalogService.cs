using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.SeasonPack;

public interface ISeasonPackCatalogService
{
    /// <summary>
    /// Parse a ZIP blob, extract <see cref="SeasonPackEntry"/> records, and persist them.
    /// </summary>
    Task CatalogAndPersistAsync(SeasonPackSubtitle seasonPack, byte[] zipBlob, CancellationToken token);

    /// <summary>
    /// Check whether a season pack has been cataloged (has any entries).
    /// </summary>
    Task<bool> IsCatalogedAsync(long seasonPackSubtitleId, CancellationToken token);
}
