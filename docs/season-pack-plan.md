# Plan: Season Pack API + Frontend UX

## TL;DR

Expose season packs via augmented existing responses and a dedicated Bazarr endpoint. Extend the download route with a `sp_` prefix scheme: `sp_{uuid}` returns a full ZIP (frontend), `sp_{uuid}_ep_{N}` extracts a single episode SRT (Bazarr fallback — **no Bazarr provider changes needed**). Add a `SeasonPackProvider` service and build `SeasonPacksSection.vue` in the frontend.

---

## Completed Pre-work

### Entity Changes (already merged)

- **`SeasonPackSubtitle`** now has:
  - `long DownloadCount` — tracks download count (atomic increment via raw SQL, same pattern as `Subtitle`)
  - `long? SeasonId` — nullable FK to `Season` entity (backfilled via one-time migration)
  - `virtual Season? SeasonEntity` — navigation property
- **`SeasonPackSubtitleRepository`** now has:
  - `GetByUniqueIdAsync(Guid, CancellationToken)` — lookup by unique ID
  - `IncrementDownloadCountAsync(SeasonPackSubtitle, CancellationToken)` — atomic raw SQL increment
  - `BulkUpsertAsync` ignores `DownloadCount` on merge update (preserves counts during re-imports)
- **`ProviderDataIngestionService.IngestSeasonPacksAsync`** now:
  - Ensures `Season` entities exist for all season numbers in the packs (handles season-pack-only shows)
  - Resolves and sets `SeasonId` FK on each pack before bulk upserting
- **EF Core migration** `20260224201536_AddSeasonPackDownloadCountAndSeasonFk` adds columns + FK + index
- **One-time migration** `BackfillSeasonPackSeasonFkMigration` (`[MigrationDate(2026, 2, 24)]`) backfills `SeasonId` on existing rows by joining `SeasonPackSubtitles.TvShowId + Season` to `Seasons.TvShowId + Number`

---

## Phase 1 — Backend: DTO + Repository

1. **Create `SeasonPackSubtitleDto`**
   - File: `AddictedProxy/Model/Dto/SeasonPackSubtitleDto.cs`
   - Fields: `SubtitleId` (string — "sp\_" + UniqueId), `Language`, `Version` (maps to Release), `Uploader`, `UploadedAt`, `Qualities`, `Source`, `DownloadUri`, `DownloadCount`
   - Constructor takes `(SeasonPackSubtitle, string downloadUri, Culture?)` — mirrors SubtitleDto pattern

2. **Augment `TvShowSubtitleResponse`** to include `SeasonPacks`
   - File: `AddictedProxy/Model/Responses/TvShowSubtitleResponse.cs`
   - Add `IEnumerable<SeasonPackSubtitleDto> SeasonPacks { get; }`

3. **Augment `MediaDetailsWithEpisodeAndSubtitlesDto`** to include SeasonPacks
   - File: `AddictedProxy/Model/Dto/MediaDetailsWithEpisodeAndSubtitlesDto.cs`
   - Add `IEnumerable<SeasonPackSubtitleDto> SeasonPacks { get; }` to the record

4. **Update `SerializationContext`** for the new DTO type
   - File: `AddictedProxy/Controllers/Rest/Serializer/SerializationContext.cs`
   - Add `[JsonSerializable(typeof(SeasonPackSubtitleDto))]`

---

## Phase 2 — Backend: Season Pack Provider + Download

5. **Add route name `DownloadSeasonPackSubtitle` to `Routes.cs`** — not strictly needed since we reuse `DownloadSubtitle` route, but document the `sp_` convention
   - File: `AddictedProxy/Controllers/Rest/Routes.cs`

6. **Create `ISeasonPackProvider`** service interface
   - File: `AddictedProxy/Services/Provider/SeasonPack/ISeasonPackProvider.cs`
   - Methods: `GetByUniqueIdAsync(Guid, CancellationToken)`, `GetSeasonPackFileAsync(SeasonPackSubtitle, CancellationToken)`

7. **Implement `SeasonPackProvider`**
   - File: `AddictedProxy/Services/Provider/SeasonPack/SeasonPackProvider.cs`
   - `GetByUniqueIdAsync` → delegates to `ISeasonPackSubtitleRepository.GetByUniqueIdAsync`
   - `GetSeasonPackFileAsync(SeasonPackSubtitle, int? episode, CancellationToken)` → mirrors SubtitleProvider pattern: check `StoragePath`/`ICachedStorageProvider` first, else call `ISuperSubtitlesClient.DownloadSubtitleAsync(externalId, episode)`
     - `episode: null` → full ZIP archive (multiple SRTs, one per episode) — for frontend download
     - `episode: N` → single SRT for that episode — for Bazarr fallback extraction
   - Increment `DownloadCount` via `ISeasonPackSubtitleRepository.IncrementDownloadCountAsync` on every successful download
   - Register in the module's `IBootstrap`

