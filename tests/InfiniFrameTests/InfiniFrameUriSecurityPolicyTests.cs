// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared.TestDoubles;

namespace InfiniFrameTests;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameUriSecurityPolicyTests {
    [Test]
    public async Task IsTrustedOrigin_MatchesBySchemeHostAndPortOnly() {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            allowedNavigationSchemes: [Uri.UriSchemeHttps],
            allowedExternalSchemes: [Uri.UriSchemeHttps],
            trustedOrigins: [new Uri("https://example.com/base-path")]
        );

        // Act
        bool trustedSameOriginDifferentPath = policy.IsTrustedOrigin(new Uri("https://example.com/some/other/path"));
        bool trustedDifferentPort = policy.IsTrustedOrigin(new Uri("https://example.com:444/"));

        // Assert
        await Assert.That(trustedSameOriginDifferentPath).IsTrue();
        await Assert.That(trustedDifferentPort).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_RequiresAllowedNavigationScheme() {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            allowedNavigationSchemes: ["app"],
            allowedExternalSchemes: [Uri.UriSchemeHttps],
            trustedOrigins: [new Uri("https://example.com/")]
        );

        // Act
        bool trusted = policy.IsTrustedOrigin(new Uri("https://example.com/"));

        // Assert
        await Assert.That(trusted).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_WithTrustAllOrigins_TrustsAnyOriginWithAllowedScheme() {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            allowedNavigationSchemes: [Uri.UriSchemeHttps],
            allowedExternalSchemes: [Uri.UriSchemeHttps],
            trustedOrigins: [],
            trustAllOrigins: true
        );

        // Act
        bool trusted = policy.IsTrustedOrigin(new Uri("https://random.example/"));
        bool trustedDisallowedScheme = policy.IsTrustedOrigin(new Uri("http://random.example/"));

        // Assert
        await Assert.That(trusted).IsTrue();
        await Assert.That(trustedDisallowedScheme).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_WithNullCandidate_ThrowsArgumentNullException() {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            allowedNavigationSchemes: [Uri.UriSchemeHttps],
            allowedExternalSchemes: [Uri.UriSchemeHttps],
            trustedOrigins: [new Uri("https://example.com/")]
        );

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => {
            policy.IsTrustedOrigin(null!);
        }));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("candidateOrigin");
    }

    [Test]
    public async Task PolicyBuilder_SetAllowedSchemes_TrimsAndIgnoresWhitespace() {
        // Arrange
        var policyBuilder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = policyBuilder
            .SetAllowedNavigationSchemes([" https ", "", "   "])
            .SetAllowedExternalSchemes([" mailto ", "\t", ""])
            .Build();

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttp)).IsFalse();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeHttps)).IsFalse();
    }

    [Test]
    public async Task PolicyBuilder_AddTrustedOrigin_IgnoresRelativeUri() {
        // Arrange
        var policyBuilder = new InfiniFrameUriSecurityPolicyBuilder();

        // Act
        InfiniFrameUriSecurityPolicy policy = policyBuilder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .AddTrustedOrigin(new Uri("/relative", UriKind.Relative))
            .AddTrustedOrigin(new Uri("https://trusted.example/"))
            .Build();

        // Assert
        await Assert.That(policy.TrustedOrigins.Count).IsEqualTo(1);
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://trusted.example/anything"))).IsTrue();
    }

    [Test]
    public async Task Registry_GetForWindow_UsesBoundPolicyWhenAvailable() {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        var policy = new InfiniFrameUriSecurityPolicy(
            allowedNavigationSchemes: [Uri.UriSchemeHttps],
            allowedExternalSchemes: [Uri.UriSchemeMailto],
            trustedOrigins: [new Uri("https://trusted.example/")]
        );

        // Act
        InfiniFrameUriSecurityPolicy defaultPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window.Window, policy);
        InfiniFrameUriSecurityPolicy boundPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);

        // Assert
        await Assert.That(defaultPolicy.IsNavigationSchemeAllowed("app")).IsTrue();
        await Assert.That(boundPolicy).IsSameReferenceAs(policy);
    }

    [Test]
    public async Task Registry_ConfigureForBuilder_UpdatesBuilderPolicy() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, policyBuilder => policyBuilder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetAllowedExternalSchemes([Uri.UriSchemeMailto])
            .SetTrustedOrigins([new Uri("https://trusted.example/")]));
        InfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsFalse();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://trusted.example/path"))).IsTrue();
    }

    [Test]
    public async Task BuilderExtensions_SetTrustedOriginsWithStrings_UpdatesBuilderPolicy() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder
            .SetAllowedNavigationSchemes(Uri.UriSchemeHttps)
            .SetTrustedOrigins("https://trusted.example/");
        InfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://trusted.example/abc"))).IsTrue();
    }

    [Test]
    public async Task BuilderExtensions_SetTrustedOriginsWithInvalidString_ThrowsArgumentException() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => {
            builder.SetTrustedOrigins("not-a-valid-origin");
        }));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("origin");
    }

    [Test]
    public async Task Registry_ConfigureForBuilder_CanAppendTrustedOriginsAcrossCalls() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, policyBuilder => policyBuilder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetTrustedOrigins([new Uri("https://one.example/")]));
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, policyBuilder => policyBuilder
            .AddTrustedOrigin(new Uri("https://two.example/")));
        InfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://one.example/path"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://two.example/path"))).IsTrue();
    }

    [Test]
    public async Task BuilderExtensions_SetTrustAllOrigins_UpdatesBuilderPolicy() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder
            .SetAllowedNavigationSchemes(Uri.UriSchemeHttps)
            .SetTrustAllOrigins();
        InfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.TrustAllOrigins).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://anywhere.example/path"))).IsTrue();
    }
}
