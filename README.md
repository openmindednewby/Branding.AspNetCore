# Branding.AspNetCore

Portfolio-wide attribution middleware for ASP.NET Core services. One package, three surfaces:

1. **HTTP response header** — `X-Built-By: dloizides.com` on every response.
2. **Swagger UI footer** — discreet "built by dloizides.com" link at the bottom of the API docs page.
3. **Health-check payload** — `builtBy` field included in `/health/*` responses.

Purpose: every service Demetris ships carries the same operator attribution without per-service boilerplate. White-label opt-out via `BrandingOptions.Disable*` flags.

## Installation

```bash
dotnet add package Branding.AspNetCore
```

## Quick Start

```csharp
using Branding.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddBranding();

var app = builder.Build();

app.UseBuiltByHeader();   // place early so every response carries X-Built-By

// Swagger UI (optional, only if the service exposes Swagger UI)
var brandingOptions = app.Services.GetBrandingOptions();
app.UseSwaggerUI(o =>
{
    o.HeadContent += SwaggerBrandingExtensions.BuildHeadSnippet(brandingOptions);
});

app.Run();
```

## Configuration

`appsettings.json`:

```json
{
  "Branding": {
    "BuiltBy": "dloizides.com",
    "BuiltByUrl": "https://dloizides.com",
    "HeaderName": "X-Built-By",
    "DisableHeader": false,
    "DisableSwaggerFooter": false,
    "DisableHealthCheckField": false
  }
}
```

All fields are optional — the defaults above are baked in.

## Code-level overrides

```csharp
builder.AddBranding(opts =>
{
    opts.DisableSwaggerFooter = true;          // white-label this service's API docs
});
```

## Health-check payload

`Branding.AspNetCore` exposes a small DTO (`BrandingHealthCheckPayload`) you wrap your existing health-check object with in your own ResponseWriter. This avoids forcing every service to re-implement health-check JSON shaping. See `ServiceDefaults.HealthChecks` for an example writer.

## Why this exists

Decided in the Questioner/OnlineMenu product split (2026-05-01). Every product Demetris ships needs to display a discreet "built by dloizides.com" attribution to act as a portfolio-wide credibility signal and traffic loop, without leaking sibling products. This package is the backend half; the frontend half is `@dloizides/ui-primitives`'s `<PoweredByFooter />`.

## License

MIT
