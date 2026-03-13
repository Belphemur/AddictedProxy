# AddictedProxy.Storage.Caching Agent Notes

## Purpose

This project provides cache-first access for binary storage objects with an explicit fallback path.
Its `GetSert` pattern is intentionally designed to keep cache behavior deterministic and avoid split
cache logic across callers.

## Caching Model

`DistributedCachedStorageProvider` uses a two-layer read strategy:

1. Check distributed cache (`IDistributedCache`) by computed cache key.
2. On miss, check compressed storage (`ICompressedStorageProvider`).
3. If storage is also missing, call a caller-provided upstream delegate (`dataGet`).
4. If the delegate returns data, persist it to compressed storage, then populate distributed cache.
5. Return a seekable in-memory stream to the caller.

This flow guarantees that all successful misses can be promoted into storage and cache through one
central code path.

## Why Delegate-Based GetSert Is The Correct Pattern

The only valid API is:

- `GetSertAsync(shardingKey, filename, dataGet, cancellationToken)`

Reasoning:

- It keeps source-of-truth fallback inside the cache provider instead of duplicating fallback logic
  at every call site.
- It ensures miss-path writes are consistent (same storage write + cache write behavior everywhere).
- It avoids subtle drift where one caller updates storage/cache and another caller does not.
- It preserves observability consistency (hit/miss metrics and span tags are emitted from one place).

## Removed Anti-Pattern

The old short overload without `dataGet` was a design mistake and should not exist.

`GetSert` is defined around one contract: if cache and storage miss, execute the provided fetch
function, return that value when available, and backfill storage plus distributed cache from the
same code path.

Callers should still defensively handle a `null` result and log it as an error because it means the
fallback source failed to provide data.

## Season Pack Example

For season pack ZIPs, callers should pass a delegate that downloads the full ZIP from upstream when
cache and storage miss. This allows `GetSert` to backfill storage and distributed cache before
returning data.

For individual episode extraction, once the ZIP stream is retrieved through delegate-based `GetSert`,
the caller can open the archive and extract the requested entry.

## Practical Rule

Always pass the upstream fetch function to `GetSert`.
