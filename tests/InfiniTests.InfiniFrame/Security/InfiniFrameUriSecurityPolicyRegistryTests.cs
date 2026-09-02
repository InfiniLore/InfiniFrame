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
public class InfiniFrameUriSecurityPolicyRegistryTests {

    [Test]
    public async Task GetForBuilder_NullBuilder_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(
            () => InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(null!)
        ).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetForBuilder_NewBuilder_ReturnsDefaultPolicy(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy).IsNotNull();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsTrue();
    }

    [Test]
    public async Task ConfigureForBuilder_NullBuilder_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(
            () => InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(null!, configure: _ => {})
        ).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ConfigureForBuilder_NullConfigure_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act & Assert
        await Assert.That(
            () => InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, null!)
        ).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetForWindow_NullWindow_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(
            () => InfiniFrameUriSecurityPolicyRegistry.GetForWindow(null!)
        ).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GetForWindow_UnboundWindow_ReturnsDefaultPolicy(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();

        // Act
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);

        // Assert
        await Assert.That(policy).IsNotNull();
        await Assert.That(policy.IsNavigationSchemeAllowed("app")).IsTrue();
    }

    [Test]
    public async Task BindToWindow_NullWindow_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var policy = InfiniFrameUriSecurityPolicy.Default;

        // Act & Assert
        await Assert.That(
            () => InfiniFrameUriSecurityPolicyRegistry.BindToWindow(null!, policy)
        ).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task BindToWindow_NullPolicy_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();

        // Act & Assert
        await Assert.That(
            () => InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window.Window, null!)
        ).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task BindToWindow_ThenGet_ReturnsBoundPolicy(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        var customPolicy = new InfiniFrameUriSecurityPolicy(
            [Uri.UriSchemeHttps],
            [],
            []
        );

        // Act
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window.Window, customPolicy);
        IInfiniFrameUriSecurityPolicy retrieved = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);

        // Assert
        await Assert.That(retrieved).IsSameReferenceAs(customPolicy);
    }

    [Test]
    public async Task BindToWindow_MultipleCalls_OverwritesPreviousPolicy(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        var policy1 = new InfiniFrameUriSecurityPolicy([Uri.UriSchemeHttps], [], []);
        var policy2 = new InfiniFrameUriSecurityPolicy([Uri.UriSchemeFtp], [], []);

        // Act
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window.Window, policy1);
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window.Window, policy2);
        IInfiniFrameUriSecurityPolicy retrieved = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window.Window);

        // Assert
        await Assert.That(retrieved).IsSameReferenceAs(policy2);
    }

    [Test]
    public async Task ConfigureForBuilder_MultipleCalls_ApplyCumulatively(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure: b => b
            .SetAllowedNavigationSchemes([Uri.UriSchemeHttps]));
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(builder, configure: b => b
            .AllowNavigationScheme(Uri.UriSchemeFtp));
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeHttps)).IsTrue();
        await Assert.That(policy.IsNavigationSchemeAllowed(Uri.UriSchemeFtp)).IsTrue();
    }

    [Test]
    public async Task GetForBuilder_ReturnsSameInstanceForSameBuilder(CancellationToken ct = default) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameUriSecurityPolicy policy1 = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        IInfiniFrameUriSecurityPolicy policy2 = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);

        // Assert
        await Assert.That(policy1).IsSameReferenceAs(policy2);
    }
}
