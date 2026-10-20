---
name: backend-testing
description: How to write and run .NET unit tests in AddictedProxy (NUnit 5 + NSubstitute + FluentAssertions). Use this skill whenever asked to add, update, fix, or debug .NET unit tests, or when a CI test failure needs investigation.
license: GPL-3.0
metadata:
  author: Antoine Aflalo (Belphemur)
---

# Backend testing (.NET)

## Stack

| Concern     | Choice                                    | Pinned in                    |
| ----------- | ------------------------------------------ | ----------------------------- |
| Framework   | NUnit 5.0.0 + `NUnit3TestAdapter` 6.3.1    | `Directory.Packages.props`    |
| Mocking     | NSubstitute 6.2.0                          | `Directory.Packages.props`    |
| Assertions  | FluentAssertions 8.11.0 (5 of 7 projects)  | `Directory.Packages.props`    |
| Coverage    | coverlet.collector 10.1.0 (all 7 projects) | `Directory.Packages.props`    |
| Target      | net10.0, Nullable + ImplicitUsings enabled | `Directory.Build.props`       |

## Test-project map

| Test project | Covers | Notes |
| --- | --- | --- |
| `AddictedProxy.Services.Tests` | Service layer of the main app | References the full `AddictedProxy` project |
| `AddictedProxy.Resilience.Tests` | Polly resilience pipelines across Upstream, ProxyScrape, TvMovieDatabaseClient, ProxyProvider, SuperSubtitleClient, IoC | Cross-cutting |
| `AddictedProxy.Upstream.Tests` | Upstream scraping/clients | |
| `InversionOfControl.Tests` | Bootstrap DI framework | Only project with explicit `<IsTestProject>true</IsTestProject>` |
| `Compressor.Tests` | Zstd wrapper | |
| `AntiCaptcha.Tests` | CAPTCHA integration | No FluentAssertions (plain `Assert.*`) |
| `ProxyProvider.Tests` | Proxy provider | No FluentAssertions (plain `Assert.*`) |

**Rule of thumb:** for a change in a shared/DI/resilience/config surface, run
`AddictedProxy.Services.Tests` and `AddictedProxy.Resilience.Tests` first —
they are the broadest nets.

## Running

```bash
dotnet test -c Release                      # whole solution (what CI runs)
dotnet test -c Release --filter "FullyQualifiedName~<Namespace>"   # one project/fixture
```

CI (`reusable-dotnet.yml`) runs `dotnet test -c Release --no-restore` over the
whole solution, gated by the `CI Gate` job in `ci-cd.yml`.

## Conventions

- **Naming:** `MethodName_Condition_ExpectedResult` (see existing tests, e.g.
  `GetEpisodeFromUpstreamAsync_EpisodeAfterKnownRange_ReturnsNotFoundWithoutCallingUpstream`).
- **Structure:** Arrange–Act–Assert; one behavior per test.
- **Mocking:** substitute only external boundaries (HTTP clients, `EntityContext`/
  repositories, upstream APIs, storage). Prefer state-based assertions over
  verifying implementation details.
- **Async:** `async Task` tests; pass `CancellationToken` when the SUT takes one.
- **Time/rate concerns:** inject fakes (e.g. fake `TimeProvider` or seeded
  repositories) rather than `Thread.Sleep`/`Task.Delay` where avoidable; the
  resilience tests deliberately use real short delays to exercise retry timing.
- **FluentAssertions style:** `result.Should().Be(...)`, collection
  `.Should().Contain(...)`; keep chains small and readable.
- **NSubstitute style:** `Substitute.For<Iface>()`, `.Received(n).Method(...)`,
  `Arg.Do<T>` for capturing arguments; avoid `Received` on properties you also
  configure.

## Adding a new test project

1. `dotnet new xunit -n Project.Tests` then switch to NUnit, or copy an
   existing test csproj (they are near-identical).
2. Add package references **without versions** — versions come from
   `Directory.Packages.props` (NUnit, NUnit3TestAdapter, NSubstitute,
   FluentAssertions, coverlet.collector, Microsoft.NET.Test.Sdk).
3. Add the project to `AddictedProxy.sln`.
4. If it tests a module with an `IBootstrap`, reference that module's project.
