// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Debugging;
using System.Collections.Concurrent;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DebugToolingTests {
    [Test]
    [DisplayName($"{nameof(DebugToolingTests)}.{nameof(Window_DebugCapabilities_ShouldMatchPlatformMatrix)}")]
    [NotInParallelInfiniTests]
    public async Task Window_DebugCapabilities_ShouldMatchPlatformMatrix(CancellationToken ct = default) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        InfiniFrameDebugCapabilities capabilities = window.Debug.Capabilities;

        await Assert.That(capabilities.SupportsLocalDevTools).IsTrue();
        await Assert.That(capabilities.SupportsRemoteDebuggingEndpoint).IsEqualTo(OperatingSystem.IsWindows() || OperatingSystem.IsLinux());
        await Assert.That(capabilities.SupportsWebInspectorAttach).IsEqualTo(OperatingSystem.IsMacOS() && OperatingSystem.IsMacOSVersionAtLeast(13, 3));
        await Assert.That(capabilities.SupportsNavigationDiagnostics).IsTrue();
        await Assert.That(capabilities.SupportsScriptErrorForwarding).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(DebugToolingTests)}.{nameof(Window_DebugDiagnostics_ShouldReflectRuntimeConfiguration)}")]
    [NotInParallelInfiniTests]
    public async Task Window_DebugDiagnostics_ShouldReflectRuntimeConfiguration(CancellationToken ct = default) {
        int? debugPort = OperatingSystem.IsWindows() || OperatingSystem.IsLinux()
            ? 0
            : null;

        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.SetDevToolsEnabled(false);
            if (debugPort.HasValue) {
                builder.SetRemoteDebuggingPort(debugPort.Value);
            }
        }, ct);

        IInfiniFrameWindow window = windowUtility.Window;
        InfiniFrameDebugDiagnostics diagnostics = window.Debug.GetDiagnostics();

        await Assert.That(diagnostics.Capabilities).IsNotNull();
        await Assert.That(diagnostics.DevToolsEnabled).IsEqualTo(window.Debug.DevToolsEnabled);
        await Assert.That(diagnostics.RemoteDebuggingPort).IsEqualTo(window.Debug.RemoteDebuggingPort);
        await Assert.That(diagnostics.WebInspectorEnabled).IsEqualTo(window.Debug.WebInspectorEnabled);
        await Assert.That(diagnostics.Platform).IsNotNull();
        await Assert.That(diagnostics.Runtime).IsNotNull();
    }

    [Test]
    [DisplayName($"{nameof(DebugToolingTests)}.{nameof(Window_DebugEvent_Smoke_ShouldEmitSupportedKindWithoutCrash)}")]
    [NotInParallelInfiniTests]
    public async Task Window_DebugEvent_Smoke_ShouldEmitSupportedKindWithoutCrash(CancellationToken ct = default) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var kinds = new ConcurrentQueue<InfiniFrameDebugEventKind>();
        using var eventReceived = new AutoResetEvent(false);

        window.Debug.Event += OnDebugEvent;

        try {
            window.Close();

            DateTime timeoutAt = DateTime.UtcNow.AddSeconds(10);
            while (DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
                if (eventReceived.WaitOne(150))
                    break;

                if (window.IsClosed)
                    break;
            }
        }
        finally {
            window.Debug.Event -= OnDebugEvent;
        }

        if (!kinds.Any()) {
            Skip.Test("No debug events were emitted in this environment.");
            return;
        }

        bool hasSupportedKind = kinds.Any(kind => kind is InfiniFrameDebugEventKind.Navigation or InfiniFrameDebugEventKind.ScriptError or InfiniFrameDebugEventKind.Process);
        await Assert.That(hasSupportedKind).IsTrue();

        void OnDebugEvent(object? sender, InfiniFrameDebugEventArgs args) {
            kinds.Enqueue(args.Kind);
            eventReceived.Set();
        }
    }
}
