using Branding.AspNetCore.HealthChecks;
using Shouldly;

namespace Branding.AspNetCore.Tests.HealthChecks;

/// <summary>
/// Unit tests for <see cref="BrandingHealthCheckPayload"/>.
/// </summary>
public class BrandingHealthCheckPayloadTests
{
    [Fact]
    public void BuiltBy_WhenSet_StoresValue()
    {
        // Arrange
        const string attribution = "dloizides.com";

        // Act
        var payload = new BrandingHealthCheckPayload { BuiltBy = attribution };

        // Assert
        payload.BuiltBy.ShouldBe(attribution);
    }

    [Fact]
    public void Health_WhenSet_StoresProvidedObject()
    {
        // Arrange
        var healthBody = new { status = "Healthy", entries = Array.Empty<string>() };

        // Act
        var payload = new BrandingHealthCheckPayload
        {
            BuiltBy = "x",
            Health = healthBody,
        };

        // Assert
        payload.Health.ShouldBeSameAs(healthBody);
    }

    [Fact]
    public void Payload_WhenInstantiatedWithBothProperties_KeepsBothValues()
    {
        // Arrange
        const string attribution = "dloizides.com";
        var healthBody = new Dictionary<string, object> { ["status"] = "Healthy" };

        // Act
        var payload = new BrandingHealthCheckPayload
        {
            BuiltBy = attribution,
            Health = healthBody,
        };

        // Assert
        payload.BuiltBy.ShouldBe(attribution);
        payload.Health.ShouldBeSameAs(healthBody);
    }
}
