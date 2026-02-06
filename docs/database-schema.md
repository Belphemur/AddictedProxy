# Database Schema

## Overview

AddictedProxy uses PostgreSQL as its primary database, accessed via Entity Framework Core 10 with the Npgsql provider. All entities inherit from `BaseEntity` which provides `CreatedAt` and `UpdatedAt` timestamps.

Schema changes are managed via **EF Core migrations** (located in `AddictedProxy.Database/Migrations/`), which are auto-applied on application startup via `dbContext.Database.MigrateAsync()`.

One-time **data migrations** (not schema changes) are handled by the `OneTimeMigration` framework (see [Background Jobs](background-jobs.md)).

## Entity Relationship Diagram

```
┌─────────────────────────────────────────┐
│                TvShow                   │
├─────────────────────────────────────────┤
│ Id (PK, long)                           │
│ UniqueId (Guid, unique, default uuidv7) │
│ ExternalId (long, unique)               │  ◄── Provider-specific ID (e.g., Addic7ed show ID)
│ Name (string)                           │
│ TmdbId (int?, indexed)                  │  ◄── The Movie Database ID
│ TvdbId (int?, indexed)                  │  ◄── TheTVDB ID
│ Type (ShowType enum)                    │  ◄── Show | Movie
│ Source (DataSource enum)                │  ◄── Where the show was first discovered
│ IsCompleted (bool)                      │
│ Priority (int, default 0)              │
│ LastUpdated (DateTime)                  │
│ LastSeasonRefreshed (DateTime?)         │
│ Discovered (DateTime)                   │
│ CreatedAt / UpdatedAt (BaseEntity)      │
├─────────────────────────────────────────┤
│ ◄──── 1:M ────► Seasons[]              │
│ ◄──── 1:M ────► Episodes[]             │
└─────────────────────────────────────────┘
          │                    │
          │ 1:M                │ 1:M
          ▼                    ▼
┌──────────────────┐  ┌──────────────────────────────────────┐
│     Season       │  │             Episode                   │
├──────────────────┤  ├──────────────────────────────────────┤
│ Id (PK, long)    │  │ Id (PK, long)                        │
│ TvShowId (FK)    │  │ ExternalId (long)                    │  ◄── Provider-specific episode ID
│ Number (int)     │  │ TvShowId (FK)                        │
│ LastRefreshed    │  │ Season (int)                         │
│ (DateTime?)      │  │ Number (int)                         │
│ CreatedAt /      │  │ Title (string)                       │
│ UpdatedAt        │  │ Discovered (DateTime)                │
├──────────────────┤  │ CreatedAt / UpdatedAt                │
│ Unique:          │  ├──────────────────────────────────────┤
│ (TvShowId,Number)│  │ Unique: (TvShowId, Season, Number)   │
└──────────────────┘  │ ◄──── 1:M ────► Subtitles[]         │
                      └──────────────────────────────────────┘
                                        │
                                        │ 1:M
                                        ▼
                      ┌──────────────────────────────────────────┐
                      │              Subtitle                     │
                      ├──────────────────────────────────────────┤
                      │ Id (PK, long)                             │
                      │ UniqueId (Guid, unique, default uuidv7)   │
                      │ EpisodeId (FK to Episode)                 │
                      │ Language (string)                         │
                      │ LanguageIsoCode (VARCHAR(7), nullable)    │
                      │ Scene (string)                            │  ◄── Release group/scene name
                      │ Version (int)                             │  ◄── Subtitle version number
                      │ Completed (bool)                          │
                      │ CompletionPct (double)                    │
                      │ HearingImpaired (bool)                    │
                      │ Corrected (bool)                          │
                      │ HD (bool)                                 │
                      │ DownloadUri (Uri, unique)                 │  ◄── Original download URL
                      │ DownloadCount (long)                      │
                      │ StoragePath (string?, nullable)           │  ◄── Path in S3/storage
                      │ StoredAt (DateTime?, nullable)            │
                      │ Source (DataSource enum)                  │  ◄── Which provider this came from
                      │ Discovered (DateTime)                     │
                      │ CreatedAt / UpdatedAt (BaseEntity)        │
                      ├──────────────────────────────────────────┤
                      │ Index: (DownloadUri) unique               │
                      │ Index: (EpisodeId, Language, Version)     │
                      │ Index: (UniqueId) unique                  │
                      └──────────────────────────────────────────┘
```

## Enums

