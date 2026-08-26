// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;
using InfiniTests.Substitutes;

namespace InfiniTests.InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameUriSecurityPolicyTests {
    [Test]
    public async Task IsTrustedOrigin_MatchesBySchemeHostAndPortOnly(CancellationToken ct = default) {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [Uri.UriSchemeHttps],
            [new Uri("https://example.com/base-path")]
        );

        // Act
        bool trustedSameOriginDifferentPath = policy.IsTrustedOrigin(new Uri("https://example.com/some/other/path"));
        bool trustedDifferentPort = policy.IsTrustedOrigin(new Uri("https://example.com:444/"));

        // Assert
        await Assert.That(trustedSameOriginDifferentPath).IsTrue();
        await Assert.That(trustedDifferentPort).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_AppOrigin_IgnoresPathQueryAndFragment(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            ["app"],
            [],
            [new Uri("app://localhost/index.html?startup=true#settings")]
        );

        bool trusted = policy.IsTrustedOrigin(new Uri("app://localhost/_framework/blazor.webview.js?cache=1#ignored"));
        bool differentHost = policy.IsTrustedOrigin(new Uri("app://other/index.html"));
        bool differentPort = policy.IsTrustedOrigin(new Uri("app://localhost:4242/index.html"));

        await Assert.That(trusted).IsTrue();
        await Assert.That(differentHost).IsFalse();
        await Assert.That(differentPort).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_DefaultPorts_AreComparedByEffectivePort(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            [new Uri("https://example.com/path")]
        );

        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com:443/other?x=1#part"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://example.com:444/"))).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_RequiresAllowedNavigationScheme(CancellationToken ct = default) {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            ["app"],
            [Uri.UriSchemeHttps],
            [new Uri("https://example.com/")]
        );

        // Act
        bool trusted = policy.IsTrustedOrigin(new Uri("https://example.com/"));

        // Assert
        await Assert.That(trusted).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_WithTrustAllOrigins_TrustsAnyOriginWithAllowedScheme(CancellationToken ct = default) {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [Uri.UriSchemeHttps],
            [],
            true
        );

        // Act
        bool trusted = policy.IsTrustedOrigin(new Uri("https://random.example/"));
        bool trustedDisallowedScheme = policy.IsTrustedOrigin(new Uri("http://random.example/"));

        // Assert
        await Assert.That(trusted).IsTrue();
        await Assert.That(trustedDisallowedScheme).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_WithNullCandidate_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [Uri.UriSchemeHttps],
            [new Uri("https://example.com/")]
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
    public async Task PolicyBuilder_SetAllowedSchemes_TrimsAndIgnoresWhitespace(CancellationToken ct = default) {
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
    public async Task PolicyBuilder_AddTrustedOrigin_IgnoresRelativeUri(CancellationToken ct = default) {
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
    public async Task Registry_GetForWindow_UsesBoundPolicyWhenAvailable(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [Uri.UriSchemeMailto],
            [new Uri("https://trusted.example/")]
        );

        // Act
        IInfiniFrameUriSecurityPolicy defaultPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window.Window, policy);
        IInfiniFrameUriSecurityPolicy boundPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);

        // Assert
        await Assert.That(defaultPolicy.IsNavigationSchemeAllowed("app")).IsTrue();
        await Assert.That(boundPolicy).IsSameReferenceAs(policy);
    }

    [Test]
    public async Task Registry_ConfigureForBuilder_UpdatesBuilderPolicy(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure: policyBuilder => policyBuilder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetAllowedExternalSchemes([Uri.UriSchemeMailto])
            .SetTrustedOrigins([new Uri("https://trusted.example/")]));
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsFalse();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://trusted.example/path"))).IsTrue();
    }

    [Test]
    public async Task BuilderExtensions_SetTrustedOriginsWithStrings_UpdatesBuilderPolicy(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder
            .SetAllowedNavigationSchemes(Uri.UriSchemeHttps)
            .SetTrustedOrigins("https://trusted.example/");
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://trusted.example/abc"))).IsTrue();
    }

    [Test]
    public async Task BuilderExtensions_SetTrustedOriginsWithInvalidString_ThrowsArgumentException(CancellationToken ct = default) {
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
    public async Task Registry_ConfigureForBuilder_CanAppendTrustedOriginsAcrossCalls(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure: policyBuilder => policyBuilder
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps])
            .SetTrustedOrigins([new Uri("https://one.example/")]));
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure: policyBuilder => policyBuilder
            .AddTrustedOrigin(new Uri("https://two.example/")));
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://one.example/path"))).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://two.example/path"))).IsTrue();
    }

    [Test]
    public async Task BuilderExtensions_SetTrustAllOrigins_UpdatesBuilderPolicy(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder
            .SetAllowedNavigationSchemes(Uri.UriSchemeHttps)
            .SetTrustAllOrigins();
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.TrustAllOrigins).IsTrue();
        await Assert.That(policy.IsTrustedOrigin(new Uri("https://anywhere.example/path"))).IsTrue();
    }
}
