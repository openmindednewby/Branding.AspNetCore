using Branding.AspNetCore.Configuration;
using Branding.AspNetCore.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Shouldly;

namespace Branding.AspNetCore.Tests.Middleware;

/// <summary>
/// Unit tests for <see cref="BuiltByHeaderMiddleware"/>.
/// Uses a custom <see cref="IHttpResponseFeature"/> implementation so we can
/// capture and replay the OnStarting callback registered by the middleware,
/// without standing up a real HTTP server.
/// </summary>
public class BuiltByHeaderMiddlewareTests
{
    private const string CustomBuiltBy = "acme.example";
    private const string CustomHeader = "X-Powered-By";

    [Fact]
    public async Task InvokeAsync_WhenDisableHeaderTrue_DoesNotRegisterOnStarting()
    {
        // Arrange
        var options = new BrandingOptions { DisableHeader = true };
        var (context, captureFeature) = CreateContext();
        var nextCalled = false;
        var middleware = CreateMiddleware(options, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);
        await captureFeature.FireOnStartingAsync();

        // Assert
        nextCalled.ShouldBeTrue();
        captureFeature.OnStartingCallbackCount.ShouldBe(0);
        context.Response.Headers.ContainsKey(BrandingOptions.DefaultHeaderName).ShouldBeFalse();
    }

    [Fact]
    public async Task InvokeAsync_WhenDefaultOptions_AddsXBuiltByHeader()
    {
        // Arrange
        var options = new BrandingOptions();
        var (context, captureFeature) = CreateContext();
        var middleware = CreateMiddleware(options, _ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);
        await captureFeature.FireOnStartingAsync();

        // Assert
        context.Response.Headers[BrandingOptions.DefaultHeaderName]
            .ToString()
            .ShouldBe(BrandingOptions.DefaultBuiltBy);
    }

    [Fact]
    public async Task InvokeAsync_WhenCustomBuiltBy_AddsCustomValue()
    {
        // Arrange
        var options = new BrandingOptions { BuiltBy = CustomBuiltBy };
        var (context, captureFeature) = CreateContext();
        var middleware = CreateMiddleware(options, _ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);
        await captureFeature.FireOnStartingAsync();

        // Assert
        context.Response.Headers[BrandingOptions.DefaultHeaderName]
            .ToString()
            .ShouldBe(CustomBuiltBy);
    }

    [Fact]
    public async Task InvokeAsync_WhenCustomHeaderName_UsesCustomHeaderName()
    {
        // Arrange
        var options = new BrandingOptions { HeaderName = CustomHeader };
        var (context, captureFeature) = CreateContext();
        var middleware = CreateMiddleware(options, _ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);
        await captureFeature.FireOnStartingAsync();

        // Assert
        context.Response.Headers[CustomHeader]
            .ToString()
            .ShouldBe(BrandingOptions.DefaultBuiltBy);
        context.Response.Headers.ContainsKey(BrandingOptions.DefaultHeaderName).ShouldBeFalse();
    }

    [Fact]
    public async Task InvokeAsync_WhenHeaderAlreadyPresent_DoesNotOverwrite()
    {
        // Arrange
        const string preExistingValue = "someone-else";
        var options = new BrandingOptions();
        var (context, captureFeature) = CreateContext();
        context.Response.Headers[BrandingOptions.DefaultHeaderName] = preExistingValue;
        var middleware = CreateMiddleware(options, _ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);
        await captureFeature.FireOnStartingAsync();

        // Assert
        context.Response.Headers[BrandingOptions.DefaultHeaderName]
            .ToString()
            .ShouldBe(preExistingValue);
    }

    [Fact]
    public async Task InvokeAsync_WhenInvoked_CallsNextDelegate()
    {
        // Arrange
        var options = new BrandingOptions();
        var (context, _) = CreateContext();
        var nextCalled = false;
        var middleware = CreateMiddleware(options, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenStatusIs204_StillAddsHeader()
    {
        // Arrange
        var options = new BrandingOptions();
        var (context, captureFeature) = CreateContext();
        var middleware = CreateMiddleware(options, ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);
        await captureFeature.FireOnStartingAsync();

        // Assert
        context.Response.Headers[BrandingOptions.DefaultHeaderName]
            .ToString()
            .ShouldBe(BrandingOptions.DefaultBuiltBy);
        context.Response.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task InvokeAsync_WhenNextThrows_PropagatesException()
    {
        // Arrange
        var options = new BrandingOptions();
        var (context, _) = CreateContext();
        var middleware = CreateMiddleware(options, _ => throw new InvalidOperationException("boom"));

        // Act
        var thrown = await Should.ThrowAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));

        // Assert
        thrown.Message.ShouldBe("boom");
    }

    [Fact]
    public void Constructor_WhenOptionsProvided_DoesNotThrow()
    {
        // Arrange
        var options = new BrandingOptions { BuiltBy = CustomBuiltBy };

        // Act
        var middleware = CreateMiddleware(options, _ => Task.CompletedTask);

        // Assert
        middleware.ShouldNotBeNull();
    }

    private static BuiltByHeaderMiddleware CreateMiddleware(
        BrandingOptions options,
        RequestDelegate next)
    {
        return new BuiltByHeaderMiddleware(next, Options.Create(options));
    }

    private static (HttpContext context, OnStartingCaptureFeature feature) CreateContext()
    {
        var feature = new OnStartingCaptureFeature();
        var context = new DefaultHttpContext();
        context.Features.Set<IHttpResponseFeature>(feature);
        return (context, feature);
    }

    private sealed class OnStartingCaptureFeature : IHttpResponseFeature
    {
        private readonly List<(Func<object, Task> Callback, object State)> _callbacks = new();
        private readonly HeaderDictionary _headers = new();

        public int OnStartingCallbackCount => _callbacks.Count;

        public int StatusCode { get; set; } = StatusCodes.Status200OK;

        public string? ReasonPhrase { get; set; }

        public IHeaderDictionary Headers
        {
            get => _headers;
            set => throw new NotSupportedException();
        }

        public Stream Body { get; set; } = Stream.Null;

        public bool HasStarted { get; private set; }

        public void OnStarting(Func<object, Task> callback, object state)
        {
            _callbacks.Add((callback, state));
        }

        public void OnCompleted(Func<object, Task> callback, object state)
        {
        }

        public async Task FireOnStartingAsync()
        {
            HasStarted = true;
            foreach (var (callback, state) in _callbacks)
            {
                await callback(state);
            }
        }
    }
}
