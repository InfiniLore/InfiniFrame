// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetDebugDiagnosticsTests {
    public static Func<int> GetPort() => PortUtils.GetOpenPortValue;

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_DefaultConfiguration(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool supportsRemoteEndpoint = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();
        InfiniFrameDebugEndpointStatus expectedStatus = supportsRemoteEndpoint
            ? InfiniFrameDebugEndpointStatus.Disabled
            : InfiniFrameDebugEndpointStatus.NotSupported;

        // Act
        InfiniFrameDebugDiagnostics diagnostics = window.Features.Debugging.GetDiagnostics();

        // Assert
        await Assert.That(diagnostics.Capabilities).IsNotNull();
        await Assert.That(diagnostics.DevToolsEnabled).IsEqualTo(window.Features.Debugging.IsDevToolsEnabled);
        await Assert.That(diagnostics.RemoteDebuggingPort).IsEqualTo(window.Features.Debugging.RemoteDebuggingPort);
        await Assert.That(diagnostics.WebInspectorEnabled).IsEqualTo(window.Features.Debugging.IsWebInspectorEnabled);
        await Assert.That(diagnostics.IsWindowClosed).IsFalse();
        await Assert.That(diagnostics.EndpointStatus).IsEqualTo(expectedStatus);
        await Assert.That(diagnostics.EndpointReason).IsEqualTo(supportsRemoteEndpoint ? "Remote debugging is disabled." : null);
        await Assert.That(diagnostics.Endpoint).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_DefaultConfiguration(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameDebugDiagnostics diagnostics = window.GetDebugDiagnostics();

        // Assert
        await Assert.That(diagnostics.Capabilities.SupportsRemoteDebuggingEndpoint)
            .IsEqualTo(window.SupportsRemoteDebuggingEndpoint());
        await Assert.That(diagnostics.Capabilities.SupportsWebInspectorAttach)
            .IsEqualTo(window.SupportsWebInspectorAttach());
        await Assert.That(diagnostics.IsWindowClosed).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [MethodDataSource(nameof(GetPort))]
    [SkipOnLinux]
    public async Task AtWindowStage_ThroughBuilderAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Debugging.EnableDevTools(false);
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;

            builder.Features.Debugging.SetRemoteDebuggingPort(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool supportsRemoteEndpoint = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

        // Act
        InfiniFrameDebugDiagnostics diagnostics = window.GetDebugDiagnostics();

        // Assert
        await Assert.That(diagnostics.DevToolsEnabled).IsFalse();
        await Assert.That(diagnostics.RemoteDebuggingPort).IsEqualTo(supportsRemoteEndpoint ? value : null);
        await Assert.That(diagnostics.Capabilities.SupportsRemoteDebuggingEndpoint).IsEqualTo(supportsRemoteEndpoint);
    }
}
