// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native.Delegates;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CustomMarshaller(
    typeof(InfiniFrameNativeParameters), 
    MarshalMode.ManagedToUnmanagedIn,
    typeof(ManagedToUnmanagedIn)
)]
internal static class InfiniFrameNativeParametersMarshaller {
    [StructLayout(LayoutKind.Sequential)]
    internal struct Unmanaged {
        internal IntPtr StartString;
        internal IntPtr StartUrl;
        internal IntPtr Title;
        internal IntPtr WindowIconFile;
        internal IntPtr TemporaryFilesPath;
        internal IntPtr UserAgent;
        internal IntPtr BrowserControlInitParameters;
        internal IntPtr NotificationRegistrationId;
        internal IntPtr NativeParent;
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
        internal int Left;
        internal int Top;
        internal int Width;
        internal int Height;
        internal int Zoom;
        internal int MinWidth;
        internal int MinHeight;
        internal int MaxWidth;
        internal int MaxHeight;
        internal byte CenterOnInitialize;
        internal byte Chromeless;
        internal byte Transparent;
        internal byte ContextMenuEnabled;
        internal byte ZoomEnabled;
        internal byte DevToolsEnabled;
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
        internal byte NotificationsEnabled;
        internal int Size;
    }

    internal ref struct ManagedToUnmanagedIn {
        private Unmanaged _unmanaged;

        public void FromManaged(InfiniFrameNativeParameters managed) {
            _unmanaged = new Unmanaged {
                StartString = ToUtf8Ptr(managed.StartString),
                StartUrl = ToUtf8Ptr(managed.StartUrl),
                Title = ToUtf8Ptr(managed.Title),
                WindowIconFile = ToUtf8Ptr(managed.WindowIconFile),
                TemporaryFilesPath = ToUtf8Ptr(managed.TemporaryFilesPath),
                UserAgent = ToUtf8Ptr(managed.UserAgent),
                BrowserControlInitParameters = ToUtf8Ptr(managed.BrowserControlInitParameters),
                NotificationRegistrationId = ToUtf8Ptr(managed.NotificationRegistrationId),
                NativeParent = managed.NativeParent,
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
                Left = managed.Left,
                Top = managed.Top,
                Width = managed.Width,
                Height = managed.Height,
                Zoom = managed.Zoom,
                MinWidth = managed.MinWidth,
                MinHeight = managed.MinHeight,
                MaxWidth = managed.MaxWidth,
                MaxHeight = managed.MaxHeight,
                CenterOnInitialize = ToByte(managed.CenterOnInitialize),
                Chromeless = ToByte(managed.Chromeless),
                Transparent = ToByte(managed.Transparent),
                ContextMenuEnabled = ToByte(managed.ContextMenuEnabled),
                ZoomEnabled = ToByte(managed.ZoomEnabled),
                DevToolsEnabled = ToByte(managed.DevToolsEnabled),
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
                NotificationsEnabled = ToByte(managed.NotificationsEnabled),
                Size = managed.Size
            };
        }

        public Unmanaged ToUnmanaged() => _unmanaged;

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
            Marshal.FreeCoTaskMem(_unmanaged.NotificationRegistrationId);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static byte ToByte(bool value) 
        => value ? (byte)1 : (byte)0;

    private static IntPtr ToUtf8Ptr(string? value) => value is null
        ? IntPtr.Zero
        : Marshal.StringToCoTaskMemUTF8(value);

    private static IntPtr ToFunctionPtr(Delegate? callback) => callback is null
        ? IntPtr.Zero
        : callback switch {
            CppClosingDelegate closing => Marshal.GetFunctionPointerForDelegate(closing),
            CppClosedDelegate closed => Marshal.GetFunctionPointerForDelegate(closed),
            CppFocusInDelegate focusIn => Marshal.GetFunctionPointerForDelegate(focusIn),
            CppFocusOutDelegate focusOut => Marshal.GetFunctionPointerForDelegate(focusOut),
            CppResizedDelegate resized => Marshal.GetFunctionPointerForDelegate(resized),
            CppMaximizedDelegate maximized => Marshal.GetFunctionPointerForDelegate(maximized),
            CppRestoredDelegate restored => Marshal.GetFunctionPointerForDelegate(restored),
            CppMinimizedDelegate minimized => Marshal.GetFunctionPointerForDelegate(minimized),
            CppMovedDelegate moved => Marshal.GetFunctionPointerForDelegate(moved),
            CppWebMessageReceivedDelegate webMessageReceived => Marshal.GetFunctionPointerForDelegate(webMessageReceived),
            CppWebResourceRequestedDelegate webResourceRequested => Marshal.GetFunctionPointerForDelegate(webResourceRequested),
            _ => throw new ArgumentOutOfRangeException(nameof(callback), callback.GetType(), "Unsupported callback delegate type.")
        };

    private static IntPtr GetCustomSchemeName(IntPtr[]? values, int index) 
        => values is not null && values.Length > index ? values[index] : IntPtr.Zero;
}
