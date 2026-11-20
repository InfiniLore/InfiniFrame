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

    /// <summary>
    ///     Returns the shared <c>NSApplication</c> instance.
    /// </summary>
    /// <remarks>
    ///     Only call this after the native host registered the application: the shared instance is
    ///     created on the thread that performs the AppKit registration.
    /// </remarks>
    private static IntPtr GetSharedApplication()
        => SendObject(
            GetClass("NSApplication"),
            RegisterSelector("sharedApplication")
        );

    /// <summary>
    ///     Determines whether the native host registered the application with AppKit.
    /// </summary>
    /// <remarks>
    ///     The native host installs an application menu while it registers the shared
    ///     <c>NSApplication</c>, so a non-nil main menu is the public AppKit signal that the host
    ///     bootstrapped the application. AppKit's <c>-[NSApplication isRunning]</c> cannot be used
    ///     for this: InfiniFrame never enters <c>-[NSApplication run]</c> (the native host pumps the
    ///     shared main run loop instead, because test hosts and embedding applications can already
    ///     own it), so <c>isRunning</c> stays <c>NO</c> for the whole lifetime of the process.
    /// </remarks>
    public static bool IsApplicationRegisteredWithAppKit()
        => SendObject(GetSharedApplication(), RegisterSelector("mainMenu")) != IntPtr.Zero;

    /// <summary>
    ///     Returns the number of windows AppKit currently tracks for the shared application.
    /// </summary>
    public static int GetApplicationWindowCount() {
        IntPtr windows = SendObject(GetSharedApplication(), RegisterSelector("windows"));
        return checked((int)SendCount(windows, RegisterSelector("count")));
    }

    /// <summary>
    ///     Returns the number of visible windows AppKit currently tracks for the shared application.
    /// </summary>
    public static int GetVisibleApplicationWindowCount() {
        IntPtr windows = SendObject(GetSharedApplication(), RegisterSelector("windows"));
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
