// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;

namespace InfiniTests.InfiniFrame.Security;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameUriSecurityPolicyAdditionalTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Default Policy
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Default_HasHttpsHttpAndAppNavigationSchemes(CancellationToken ct = default) {
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicy.Default;

        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttp)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsTrue();
    }

    [Test]
    public async Task Default_HasHttpsHttpAndMailtoExternalSchemes(CancellationToken ct = default) {
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicy.Default;

        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeHttp)).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed(Uri.UriSchemeMailto)).IsTrue();
    }

    [Test]
    public async Task Default_TrustAllOriginsIsFalse(CancellationToken ct = default) {
        await Assert.That(InfiniFrameUriSecurityPolicy.Default.TrustAllOrigins).IsFalse();
    }

    [Test]
    public async Task Default_HasNoTrustedOrigins(CancellationToken ct = default) {
        await Assert.That(InfiniFrameUriSecurityPolicy.Default.TrustedOrigins.Count).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Two-argument IsTrustedOrigin
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task IsTrustedOrigin_TwoArgs_MatchingOrigin_ReturnsTrue(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            []
        );

        bool result = policy.IsTrustedOrigin(
            new Uri("https://example.com/page"),
            new Uri("https://example.com/")
        );

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsTrustedOrigin_TwoArgs_DifferentOrigin_ReturnsFalse(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            []
        );

        bool result = policy.IsTrustedOrigin(
            new Uri("https://example.com/page"),
            new Uri("https://other.com/")
        );

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_TwoArgs_WithTrustAll_ReturnsTrue(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            [],
            true
        );

        bool result = policy.IsTrustedOrigin(
            new Uri("https://anything.com/page"),
            new Uri("https://example.com/")
        );

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsTrustedOrigin_TwoArgs_DisallowedScheme_ReturnsFalse(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            [],
            true
        );

        bool result = policy.IsTrustedOrigin(
            new Uri("ftp://example.com/"),
            new Uri("https://example.com/")
        );

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsTrustedOrigin_TwoArgs_NullCandidate_Throws(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy([Uri.UriSchemeHttps], [], []);

        await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() =>
            policy.IsTrustedOrigin(null!, new Uri("https://example.com/"))
        ));
    }

    [Test]
    public async Task IsTrustedOrigin_TwoArgs_NullTrusted_Throws(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy([Uri.UriSchemeHttps], [], []);

        await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() =>
            policy.IsTrustedOrigin(new Uri("https://example.com/"), null!)
        ));
    }

    // -----------------------------------------------------------------------------------------------------------------
    // WithTrustedOrigin
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WithTrustedOrigin_AddsSingleOrigin(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            []
        );

        IInfiniFrameUriSecurityPolicy newPolicy = policy.WithTrustedOrigin(new Uri("https://trusted.example/"));

        await Assert.That(newPolicy.IsTrustedOrigin(new Uri("https://trusted.example/page"))).IsTrue();
        await Assert.That(policy.TrustedOrigins.Count).IsEqualTo(0);
    }

    [Test]
    public async Task WithTrustedOrigin_NullOrigin_Throws(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy([Uri.UriSchemeHttps], [], []);

        await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() =>
            policy.WithTrustedOrigin(null!)
        ));
    }

    // -----------------------------------------------------------------------------------------------------------------
    // WithTrustedOrigins
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WithTrustedOrigins_AddsMultipleOrigins(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            []
        );

        IInfiniFrameUriSecurityPolicy newPolicy = policy.WithTrustedOrigins([
            new Uri("https://one.example/"),
            new Uri("https://two.example/")
        ]);

        await Assert.That(newPolicy.IsTrustedOrigin(new Uri("https://one.example/page"))).IsTrue();
        await Assert.That(newPolicy.IsTrustedOrigin(new Uri("https://two.example/page"))).IsTrue();
    }

    [Test]
    public async Task WithTrustedOrigins_MergesWithExistingOrigins(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            [new Uri("https://existing.example/")]
        );

        IInfiniFrameUriSecurityPolicy newPolicy = policy.WithTrustedOrigins([
            new Uri("https://new.example/")
        ]);

        await Assert.That(newPolicy.IsTrustedOrigin(new Uri("https://existing.example/"))).IsTrue();
        await Assert.That(newPolicy.IsTrustedOrigin(new Uri("https://new.example/"))).IsTrue();
    }

    [Test]
    public async Task WithTrustedOrigins_NullOrigins_Throws(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy([Uri.UriSchemeHttps], [], []);

        await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() =>
            policy.WithTrustedOrigins(null!)
        ));
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Scheme normalization
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_SchemesNormalizedCaseInsensitive(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            ["HTTPS"],
            ["MAILTO"],
            []
        );

        await Assert.That(policy.IsNavigationSchemeAllowed("https")).IsTrue();
        await Assert.That(policy.IsExternalSchemeAllowed("mailto")).IsTrue();
    }

    [Test]
    public async Task Constructor_NullAndWhitespaceSchemesAreIgnored(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [null!, "", "  ", Uri.UriSchemeHttps],
            [null!, "\t", Uri.UriSchemeMailto],
            []
        );

        await Assert.That(policy.AllowedNavigationSchemes.Count).IsEqualTo(1);
        await Assert.That(policy.AllowedExternalSchemes.Count).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TrustedOrigins normalization
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_RelativeUrisAreIgnored(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            [new Uri("https://valid.example/"), new Uri("/relative", UriKind.Relative)]
        );

        await Assert.That(policy.TrustedOrigins.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Constructor_DuplicateOriginsByOriginAreDeduplicated(CancellationToken ct = default) {
        var policy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            [new Uri("https://example.com/a"), new Uri("https://example.com/b")]
        );

        await Assert.That(policy.TrustedOrigins.Count).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // IsNavigationSchemeAllowed
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task IsNavigationSchemeAllowed_UnknownScheme_ReturnsFalse(CancellationToken ct = default) {
        await Assert.That(InfiniFrameUriSecurityPolicy.Default.IsNavigationSchemeAllowed("ftp")).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // IsExternalSchemeAllowed
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task IsExternalSchemeAllowed_UnknownScheme_ReturnsFalse(CancellationToken ct = default) {
        await Assert.That(InfiniFrameUriSecurityPolicy.Default.IsExternalSchemeAllowed("ftp")).IsFalse();
    }
}
