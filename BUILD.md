# Building and Publishing Branding.AspNetCore

## Prerequisites

- .NET 10 SDK
- nuget.org API key with push permissions for `Branding.AspNetCore*`

## Build

```bash
dotnet restore src/Branding.AspNetCore/Branding.AspNetCore.csproj
dotnet build src/Branding.AspNetCore/Branding.AspNetCore.csproj -c Release --no-restore
dotnet pack src/Branding.AspNetCore/Branding.AspNetCore.csproj -c Release -o artifacts --no-build
```

## Publish

```powershell
.\publish.ps1 -Bump patch -ApiKey YOUR_KEY
```

The script auto-bumps the version in `Directory.Build.props`, builds, packs, pushes, and rolls back on failure.

## Versioning

Semantic versioning: `patch` for bug fixes, `minor` for new features, `major` for breaking changes.

## Release Checklist

- [ ] Update `Directory.Build.props` version (handled by publish.ps1)
- [ ] Run `dotnet build -c Release`
- [ ] Test consumption locally in one service
- [ ] Run `.\publish.ps1`
- [ ] Wait 5–15 min for nuget.org indexing
- [ ] Verify on https://www.nuget.org/packages/Branding.AspNetCore
- [ ] Bump consumers' `Directory.Packages.props` references
