// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using JetBrains.Annotations;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameTestWindow {
    #if NET9_0_OR_GREATER
    private static readonly Lock HostLock = new();
    #else
    // ReSharper disable once ChangeFieldTypeToSystemThreadingLock
    private static readonly object HostLock = new();
    #endif

    // Resolves a weird bug where if certain tests are ran on linux, the window is not properly disposed of
    // Unless there is some form of a parent-child relationship, the closing and cleanup mechanism is not properly handled
    private static IInfiniFrameWindow? _hostWindow;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static IInfiniFrameWindow EnsureGtkHost() {
        if (_hostWindow is not null) return _hostWindow;

        lock (HostLock) {
            if (_hostWindow is not null) return _hostWindow;

            var hostSource = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

            var thread = new Thread(() => {
                try {
                    var hostBuilder = InfiniFrameWindowBuilder.Create();
                    hostBuilder.SetString(StartString);

                    IInfiniFrameWindow host = hostBuilder.Build();

                    hostSource.SetResult(host);

                    host.WaitForClose();
                }
                catch (Exception ex)
                    when (ExceptionsUtility.IsNonFatalException(ex)) {
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

    [SupportedOSPlatform("linux")]
    [MustDisposeResource]
    private static partial InfiniFrameTestWindow CreateLinux(InfiniFrameWindowBuilder windowBuilder) {
        IInfiniFrameWindow host = EnsureGtkHost();

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

        return new InfiniFrameTestWindow {
            Window = built!,
            BuilderSnapshot = windowBuilder,
            _windowThread = null
        };
    }
}
