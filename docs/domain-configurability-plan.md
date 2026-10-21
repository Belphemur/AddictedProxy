# Domain Configurability Plan

## Context

The public frontend moves from `gestdown.info` to `subvault.tv`; the API keeps
`api.gestdown.info` (the gestdown identity survives on the API surface only).
The repository's standing rule is that **production domain knowledge lives only
in Traefik labels and environment variables** — never as literals in code, and
never baked into container images.

A production audit (`/var/lib/addicted/docker-compose.yml`, Traefik v3 labels)
confirmed routing, rate limits, redirects, and frontend URLs are already
config-layer (compose labels + `NUXT_PUBLIC_*` / `A7D_*` env). Three C#
violations remained.

## Findings (hardcoded domain / config literals in code)

| Location | Literal | Violation |
|---|---|---|
| `AddictedProxy/Program.cs:46` | `https://api.gestdown.info` (Swagger production server URL) | domain literal in code |
| `AddictedProxy/Program.cs:53` | `"Gestdown: Addicted Proxy"` (Swagger title) | brand string in code |
| `AddictedProxy/Controllers/Config/CorsConfig.cs:15-23` | allowed-origin defaults duplicating `appsettings.json` exactly | DRY: two sources of truth for one list |

## Chosen approach

### CORS — single source of truth in configuration

`CorsConfig.AllowedOrigins` no longer carries a compiled-in default list; the
allowed origins come solely from the `Cors:AllowedOrigins` configuration
section (`appsettings.json`, overridable via `A7D_Cors__AllowedOrigins__0..N`).

`BootstrapController` treats an unbound (null) list as **empty → deny-all**:

```csharp
policyBuilder
    .WithOrigins(corsConfig.AllowedOrigins ?? [])
    .SetIsOriginAllowedToAllowWildcardSubdomains();
```

**Trade-off (coding-philosophy, strict priority):** DRY wins — the C#
initializer duplicated `appsettings.json` byte-for-byte, and any future domain
change had to edit both or risk deploy-vs-fallback divergence. SOLID's
fail-closed requirement is preserved: missing configuration denies all origins
instead of silently reflecting an unintended one (and `AllowCredentials()` is
set, so an origin wildcard is never acceptable). KISS: no new abstraction — the
config binding already existed; only the duplicate copy was deleted.

### Swagger — config section instead of literals

New `Swagger` configuration section in `appsettings.json`:

```json
"Swagger": {
  "ServerUrl": "https://api.gestdown.info",
  "Title": "Gestdown: Addicted Proxy"
}
```

`Program.cs` reads both values; overridable in production via
`A7D_Swagger__ServerUrl` / `A7D_Swagger__Title`. Defaults keep the subvault
API identity, which survives the frontend migration. The Swagger *UI* is
dev-only (`UseDeveloperExceptionPage` gate) and the public API docs are served
on docs.subvault.tv — but the OpenAPI document itself (`app.UseSwagger()`) is
served in all environments, so its server URL and title must not be literals.

## Rejected alternatives

- **Keep the C# defaults as a fallback beside appsettings.json** — rejected
  (DRY): two copies of one list must be kept in lock-step, and the fallback
  would mask a misconfigured deployment instead of failing closed.
- **One shared `Domain` config section for API + frontend URLs** — rejected
  (KISS): sitemap (frontend base, `Sitemap:BaseUrl`) and Swagger (API server)
  have different consumers and change independently; unifying them adds
  machinery without removing a real duplicate.
- **Fix robots.txt / brand copy in the same change** — deferred (scope): those
  are Nuxt-side (static `public/robots.txt`, `dmca.vue`, `privacy.vue`,
  `MediaDetailView.vue` SEO title) and need a `runtimeConfig.public`-driven
  approach plus copy decisions; the API-side configurability is not blocked by
  them.

## Frontend branding — composable-driven, env-configurable

A new `addicted.nuxt/app/composables/useSiteBrand.ts` is the single source of
truth for the user-facing brand. It reads `runtimeConfig.public` and exposes
`siteName`, `siteUrl`, `siteDomain` (derived from `url`), and `apiDocsUrl`.
Pages consume it instead of hard-coding the brand, so no brand string is baked
into static prerender output and every knob is env-overridable.

| Value | `runtimeConfig.public` key | Build env | Runtime env | Default |
|---|---|---|---|---|
| Site name | `siteName` | `APP_SITENAME` | `NUXT_PUBLIC_SITENAME` | `Subvault` |
| Site URL | `url` | `APP_URL` | `NUXT_PUBLIC_URL` | `https://www.subvault.tv` |
| API docs URL | `apiDocsUrl` | `APP_API_DOCS_URL` | `NUXT_PUBLIC_API_DOCS_URL` | `https://docs.subvault.tv/` (API docs host; the API itself stays `api.subvault.tv`) |

All `Gestdown` brand literals in `app.vue`, `AppBar.vue`, `index.vue`,
`api.vue`, `MediaDetailView.vue`, `dmca.vue`, `privacy.vue` were replaced with
`{{ siteName }}` / `` `${siteName}` ``; the API-doc link in `api.vue` now binds
to `apiDocsUrl`. `robots.txt` still ships in `public/` (the backend
`RobotsController` route is dormant until the prod DNS switch); only its
`Sitemap:` URL was repointed to `https://www.subvault.tv/sitemap.xml`.

**Trade-off (coding-philosophy, strict priority):** DRY — one composable funnels
every brand value instead of N hard-coded copies. The docs default now points
at `https://docs.subvault.tv/` (the subvault docs host); *env-configurability*
keeps it overridable for non-subvault deployments. KISS: no new abstraction
beyond the `runtimeConfig.public` + composable idiom already established by
`usePageLayout`.

## Remaining out of scope

- Cloudflare Worker `AddictedProxy.Cloudflare.Worker` — dormant, not in the
  serving path; its endpoint literal (`https://api.gestdown.info`, the API that
  survives the migration) must not be treated as frontend configuration.
- Backend `RobotsController.cs` (`/robots.txt`) ships dormant; the Traefik route
  is deferred pending the prod DNS switch.

## Production wiring (deploy-time, no image rebuild)

```bash
# /var/lib/addicted/.env
A7D_Cors__AllowedOrigins__0=https://subvault.tv
A7D_Cors__AllowedOrigins__1=https://*.subvault.tv
A7D_Cors__AllowedOrigins__2=https://api.gestdown.info   # as long as gestdown fronts exist
A7D_Swagger__ServerUrl=https://api.gestdown.info
A7D_Swagger__Title=Gestdown: Addicted Proxy
```

Traefik labels remain the routing source of truth; CORS origins are now a pure
deploy-time setting.

## Verification

- `dotnet build -c Release` — 0 errors / 0 warnings.
- `dotnet test -c Release` — all green (0 failures).
- Behavior in production is unchanged until env overrides are set:
  `appsettings.json` carries the same origin list that was previously duplicated
  in C#.
