using Branding.AspNetCore.Configuration;
using Branding.AspNetCore.Extensions;
using Shouldly;

namespace Branding.AspNetCore.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="SwaggerBrandingExtensions"/>.
/// </summary>
public class SwaggerBrandingExtensionsTests
{
    [Fact]
    public void BuildHeadSnippet_WhenDisableSwaggerFooterTrue_ReturnsEmptyString()
    {
        // Arrange
        var options = new BrandingOptions { DisableSwaggerFooter = true };

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldBe(string.Empty);
    }

    [Fact]
    public void BuildHeadSnippet_WhenEnabled_ContainsStyleAndScriptBlocks()
    {
        // Arrange
        var options = new BrandingOptions();

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain("<style>");
        snippet.ShouldContain("</style>");
        snippet.ShouldContain("<script>");
        snippet.ShouldContain("</script>");
    }

    [Fact]
    public void BuildHeadSnippet_WhenEnabled_ReferencesBuiltByFooterCssClass()
    {
        // Arrange
        var options = new BrandingOptions();

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain(".built-by-footer");
        snippet.ShouldContain("built-by-footer");
    }

    [Fact]
    public void BuildHeadSnippet_WhenDefaultOptions_IncludesDefaultBuiltByText()
    {
        // Arrange
        var options = new BrandingOptions();

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain(BrandingOptions.DefaultBuiltBy);
    }

    [Fact]
    public void BuildHeadSnippet_WhenDefaultOptions_IncludesDefaultBuiltByUrl()
    {
        // Arrange
        var options = new BrandingOptions();

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain(BrandingOptions.DefaultBuiltByUrl);
    }

    [Fact]
    public void BuildHeadSnippet_WhenBuiltByContainsScriptTag_HtmlEncodesText()
    {
        // Arrange
        var options = new BrandingOptions
        {
            BuiltBy = "<script>alert('xss')</script>",
        };

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldNotContain("<script>alert('xss')</script>");
        snippet.ShouldContain("&lt;script&gt;");
        snippet.ShouldContain("&lt;/script&gt;");
    }

    [Fact]
    public void BuildHeadSnippet_WhenBuiltByUrlContainsHtml_HtmlEncodesUrl()
    {
        // Arrange
        var options = new BrandingOptions
        {
            BuiltByUrl = "https://example.com\" onclick=\"alert('xss')",
        };

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldNotContain("\" onclick=\"alert");
        snippet.ShouldContain("&quot;");
    }

    [Fact]
    public void BuildHeadSnippet_WhenBuiltByContainsAmpersand_EncodesAsAmp()
    {
        // Arrange
        var options = new BrandingOptions { BuiltBy = "Tom & Jerry" };

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain("Tom &amp; Jerry");
    }

    [Fact]
    public void BuildHeadSnippet_WhenEnabled_RegistersDomContentLoadedListener()
    {
        // Arrange
        var options = new BrandingOptions();

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain("DOMContentLoaded");
        snippet.ShouldContain("createElement");
        snippet.ShouldContain("appendChild");
    }

    [Fact]
    public void BuildHeadSnippet_WhenEnabled_LinkOpensInNewTabWithNoopener()
    {
        // Arrange
        var options = new BrandingOptions();

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain("target=\"_blank\"");
        snippet.ShouldContain("noopener");
        snippet.ShouldContain("noreferrer");
    }

    [Fact]
    public void BuildHeadSnippet_WhenBuiltByForcedToNull_FallsBackToEmptyString()
    {
        // Arrange — force the non-nullable property to null to exercise the
        // defensive `value ?? string.Empty` branch inside the private HtmlEncode helper.
        var options = new BrandingOptions
        {
            BuiltBy = null!,
            BuiltByUrl = null!,
        };

        // Act
        var snippet = SwaggerBrandingExtensions.BuildHeadSnippet(options);

        // Assert
        snippet.ShouldContain("<style>");
        snippet.ShouldContain("built by <a href=\"\"");
    }
}
