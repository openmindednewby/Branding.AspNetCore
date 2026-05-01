namespace Branding.AspNetCore.Configuration;

/// <summary>
/// Configuration for portfolio-wide attribution.
/// </summary>
public sealed class BrandingOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Branding";

    /// <summary>
    /// Default attribution text rendered in headers, Swagger UI, and health-check payloads.
    /// </summary>
    public const string DefaultBuiltBy = "dloizides.com";

    /// <summary>
    /// Default URL the attribution links to.
    /// </summary>
    public const string DefaultBuiltByUrl = "https://dloizides.com";

    /// <summary>
    /// HTTP response header name for the attribution.
    /// </summary>
    public const string DefaultHeaderName = "X-Built-By";

    /// <summary>
    /// Attribution text. Defaults to <see cref="DefaultBuiltBy"/>.
    /// </summary>
    public string BuiltBy { get; set; } = DefaultBuiltBy;

    /// <summary>
    /// Attribution link target. Defaults to <see cref="DefaultBuiltByUrl"/>.
    /// </summary>
    public string BuiltByUrl { get; set; } = DefaultBuiltByUrl;

    /// <summary>
    /// HTTP response header name. Defaults to <see cref="DefaultHeaderName"/>.
    /// </summary>
    public string HeaderName { get; set; } = DefaultHeaderName;

    /// <summary>
    /// When true, suppresses the response header. White-label / B2B opt-out hook.
    /// </summary>
    public bool DisableHeader { get; set; }

    /// <summary>
    /// When true, suppresses the Swagger UI footer injection.
    /// </summary>
    public bool DisableSwaggerFooter { get; set; }

    /// <summary>
    /// When true, suppresses the health-check payload field.
    /// </summary>
    public bool DisableHealthCheckField { get; set; }
}
