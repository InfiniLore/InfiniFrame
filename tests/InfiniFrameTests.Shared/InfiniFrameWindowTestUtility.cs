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
        windowBuilder.SetTemporaryFilesPath(CreateUniqueTemporaryFilesPath());
        builder?.Invoke(windowBuilder);

        // Windows: WebView2 requires STA thread for COM initialization
        // Linux: GTK implicitly treats the calling thread as the main UI thread
        // macOS: Similar to Linux, but with additional main-thread restrictions for menu operations
        if (OperatingSystem.IsWindows()) return CreateOnStaThread(windowBuilder);

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

    private static string CreateUniqueTemporaryFilesPath() {
        string uniqueSuffix = $"{Environment.ProcessId}-{Environment.CurrentManagedThreadId}-{Guid.NewGuid():N}";
        return Path.Combine(Path.GetTempPath(), "InfiniFrameTests", uniqueSuffix);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // ----------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        string? tempFolder = Window.TemporaryFilesPath;

        try {
            Window.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        if (_windowThread is not null) {
            try {
                // ARM64 runners can take noticeably longer to unwind WebView/native teardown.
                // Interrupting a thread that is inside native shutdown has caused host instability.
                // Wait longer and avoid Thread.Interrupt to keep teardown deterministic.
                TimeSpan joinTimeout = OperatingSystem.IsWindows()
                    && RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? TimeSpan.FromSeconds(30)
                    : TimeSpan.FromSeconds(10);

                _windowThread.Join(joinTimeout);
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

        // ReSharper disable once InvertIf
        if (tempFolder is not null) {
            try {
                FileUtility.SafeDeleteDirectory(tempFolder);
            }
            catch (ApplicationException) {
                // ignored
            }
            catch (OperationCanceledException) {
                // ignored
            }
        }
    }
}
