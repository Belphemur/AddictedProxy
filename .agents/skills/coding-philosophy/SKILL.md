---
name: coding-philosophy
description: Mandatory design decision framework for ALL code changes in this repository. Load BEFORE writing or reviewing any code. Orders design trade-offs strictly as DRY over SOLID over KISS and defines how conflicts are resolved.
license: GPL-3.0
metadata:
  author: Antoine Aflalo (Belphemur)
  mandatory: true
---

# Coding Philosophy: DRY > SOLID > KISS (strict priority order)

This repository's paradigm order is **DRY > SOLID > KISS**. The ordering is the
point: each principle wins ties against the ones below it. Every non-trivial
design decision MUST be resolvable in these terms, and the trade must be named
(commit body, PR description, or design doc) — documenting the order is not
following it.

## The precedence rule

1. **DRY wins over SOLID.** If a SOLID pattern (interface, abstraction layer,
   dependency inversion) would force a second copy of the same knowledge, DRY
   wins: one source of truth beats a decoupled duplicate. Do not add an
   abstraction boundary that duplicates behavior.
2. **SOLID wins over KISS.** If the simplest code would corrupt state under
   concurrency, bad input, or partial failure, the simpler structure is wrong:
   correctness invariants (single writer, atomic transition, validation on
   load) are not optional even when they add machinery.
3. **KISS breaks ties within DRY.** Once knowledge exists exactly once, the
   simplest structure that satisfies the design wins: prefer a plain constant
   over a config row, a single `if` over a state machine, deletion over
   re-architecture. If a change needs a diagram to explain, it is too complex.

## Decision procedure (ask in order)

1. **Where does this knowledge live?** (DRY) — one source of truth per
   behavior, per format, per constant. Refactor duplicate paths into one
   funnel before writing new code next to them.
2. **What happens on bad input / concurrent callers / partial failure?**
   (SOLID) — fail closed, never corrupt the only copy of data, never leave two
   writers racing on one resource. If a lock, clamp, or validation is needed,
   it is needed — KISS does not excuse its absence.
3. **What can be deleted now?** (KISS) — machinery whose job vanished is
   removed in the same change, not tolerated alongside the simpler structure.

## Repo-specific applications

- **Central package management is DRY:** every NuGet version lives in
  `Directory.Packages.props` exactly once. Never inline a version in a
  `.csproj`.
- **Bootstrap registration is the one DI funnel:** services are registered via
  `IBootstrap` implementations, not scattered `services.AddX` calls.
- **Endpoint result types:** new REST endpoints use ASP.NET Core `TypedResults`
  with `Results<T1, T2, ...>` — do not revive the legacy `Ardalis.Result`
  pattern for new code.
- **Async concurrency (`Locking/`, background jobs):** keyed locking and
  Hangfire job idempotency are SOLID territory — a "simpler" design that lets
  two writers race on one resource (episode catalog row, user credential,
  storage object) is rejected regardless of line count.
- **Raw SQL (`EpisodeRepository.MergeEpisodeWithSubtitleAsync`):** the SQL
  duplicates schema knowledge the compiler cannot check. It exists for
  performance, not convenience — when changing `Episode`/`Subtitle`/
  `EpisodeExternalId` columns, the SQL must be updated in the same change.
  Do not add NEW raw SQL where an EF expression suffices.
- **When a dependency bump makes local code redundant** (e.g. the SDK now
  ships a type we copied), delete the local copy and use the SDK's — this is
  the cheapest DRY win available.

## Review discipline

When a reviewer (human or bot) proposes a fix, map it to the paradigm before
acting: is it a DRY violation (two copies of one behavior), a SOLID hole
(race/bad-input/partial-failure), or a KISS objection (structure too heavy for
the problem)? Reply in those terms so the fix and its rationale are auditable
against the priority order. Do not blanket-apply review suggestions that
trade down the order (e.g. an interface with one implementation duplicating
an existing helper is DRY loss — say so).

## Pitfalls

- **KISS used to justify a race** — "simplest" that loses under concurrent
  callers is a SOLID violation wearing KISS's clothes.
- **SOLID used to justify duplication** — extract the shared knowledge first,
  then decide whether an abstraction is still needed.
- **DRY used to justify premature unification** — two call sites with
  different futures are not one behavior; unify when the copies actually
  agree, and record the decision.
- **Equal values do not mean equal jobs** — audit every consumer before
  collapsing twin constants; one may drive a different artifact's lifetime.
