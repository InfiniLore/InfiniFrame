// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using JetBrains.Annotations;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MustDisposeResource]
public sealed partial class InfiniFrameTestWindow : IDisposable {

    private const string StartString = """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
        </head>
        <body>
        </body>
        </html>
        """;
    private int _disposed;

    private Thread? _windowThread;

    public required IInfiniFrameWindow Window { get; init; }
    public required IInfiniFrameWindowBuilder BuilderSnapshot { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameTestWindow() {}

    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        try {
            Window.Close();
        }
        catch (ApplicationException) {
        }
        catch (ObjectDisposedException) {
        }

        if (_windowThread is null) {
            DisposeWindowAfterThreadExit();
            return;
        }

        try {
            // A test must never return while its native window thread is still alive. In particular,
            // WebView2 controller creation is noticeably slower on Windows ARM64 runners. The old
            // ARM64-specific 2 + 2 second best-effort joins could leave a live STA thread behind and
            // let it call managed delegates after the test (or test host) had started tearing down.
            if (_windowThread.Join(TimeSpan.FromSeconds(5))) {
                DisposeWindowAfterThreadExit();
                return;
            }

            try {
                Window.Close();
            }
            catch (ApplicationException) {
            }
            catch (ObjectDisposedException) {
            }

            if (!_windowThread.Join(TimeSpan.FromSeconds(10))) {
                throw new TimeoutException(
                    "The InfiniFrame test window thread did not terminate after its window was closed."
                );
            }

            DisposeWindowAfterThreadExit();
        }
        catch (ThreadInterruptedException) {
        }
        catch (ThreadStateException) {
        }
        catch (ObjectDisposedException) {
        }
    }

    private void DisposeWindowAfterThreadExit() {
        // Native callbacks may only be unrooted once WaitForClose has returned on the STA thread.
        // Disposing earlier can race an in-flight reverse P/Invoke during WebView2 teardown.
        if (Window is IDisposable disposableWindow) disposableWindow.Dispose();
    }

    [MustDisposeResource]
    public static InfiniFrameTestWindow Create(CancellationToken cancellationToken = default)
        => Create(null, cancellationToken);

    // ReSharper disable once ConvertIfStatementToReturnStatement
    [MustDisposeResource]
    public static InfiniFrameTestWindow Create(Action<IInfiniFrameWindowBuilder>? builder = null, CancellationToken ct = default) {
        ct.ThrowIfCancellationRequested();

        var windowBuilder = InfiniFrameWindowBuilder.Create();
        windowBuilder
            .SetIconFile("favicon.ico")
            .SetStartPageContent(StartString);

        builder?.Invoke(windowBuilder);

        if (OperatingSystem.IsWindows()) return CreateWindows(windowBuilder);
        if (OperatingSystem.IsLinux()) return CreateLinux(windowBuilder);
        if (OperatingSystem.IsMacOS()) return CreateMacOs(windowBuilder);

        throw new PlatformNotSupportedException("Unsupported operating system");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static partial InfiniFrameTestWindow CreateWindows(
        InfiniFrameWindowBuilder windowBuilder
    );

    private static partial InfiniFrameTestWindow CreateLinux(
        InfiniFrameWindowBuilder windowBuilder
    );

    private static partial InfiniFrameTestWindow CreateMacOs(
        InfiniFrameWindowBuilder windowBuilder
    );
}
