# ImageSharp license-check timing

## Answer

SixLabors ImageSharp **4.1.3** (and ImageSharp.Web **4.0.2**) check the
community license at **build time**, not at runtime. It is **not** checked when
the library is loaded and **not** on the first image operation. The current
AddictedProxy production container image does **not** need `sixlabors.lic`
shipped inside it.

## Evidence

The check lives in the package's MSBuild targets, evaluated during
`dotnet build` / `dotnet publish` (before compilation), not in any runtime DLL.

- `SixLabors.ImageSharp.targets` v4.1.3 (NuGet `build/` folder):
  ```xml
  <Target Name="SixLabors_ValidateLicense"
          BeforeTargets="CoreCompile"
          Condition="'$(DesignTimeBuild)' != 'true'">
    ...
    <ValidateLicenseTask
      IsDebugBuild="$(Configuration.StartsWith('Debug'))"
      LicenseFilePath="@(LicenseFiles)"
      ...
      ContinueOnError="$(Configuration.StartsWith('Debug'))" />
  </Target>
  ```
  - `BeforeTargets="CoreCompile"` → fires during the build, before source
    compiles.
  - `ContinueOnError` is true only in Debug → in **Release the missing/invalid
    license is a hard build error**; in Debug it is a warning that allows
    compilation to proceed.

- The license verifier (`SixLabors.Licensing.dll`) ships only under the
  package's `build/{net6.0,net8.0,net472}/` folders — i.e. it is a
  **build-time** MSBuild task/analyzer, with no runtime equivalent in the
  published output. The runtime DLLs contain no license-read code path.

- License discovery (same target): `SixLaborsLicenseFile` MSBuild property →
  recursive `**/sixlabors.lic` search → `SixLaborsLicenseKey` environment
  variable. Discovery is scoped to `$(AssemblyName)` (the assembly being
  compiled), which for AddictedProxy is `AddictedProxy.Image` — the only
  project referencing ImageSharp.Web, and the assembly the committed
  `sixlabors.lic` was generated for.

Upstream corroboration: SixLabors/ImageSharp discussion #3139 ("make the license
check only apply to apps built in RELEASE mode") and
jrmitch120/wooly#386 ("build-time license check … the
`SixLabors_ValidateLicense` target, which runs before CoreCompile").

## Why production needs no image change

- AddictedProxy builds green in Release (`dotnet build -c Release`: 0 errors /
  0 warnings; v5.3.0 CI green) because `sixlabors.lic` is committed at
  `AddictedProxy.Image/sixlabors.lic` and the recursive `**/sixlabors.lic`
  search finds it during the Dockerfile `dotnet publish` stage.
- Since the check is purely at build time, the published runtime image need not
  contain `sixlabors.lic`. (Adding it would be dead weight, not a compliance
  requirement.)
- If a second project ever adds a direct ImageSharp reference, add that
  project's assembly name to `sixlabors.lic` (the license is assembly-scoped) so
  the build-time check passes for it too.

## References
- Package targets/props: `SixLabors.ImageSharp.targets`,
  `SixLabors.ImageSharp.Web.targets` (`build/` under
  `~/.nuget/packages/sixlabors.imagesharp{,.web)/<ver>/`).
- Discussion: https://github.com/SixLabors/ImageSharp/discussions/3139
- Existing plan: docs/domain-configurability-plan.md
