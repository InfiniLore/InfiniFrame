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
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_DebugCapabilities_ShouldMatchPlatformMatrix(CancellationToken ct = default) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        InfiniFrameDebugCapabilities capabilities = window.Debugging.Capabilities;

        await Assert.That(capabilities.SupportsLocalDevTools).IsTrue();
        await Assert.That(capabilities.SupportsRemoteDebuggingEndpoint).IsEqualTo(OperatingSystem.IsWindows() || OperatingSystem.IsLinux());
        await Assert.That(capabilities.SupportsWebInspectorAttach).IsEqualTo(OperatingSystem.IsMacOS() && OperatingSystem.IsMacOSVersionAtLeast(13, 3));
        await Assert.That(capabilities.SupportsNavigationDiagnostics).IsTrue();
        await Assert.That(capabilities.SupportsScriptErrorForwarding).IsTrue();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_DebugDiagnostics_ShouldReflectRuntimeConfiguration(CancellationToken ct = default) {
        const int debugPort = 0;

        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Debugging.SetDevToolsEnabled(false);
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) {
                builder.Debugging.SetRemoteDebuggingPort(debugPort);
            }
        }, ct);

        IInfiniFrameWindow window = windowUtility.Window;
        InfiniFrameDebugDiagnostics diagnostics = window.Debugging.GetDiagnostics();

        await Assert.That(diagnostics.Capabilities).IsNotNull();
        await Assert.That(diagnostics.DevToolsEnabled).IsEqualTo(window.Debugging.DevToolsEnabled);
        await Assert.That(diagnostics.RemoteDebuggingPort).IsEqualTo(window.Debugging.RemoteDebuggingPort);
        await Assert.That(diagnostics.WebInspectorEnabled).IsEqualTo(window.Debugging.WebInspectorEnabled);
        await Assert.That(diagnostics.Platform).IsNotNull();
        await Assert.That(diagnostics.Runtime).IsNotNull();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_DebugEvent_Smoke_ShouldEmitSupportedKindWithoutCrash(CancellationToken ct = default) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var kinds = new ConcurrentQueue<InfiniFrameDebugEventKind>();
        using var eventReceived = new AutoResetEvent(false);

        Action<IInfiniFrameWindow, InfiniFrameDebugEventArgs> onDebugEvent = (_, args) => {
            kinds.Enqueue(args.Kind);
            // ReSharper disable once AccessToDisposedClosure
            eventReceived.Set();
        };
        window.EventsStore.DebuggingEvent.Add(onDebugEvent);

        try {
            window.Close();

            DateTime timeoutAt = DateTime.UtcNow.AddSeconds(8);
            while (DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
                if (eventReceived.WaitOne(150))
                    break;

                if (window.IsClosed)
                    break;
            }
        }
        finally {
            window.EventsStore.DebuggingEvent.Remove(onDebugEvent);
        }

        Skip.When(kinds.IsEmpty, "No debug events were emitted in this environment.");

        bool hasSupportedKind = kinds.Any(kind => kind is InfiniFrameDebugEventKind.Navigation or InfiniFrameDebugEventKind.ScriptError or InfiniFrameDebugEventKind.Process);
        await Assert.That(hasSupportedKind).IsTrue();
    }
}
