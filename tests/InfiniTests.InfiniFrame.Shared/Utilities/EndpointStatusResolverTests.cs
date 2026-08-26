// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class EndpointStatusResolverTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Resolve
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Resolve_PlatformNotSupported_ReturnsNotSupported(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            false, 9222,
            false, true,
            true, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.NotSupported);
    }

    [Test]
    public async Task Resolve_PortNull_ReturnsDisabled(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, null,
            false, true,
            true, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Disabled);
    }

    [Test]
    public async Task Resolve_WindowClosed_ReturnsUnavailable(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 9222,
            true, true,
            true, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unavailable);
    }

    [Test]
    public async Task Resolve_NoEndpoint_ReturnsUnavailable(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 9222,
            false, false,
            true, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unavailable);
    }

    [Test]
    public async Task Resolve_ProbeSucceeded_ReturnsReachable(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 9222,
            false, true,
            true, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Reachable);
    }

    [Test]
    public async Task Resolve_ProbeNotSucceeded_NoReason_ReturnsConfigured(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 9222,
            false, true,
            false, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Configured);
    }

    [Test]
    public async Task Resolve_ProbeNotSucceeded_EmptyReason_ReturnsConfigured(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 9222,
            false, true,
            false, "  ");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Configured);
    }

    [Test]
    public async Task Resolve_ProbeNotSucceeded_WithReason_ReturnsUnreachable(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 9222,
            false, true,
            false, "Connection refused");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unreachable);
    }

    [Test]
    public async Task Resolve_PortZero_ReturnsUnavailable(CancellationToken ct = default) {
        // Arrange

        // Act
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            true, 0,
            false, false,
            false, null);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unavailable);
    }
}
