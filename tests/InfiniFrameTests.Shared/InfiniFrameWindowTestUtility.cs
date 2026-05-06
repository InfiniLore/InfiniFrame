// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using JetBrains.Annotations;
using System.Runtime.Versioning;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MustDisposeResource]
public sealed class InfiniFrameWindowTestUtility : IDisposable {
    public required IInfiniFrameWindow Window { get; init; }

    private Thread? _windowThread;
    private int _disposed;

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

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameWindowTestUtility() {}

    [MustDisposeResource]
    public static InfiniFrameWindowTestUtility Create(CancellationToken cancellationToken = default)
        => Create(null, cancellationToken);

    [MustDisposeResource]
    public static InfiniFrameWindowTestUtility Create(
        Action<IInfiniFrameWindowBuilder>? builder = null,
        CancellationToken cancellationToken = default
    ) {
        cancellationToken.ThrowIfCancellationRequested();

        var windowBuilder = InfiniFrameWindowBuilder.Create();
        windowBuilder.SetStartString(StartString);
        builder?.Invoke(windowBuilder);

        // Windows: WebView2 requires STA thread for COM initialization
        // Linux: GTK implicitly treats the calling thread as the main UI thread
        // macOS: Similar to Linux, but with additional main-thread restrictions for menu operations
        if (OperatingSystem.IsWindows()) {
            return CreateOnStaThread(windowBuilder);
        }
        else {
            // On Linux/macOS, create the window in the current thread to ensure proper GTK initialization
            IInfiniFrameWindow window = windowBuilder.Build();

            var utility = new InfiniFrameWindowTestUtility {
                Window = window
            };

            var thread = new Thread(() => {
                try {
                    window.WaitForClose();
                }
                catch (ApplicationException) {
                    // Ignore shutdown exceptions during test cleanup
                }
            }) {
                IsBackground = true
            };

            utility._windowThread = thread;
            thread.Start();

            return utility;
        }
    }

    [SupportedOSPlatform("windows")]
    private static InfiniFrameWindowTestUtility CreateOnStaThread(
        InfiniFrameWindowBuilder windowBuilder
    ) {
        var windowSource = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() => {
            try {
                IInfiniFrameWindow window = windowBuilder.Build();
                windowSource.SetResult(window);
                window.WaitForClose();
            }
            catch (Exception ex) when (IsNonFatalException(ex)) {
                windowSource.TrySetException(ex);
            }
        }) {
            IsBackground = true,
            Name = "InfiniFrame Test Window Thread"
        };
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();

        var utility = new InfiniFrameWindowTestUtility {
            Window = windowSource.Task.GetAwaiter().GetResult(),
            _windowThread = thread
        };

        return utility;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // ----------------------------------------------------------------------------------------------------------------
    public async Task WaitForCloseAsync(CancellationToken ct = default) {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        try {
            await Window.WaitForCloseAsync(ct);
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        try {
            if (_windowThread == null) return;
            if (!_windowThread.Join(TimeSpan.FromSeconds(5)))
                _windowThread.Interrupt();
        }
        catch (ThreadInterruptedException) {
            // ignored
        }
        catch (ThreadStateException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }
    }
    
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        try {
            Window.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        try {
            if (_windowThread == null)
                return;

            if (!_windowThread.Join(TimeSpan.FromSeconds(5)))
                _windowThread.Interrupt();
        }
        catch (ThreadInterruptedException) {
            // ignored
        }
        catch (ThreadStateException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
