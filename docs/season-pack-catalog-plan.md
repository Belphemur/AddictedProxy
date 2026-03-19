# Season Pack Episode Catalog & Self-Extraction

## Overview

Introduce a **`SeasonPackEntry`** child table that catalogs every episode file inside each season pack ZIP.
This enables:

1. **Reliable episode availability checks** — the controller knows exactly which episodes a pack contains before offering it as a fallback.
2. **Self-extraction from stored ZIPs** — when a pack is already stored in S3 and cataloged, the provider extracts the requested SRT locally instead of calling upstream.
3. **Metadata enrichment** — episode titles and release groups are parsed from ZIP entry filenames.

## Entity: `SeasonPackEntry`

| Column                    | Type       | Notes                                                                 |
| ------------------------- | ---------- | --------------------------------------------------------------------- |
| `Id`                      | `long`     | PK, identity                                                          |
| `SeasonPackSubtitleId`    | `long`     | FK → `SeasonPackSubtitle.Id`, cascade delete                          |
| `EpisodeNumber`           | `int`      | Parsed via `S\d{2}E(\d{2,3})` (case-insensitive)                      |
| `FileName`                | `string`   | Original entry name inside the ZIP                                    |
| `EpisodeTitle`            | `string?`  | Parsed from filename (text between `SxxExx` and first release marker) |
| `ReleaseGroup`            | `string?`  | Comma-separated release groups parsed from filename                   |
| `CreatedAt` / `UpdatedAt` | `DateTime` | Inherited from `BaseEntity`                                           |

**Indexes:**

- `(SeasonPackSubtitleId, FileName)` — unique; allows dubtitle variants (same episode, different file)
- `(SeasonPackSubtitleId, EpisodeNumber)` — non-unique; fast lookups for a specific episode

## ZIP Filename Parsing Strategy

### Episode Number

Regex: `S\d{2}E(\d{2,3})` (case-insensitive). Entries that don't match are skipped (e.g., `nfo` files, `readme.txt`).

### Episode Title

Text between the `SxxExx` token and the first known release marker, with dots replaced by spaces and trimmed.

**Known release markers** (case-insensitive):
`AMZN`, `NF`, `ATVP`, `HMAX`, `DSNP`, `PCOK`, `PMTP`, `iT`, `WEB-DL`, `WEBRip`, `BluRay`, `BDRip`, `HDTV`, `DVDRip`, `720p`, `1080p`, `2160p`, `4K`

**Example:** `Scarpetta.S01E03.Dot.AMZN.WEB-DL.en.srt` → Episode title: `"Dot"`, Release groups: `"AMZN, WEB-DL"`

### Release Groups

All matched release markers from the filename, comma-separated.

## Service: `ISeasonPackCatalogService`

Located in `AddictedProxy/Services/Provider/SeasonPack/`.

```csharp
public interface ISeasonPackCatalogService
{
    /// <summary>
    /// Parse a ZIP blob, extract SeasonPackEntry records, and persist them.
    /// </summary>
    Task CatalogAndPersistAsync(SeasonPackSubtitle seasonPack, byte[] zipBlob, CancellationToken token);

    /// <summary>
    /// Check whether a season pack has been cataloged.
    /// </summary>
    Task<bool> IsCatalogedAsync(long seasonPackSubtitleId, CancellationToken token);
}
```

### ZIP Parsing Logic

1. Open `ZipArchive` from blob (read-only).
2. For each entry, apply episode-number regex.
3. Parse episode title and release groups from filename.
4. Build `SeasonPackEntry` list.
5. Synchronize via `ISeasonPackEntryRepository.BulkSyncAsync` so removed ZIP entries are deleted.

## Updated Data Flow

### Store Flow (StoreSeasonPackJob)

```
ZIP blob arrives
  → Store to S3 (existing)
  → ISeasonPackCatalogService.CatalogAndPersistAsync(seasonPack, blob)
  → SeasonPackEntry rows written
```

### Download Flow (SeasonPackProvider)

```
Episode requested?
  ├── YES: Is pack stored + cataloged?
  │     ├── YES: Does catalog contain this episode?
  │     │     ├── YES → self-extract from S3 ZIP, return SRT stream
  │     │     └── NO  → throw EpisodeNotInSeasonPackException
  │     └── NO  → fallback to upstream DownloadFromUpstreamAsync
  └── NO: Return full ZIP (existing logic unchanged)
```

### Search Fallback (SubtitlesController)

```
No episode subtitles found
  → Query season packs for show+season
  → For each pack:
  │   ├── Has entries? → offer only if entry exists for requested episode
  │   └── No entries?  → offer blindly (graceful degradation during backfill)
  → Return SubtitleDto list
```

## Import/Refresh Job Updates

After `IngestSeasonPacksAsync` completes in both `ImportSuperSubtitlesJob` and `RefreshSuperSubtitlesJob`,
enqueue `StoreSeasonPackJob` for each newly-ingested pack that has no `StoragePath`.
The store job already handles the download + store; with the catalog service injected, it will also
catalog the ZIP entries in the same pass.

## One-Time Catchup Migrations

`CatalogExistingSeasonPacksMigration` implements `IMigration` with `[MigrationDate(2026, 3, 12)]`.

1. Query stored `SeasonPackSubtitle` rows that do not yet have catalog entries.
2. For each, download the ZIP from S3 and catalog via `ISeasonPackCatalogService`.
3. Report progress via Hangfire console.
4. Season packs without a `StoragePath` are skipped — they'll be cataloged when next downloaded.

`RecatalogStoredSeasonPacksMigration` implements `IMigration` with `[MigrationDate(2026, 3, 19)]`.

1. Query all `SeasonPackSubtitle` rows where `StoragePath IS NOT NULL`.
2. For each, download the ZIP from storage and recatalog via `ISeasonPackCatalogService`.
3. Synchronize entries so stale rows from older ZIP contents are removed.
4. Report progress via Hangfire console.

## Design Decisions

| Decision                                                    | Rationale                                                                         |
| ----------------------------------------------------------- | --------------------------------------------------------------------------------- |
| Unique index on `(SeasonPackSubtitleId, FileName)`          | A pack may contain both regular and dubtitle variants for the same episode number |
| Non-unique index on `(SeasonPackSubtitleId, EpisodeNumber)` | Fast lookups; multiple files per episode are valid                                |
| Self-extract when stored + cataloged, upstream fallback     | Reduces upstream calls for common case; graceful for uncataloged packs            |
| Graceful degradation for uncataloged packs                  | During backfill or failed recataloging, packs without entries are still offered blindly |
| IMigration framework for catchup                            | Consistent with existing one-time job pattern; auto-discovered, tracked           |
| Parse release groups from filename                          | Enriches metadata for display without requiring upstream changes                  |
