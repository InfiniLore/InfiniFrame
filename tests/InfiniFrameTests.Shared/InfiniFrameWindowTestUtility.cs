// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using JetBrains.Annotations;
using System.Runtime.InteropServices;
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
        // Linux: gtk_init() and gtk_main() must run on the same thread; using a dedicated thread
        //        avoids the deadlock that occurs when WaitForClose() is called from a different
        //        thread than the one that called gtk_init() (which happens during Build()).
        // macOS: Similar to Windows — Cocoa/AppKit requires a dedicated UI thread.
        if (OperatingSystem.IsWindows()) return CreateOnStaThread(windowBuilder);
        if (!OperatingSystem.IsMacOS()) return CreateOnDedicatedThread(windowBuilder);

        // macOS: NSApp requires the UI to run on the process main thread, which is the test
        // runner thread itself, so we cannot move Build() to a background thread.
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

    [MustDisposeResource]
    private static InfiniFrameWindowTestUtility CreateOnDedicatedThread(
        InfiniFrameWindowBuilder windowBuilder
    ) {
        var windowSource = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() => {
            try {
                IInfiniFrameWindow window = windowBuilder.Build();
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

        thread.Start();

        return new InfiniFrameWindowTestUtility {
            Window = windowSource.Task.GetAwaiter().GetResult(),
            _windowThread = thread
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
