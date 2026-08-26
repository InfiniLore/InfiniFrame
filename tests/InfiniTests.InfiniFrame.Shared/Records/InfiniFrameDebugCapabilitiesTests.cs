// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDebugCapabilitiesTests {

    [Test]
    public async Task Record_CanBeConstructed(CancellationToken ct = default) {
        // Arrange & Act
        var capabilities = new InfiniFrameDebugCapabilities {
            SupportsLocalDevTools = true,
            SupportsRemoteDebuggingEndpoint = false,
            SupportsWebInspectorAttach = true,
            SupportsScriptErrorForwarding = false,
            SupportsNavigationDiagnostics = true
        };

        // Assert
        await Assert.That(capabilities.SupportsLocalDevTools).IsTrue();
        await Assert.That(capabilities.SupportsRemoteDebuggingEndpoint).IsFalse();
        await Assert.That(capabilities.SupportsWebInspectorAttach).IsTrue();
        await Assert.That(capabilities.SupportsScriptErrorForwarding).IsFalse();
        await Assert.That(capabilities.SupportsNavigationDiagnostics).IsTrue();
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var caps1 = new InfiniFrameDebugCapabilities {
            SupportsLocalDevTools = true,
            SupportsRemoteDebuggingEndpoint = false,
            SupportsWebInspectorAttach = true,
            SupportsScriptErrorForwarding = false,
            SupportsNavigationDiagnostics = true
        };
        var caps2 = new InfiniFrameDebugCapabilities {
            SupportsLocalDevTools = true,
            SupportsRemoteDebuggingEndpoint = false,
            SupportsWebInspectorAttach = true,
            SupportsScriptErrorForwarding = false,
            SupportsNavigationDiagnostics = true
        };

        // Act & Assert
        await Assert.That(caps1).IsEqualTo(caps2);
    }
}
