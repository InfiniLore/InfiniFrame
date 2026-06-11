// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using JetBrains.Annotations;
using System.Runtime.Versioning;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameTestWindow {
    [SupportedOSPlatform("windows")]
    [MustDisposeResource]
    private static partial InfiniFrameTestWindow CreateWindows(InfiniFrameWindowBuilder windowBuilder) {
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

        return new InfiniFrameTestWindow {
            BuilderSnapshot = windowBuilder,
            Window = windowSource.Task.GetAwaiter().GetResult(),
            _windowThread = thread
        };
    }
}
