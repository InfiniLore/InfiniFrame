// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureDebugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DebuggingStartupParametersTests {
    [Test]
    public async Task Builder_DebuggingProperty_UsesDebuggingFeatureInstance(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Assert
        await Assert.That(builder.Debugging).IsSameReferenceAs(builder.Features.Debugging);
    }

    [Test]
    [Arguments(true, 0)]
    [Arguments(false, 0)]
    [Arguments(true, 9222)]
    public async Task AtBuilderStage_DebuggingBuilderValuesPropagateToNativeParameters(
        bool devToolsEnabled,
        int remoteDebuggingPort,
        CancellationToken ct
    ) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Debugging.EnableDevTools(devToolsEnabled);
        if (remoteDebuggingPort != 0
            && (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
            && builder.Debugging.SupportsRemoteDebuggingEndpoint) {
            builder.Debugging.SetRemoteDebuggingPort(remoteDebuggingPort);
        }

        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(initParameters.DevToolsEnabled).IsEqualTo(devToolsEnabled);
        await Assert.That(initParameters.RemoteDebuggingPort)
            .IsEqualTo(remoteDebuggingPort != 0 && builder.Debugging.SupportsRemoteDebuggingEndpoint ? remoteDebuggingPort : 0);
    }
}
