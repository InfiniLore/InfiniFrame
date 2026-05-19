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

    // pthread_main_np() returns 1 if the calling thread is the main thread
    [LibraryImport("/usr/lib/libSystem.B.dylib", EntryPoint = "pthread_main_np")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int PThreadMainNp();

    // dispatch_get_main_queue() returns the main GCD queue
    [LibraryImport(DispatchLib, EntryPoint = "dispatch_get_main_queue")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial IntPtr DispatchGetMainQueue();

    // dispatch_async_f(queue, context, work)
    [LibraryImport(DispatchLib, EntryPoint = "dispatch_async_f")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void DispatchAsyncF(
        IntPtr queue,
        IntPtr context,
        IntPtr work
    );

    public static bool IsMainThread() => PThreadMainNp() == 1;

    public static void DispatchOnMainThread(MainThreadCallback callback) {
        IntPtr queue = DispatchGetMainQueue();

        GCHandle handle = GCHandle.Alloc(callback);
        IntPtr context = GCHandle.ToIntPtr(handle);

        DispatchAsyncF(queue, context, MainThreadTrampoline);
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
