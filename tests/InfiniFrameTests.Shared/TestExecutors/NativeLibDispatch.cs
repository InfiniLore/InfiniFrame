// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniFrameTests.Shared.TestExecutors;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Thin P/Invoke layer over libdispatch.
/// </summary>
internal static partial class NativeMacOsLibDispatch {
    private const string DispatchLib = "/usr/lib/system/libdispatch.dylib";

    public delegate void MainThreadCallback();

    [LibraryImport("/usr/lib/libSystem.B.dylib", EntryPoint = "pthread_main_np")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int PThreadMainNp();

    [LibraryImport(DispatchLib, EntryPoint = "dispatch_async_f")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void DispatchAsyncF(
        IntPtr queue,
        IntPtr context,
        IntPtr work
    );

    private static readonly IntPtr MainQueue = ResolveMainQueue();

    public static bool IsMainThread() => PThreadMainNp() == 1;

    public static void DispatchOnMainThread(MainThreadCallback callback) {
        GCHandle handle = GCHandle.Alloc(callback);
        IntPtr context = GCHandle.ToIntPtr(handle);

        DispatchAsyncF(MainQueue, context, MainThreadTrampoline);
    }

    private static IntPtr ResolveMainQueue() {
        IntPtr lib = NativeLibrary.Load(DispatchLib);

        // dispatch_get_main_queue is not reliably exported
        // on modern macOS environments.
        return NativeLibrary.GetExport(lib, "_dispatch_main_q");
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void TrampolineImpl(IntPtr context) {
        GCHandle handle = GCHandle.FromIntPtr(context);

        try {
            var callback = (MainThreadCallback)handle.Target!;
            callback();
        }
        finally {
            handle.Free();
        }
    }

    private static readonly unsafe IntPtr MainThreadTrampoline =
        (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, void>)&TrampolineImpl;
}