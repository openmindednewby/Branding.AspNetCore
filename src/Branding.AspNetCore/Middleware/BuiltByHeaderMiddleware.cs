using Branding.AspNetCore.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Branding.AspNetCore.Middleware;

/// <summary>
/// Adds the configured "built by" attribution as an HTTP response header on every response.
/// </summary>
public sealed class BuiltByHeaderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BrandingOptions _options;

    /// <summary>
    /// Creates the middleware with the resolved <see cref="BrandingOptions"/>.
    /// </summary>
    public BuiltByHeaderMiddleware(RequestDelegate next, IOptions<BrandingOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    /// <summary>
    /// Hooks the response start event to write the attribution header.
    /// </summary>
    public Task InvokeAsync(HttpContext context)
    {
        if (!_options.DisableHeader)
        {
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(_options.HeaderName))
                {
                    context.Response.Headers[_options.HeaderName] = _options.BuiltBy;
                }
                return Task.CompletedTask;
            });
        }

        return _next(context);
    }
}
