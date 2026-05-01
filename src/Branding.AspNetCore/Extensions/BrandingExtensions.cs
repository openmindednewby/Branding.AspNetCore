using Branding.AspNetCore.Configuration;
using Branding.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Branding.AspNetCore.Extensions;

/// <summary>
/// Builder extensions for registering portfolio-wide attribution.
/// </summary>
public static class BrandingExtensions
{
    /// <summary>
    /// Registers the <see cref="BrandingOptions"/> bound to the "Branding" section
    /// of configuration, with optional code-level overrides.
    /// </summary>
    public static TBuilder AddBranding<TBuilder>(
        this TBuilder builder,
        Action<BrandingOptions>? configure = null)
        where TBuilder : IHostApplicationBuilder
    {
        var section = builder.Configuration.GetSection(BrandingOptions.SectionName);

        builder.Services
            .AddOptions<BrandingOptions>()
            .Bind(section)
            .Configure(opts =>
            {
                configure?.Invoke(opts);
            });

        return builder;
    }

    /// <summary>
    /// Adds the <see cref="BuiltByHeaderMiddleware"/> to the pipeline. Place it early
    /// (before MVC/endpoint routing) so the header is set on every response.
    /// </summary>
    public static IApplicationBuilder UseBuiltByHeader(this IApplicationBuilder app)
    {
        return app.UseMiddleware<BuiltByHeaderMiddleware>();
    }

    /// <summary>
    /// Convenience accessor used by Swagger UI builders and health-check writers.
    /// </summary>
    public static BrandingOptions GetBrandingOptions(this IServiceProvider services)
    {
        return services.GetRequiredService<IOptions<BrandingOptions>>().Value;
    }
}
