// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    /// <summary>
    ///     Enables or disables transparent background mode.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether to enable transparency.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetTransparentEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    /// <summary>
    ///     Enables or disables the browser context menu.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether to enable the context menu.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetContextMenuEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    /// <summary>
    ///     Enables or disables media autoplay.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether to enable media autoplay.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMediaAutoplayEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    /// <summary>
    ///     Sets the user agent string for the browser control.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="userAgent">The user agent string.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetUserAgent", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetUserAgent(IntPtr instance, string? userAgent);

    /// <summary>
    ///     Enables or disables zoom functionality.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="zoomEnabled">Whether to enable zoom.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetZoomEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool zoomEnabled);

    /// <summary>
    ///     Enables or disables developer tools.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether to enable dev tools.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetDevToolsEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    /// <summary>
    ///     Sets the full-screen mode of the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="fullScreen">Whether to enter full-screen mode.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFullScreen", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool fullScreen);

    /// <summary>
    ///     Maximizes or restores the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="maximized">Whether to maximize the window.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMaximized", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool maximized);

    /// <summary>
    ///     Minimizes or restores the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="minimized">Whether to minimize the window.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMinimized", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool minimized);

    /// <summary>
    ///     Sets whether the window is resizable by the user.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="resizable">Whether the window is resizable.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetResizable", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool resizable);

    /// <summary>
    ///     Sets whether the window is topmost (always on top).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="topmost">Whether the window is topmost.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetTopmost", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool topmost);

    /// <summary>
    ///     Sets the window icon from a file.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="filename">The path to the icon file.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetIconFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetIconFile(IntPtr instance, string filename);

    /// <summary>
    ///     Sets the window title.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The window title.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetTitle", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTitle(IntPtr instance, string? title);

    /// <summary>
    ///     Sets the zoom level of the browser control.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="zoom">The zoom level (e.g. 100 = 100%).</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetZoom", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetZoom(IntPtr instance, int zoom);

    /// <summary>
    ///     Sets the window position in screen coordinates.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetPosition", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetPosition(IntPtr instance, int x, int y);

    /// <summary>
    ///     Sets the window size in pixels.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetSize", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetSize(IntPtr instance, int width, int height);

    /// <summary>
    ///     Sets the maximum window size.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="maxWidth">The maximum width.</param>
    /// <param name="maxHeight">The maximum height.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMaxSize", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMaxSize(IntPtr instance, int maxWidth, int maxHeight);

    /// <summary>
    ///     Sets the minimum window size.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="minWidth">The minimum width.</param>
    /// <param name="minHeight">The minimum height.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMinSize", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMinSize(IntPtr instance, int minWidth, int minHeight);
}