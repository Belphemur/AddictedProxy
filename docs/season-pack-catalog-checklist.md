# Season Pack Episode Catalog — Implementation Checklist

> **Status as of 2026-10-20:** Phases 1–9 are implemented and verified against the code. Only the *Verification* section is unchecked (build/tests could not be run in the doc-audit environment — no `dotnet` CLI available).

## Phase 1: Database Entity & Migration

- [x] Create `SeasonPackEntry` entity in `AddictedProxy.Database/Model/Shows/`
- [x] Add `ICollection<SeasonPackEntry> Entries` navigation to `SeasonPackSubtitle`
- [x] Add `DbSet<SeasonPackEntry>` to `EntityContext`
- [x] Generate EF Core migration (`dotnet ef migrations add AddSeasonPackEntries`)
- [x] Verify migration SQL creates correct indexes

## Phase 2: Repository

- [x] Create `ISeasonPackEntryRepository` interface in `AddictedProxy.Database/Repositories/Shows/`
- [x] Create `SeasonPackEntryRepository` implementation
- [x] Register in `BootstrapDatabase.ConfigureServices`

## Phase 3: ZIP Parsing & Catalog Service

- [x] Create `ISeasonPackCatalogService` interface in `AddictedProxy/Services/Provider/SeasonPack/`
- [x] Create `SeasonPackCatalogService` implementation with ZIP parsing logic
- [x] Register in relevant bootstrap class (`BootstrapProvider`)

## Phase 4: Update StoreSeasonPackJob

- [x] Inject `ISeasonPackCatalogService` into `StoreSeasonPackJob`
- [x] Call `CatalogAndPersistAsync` after successful S3 storage (skips re-catalog when already stored + cataloged, unless `ForceRedownload`)

## Phase 5: Update SeasonPackProvider (Self-Extraction)

- [x] Inject `ISeasonPackEntryRepository` (self-extraction goes through the entry repository; cataloging stays in `StoreSeasonPackJob`)
- [x] When episode requested + pack is stored + cataloged → self-extract from S3 ZIP (`GetEntryFileAsync`)
- [x] When episode not in catalog / out of `RangeStart`–`RangeEnd` → `EpisodeNotInSeasonPackException`
- [x] When pack not cataloged / ZIP missing from storage → fallback to upstream (existing behavior)

## Phase 6: Update Import/Refresh Jobs

- [x] In `ImportSuperSubtitlesJob` (show collection processing): enqueue `StoreSeasonPackJob` for new packs without `StoragePath`
- [x] In `RefreshSuperSubtitlesJob` (show collection processing): enqueue `StoreSeasonPackJob` for new packs without `StoragePath`

## Phase 7: One-Time Catchup Migration

- [x] Create `CatalogExistingSeasonPacksMigration` in `AddictedProxy/Migrations/Services/`
- [x] Implement `IMigration` with `[MigrationDate(2026, 3, 12)]`
- [x] Query stored season packs, download ZIPs from S3, catalog entries
  - Follow-ups also shipped: `RecatalogStoredSeasonPacksMigration` (`[MigrationDate(2026, 3, 19)]`) and `EnqueueSeasonPackRedownloadMigration` / `EnqueueSeasonPackRedownloadMigrationAgain`

## Phase 8: Update SubtitlesController Fallback

- [x] Update the season pack fallback path in `SubtitlesController` to use catalog entries (`pack.Entries` filtered by requested episode)
- [x] Include `Entries` navigation in season pack query
- [x] Offer pack only if it has an entry for the requested episode (packs with no entries yet are still offered — graceful degradation)

## Phase 9: Unit Tests

- [x] Test ZIP filename parsing (episode number, title, release groups) — `SeasonPackCatalogServiceParsingTests`
- [x] Test catalog service with sample ZIP blobs — `SeasonPackCatalogServiceTests`
- [x] Test self-extraction logic — `SeasonPackProviderTests.GetEntryFileAsync_*`
- [x] Test fallback behavior with cataloged vs uncataloged packs — `SeasonPackProviderTests.GetEntryFileAsync_*_FallsBackToUpstream`

## Verification

- [x] `dotnet build -c Release` passes *(verified 2026-10-20: 0 errors)*
- [x] `dotnet test -c Release` passes *(verified 2026-10-20: 112 passed / 3 skipped / 0 failed)*
- [ ] EF migration applies cleanly
- [ ] Existing season pack download flow still works
