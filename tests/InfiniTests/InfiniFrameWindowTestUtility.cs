// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using JetBrains.Annotations;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniTests;
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

        // Windows: WebView2 requires STA thread for COM initialization.
        // Linux: GTK/WebKit have hard single-thread, process-wide affinity. gtk_init() binds GTK to the FIRST
        //        thread that calls it, and the default WebKitWebContext is a process-global singleton, so EVERY
        //        window in the process must be created, driven and torn down on that one thread. Creating a
        //        WebKitWebView on any other thread makes WebKit abort() the process (exit code 134). A new
        //        thread per window therefore works only for the first window; the rest abort. We instead run a
        //        single process-wide GTK host thread (see CreateOnSharedGtkThread) and marshal every window's
        //        Build() onto it; cross-thread access from test methods is marshalled back via
        //        InfiniFrameWindow.Invoke().
        // macOS: NSApp requires the UI to run on the process main thread, so Build() stays here.
        if (OperatingSystem.IsWindows()) return CreateOnStaThread(windowBuilder);
        if (OperatingSystem.IsLinux()) return CreateOnSharedGtkThread(windowBuilder);

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

    // The process-wide GTK host. Lazily started once; owns gtk_init() + gtk_main() for the whole test process.
    private static readonly object HostLock = new();
    private static IInfiniFrameWindow? _hostWindow;

    /// <summary>
    ///     Returns the process-wide GTK host window, starting the host thread on first use. A persistent keep-alive
    ///     window owns gtk_main() for the process lifetime so that every test window can be created on — and marshalled
    ///     onto — the single thread GTK and WebKit are bound to.
    /// </summary>
    private static IInfiniFrameWindow EnsureGtkHost() {
        if (_hostWindow is not null) return _hostWindow;

        lock (HostLock) {
            if (_hostWindow is not null) return _hostWindow;

            var hostSource = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

            var thread = new Thread(() => {
                try {
                    var hostBuilder = InfiniFrameWindowBuilder.Create();
                    hostBuilder.SetStartString(StartString);

                    IInfiniFrameWindow host = hostBuilder.Build();
                    hostSource.SetResult(host);

                    // Runs gtk_main() on this thread for the rest of the process. Test windows are built and torn
                    // down via this loop without quitting it (only this host window's destroy calls gtk_main_quit).
                    host.WaitForClose();
                }
                catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                    hostSource.TrySetException(ex);
                }
            }) {
                IsBackground = true,
                Name = "InfiniFrame GTK Host Thread"
            };

            thread.Start();

            _hostWindow = hostSource.Task.GetAwaiter().GetResult();
            return _hostWindow;
        }
    }

    [MustDisposeResource]
    private static InfiniFrameWindowTestUtility CreateOnSharedGtkThread(
        InfiniFrameWindowBuilder windowBuilder
    ) {
        IInfiniFrameWindow host = EnsureGtkHost();

        // Marshal Build() (gtk_init is a no-op here, plus WebKitWebView creation) onto the host thread so the
        // webview is created on the thread GTK/WebKit are bound to. Build() runs on the host thread, so the new
        // window captures it as its UI thread and InfiniFrameWindow.Invoke() routes later calls back here.
        IInfiniFrameWindow? built = null;
        ExceptionDispatchInfo? failure = null;
        host.Invoke(() => {
            try {
                built = windowBuilder.Build();
            }
            catch (Exception ex) {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
        });
        failure?.Throw();

        return new InfiniFrameWindowTestUtility {
            Window = built!,
            // The GTK loop is owned by the shared host thread, not by this test, so there is nothing to join.
            _windowThread = null
        };
    }

    [SupportedOSPlatform("windows"), MustDisposeResource]
    private static InfiniFrameWindowTestUtility CreateOnStaThread(
        InfiniFrameWindowBuilder windowBuilder
    ) {
        var windowSource = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() => {
            try {
                Console.Error.WriteLine(
                    $"[InfiniFrameWindowTestUtility] STA thread started managedThreadId={Environment.CurrentManagedThreadId} apt={Thread.CurrentThread.GetApartmentState()} pid={Environment.ProcessId}");

                IInfiniFrameWindow window = windowBuilder.Build();
                Console.Error.WriteLine(
                    $"[InfiniFrameWindowTestUtility] window initialized instance=0x{window.InstanceHandle.ToInt64():X} hwnd=0x{window.WindowHandle.ToInt64():X} thread={Environment.CurrentManagedThreadId}");
                windowSource.SetResult(window);
                window.WaitForClose();
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
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
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        try {
            Window.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        if (_windowThread is null) return;

        try {
            // Keep test disposal bounded so per-test timeout policies remain reliable.
            TimeSpan firstJoinTimeout = OperatingSystem.IsWindows()
                && RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? TimeSpan.FromSeconds(2)
                    : TimeSpan.FromSeconds(3);

            if (!_windowThread.Join(firstJoinTimeout)) {
                try {
                    Window.Close();
                }
                catch (ApplicationException) {
                    // ignored
                }
                catch (ObjectDisposedException) {
                    // ignored
                }

                _windowThread.Join(TimeSpan.FromSeconds(2));
            }
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
}
