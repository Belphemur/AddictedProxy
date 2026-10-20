---
name: design-doc-authoring
description: How to author design documents and ADRs in this repository — writes docs/YYYY-MM-DD-<slug>.md combining ADR decision-record format with the repo's design/plan format. Use this skill whenever asked to design a non-trivial change, write a design doc, plan, or ADR, or to record an architectural decision.
license: GPL-3.0
metadata:
  author: Antoine Aflalo (Belphemur)
---

# Design doc authoring (ADR + design plan)

## When to produce a design doc

Non-trivial or API-visible changes need a short design note **before code**:
a pinned section in the PR body, or — for reusable decisions — a full
`docs/*-plan.md` / design entry. Use this skill for the full-document case:

- Architectural or cross-module changes
- Public API / contract changes (REST surface, DTOs, routes)
- New subsystems or persisted formats
- Any change where the trade-offs must be auditable against the
  coding-philosophy order (DRY > SOLID > KISS)

**Mandatory preamble:** load the `coding-philosophy` skill first — every
design decision in the document must resolve (or at least be nameable) in its
terms, and rejected alternatives must name the *losing principle*.

## File naming and placement

- One file per design: `docs/<ISO-date>-<slug>.md` using an ISO-8601 date
  prefix (`YYYY-MM-DD`) and a lowercase-hyphen slug, e.g.
  `docs/2026-10-20-api-id-base62.md`.
- Pure *design-with-plan* docs may instead follow the repo's legacy
  `<topic>-plan.md` convention (see `season-pack-plan.md`) — but new
  documents prefer the date-prefixed name so the folder reads chronologically.
- If the repo adopts numbered ADRs (`docs/adr/NNNN-title.md`) later, migrate
  by linking; do not leave two copies.
- Link the document from `AGENTS.md`'s docs table — `AGENTS.md` is the
  single index that must stay in sync on every change.

## Document structure (one file, both formats)

Combine the ADR decision record with the repo design-plan format:

1. **H1 title** — `# Design: <short human topic>`
2. **Metadata block** (quoted) declaring this is ADR + design, with a table:
   `Status` (Draft → Approved → Implemented → Superseded by …), `Date` (ISO),
   `Deciders`, `Scope` (touched projects/surfaces), `Breaking?` (explicit
   YES/NO — never omit), `Supersedes`, `Related docs` (relative links).
3. **TL;DR** — ≤ 8 lines; a reader who stops here must understand what will
   exist afterwards and roughly how.
4. **Decision record** — `### Context` (current reality, with evidence:
   file paths, tables of current shapes), `### Decision` (normative "decide
   to…", pinned constants — length, alphabet, constraints), `### Rejected
   alternatives` (table; each row names the alternative and the **losing
   principle**: DRY, SOLID, or KISS, with a concrete reason).
5. **Detailed design** — per surface: new files (with target paths), one
   minimal code sketch per mechanism, call-site inventory lists, an explicit
   "What does NOT change" list (scope firewall — internal services, DB
   schema, etc.).
6. **Implementation phases** — table: phase, content, tests. Each phase must
   be an independently landable Conventional Commit **with its tests**.
   Include peripheral work (docs, mock servers, e2e fixtures, deprecation
   notes) as its own phase — integer work isn't real until listed.
7. **Verification (definition of done)** — checkbox list; hard gates
   (round-trip invariants, fail-closed behavior on bad input, whole-solution
   test green) plus doc updates and "PR description names the trade".
8. **Consequences** — positive / negative / neutral. Negative lists must be
   honest: maintenance cost, breaking-change windows, second
   implementations (e.g. a Go mock server duplicating a format), cache
   roll-out windows.
9. **Open items** — unresolved specifics that require verification during
   implementation (framework quirks, consumer assumptions).

## Style rules

- Reference real code: file paths, line-level locations, existing tests —
  designs are easiest to verify against named reality, and the readers
  (agents and humans) cannot audit a design against an unspecified codebase.
- Tables over prose for enumerations (surfaces, formats, alternatives).
- Constants belong in the document exactly once and verbatim as they will
  live in code (alphabet strings, lengths, route tokens).
- Update `docs/api-surface.md` and any domain doc the design touches **in
  the same change that implements it** — undocumented behavior is a smell.
- **Breaking changes** get the `Breaking?` flag, the `BREAKING CHANGE:`
  footer plan, and a rollout/migration-window consequence (e.g. response
  caches serving legacy URLs).

## Before you finish

- [ ] `coding-philosophy` loaded; every do/don't decision names its principle
- [ ] File named `docs/YYYY-MM-DD-<slug>.md`, linked from `AGENTS.md`
- [ ] Status/Date/Breaking filled; alternatives table names losing principles
- [ ] Every phase has tests named (project + class, not "add tests")
- [ ] "What does NOT change" list present (scope-firewall against creep)
- [ ] Verification checklist includes fail-closed behavior and format
      round-trip where the design defines a wire format
- [ ] Peripheral systems enumerated (mock-server, e2e, frontend, docs)
