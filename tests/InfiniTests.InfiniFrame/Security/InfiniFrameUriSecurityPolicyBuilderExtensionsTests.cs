// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;

namespace InfiniTests.InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameUriSecurityPolicyBuilderExtensionsTests {

    [Test]
    public async Task SetAllowedNavigationSchemes_WithValidSchemes_UpdatesPolicy(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        IInfiniFrameWindowBuilder result = builder.SetAllowedNavigationSchemes("https", "ftp");

        // Assert
        await Assert.That(result).IsSameReferenceAs(builder);
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsNavigationSchemeAllowed("https")).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("ftp")).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("http")).IsFalse();
    }

    [Test]
    public async Task SetAllowedNavigationSchemes_WithEmptyArray_ClearsSchemes(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetAllowedNavigationSchemes([]);

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsNavigationSchemeAllowed("https")).IsFalse();
    }

    [Test]
    public async Task SetAllowedNavigationSchemes_WithNullAndWhitespace_IgnoresThem(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetAllowedNavigationSchemes("https", null!, "  ", "");

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsNavigationSchemeAllowed("https")).IsTrue();
    }

    [Test]
    public async Task SetAllowedExternalSchemes_WithValidSchemes_UpdatesPolicy(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetAllowedExternalSchemes("https", "mailto", "ftp");

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsExternalSchemeAllowed("https")).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed("mailto")).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed("ftp")).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed("http")).IsFalse();
    }

    [Test]
    public async Task SetAllowedExternalSchemes_WithEmptyArray_ClearsSchemes(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetAllowedExternalSchemes([]);

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsExternalSchemeAllowed("https")).IsFalse();
    }

    [Test]
    public async Task SetTrustedOrigins_WithStringArray_UpdatesPolicy(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetTrustedOrigins("https://example.com", "https://localhost:5001");

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://localhost:5001"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://evil.com"))).IsFalse();
    }

    [Test]
    public async Task SetTrustedOrigins_WithInvalidString_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act & Assert
        await Assert.That(() => builder.SetTrustedOrigins("not-a-valid-uri"))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SetTrustedOrigins_WithUriArray_UpdatesPolicy(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
        Uri origin1 = new("https://example.com");
        Uri origin2 = new("https://localhost:5001");

        // Act
        builder.SetTrustedOrigins(origin1, origin2);

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsTrustedOrigin(origin1)).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(origin2)).IsTrue();
    }

    [Test]
    public async Task AddTrustedOrigin_WithString_AbsoluteUri_AddsOrigin(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.AddTrustedOrigin("https://example.com");

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com"))).IsTrue();
    }

    [Test]
    public async Task AddTrustedOrigin_WithString_RelativeUri_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act & Assert — use "://invalid" which has no scheme and is invalid on all platforms
        // ("/relative/path" is a valid file URI on Linux)
        await Assert.That(() => builder.AddTrustedOrigin("://invalid"))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task AddTrustedOrigin_WithUri_AddsOrigin(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
        Uri origin = new("https://example.com");

        // Act
        builder.AddTrustedOrigin(origin);

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsTrustedOrigin(origin)).IsTrue();
    }

    [Test]
    public async Task SetTrustAllOrigins_True_TrustsAnyOrigin(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetTrustAllOrigins();

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.TrustAllOrigins).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://evil.com"))).IsTrue();
    }

    [Test]
    public async Task SetTrustAllOrigins_False_DoesNotTrustAll(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
        builder.SetTrustAllOrigins();

        // Act
        builder.SetTrustAllOrigins(false);

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.TrustAllOrigins).IsFalse();
    }

    [Test]
    public async Task SetTrustAllOrigins_DefaultParameter_TrustsAll(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetTrustAllOrigins();

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.TrustAllOrigins).IsTrue();
    }

    [Test]
    public async Task AllMethods_ReturnBuilder_ForChaining(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act & Assert - All methods should return the builder for fluent chaining
        IInfiniFrameWindowBuilder result = builder
            .SetAllowedNavigationSchemes("https")
            .SetAllowedExternalSchemes("https")
            .SetTrustedOrigins("https://example.com")
            .AddTrustedOrigin("https://localhost:5001")
            .SetTrustAllOrigins(false);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task SetTrustedOrigins_MultipleCalls_ReplacesPreviousOrigins(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.SetTrustedOrigins("https://example.com");
        builder.SetTrustedOrigins("https://other.com");

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com"))).IsFalse();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://other.com"))).IsTrue();
    }

    [Test]
    public async Task AddTrustedOrigin_MultipleCalls_AccumulatesOrigins(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.AddTrustedOrigin("https://example.com");
        builder.AddTrustedOrigin("https://other.com");

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://other.com"))).IsTrue();
    }
}