---

## Phase 3 — Backend: Controller Updates

8. **Add dedicated `GET /shows/{showId:guid}/{seasonNumber:int}/{language}/season-packs` endpoint** (Bazarr enumeration / external tools)
   - File: `AddictedProxy/Controllers/Rest/TvShowsController.cs`
   - New action `GetSeasonPacksForSeason([FromRoute] Guid showId, [FromRoute] int seasonNumber, [FromRoute] string language, CancellationToken)` — returns `SeasonPackResponse`
   - `SeasonPackResponse` record: `IEnumerable<SeasonPackSubtitleDto> SeasonPacks`
   - Fetches show → validates language → calls `GetByShowAndSeasonAsync` filtered by `LanguageIsoCode` → maps to `SeasonPackSubtitleDto[]`
   - `ResponseCache` 2 hours (matches season subtitle listing)
   - Full XML doc comments so it surfaces cleanly in Swagger/OpenAPI
   - Register `SeasonPackResponse` in `SerializationContext`
   - _Note: Bazarr's primary integration path is the seamless fallback in the existing `/subtitles/get/` endpoint (step 9a), not this endpoint. This endpoint is for tools that want to enumerate season packs for a season explicitly._

9. **Modify `SubtitlesController.Download`** to handle `sp_` prefix scheme
   - File: `AddictedProxy/Controllers/Rest/SubtitlesController.cs`
   - Change route from `{subtitleId:guid}` → `{subtitleId}` (plain string) — route constraint validation is lost, so explicit validation is required in the action body
   - Inject `ISeasonPackProvider`
   - Parse logic (mutually exclusive, checked top-to-bottom; return `BadRequest` on any parse failure):
     - **`sp_{uuid}_ep_{N}`** (contains `_ep_` after the prefix) → season pack single episode extraction
       - Split on `_ep_`: left part (after `sp_`) must parse as `Guid`, right part must parse as positive `int` — `BadRequest` if either fails
       - Look up `SeasonPackSubtitle` by that `Guid` — `NotFound` if absent
       - Calls `ISeasonPackProvider.GetSeasonPackFileAsync(pack, episode: N)`
       - Content-Type: `text/srt`
       - Filename: `ShowName.S{NN}E{NN}.{lang}.srt`
     - **`sp_{uuid}`** (starts with `sp_`, no `_ep_`) → full ZIP
       - Strip `sp_` prefix; must parse as `Guid` — `BadRequest` if not
       - Look up `SeasonPackSubtitle` by that `Guid` — `NotFound` if absent
       - Calls `ISeasonPackProvider.GetSeasonPackFileAsync(pack, episode: null)`
       - Content-Type: `application/zip`
       - Filename: `ShowName.S{NN}.{lang}.zip`
     - **plain string** (no `sp_` prefix) → regular episode subtitle
       - Must parse as `Guid` — `BadRequest` if not
       - Existing lookup + streaming path, unchanged
   - ETag in all cases: `UniqueId` + `StoredAt`
   - Add `BadRequest<ErrorResponse>` to the `Results<...>` return type union to cover the new validation failure cases

9a. **Modify `SubtitlesController.GetSubtitles`** (the `/subtitles/get/` endpoint) to fall back to season packs - File: `AddictedProxy/Controllers/Rest/SubtitlesController.cs` - After `_searchSubtitlesService.FindSubtitlesAsync` returns empty `MatchingSubtitles`, query `ISeasonPackSubtitleRepository.GetByShowAndSeasonAsync` filtered by `LanguageIsoCode` - Map each `SeasonPackSubtitle` to a `SubtitleDto`-shaped entry with: - `SubtitleId` = `"sp_{uniqueId}_ep_{episodeNumber}"` (episode number from the request) - `DownloadUri` = route URL for that `subtitleId` - `HearingImpaired` = `false` (season packs have no HI flag) - `Version` = `pack.Release ?? pack.Filename` - `Qualities` from the pack - `Completed` = `true` (season packs are always complete) - `DownloadCount` from the pack - Append these to `MatchingSubtitles` in the existing `SubtitleSearchResponse` - **The Bazarr provider (`GestdownProvider`) requires zero code changes** — it already reads `subtitleId`, `downloadUri`, `hearingImpaired`, `version`, `qualities` from each entry and calls `downloadUri` directly

