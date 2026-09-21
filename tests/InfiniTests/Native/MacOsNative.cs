// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniTests.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class MacOsNative {
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const string ObjCRuntime = "/usr/lib/libobjc.A.dylib";
    private const string LibDispatch = "/usr/lib/system/libdispatch.dylib";
    private const string LibSystem = "/usr/lib/libSystem.dylib";

    [LibraryImport(CoreFoundation, EntryPoint = "CFRunLoopGetMain")]
    public static partial IntPtr GetMainRunLoop();

    [LibraryImport(CoreFoundation, EntryPoint = "CFRunLoopRunInMode")]
    public static partial int RunLoopInMode(IntPtr mode, double seconds, [MarshalAs(UnmanagedType.I1)] bool returnAfterSourceHandled);

    [LibraryImport(CoreFoundation, EntryPoint = "CFRunLoopStop")]
    public static partial void StopRunLoop(IntPtr runLoop);

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_autoreleasePoolPush")]
    public static partial IntPtr PushAutoreleasePool();

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_autoreleasePoolPop")]
    public static partial void PopAutoreleasePool(IntPtr pool);

    [LibraryImport(LibDispatch, EntryPoint = "dispatch_async_f")]
    public static partial void DispatchAsync(IntPtr queue, IntPtr context, IntPtr work);

    [LibraryImport(LibSystem, EntryPoint = "pthread_main_np")]
    public static partial int IsMainThread();
}
