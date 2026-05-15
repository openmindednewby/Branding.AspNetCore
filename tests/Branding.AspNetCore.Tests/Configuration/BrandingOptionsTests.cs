using Branding.AspNetCore.Configuration;
using Shouldly;

namespace Branding.AspNetCore.Tests.Configuration;

/// <summary>
/// Unit tests for <see cref="BrandingOptions"/>.
/// Verifies default values, constants, and that property setters honour overrides.
/// </summary>
public class BrandingOptionsTests
{
    [Fact]
    public void SectionName_Constant_EqualsBranding()
    {
        // Assert
        BrandingOptions.SectionName.ShouldBe("Branding");
    }

    [Fact]
    public void DefaultBuiltBy_Constant_EqualsDloizidesCom()
    {
        // Assert
        BrandingOptions.DefaultBuiltBy.ShouldBe("dloizides.com");
    }

    [Fact]
    public void DefaultBuiltByUrl_Constant_EqualsHttpsDloizidesCom()
    {
        // Assert
        BrandingOptions.DefaultBuiltByUrl.ShouldBe("https://dloizides.com");
    }

    [Fact]
    public void DefaultHeaderName_Constant_EqualsXBuiltBy()
    {
        // Assert
        BrandingOptions.DefaultHeaderName.ShouldBe("X-Built-By");
    }

    [Fact]
    public void Constructor_WhenInstantiated_BuiltByMatchesDefault()
    {
        // Act
        var options = new BrandingOptions();

        // Assert
        options.BuiltBy.ShouldBe(BrandingOptions.DefaultBuiltBy);
    }

    [Fact]
    public void Constructor_WhenInstantiated_BuiltByUrlMatchesDefault()
    {
        // Act
        var options = new BrandingOptions();

        // Assert
        options.BuiltByUrl.ShouldBe(BrandingOptions.DefaultBuiltByUrl);
    }

    [Fact]
    public void Constructor_WhenInstantiated_HeaderNameMatchesDefault()
    {
        // Act
        var options = new BrandingOptions();

        // Assert
        options.HeaderName.ShouldBe(BrandingOptions.DefaultHeaderName);
    }

    [Fact]
    public void Constructor_WhenInstantiated_DisableHeaderIsFalse()
    {
        // Act
        var options = new BrandingOptions();

        // Assert
        options.DisableHeader.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WhenInstantiated_DisableSwaggerFooterIsFalse()
    {
        // Act
        var options = new BrandingOptions();

        // Assert
        options.DisableSwaggerFooter.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WhenInstantiated_DisableHealthCheckFieldIsFalse()
    {
        // Act
        var options = new BrandingOptions();

        // Assert
        options.DisableHealthCheckField.ShouldBeFalse();
    }

    [Fact]
    public void BuiltBy_WhenOverridden_StoresProvidedValue()
    {
        // Arrange
        const string customAttribution = "acme.example";

        // Act
        var options = new BrandingOptions { BuiltBy = customAttribution };

        // Assert
        options.BuiltBy.ShouldBe(customAttribution);
    }

    [Fact]
    public void BuiltByUrl_WhenOverridden_StoresProvidedValue()
    {
        // Arrange
        const string customUrl = "https://acme.example";

        // Act
        var options = new BrandingOptions { BuiltByUrl = customUrl };

        // Assert
        options.BuiltByUrl.ShouldBe(customUrl);
    }

    [Fact]
    public void HeaderName_WhenOverridden_StoresProvidedValue()
    {
        // Arrange
        const string customHeader = "X-Powered-By";

        // Act
        var options = new BrandingOptions { HeaderName = customHeader };

        // Assert
        options.HeaderName.ShouldBe(customHeader);
    }

    [Fact]
    public void DisableHeader_WhenSetTrue_StoresTrue()
    {
        // Act
        var options = new BrandingOptions { DisableHeader = true };

        // Assert
        options.DisableHeader.ShouldBeTrue();
    }

    [Fact]
    public void DisableSwaggerFooter_WhenSetTrue_StoresTrue()
    {
        // Act
        var options = new BrandingOptions { DisableSwaggerFooter = true };

        // Assert
        options.DisableSwaggerFooter.ShouldBeTrue();
    }

    [Fact]
    public void DisableHealthCheckField_WhenSetTrue_StoresTrue()
    {
        // Act
        var options = new BrandingOptions { DisableHealthCheckField = true };

        // Assert
        options.DisableHealthCheckField.ShouldBeTrue();
    }
}