### DataSource

Identifies the upstream provider that contributed a show or subtitle.

```csharp
public enum DataSource
{
    Addic7ed,    // Currently the only provider
}
```

### ShowType

```csharp
public enum ShowType
{
    Show,   // TV series
    Movie   // Film
}
```

### MigrationState (OneTimeMigrationRelease)

```csharp
public enum MigrationState
{
    Success,
    Running,
    Fail
}
```

## Other Entities

### AddictedUserCredentials

Stores Addic7ed account credentials for authenticated API access. Multiple accounts are rotated to distribute rate limiting.

```
┌──────────────────────────────────────┐
│     AddictedUserCredentials          │
├──────────────────────────────────────┤
│ Id (PK, long)                        │
│ Cookie (string, unique)              │  ◄── Session cookie for Addic7ed
│ Usage (int)                          │  ◄── Query usage counter
│ DownloadUsage (int)                  │  ◄── Download usage counter
│ LastUsage (DateTime?)                │
│ DownloadExceededDate (DateTime?)     │  ◄── When download limit was hit
│ CreatedAt / UpdatedAt (BaseEntity)   │
└──────────────────────────────────────┘
```

### OneTimeMigrationRelease

Tracks one-time data migration execution status. Prevents duplicate/concurrent migrations.

```
┌──────────────────────────────────────┐
│     OneTimeMigrationRelease          │
├──────────────────────────────────────┤
│ Id (Guid, PK)                        │
│ MigrationType (string)               │  ◄── Format: "YYYY-M-D_ClassName"
│ State (MigrationState enum)          │
│ CreatedAt / UpdatedAt (BaseEntity)   │
├──────────────────────────────────────┤
│ Unique: (MigrationType, State)       │
└──────────────────────────────────────┘
```

## Repository Interfaces

### ITvShowRepository

```csharp
FindAsync(name)                              // Full-text search by name
GetByIdAsync(id)                             // Get by primary key
GetByGuidAsync(guid)                         // Get by UniqueId
GetByTvdbIdAsync(tvdbId)                     // Get by TheTVDB ID
GetAllAsync()                                // All shows
GetAllHavingSubtitlesAsync()                 // Shows with at least one subtitle
GetShowsByTmdbIdAsync(tmdbId)                // Get by TMDB ID
GetShowWithoutTmdbIdAsync()                  // Shows missing TMDB mapping
GetShowsWithoutTvdbIdWithTmdbIdAsync()       // Shows with TMDB but missing TVDB
GetCompletedShows()                          // Completed shows
GetDuplicateTvShowByTmdbIdAsync()            // Find duplicate TMDB mappings
UpsertRefreshedShowsAsync(shows)             // Bulk upsert shows
UpdateShowAsync(show)                        // Single show update
BulkSaveChangesAsync()                       // Batch save
```

### IEpisodeRepository

```csharp
UpsertEpisodes(episodes)                     // Bulk upsert episodes with subtitles
GetEpisodeUntrackedAsync(showId, season, ep) // Get episode (no change tracking)
GetSeasonEpisodesAsync(showId, season)       // All episodes in a season
GetSeasonEpisodesByLangUntrackedAsync(...)   // Episodes filtered by language
```

### ISeasonRepository

```csharp
InsertNewSeasonsAsync(showId, seasons)        // Insert new seasons
GetSeasonForShowAsync(showId, seasonNumber)   // Get specific season
GetSeasonsForShowAsync(showId)                // All seasons for show
SaveChangesAsync()                            // Save changes
UpdateLastRefreshedFromIdAsync(id, date)      // Update refresh timestamp
```

### ISubtitleRepository

```csharp
GetSubtitleByIdAsync(id, includeEpisode, includeShow)    // Get by PK
GetSubtitleByGuidAsync(guid, includeEpisode, includeShow) // Get by UniqueId
SaveChangeAsync()                                         // Save changes
IncrementDownloadCountAsync(subtitle)                     // Atomic counter increment
TagForRemoval(subtitle)                                   // Mark for deletion
```

## Database Configuration

- **Connection String**: Configured via `appsettings.json` or environment variables
- **UUID Generation**: `uuidv7()` PostgreSQL function for sortable unique IDs
- **Collation**: Case-insensitive collation for text search
- **Migrations**: Auto-applied at startup via `MigrateAsync()`
- **Transactions**: Managed via `ITransactionManager<EntityContext>` for multi-step operations
