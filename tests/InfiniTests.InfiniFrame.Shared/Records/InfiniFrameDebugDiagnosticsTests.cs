// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDebugDiagnosticsTests {

    [Test]
    public async Task Constructor_SetsRequiredProperties(CancellationToken ct = default) {
        // Arrange & Act
        var diagnostics = new InfiniFrameDebugDiagnostics {
            Platform = "Windows",
            Runtime = "win-x64",
            Capabilities = new InfiniFrameDebugCapabilities {
                SupportsLocalDevTools = true,
                SupportsRemoteDebuggingEndpoint = false,
                SupportsWebInspectorAttach = false,
                SupportsScriptErrorForwarding = true,
                SupportsNavigationDiagnostics = false
            },
            DevToolsEnabled = true,
            RemoteDebuggingPort = null,
            WebInspectorEnabled = false,
            EndpointStatus = InfiniFrameDebugEndpointStatus.Disabled,
            IsWindowClosed = false
        };

        // Assert
        await Assert.That(diagnostics.Platform).IsEqualTo("Windows");
        await Assert.That(diagnostics.Runtime).IsEqualTo("win-x64");
        await Assert.That(diagnostics.DevToolsEnabled).IsTrue();
        await Assert.That(diagnostics.RemoteDebuggingPort).IsNull();
        await Assert.That(diagnostics.WebInspectorEnabled).IsFalse();
        await Assert.That(diagnostics.EndpointStatus).IsEqualTo(InfiniFrameDebugEndpointStatus.Disabled);
        await Assert.That(diagnostics.IsWindowClosed).IsFalse();
    }

    [Test]
    public async Task OptionalProperties_DefaultToNull(CancellationToken ct = default) {
        // Arrange & Act
        var diagnostics = new InfiniFrameDebugDiagnostics {
            Platform = "Linux",
            Runtime = "linux-x64",
            Capabilities = new InfiniFrameDebugCapabilities {
                SupportsLocalDevTools = false,
                SupportsRemoteDebuggingEndpoint = true,
                SupportsWebInspectorAttach = false,
                SupportsScriptErrorForwarding = false,
                SupportsNavigationDiagnostics = true
            },
            DevToolsEnabled = false,
            RemoteDebuggingPort = null,
            WebInspectorEnabled = true,
            EndpointStatus = InfiniFrameDebugEndpointStatus.Reachable,
            IsWindowClosed = false
        };

        // Assert
        await Assert.That(diagnostics.BrowserRuntime).IsNull();
        await Assert.That(diagnostics.Endpoint).IsNull();
        await Assert.That(diagnostics.EndpointReason).IsNull();
        await Assert.That(diagnostics.PlatformNotes).IsNull();
    }

    [Test]
    public async Task AllProperties_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var diagnostics = new InfiniFrameDebugDiagnostics {
            Platform = "macOS",
            Runtime = "osx-arm64",
            BrowserRuntime = "WebKit",
            Capabilities = new InfiniFrameDebugCapabilities {
                SupportsLocalDevTools = false,
                SupportsRemoteDebuggingEndpoint = false,
                SupportsWebInspectorAttach = true,
                SupportsScriptErrorForwarding = false,
                SupportsNavigationDiagnostics = false
            },
            DevToolsEnabled = false,
            RemoteDebuggingPort = 9222,
            WebInspectorEnabled = true,
            EndpointStatus = InfiniFrameDebugEndpointStatus.Reachable,
            Endpoint = new Uri("http://localhost:9222"),
            EndpointReason = "Probed successfully",
            IsWindowClosed = false,
            PlatformNotes = "WebKit inspector available"
        };

        // Assert
        await Assert.That(diagnostics.BrowserRuntime).IsEqualTo("WebKit");
        await Assert.That(diagnostics.RemoteDebuggingPort).IsEqualTo(9222);
        await Assert.That(diagnostics.Endpoint).IsEqualTo(new Uri("http://localhost:9222"));
        await Assert.That(diagnostics.EndpointReason).IsEqualTo("Probed successfully");
        await Assert.That(diagnostics.PlatformNotes).IsEqualTo("WebKit inspector available");
    }
}