10. **Modify `TvShowsController.GetSubtitlesForSeason`**
    - File: `AddictedProxy/Controllers/Rest/TvShowsController.cs`
    - Inject `ISeasonPackSubtitleRepository` and `ICultureParser`
    - After fetching episodes, also call `GetByShowAndSeasonAsync(show.Id, seasonNumber, token)` — filter by `LanguageIsoCode` matching `searchLanguage`
    - Map to `SeasonPackSubtitleDto[]` with `sp_` prefixed download URI
    - Pass to updated `TvShowSubtitleResponse` — _depends on Phase 1 step 2_

11. **Modify `MediaController.GetShowDetails`**
    - File: `AddictedProxy/Controllers/Rest/MediaController.cs`
    - Inject `ISeasonPackSubtitleRepository`
    - Also fetch season packs for last season, map to DTOs, include in response — _depends on Phase 1 step 3_

---

## Phase 4 — Frontend

12. **Update `data-contracts.ts`** with new types
    - File: `addicted.nuxt/app/composables/api/data-contracts.ts`
    - Add `SeasonPackSubtitleDto` interface (including `downloadCount`)
    - Add `SeasonPackResponse` interface (`{ seasonPacks: SeasonPackSubtitleDto[] }`)
    - Update `TvShowSubtitleResponse` to add `seasonPacks?: SeasonPackSubtitleDto[]`
    - Update `MediaDetailsWithEpisodeAndSubtitlesDto` to add `seasonPacks?: SeasonPackSubtitleDto[]`

13. **Create `SeasonPacksSection.vue`** component
    - File: `addicted.nuxt/app/components/media/SeasonPacksSection.vue`
    - Receives `seasonPacks: SeasonPackSubtitleDto[]` prop
    - Desktop: `v-data-table` with columns: Language, Version/Release, Qualities, Uploader, Source chip (teal=SuperSubtitles), Downloads (count), Download button
    - Mobile: `v-expansion-panels` with one panel per pack, download button
    - Download calls `subtitlesApi.downloadSubtitle(pack.subtitleId)` — `subtitleId` already has `sp_` prefix; server returns a **ZIP** so save with `.zip` extension (read `Content-Disposition` filename from response headers, same pattern as existing subtitle download)
    - Always wrapped in a labelled glassmorphism `v-sheet` section header ("Season Packs") — hidden entirely when `seasonPacks.length === 0`

14. **Update `MediaDetailView.vue`** to handle season packs + episode-less fallback
    - File: `addicted.nuxt/app/components/media/MediaDetailView.vue`
    - Add `seasonPacks = ref<SeasonPackSubtitleDto[]>([])`
    - `loadViewData()`: populate from `data.value?.seasonPacks`
    - `watch([currentSeason, language])`: extract `seasonPacks` from `showsDetail` response
    - `doneHandler`: after `getEpisodes` (SignalR), also call `showsApi.showsDetail(...)` to refresh season packs from REST
    - **Display logic**:
      - Season packs always appear **above** the episodes table when available
      - `showSeasonPackFallback` computed: `episodes` is empty/null **and** `seasonPacks.length > 0`
      - When `showSeasonPackFallback` is true: hide `SubtitlesTable`, render `SeasonPacksSection` prominently with an info banner ("No per-episode subtitles found for this language — showing season packs instead")
      - When both episodes and season packs coexist: render `SeasonPacksSection` first, then `SubtitlesTable` below it

---

