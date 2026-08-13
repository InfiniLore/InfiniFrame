// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using InfiniFrame.NativeBridge.Delegates;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Custom marshaller for converting <see cref="InfiniFrameNativeParameters" />
///     to an unmanaged representation for native interop calls.
/// </summary>
[CustomMarshaller(
    typeof(InfiniFrameNativeParameters),
    MarshalMode.ManagedToUnmanagedIn,
    typeof(ManagedToUnmanagedIn)
)]
internal static class InfiniFrameNativeParametersMarshaller {
    /// <summary>
    ///     Unmanaged layout of <see cref="InfiniFrameNativeParameters" /> used for native interop.
    ///     Field order must match the C++ InfiniFrameInitParams struct exactly.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Unmanaged {
        // ── Content strings ────────────────────────────────────────────
        internal IntPtr StartString;
        internal IntPtr StartUrl;

        // ── Window identity / appearance strings ───────────────────────
        internal IntPtr Title;
        internal IntPtr WindowIconFile;
        internal IntPtr TemporaryFilesPath;
        internal IntPtr UserAgent;
        internal IntPtr BrowserControlInitParameters;
        internal IntPtr WebView2RuntimePath;
        internal IntPtr NotificationRegistrationId;
        internal IntPtr WindowsAppUserModelId;
        internal IntPtr DefaultNotificationIcon;

        // ── Runtime configuration ──────────────────────────────────────
        internal int RemoteDebuggingPort;

        // ── Parent window ──────────────────────────────────────────────
        internal IntPtr NativeParent;

        // ── Event callbacks ────────────────────────────────────────────
        internal IntPtr ClosingHandler;
        internal IntPtr ClosedHandler;
        internal IntPtr FocusInHandler;
        internal IntPtr FocusOutHandler;
        internal IntPtr ResizedHandler;
        internal IntPtr MaximizedHandler;
        internal IntPtr RestoredHandler;
        internal IntPtr MinimizedHandler;
        internal IntPtr MovedHandler;
        internal IntPtr WebMessageReceivedHandler;
        internal IntPtr DebugEventHandler;

        // ── Custom scheme support ──────────────────────────────────────
        internal IntPtr CustomSchemeNames0;
        internal IntPtr CustomSchemeNames1;
        internal IntPtr CustomSchemeNames2;
        internal IntPtr CustomSchemeNames3;
        internal IntPtr CustomSchemeNames4;
        internal IntPtr CustomSchemeNames5;
        internal IntPtr CustomSchemeNames6;
        internal IntPtr CustomSchemeNames7;
        internal IntPtr CustomSchemeNames8;
        internal IntPtr CustomSchemeNames9;
        internal IntPtr CustomSchemeNames10;
        internal IntPtr CustomSchemeNames11;
        internal IntPtr CustomSchemeNames12;
        internal IntPtr CustomSchemeNames13;
        internal IntPtr CustomSchemeNames14;
        internal IntPtr CustomSchemeNames15;
        internal IntPtr CustomSchemeHandler;
        internal IntPtr NavigationStartingHandler;

        // ── Drag-and-drop ──────────────────────────────────────────────
        internal IntPtr DragDropHandler;
        internal byte DragDropEnabled;

        // ── Window geometry ────────────────────────────────────────────
        internal int Left;
        internal int Top;
        internal int Width;
        internal int Height;
        internal int Zoom;
        internal int MinWidth;
        internal int MinHeight;
        internal int MaxWidth;
        internal int MaxHeight;

        // ── Behavior flags ─────────────────────────────────────────────
        internal byte CenterOnInitialize;
        internal byte Chromeless;
        internal byte Transparent;
        internal byte ContextMenuEnabled;
        internal byte ZoomEnabled;
        internal byte DevToolsEnabled;
        internal byte WebInspectorEnabled;
        internal byte FullScreen;
        internal byte Maximized;
        internal byte Minimized;
        internal byte Resizable;
        internal byte Topmost;
        internal byte UseOsDefaultLocation;
        internal byte UseOsDefaultSize;
        internal byte GrantBrowserPermissions;
        internal byte MediaAutoplayEnabled;
        internal byte FileSystemAccessEnabled;
        internal byte WebSecurityEnabled;
        internal byte JavascriptClipboardAccessEnabled;
        internal byte MediaStreamEnabled;
        internal byte SmoothScrollingEnabled;
        internal byte IgnoreCertificateErrorsEnabled;
        internal byte StatusBarEnabled;
        internal byte BrowserShortcutsEnabled;
        internal byte NotificationsEnabled;

