// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class EndpointStatusResolverTests {

    [Test]
    public async Task Resolve_PlatformNotSupported_ReturnsNotSupported(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: false,
            remoteDebuggingPort: 9222,
            isWindowClosed: false,
            hasEndpoint: true,
            probeSucceeded: true,
            probeReason: null
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.NotSupported);
    }

    [Test]
    public async Task Resolve_PortNull_ReturnsDisabled(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: true,
            remoteDebuggingPort: null,
            isWindowClosed: false,
            hasEndpoint: true,
            probeSucceeded: true,
            probeReason: null
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Disabled);
    }

    [Test]
    public async Task Resolve_WindowClosed_ReturnsUnavailable(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: true,
            remoteDebuggingPort: 9222,
            isWindowClosed: true,
            hasEndpoint: true,
            probeSucceeded: false,
            probeReason: null
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unavailable);
    }

    [Test]
    public async Task Resolve_NoEndpoint_ReturnsUnavailable(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: true,
            remoteDebuggingPort: 9222,
            isWindowClosed: false,
            hasEndpoint: false,
            probeSucceeded: false,
            probeReason: null
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unavailable);
    }

    [Test]
    public async Task Resolve_ProbeSucceeded_ReturnsReachable(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: true,
            remoteDebuggingPort: 9222,
            isWindowClosed: false,
            hasEndpoint: true,
            probeSucceeded: true,
            probeReason: null
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Reachable);
    }

    [Test]
    public async Task Resolve_ProbeFailed_EmptyReason_ReturnsConfigured(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: true,
            remoteDebuggingPort: 9222,
            isWindowClosed: false,
            hasEndpoint: true,
            probeSucceeded: false,
            probeReason: ""
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Configured);
    }

    [Test]
    public async Task Resolve_ProbeFailed_WithReason_ReturnsUnreachable(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus result = EndpointStatusResolver.Resolve(
            isPlatformSupported: true,
            remoteDebuggingPort: 9222,
            isWindowClosed: false,
            hasEndpoint: true,
            probeSucceeded: false,
            probeReason: "Connection refused"
        );
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.Unreachable);
    }
}
