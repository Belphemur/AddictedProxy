# Season Pack Episode Catalog — Implementation Checklist

## Phase 1: Database Entity & Migration

- [ ] Create `SeasonPackEntry` entity in `AddictedProxy.Database/Model/Shows/`
- [ ] Add `ICollection<SeasonPackEntry> Entries` navigation to `SeasonPackSubtitle`
- [ ] Add `DbSet<SeasonPackEntry>` to `EntityContext`
- [ ] Generate EF Core migration (`dotnet ef migrations add AddSeasonPackEntries`)
- [ ] Verify migration SQL creates correct indexes

## Phase 2: Repository

- [ ] Create `ISeasonPackEntryRepository` interface in `AddictedProxy.Database/Repositories/Shows/`
- [ ] Create `SeasonPackEntryRepository` implementation
- [ ] Register in `BootstrapDatabase.ConfigureServices`

## Phase 3: ZIP Parsing & Catalog Service

- [ ] Create `ISeasonPackCatalogService` interface in `AddictedProxy/Services/Provider/SeasonPack/`
- [ ] Create `SeasonPackCatalogService` implementation with ZIP parsing logic
- [ ] Register in relevant bootstrap class

## Phase 4: Update StoreSeasonPackJob

- [ ] Inject `ISeasonPackCatalogService` into `StoreSeasonPackJob`
- [ ] Call `CatalogAndPersistAsync` after successful S3 storage

## Phase 5: Update SeasonPackProvider (Self-Extraction)

- [ ] Inject `ISeasonPackCatalogService` and `ISeasonPackEntryRepository`
- [ ] When episode requested + pack is stored + cataloged → self-extract from S3 ZIP
- [ ] When episode not in catalog → throw `EpisodeNotInSeasonPackException`
- [ ] When pack not cataloged → fallback to upstream (existing behavior)

## Phase 6: Update Import/Refresh Jobs

- [ ] In `ImportSuperSubtitlesJob.ProcessShowCollectionAsync`: enqueue `StoreSeasonPackJob` for new packs without `StoragePath`
- [ ] In `RefreshSuperSubtitlesJob.ProcessShowCollectionAsync`: enqueue `StoreSeasonPackJob` for new packs without `StoragePath`

## Phase 7: One-Time Catchup Migration

- [ ] Create `CatalogExistingSeasonPacksMigration` in `AddictedProxy/Migrations/Services/`
- [ ] Implement `IMigration` with `[MigrationDate(2026, 3, 12)]`
- [ ] Query stored season packs, download ZIPs from S3, catalog entries

## Phase 8: Update SubtitlesController Fallback

- [ ] Update `GetSeasonPackFallbackSubtitleDtos` to use catalog entries
- [ ] Include `Entries` navigation in season pack query
- [ ] Offer pack only if it has an entry for the requested episode (or has no entries yet — graceful degradation)

## Phase 9: Unit Tests

- [ ] Test ZIP filename parsing (episode number, title, release groups)
- [ ] Test catalog service with sample ZIP blobs
- [ ] Test self-extraction logic
- [ ] Test fallback behavior with cataloged vs uncataloged packs

## Verification

- [ ] `dotnet build -c Release` passes
- [ ] `dotnet test -c Release` passes
- [ ] EF migration applies cleanly
- [ ] Existing season pack download flow still works