        // ── Background color (RGBA) ────────────────────────────────────
        internal byte BackgroundColorR;
        internal byte BackgroundColorG;
        internal byte BackgroundColorB;
        internal byte BackgroundColorA;

        // ── Menu ───────────────────────────────────────────────────────
        internal IntPtr MenuBarJson;

        // ── ABI version ────────────────────────────────────────────────
        internal int Size;
    }

    /// <summary>
    ///     Marshals managed <see cref="InfiniFrameNativeParameters" /> to the native <see cref="Unmanaged" /> layout.
    /// </summary>
    internal ref struct ManagedToUnmanagedIn {
        private Unmanaged _unmanaged;

        /// <summary>
        ///     Copies all values from the managed source into the unmanaged representation.
        /// </summary>
        /// <param name="managed">The managed parameters source.</param>
        public void FromManaged(InfiniFrameNativeParameters managed) {
            _unmanaged = new Unmanaged {
                // Content strings
                StartString = ToUtf8Ptr(managed.StartString),
                StartUrl = ToUtf8Ptr(managed.StartUrl),

                // Window identity strings
                Title = ToUtf8Ptr(managed.Title),
                WindowIconFile = ToUtf8Ptr(managed.WindowIconFile),
                TemporaryFilesPath = ToUtf8Ptr(managed.TemporaryFilesPath),
                UserAgent = ToUtf8Ptr(managed.UserAgent),
                BrowserControlInitParameters = ToUtf8Ptr(managed.BrowserControlInitParameters),
                WebView2RuntimePath = ToUtf8Ptr(managed.WebView2RuntimePath),
                NotificationRegistrationId = ToUtf8Ptr(managed.NotificationRegistrationId),
                WindowsAppUserModelId = ToUtf8Ptr(managed.WindowsAppUserModelId),
                DefaultNotificationIcon = ToUtf8Ptr(managed.DefaultNotificationIcon),

                // Runtime configuration
                RemoteDebuggingPort = managed.RemoteDebuggingPort,

                // Parent window
                NativeParent = managed.NativeParent,

                // Event callbacks
                ClosingHandler = ToFunctionPtr(managed.ClosingHandler),
                ClosedHandler = ToFunctionPtr(managed.ClosedHandler),
                FocusInHandler = ToFunctionPtr(managed.FocusInHandler),
                FocusOutHandler = ToFunctionPtr(managed.FocusOutHandler),
                ResizedHandler = ToFunctionPtr(managed.ResizedHandler),
                MaximizedHandler = ToFunctionPtr(managed.MaximizedHandler),
                RestoredHandler = ToFunctionPtr(managed.RestoredHandler),
                MinimizedHandler = ToFunctionPtr(managed.MinimizedHandler),
                MovedHandler = ToFunctionPtr(managed.MovedHandler),
                WebMessageReceivedHandler = ToFunctionPtr(managed.WebMessageReceivedHandler),
                DebugEventHandler = ToFunctionPtr(managed.DebugEventHandler),

                // Custom scheme support
                CustomSchemeNames0 = GetCustomSchemeName(managed.CustomSchemeNames, 0),
                CustomSchemeNames1 = GetCustomSchemeName(managed.CustomSchemeNames, 1),
                CustomSchemeNames2 = GetCustomSchemeName(managed.CustomSchemeNames, 2),
                CustomSchemeNames3 = GetCustomSchemeName(managed.CustomSchemeNames, 3),
                CustomSchemeNames4 = GetCustomSchemeName(managed.CustomSchemeNames, 4),
                CustomSchemeNames5 = GetCustomSchemeName(managed.CustomSchemeNames, 5),
                CustomSchemeNames6 = GetCustomSchemeName(managed.CustomSchemeNames, 6),
                CustomSchemeNames7 = GetCustomSchemeName(managed.CustomSchemeNames, 7),
                CustomSchemeNames8 = GetCustomSchemeName(managed.CustomSchemeNames, 8),
                CustomSchemeNames9 = GetCustomSchemeName(managed.CustomSchemeNames, 9),
                CustomSchemeNames10 = GetCustomSchemeName(managed.CustomSchemeNames, 10),
                CustomSchemeNames11 = GetCustomSchemeName(managed.CustomSchemeNames, 11),
                CustomSchemeNames12 = GetCustomSchemeName(managed.CustomSchemeNames, 12),
                CustomSchemeNames13 = GetCustomSchemeName(managed.CustomSchemeNames, 13),
                CustomSchemeNames14 = GetCustomSchemeName(managed.CustomSchemeNames, 14),
                CustomSchemeNames15 = GetCustomSchemeName(managed.CustomSchemeNames, 15),
                CustomSchemeHandler = ToFunctionPtr(managed.CustomSchemeHandler),
                NavigationStartingHandler = ToFunctionPtr(managed.NavigationStartingHandler),

                // Drag-and-drop
                DragDropHandler = ToFunctionPtr(managed.FileDroppedHandler),
                DragDropEnabled = ToByte(managed.DragDropEnabled),

                // Window geometry
                Left = managed.Left,
                Top = managed.Top,
                Width = managed.Width,
                Height = managed.Height,
                Zoom = managed.Zoom,
                MinWidth = managed.MinWidth,
                MinHeight = managed.MinHeight,
                MaxWidth = managed.MaxWidth,
                MaxHeight = managed.MaxHeight,

                // Behavior flags
                CenterOnInitialize = ToByte(managed.CenterOnInitialize),
                Chromeless = ToByte(managed.Chromeless),
                Transparent = ToByte(managed.Transparent),
                ContextMenuEnabled = ToByte(managed.ContextMenuEnabled),
                ZoomEnabled = ToByte(managed.ZoomEnabled),
                DevToolsEnabled = ToByte(managed.DevToolsEnabled),
                WebInspectorEnabled = ToByte(managed.WebInspectorEnabled),
                FullScreen = ToByte(managed.FullScreen),
                Maximized = ToByte(managed.Maximized),
                Minimized = ToByte(managed.Minimized),
                Resizable = ToByte(managed.Resizable),
                Topmost = ToByte(managed.Topmost),
                UseOsDefaultLocation = ToByte(managed.UseOsDefaultLocation),
                UseOsDefaultSize = ToByte(managed.UseOsDefaultSize),
                GrantBrowserPermissions = ToByte(managed.GrantBrowserPermissions),
                MediaAutoplayEnabled = ToByte(managed.MediaAutoplayEnabled),
                FileSystemAccessEnabled = ToByte(managed.FileSystemAccessEnabled),
                WebSecurityEnabled = ToByte(managed.WebSecurityEnabled),
                JavascriptClipboardAccessEnabled = ToByte(managed.JavascriptClipboardAccessEnabled),
                MediaStreamEnabled = ToByte(managed.MediaStreamEnabled),
                SmoothScrollingEnabled = ToByte(managed.SmoothScrollingEnabled),
                IgnoreCertificateErrorsEnabled = ToByte(managed.IgnoreCertificateErrorsEnabled),
                StatusBarEnabled = ToByte(managed.StatusBarEnabled),
                BrowserShortcutsEnabled = ToByte(managed.BrowserShortcutsEnabled),
                NotificationsEnabled = ToByte(managed.NotificationsEnabled),

                // Background color
                BackgroundColorR = managed.BackgroundColorR,
                BackgroundColorG = managed.BackgroundColorG,
                BackgroundColorB = managed.BackgroundColorB,
                BackgroundColorA = managed.BackgroundColorA,

                // Menu
                MenuBarJson = ToUtf8Ptr(managed.MenuBarJson),

                // ABI version
                Size = managed.Size
            };
        }

        /// <summary>
        ///     Returns the populated unmanaged representation.
        /// </summary>
        /// <returns>The <see cref="Unmanaged" /> instance.</returns>
        public Unmanaged ToUnmanaged() => _unmanaged;

        /// <summary>
        ///     Frees CoTaskMem-allocated string pointers marshaled to the unmanaged struct.
        /// </summary>
        public void Free() {
            // CustomSchemeNames are unmanaged HGlobal pointers allocated by managed builders.
            // Their lifetime is owned by the managed window initialization flow, not by this marshaller.
            Marshal.FreeCoTaskMem(_unmanaged.StartString);
            Marshal.FreeCoTaskMem(_unmanaged.StartUrl);
            Marshal.FreeCoTaskMem(_unmanaged.Title);
            Marshal.FreeCoTaskMem(_unmanaged.WindowIconFile);
            Marshal.FreeCoTaskMem(_unmanaged.TemporaryFilesPath);
            Marshal.FreeCoTaskMem(_unmanaged.UserAgent);
            Marshal.FreeCoTaskMem(_unmanaged.BrowserControlInitParameters);
            Marshal.FreeCoTaskMem(_unmanaged.WebView2RuntimePath);
            Marshal.FreeCoTaskMem(_unmanaged.NotificationRegistrationId);
            Marshal.FreeCoTaskMem(_unmanaged.WindowsAppUserModelId);
            Marshal.FreeCoTaskMem(_unmanaged.DefaultNotificationIcon);
            Marshal.FreeCoTaskMem(_unmanaged.MenuBarJson);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Converts a <see cref="bool" /> to a <see cref="byte" /> (1 for <c>true</c>, 0 for <c>false</c>).
    /// </summary>
    private static byte ToByte(bool value)
        => value ? (byte)1 : (byte)0;

    /// <summary>
    ///     Marshals a managed string to a CoTaskMem-allocated UTF-8 pointer, or <see cref="IntPtr.Zero" /> if null.
    /// </summary>
    private static IntPtr ToUtf8Ptr(string? value) => value is null
        ? IntPtr.Zero
        : Marshal.StringToCoTaskMemUTF8(value);

    /// <summary>
    ///     Converts a managed delegate to a function pointer suitable for native callbacks.
    /// </summary>
    /// <param name="callback">The managed delegate.</param>
    /// <returns>A native function pointer, or <see cref="IntPtr.Zero" /> if null.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the delegate type is not recognized.</exception>
    private static IntPtr ToFunctionPtr(Delegate? callback) => callback is null
        ? IntPtr.Zero
        : callback switch {
            CppClosedDelegate closed => Marshal.GetFunctionPointerForDelegate(closed),
            CppClosingDelegate closing => Marshal.GetFunctionPointerForDelegate(closing),
            CppFocusInDelegate focusIn => Marshal.GetFunctionPointerForDelegate(focusIn),
            CppFocusOutDelegate focusOut => Marshal.GetFunctionPointerForDelegate(focusOut),
            CppMaximizedDelegate maximized => Marshal.GetFunctionPointerForDelegate(maximized),
            CppMinimizedDelegate minimized => Marshal.GetFunctionPointerForDelegate(minimized),
            CppMovedDelegate moved => Marshal.GetFunctionPointerForDelegate(moved),
            CppResizedDelegate resized => Marshal.GetFunctionPointerForDelegate(resized),
            CppRestoredDelegate restored => Marshal.GetFunctionPointerForDelegate(restored),
            CppWebMessageReceivedDelegate webMessageReceived => Marshal.GetFunctionPointerForDelegate(webMessageReceived),
            CppDebugEventDelegate debugEvent => Marshal.GetFunctionPointerForDelegate(debugEvent),
            CppWebResourceRequestedDelegate webResourceRequested => Marshal.GetFunctionPointerForDelegate(webResourceRequested),
            CppNavigationStartingDelegate navigationStarting => Marshal.GetFunctionPointerForDelegate(navigationStarting),
            CppFileDroppedDelegate fileDropped => Marshal.GetFunctionPointerForDelegate(fileDropped),
            _ => throw new ArgumentOutOfRangeException(nameof(callback), callback.GetType(), "Unsupported callback delegate type.")
        };

    /// <summary>
    ///     Gets a custom scheme name pointer from the array at the specified index, or <see cref="IntPtr.Zero" /> if
    ///     unavailable.
    /// </summary>
    private static IntPtr GetCustomSchemeName(IntPtr[]? values, int index)
        => values is not null && values.Length > index ? values[index] : IntPtr.Zero;
}