## ASCII Mockup — Desktop Show Page

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│  🖥  Gestdown                                         Home  API  Privacy Policy  │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│  [Poster]  Stranger Things (2016)                                               │
│            It only gets stranger...              User Score  ████████░ 8.6      │
│                                                  Genres      Sci-Fi, Mystery    │
│  Overview:                                                                       │
│  When a young boy vanishes, a small town uncovers a mystery...                  │
│                                                                                 │
│  Language [English ▾]        Season [5 ▾]                                       │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│  Season 5                                    [↺ Refresh]  [⬇ Download season]  │
│─────────────────────────────────────────────────────────────────────────────────│
│  📦 Season Packs                                                                 │
│─────────────────────────────────────────────────────────────────────────────────│
│  Release              Uploader    Quality       Source           DL   Download   │
│─────────────────────────────────────────────────────────────────────────────────│
│  WEBRip.NTb           NTb         720p 1080p    [SuperSubtitles]  42  [⬇ ZIP]   │
│  BluRay.TrollHD       TrollHD     2160p         [SuperSubtitles]  18  [⬇ ZIP]   │
│─────────────────────────────────────────────────────────────────────────────────│
│  Group              Version   ✓  HI  Quality       Source      Downloads        │
│─────────────────────────────────────────────────────────────────────────────────│
│  ▶ 1 — Chapter One: The Crawl                                                   │
│─────────────────────────────────────────────────────────────────────────────────│
│    │              playWEB   ✓      720p 1080p  [Addic7ed  ]  [⬇ 10]            │
│    │              playWEB   ✓  👂  720p 1080p  [Addic7ed  ]  [⬇  8]            │
│─────────────────────────────────────────────────────────────────────────────────│
│  ▶ 2 — Chapter Two: The Vanishing of Holly Wheeler                              │
│  ▶ 3 — Chapter Three: The Turnbow Trap                                          │
│  ...                                                                            │
└─────────────────────────────────────────────────────────────────────────────────┘
```

**Notes:**

- Season Packs section appears **above** the episodes table when season packs are available
- When **no episode subtitles exist** for the selected language, episodes table is hidden and season packs section is shown with an info banner:
  ```
  ┌─────────────────────────────────────────────────────────────────────────────┐
  │  ℹ  No per-episode subtitles available for English — showing season packs   │
  └─────────────────────────────────────────────────────────────────────────────┘
  ┌─────────────────────────────────────────────────────────────────────────────┐
  │  📦 Season Packs                                                             │
  │  ...                                                                         │
  └─────────────────────────────────────────────────────────────────────────────┘
  ```
- `📦 Season Packs` header is a compact single line — no extra card wrapper, just a heading row flush with the panel, same visual weight as `Season 5`
- `[⬇ ZIP]` button is compact (`size="small"`) to avoid the column feeling heavy — season packs have far fewer rows than episodes
- `DL` column shows the cumulative download count for each season pack

---

## Phase 5 — Documentation

15. **Update `docs/api-surface.md`**
    - Add `GET /shows/{showId}/{seasonNumber}/{language}/season-packs` to the Shows Controller table
    - Add full endpoint section (parameters, response shape, cache, error codes, usage note)
    - Update `GET /subtitles/get/` docs: note that when no episode subtitle is found, the response contains season pack fallback entries with `sp_{uuid}_ep_{N}` IDs — **transparent to the existing Bazarr `GestdownProvider` implementation**
    - Update Download Subtitle section to document the full `sp_` scheme: `sp_{uuid}` = ZIP, `sp_{uuid}_ep_{N}` = single SRT
    - Add `SeasonPackSubtitleDto` and `SeasonPackResponse` to the DTOs section
    - Update `TvShowSubtitleResponse` and `MediaDetailsWithEpisodeAndSubtitlesDto` entries to note the new `seasonPacks` field

16. **Update `docs/frontend-ux-design.md`**
    - Add `SeasonPacksSection.vue` to the component catalogue
    - Document the episode-less fallback behaviour and the display priority logic

---

## Relevant Files

- `AddictedProxy.Database/Model/Shows/SeasonPackSubtitle.cs` — entity (DownloadCount + SeasonId FK added)
- `AddictedProxy.Database/Repositories/Shows/ISeasonPackSubtitleRepository.cs` — `GetByUniqueIdAsync`, `IncrementDownloadCountAsync`
- `AddictedProxy.Database/Repositories/Shows/SeasonPackSubtitleRepository.cs` — implementation
- `AddictedProxy/Services/Provider/Merging/ProviderDataIngestionService.cs` — `IngestSeasonPacksAsync` resolves SeasonId
- `AddictedProxy/Migrations/Services/BackfillSeasonPackSeasonFkMigration.cs` — one-time FK backfill
- `AddictedProxy/Model/Dto/SeasonPackSubtitleDto.cs` — new DTO
- `AddictedProxy/Model/Responses/TvShowSubtitleResponse.cs` — add SeasonPacks
- `AddictedProxy/Model/Responses/SeasonPackResponse.cs` — new dedicated response
- `AddictedProxy/Model/Dto/MediaDetailsWithEpisodeAndSubtitlesDto.cs` — add SeasonPacks
- `AddictedProxy/Controllers/Rest/SubtitlesController.cs` — `sp_`/`sp_{uuid}_ep_{N}` routing; season pack fallback in GetSubtitles
- `AddictedProxy/Controllers/Rest/TvShowsController.cs` — fetch + return season packs; new enumeration endpoint
- `AddictedProxy/Controllers/Rest/MediaController.cs` — fetch + return season packs
- `AddictedProxy/Controllers/Rest/Serializer/SerializationContext.cs` — register new types
- `AddictedProxy/Services/Provider/SeasonPack/ISeasonPackProvider.cs` — new service
- `AddictedProxy/Services/Provider/SeasonPack/SeasonPackProvider.cs` — new service impl
- `addicted.nuxt/app/composables/api/data-contracts.ts` — new types
- `addicted.nuxt/app/components/media/SeasonPacksSection.vue` — new component
- `addicted.nuxt/app/components/media/MediaDetailView.vue` — integrate section + fallback logic
- `docs/api-surface.md` — updated
- `docs/frontend-ux-design.md` — updated

---

## Verification

1. `dotnet build -c Release` — no compile errors
2. POST to `/shows/{showId}/refresh` then GET `/shows/{showId}/{season}/{lang}` → confirm `seasonPacks` array in JSON
3. GET `/shows/{showId}/{season}/{lang}/season-packs` → returns `SeasonPackResponse` with correct packs
4. GET `/media/{showId}/episodes/{lang}` → confirm `seasonPacks` in initial response
5. GET `/subtitles/get/{showId}/{season}/{episode}/{lang}` when no episode subtitle exists → `matchingSubtitles` contains season pack entries with `sp_{uuid}_ep_{N}` IDs
6. GET `/subtitles/get/{showId}/{season}/{episode}/{lang}` when episode subtitle exists → no season pack fallback entries included
7. GET `/subtitles/download/sp_{uuid}` → returns `application/zip` with correct `ShowName.S{NN}.{lang}.zip` filename containing per-episode SRT files
8. GET `/subtitles/download/sp_{uuid}_ep_{N}` → returns `text/srt` with correct `ShowName.S{NN}E{NN}.{lang}.srt` filename (single episode extraction)
9. GET `/subtitles/download/{regularGuid}` → existing subtitle download unaffected
10. Frontend (episodes exist): SubtitlesTable shown first, SeasonPacksSection rendered below
11. Frontend (no episodes for language): SubtitlesTable hidden, SeasonPacksSection shown prominently with info banner
12. Frontend download button triggers ZIP save; `Content-Disposition` filename read from response headers
13. Playwright screenshot: desktop + mobile, both episode-present and episode-absent states
14. `docs/api-surface.md` and `docs/frontend-ux-design.md` reflect all changes

---

## Decisions

- **`sp_` prefix scheme** (baked into `SubtitleId`/`DownloadUri` by the backend):
  - `sp_{uuid}` → full ZIP, all episodes in the season (frontend download button)
  - `sp_{uuid}_ep_{N}` → single SRT extracted for episode N (Bazarr-compatible download)
- **Bazarr needs zero code changes**: the existing `GestdownProvider` already reads `subtitleId`, `downloadUri`, `hearingImpaired`, `version`, `qualities` from `matchingSubtitles` and blindly GETs `downloadUri`. The season pack fallback in `/subtitles/get/` returns entries in the same shape, so Bazarr picks them up automatically when no episode subtitle is available.
- **No second download endpoint**: the string route `{subtitleId}` (replacing `{subtitleId:guid}`) handles all three cases (regular subtitle, full ZIP, episode SRT extraction) in one place via prefix parsing.
- The dedicated `/season-packs` endpoint is for explicit enumeration by external tools, not the Bazarr primary integration path.
- **Episode-list is always the default**: season packs are a fallback (no episodes) or a supplement (both exist). Never the primary view when episodes are available.
- Season packs are filtered by `LanguageIsoCode` on the server so only packs matching the selected language appear — consistent with episode filtering.
- Season pack storage/caching reuses `ICachedStorageProvider` pattern from `SubtitleProvider`.
- `data-contracts.ts` is normally auto-generated from Swagger — manual edits will need re-applying if regenerated; note in the PR.
- **Download count tracking**: every `SeasonPackProvider` download (ZIP or single SRT) increments `DownloadCount` atomically. `BulkUpsertAsync` ignores `DownloadCount` on merge to prevent re-import from resetting counts.
- **Season FK**: `SeasonId` is nullable to handle edge cases where a Season entity doesn't exist yet. The one-time migration backfills existing rows; new packs get `SeasonId` set during ingestion.
