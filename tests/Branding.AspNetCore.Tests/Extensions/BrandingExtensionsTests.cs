using Branding.AspNetCore.Configuration;
using Branding.AspNetCore.Extensions;
using Branding.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shouldly;

namespace Branding.AspNetCore.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="BrandingExtensions"/>.
/// </summary>
public class BrandingExtensionsTests
{
    private const string ConfigBuiltBy = "from-config.example";
    private const string ConfigBuiltByUrl = "https://from-config.example";
    private const string OverrideBuiltBy = "from-code.example";

    [Fact]
    public void AddBranding_WhenConfigurationHasSection_BindsValuesFromConfig()
    {
        // Arrange
        var builder = CreateHostBuilder(new Dictionary<string, string?>
        {
            [$"{BrandingOptions.SectionName}:BuiltBy"] = ConfigBuiltBy,
            [$"{BrandingOptions.SectionName}:BuiltByUrl"] = ConfigBuiltByUrl,
        });

        // Act
        var result = builder.AddBranding();
        using var provider = builder.Services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IOptions<BrandingOptions>>().Value;

        // Assert
        result.ShouldBeSameAs(builder);
        resolved.BuiltBy.ShouldBe(ConfigBuiltBy);
        resolved.BuiltByUrl.ShouldBe(ConfigBuiltByUrl);
    }

    [Fact]
    public void AddBranding_WhenNoConfigurationSection_RetainsDefaultValues()
    {
        // Arrange
        var builder = CreateHostBuilder(new Dictionary<string, string?>());

        // Act
        builder.AddBranding();
        using var provider = builder.Services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IOptions<BrandingOptions>>().Value;

        // Assert
        resolved.BuiltBy.ShouldBe(BrandingOptions.DefaultBuiltBy);
        resolved.BuiltByUrl.ShouldBe(BrandingOptions.DefaultBuiltByUrl);
        resolved.HeaderName.ShouldBe(BrandingOptions.DefaultHeaderName);
    }

    [Fact]
    public void AddBranding_WhenConfigureCallbackProvided_AppliesOverridesAfterConfigBinding()
    {
        // Arrange
        var builder = CreateHostBuilder(new Dictionary<string, string?>
        {
            [$"{BrandingOptions.SectionName}:BuiltBy"] = ConfigBuiltBy,
        });

        // Act
        builder.AddBranding(opts =>
        {
            opts.BuiltBy = OverrideBuiltBy;
            opts.DisableHeader = true;
        });
        using var provider = builder.Services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IOptions<BrandingOptions>>().Value;

        // Assert
        resolved.BuiltBy.ShouldBe(OverrideBuiltBy);
        resolved.DisableHeader.ShouldBeTrue();
    }

    [Fact]
    public void AddBranding_WhenConfigureCallbackNull_DoesNotThrow()
    {
        // Arrange
        var builder = CreateHostBuilder(new Dictionary<string, string?>());

        // Act
        var act = () => builder.AddBranding(configure: null);

        // Assert
        act.ShouldNotThrow();
    }

    [Fact]
    public void GetBrandingOptions_WhenOptionsRegistered_ReturnsResolvedInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Configure<BrandingOptions>(o => o.BuiltBy = ConfigBuiltBy);
        using var provider = services.BuildServiceProvider();

        // Act
        var resolved = provider.GetBrandingOptions();

        // Assert
        resolved.BuiltBy.ShouldBe(ConfigBuiltBy);
    }

    [Fact]
    public void GetBrandingOptions_WhenOptionsNotRegistered_ReturnsDefaultInstance()
    {
        // Arrange — IOptions<T> resolves a default instance even without explicit Configure
        var services = new ServiceCollection();
        services.AddOptions<BrandingOptions>();
        using var provider = services.BuildServiceProvider();

        // Act
        var resolved = provider.GetBrandingOptions();

        // Assert
        resolved.BuiltBy.ShouldBe(BrandingOptions.DefaultBuiltBy);
    }

    [Fact]
    public async Task UseBuiltByHeader_WhenInvoked_AddsMiddlewareThatRunsBeforeTerminal()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOptions<BrandingOptions>();
        services.AddLogging();
        services.AddSingleton<BuiltByHeaderMiddleware>();
        var provider = services.BuildServiceProvider();

        var app = new ApplicationBuilder(provider);
        var terminalCalled = false;
        app.Run(_ =>
        {
            terminalCalled = true;
            return Task.CompletedTask;
        });

        // Act
        var resultBuilder = new ApplicationBuilder(provider);
        var sameApp = resultBuilder.UseBuiltByHeader();
        sameApp.Run(_ =>
        {
            terminalCalled = true;
            return Task.CompletedTask;
        });
        var pipeline = sameApp.Build();

        var context = new DefaultHttpContext { RequestServices = provider };
        await pipeline(context);

        // Assert
        sameApp.ShouldBeSameAs(resultBuilder);
        terminalCalled.ShouldBeTrue();
    }

    private static HostApplicationBuilder CreateHostBuilder(IDictionary<string, string?> configValues)
    {
        var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
        builder.Configuration.AddInMemoryCollection(configValues);
        return builder;
    }
}
