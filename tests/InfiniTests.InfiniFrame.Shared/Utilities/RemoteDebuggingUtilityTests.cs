// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RemoteDebuggingUtilityTests {

    // -----------------------------------------------------------------------------------------------------------------
    // NormalizePort
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task NormalizePort_Zero_ReturnsZero(CancellationToken ct = default) {
        // Arrange & Act
        int result = RemoteDebuggingUtility.NormalizePort(0);

        // Assert
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    [Arguments(1)]
    [Arguments(8080)]
    [Arguments(65535)]
    public async Task NormalizePort_ValidPort_ReturnsSameValue(int port, CancellationToken ct = default) {
        // Arrange & Act
        int result = RemoteDebuggingUtility.NormalizePort(port);

        // Assert
        await Assert.That(result).IsEqualTo(port);
    }

    [Test]
    [Arguments(-1)]
    [Arguments(65536)]
    [Arguments(int.MaxValue)]
    public async Task NormalizePort_InvalidPort_ThrowsArgumentOutOfRangeException(int port, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => RemoteDebuggingUtility.NormalizePort(port))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task NormalizePort_InvalidPort_ExceptionContainsParameterName(CancellationToken ct = default) {
        // Arrange & Act
        ArgumentOutOfRangeException? ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => Task.Run(() => RemoteDebuggingUtility.NormalizePort(-1, "myPort"))
        );

        // Assert
        await Assert.That(ex).IsNotNull();
        await Assert.That(ex!.ParamName).IsEqualTo("myPort");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // CreateEndpointUri
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task CreateEndpointUri_ReturnsLoopbackUri(CancellationToken ct = default) {
        // Arrange & Act
        Uri uri = RemoteDebuggingUtility.CreateEndpointUri(9222);

        // Assert
        await Assert.That(uri.Host).IsEqualTo("127.0.0.1");
        await Assert.That(uri.Port).IsEqualTo(9222);
        await Assert.That(uri.Scheme).IsEqualTo("http");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComposeBrowserControlInitParameters
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComposeBrowserControlInitParameters_PortZero_ReturnsSanitizedNull(CancellationToken ct = default) {
        // Arrange & Act
        string? result = RemoteDebuggingUtility.ComposeBrowserControlInitParameters(null, 0);

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task ComposeBrowserControlInitParameters_PortZero_StripsExistingSwitches(CancellationToken ct = default) {
        // Arrange
        string raw = "--remote-debugging-port=9222 --remote-debugging-address=127.0.0.1 --other-flag";

        // Act
        string? result = RemoteDebuggingUtility.ComposeBrowserControlInitParameters(raw, 0);

        // Assert
        await Assert.That(result).Contains("--other-flag");
        await Assert.That(result).DoesNotContain("--remote-debugging-port");
        await Assert.That(result).DoesNotContain("--remote-debugging-address");
    }

    [Test]
    public async Task ComposeBrowserControlInitParameters_NullRaw_ReturnsNull_OnNonWindows(CancellationToken ct = default) {
        if (OperatingSystem.IsWindows()) return;

        // Arrange & Act
        string? result = RemoteDebuggingUtility.ComposeBrowserControlInitParameters(null, 9222);

        // Assert
        await Assert.That(result).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // EnsureSupportedPlatform
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task EnsureSupportedPlatform_Zero_DoesNotThrow(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => RemoteDebuggingUtility.EnsureSupportedPlatform(0)).ThrowsNothing();
    }

    [Test]
    [Arguments(-1)]
    [Arguments(65536)]
    public async Task EnsureSupportedPlatform_InvalidPort_ThrowsArgumentOutOfRangeException(int port, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => RemoteDebuggingUtility.EnsureSupportedPlatform(port))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task EnsureSupportedPlatform_ValidPort_OnSupportedPlatform_DoesNotThrow(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;

        // Arrange & Act & Assert
        await Assert.That(() => RemoteDebuggingUtility.EnsureSupportedPlatform(9222)).ThrowsNothing();
    }
}
