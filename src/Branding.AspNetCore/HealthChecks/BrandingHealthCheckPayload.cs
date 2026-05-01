namespace Branding.AspNetCore.HealthChecks;

/// <summary>
/// Lightweight DTO that wraps any health-check response with a standardized
/// <c>builtBy</c> field so health probes also carry the portfolio attribution.
/// Consumers serialize this in their health-check ResponseWriter.
/// </summary>
public sealed class BrandingHealthCheckPayload
{
    /// <summary>
    /// Attribution string (e.g. "dloizides.com").
    /// </summary>
    public string BuiltBy { get; set; } = default!;

    /// <summary>
    /// Original health-check payload object as returned by the host's writer.
    /// </summary>
    public object Health { get; set; } = default!;
}
