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

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_getClass", StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetClass(string name);

    [LibraryImport(ObjCRuntime, EntryPoint = "sel_registerName", StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr RegisterSelector(string name);

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
    private static partial IntPtr SendObject(IntPtr receiver, IntPtr selector);

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial bool SendBool(IntPtr receiver, IntPtr selector);

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
    private static partial IntPtr SendCount(IntPtr receiver, IntPtr selector);

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

    public static bool IsApplicationRunning() {
        IntPtr application = SendObject(
            GetClass("NSApplication"),
            RegisterSelector("sharedApplication")
        );
        return SendBool(application, RegisterSelector("isRunning"));
    }

    public static bool IsApplicationFinishedLaunching() {
        IntPtr application = SendObject(
            GetClass("NSApplication"),
            RegisterSelector("sharedApplication")
        );
        return SendBool(application, RegisterSelector("isFinishedLaunching"));
    }

    public static int GetApplicationWindowCount() {
        IntPtr application = SendObject(
            GetClass("NSApplication"),
            RegisterSelector("sharedApplication")
        );
        IntPtr windows = SendObject(application, RegisterSelector("windows"));
        return checked((int)SendCount(windows, RegisterSelector("count")));
    }

    public static int GetVisibleApplicationWindowCount() {
        IntPtr application = SendObject(
            GetClass("NSApplication"),
            RegisterSelector("sharedApplication")
        );
        IntPtr windows = SendObject(application, RegisterSelector("windows"));
        int count = checked((int)SendCount(windows, RegisterSelector("count")));
        IntPtr objectAtIndex = RegisterSelector("objectAtIndex:");
        IntPtr isVisible = RegisterSelector("isVisible");
        int visible = 0;

        for (nuint index = 0; index < (nuint)count; index++) {
            IntPtr window = SendObjectAtIndex(windows, objectAtIndex, index);
            if (SendBool(window, isVisible)) visible++;
        }

        return visible;
    }

    [LibraryImport(ObjCRuntime, EntryPoint = "objc_msgSend")]
    private static partial IntPtr SendObjectAtIndex(IntPtr receiver, IntPtr selector, nuint index);
    
    [LibraryImport("/usr/lib/libc.dylib", EntryPoint = "_exit")]
    public static partial void PosixExit(int status);
}
