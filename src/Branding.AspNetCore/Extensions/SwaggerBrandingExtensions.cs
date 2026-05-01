using Branding.AspNetCore.Configuration;

namespace Branding.AspNetCore.Extensions;

/// <summary>
/// Helpers for embedding the attribution into Swagger UI without taking a
/// hard dependency on Swashbuckle. Consumers paste the returned HTML into
/// their Swagger UI HeadContent / IndexStream / custom HTML page.
/// </summary>
public static class SwaggerBrandingExtensions
{
    /// <summary>
    /// Returns a self-contained &lt;style&gt; + &lt;script&gt; snippet that injects a
    /// discreet "built by" footer into a Swagger UI page. Renders at ~70% opacity,
    /// 11px, links to <see cref="BrandingOptions.BuiltByUrl"/>.
    /// </summary>
    /// <remarks>
    /// Use as the <c>HeadContent</c> of <c>SwaggerUIOptions</c>:
    /// <code>
    /// app.UseSwaggerUI(o =>
    /// {
    ///     o.HeadContent += SwaggerBrandingExtensions.BuildHeadSnippet(brandingOptions);
    /// });
    /// </code>
    /// </remarks>
    public static string BuildHeadSnippet(BrandingOptions options)
    {
        if (options.DisableSwaggerFooter)
        {
            return string.Empty;
        }

        var safeText = HtmlEncode(options.BuiltBy);
        var safeUrl = HtmlEncode(options.BuiltByUrl);

        return $@"
<style>
  .built-by-footer {{
    position: fixed;
    bottom: 8px;
    right: 12px;
    font-family: system-ui, -apple-system, sans-serif;
    font-size: 11px;
    opacity: 0.65;
    z-index: 9999;
    pointer-events: auto;
  }}
  .built-by-footer a {{
    color: inherit;
    text-decoration: none;
  }}
  .built-by-footer a:hover {{
    text-decoration: underline;
  }}
</style>
<script>
  document.addEventListener('DOMContentLoaded', function () {{
    var footer = document.createElement('div');
    footer.className = 'built-by-footer';
    footer.innerHTML = 'built by <a href=""{safeUrl}"" target=""_blank"" rel=""noopener noreferrer"">{safeText}</a>';
    document.body.appendChild(footer);
  }});
</script>";
    }

    private static string HtmlEncode(string value)
    {
        return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
    }
}
