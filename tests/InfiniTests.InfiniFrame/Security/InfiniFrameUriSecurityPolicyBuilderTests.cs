// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Security;

namespace InfiniTests.InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameUriSecurityPolicyBuilderTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_NoBasePolicy_UsesDefault(CancellationToken ct = default) {
        // Arrange & Act
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Assert, default policy allows app scheme
        InfiniFrameUriSecurityPolicy policy = builder.Build();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsTrue();
    }

    [Test]
    public async Task Constructor_WithBasePolicy_CopiesSettings(CancellationToken ct = default) {
        // Arrange
        var basePolicy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [Uri.UriSchemeMailto],
            [new Uri("https://trusted.example/")]
        );

        // Act
        var builder = new InfiniFrameUriSecurityPolicyBuilder(basePolicy);
        InfiniFrameUriSecurityPolicy policy = builder.Build();

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsFalse();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://trusted.example/path"))).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetAllowedNavigationSchemes
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SetAllowedNavigationSchemes_ReplacesExistingSchemes(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps, Uri.UriSchemeFtp])
            .Build();

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeFtp)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsFalse();
    }

    [Test]
    public async Task SetAllowedNavigationSchemes_IgnoresNullOrWhitespace(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetAllowedNavigationSchemes([null!, "", "  ", Uri.UriSchemeHttps])
            .Build();

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.AllowedNavigationSchemes.Count).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetAllowedExternalSchemes
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SetAllowedExternalSchemes_ReplacesExistingSchemes(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetAllowedExternalSchemes([Uri.UriSchemeMailto])
            .Build();

        // Assert
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeHttps)).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AllowNavigationScheme
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AllowNavigationScheme_AddsScheme(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .AllowNavigationScheme(Uri.UriSchemeHttps)
            .AllowNavigationScheme(Uri.UriSchemeFtp)
            .Build();

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeFtp)).IsTrue();
    }

    [Test]
    public async Task AllowNavigationScheme_IgnoresNull(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act & Assert
        await Assert.That(() => builder.AllowNavigationScheme(null!)).ThrowsNothing();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AllowExternalScheme
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AllowExternalScheme_AddsScheme(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .AllowExternalScheme(Uri.UriSchemeMailto)
            .Build();

        // Assert
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetTrustedOrigins
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SetTrustedOrigins_ReplacesExistingOrigins(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetTrustedOrigins([new Uri("https://one.example/"), new Uri("https://two.example/")])
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .Build();

        // Assert
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://one.example/path"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://two.example/path"))).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AddTrustedOrigin
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AddTrustedOrigin_AbsoluteUri_AddsOrigin(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .AddTrustedOrigin(new Uri("https://example.com"))
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .Build();

        // Assert
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com/path"))).IsTrue();
    }

    [Test]
    public async Task AddTrustedOrigin_RelativeUri_Ignores(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .AddTrustedOrigin(new Uri("/relative", UriKind.Relative))
            .Build();

        // Assert
        await Assert.That(policy.TrustedOrigins.Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetTrustAllOrigins
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SetTrustAllOrigins_True_TrustsAnyOrigin(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetTrustAllOrigins()
            .Build();

        // Assert
        await Assert.That(policy.TrustAllOrigins).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://anywhere.example/path"))).IsTrue();
    }

    [Test]
    public async Task SetTrustAllOrigins_False_DoesNotTrustAll(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetTrustAllOrigins(false)
            .Build();

        // Assert
        await Assert.That(policy.TrustAllOrigins).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Build
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Build_ReturnsPolicyWithConfiguredValues(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = builder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetAllowedExternalSchemes([Uri.UriSchemeMailto])
            .AddTrustedOrigin(new Uri("https://trusted.example/"))
            .SetTrustAllOrigins()
            .Build();

        // Assert
        await Assert.That(policy.AllowedNavigationSchemes).Contains(Uri.UriSchemeHttps);
        await Assert.That(policy.AllowedExternalSchemes).Contains(Uri.UriSchemeMailto);
        await Assert.That(policy.TrustedOrigins.Count).IsEqualTo(1);
        await Assert.That(policy.TrustAllOrigins).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Chaining
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AllMethods_ReturnBuilder_ForChaining(CancellationToken ct = default) {
        // Arrange & Act
        var builder = new InfiniFrameUriSecurityPolicyBuilder();
        InfiniFrameUriSecurityPolicyBuilder result = builder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetAllowedExternalSchemes([Uri.UriSchemeMailto])
            .AllowNavigationScheme(Uri.UriSchemeFtp)
            .AllowExternalScheme("custom")
            .SetTrustedOrigins([new Uri("https://example.com/")])
            .AddTrustedOrigin(new Uri("https://other.com/"))
            .SetTrustAllOrigins();

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
    }
}
