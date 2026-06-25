// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using JetBrains.Annotations;
using System.Runtime.InteropServices;

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

        if (_windowThread is null)
            return;

        try {
            TimeSpan firstJoinTimeout =
                OperatingSystem.IsWindows() &&
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? TimeSpan.FromSeconds(2)
                    : TimeSpan.FromSeconds(3);

            if (_windowThread.Join(firstJoinTimeout)) return;

            try {
                Window.Close();
            }
            catch (ApplicationException) {
            }
            catch (ObjectDisposedException) {
            }

            _windowThread.Join(TimeSpan.FromSeconds(2));
        }
        catch (ThreadInterruptedException) {
        }
        catch (ThreadStateException) {
        }
        catch (ObjectDisposedException) {
        }
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
            .EnableNotifications(false)
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
