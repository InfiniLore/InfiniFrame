// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge.Delegates;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    /// <summary>
    ///     Sets the callback invoked when the native window is about to close.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The closing delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetClosingCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetClosingCallback(IntPtr instance, CppClosingCallback callback);

    /// <summary>
    ///     Sets the callback invoked when the native window has been closed.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The closed delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetClosedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetClosedCallback(IntPtr instance, CppClosedCallback callback);

    /// <summary>
    ///     Sets the callback invoked when the native window receives focus.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The focus-in delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFocusInCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFocusInCallback(IntPtr instance, CppFocusInCallback callback);

    /// <summary>
    ///     Sets the callback invoked when the native window loses focus.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The focus-out delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFocusOutCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFocusOutCallback(IntPtr instance, CppFocusOutCallback callback);

    /// <summary>
    ///     Sets the callback invoked when the native window is moved.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The moved delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMovedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMovedCallback(IntPtr instance, CppMovedCallback callback);

    /// <summary>
    ///     Sets the callback invoked when the native window is resized.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The resized delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetResizedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetResizedCallback(IntPtr instance, CppResizedCallback callback);

    /// <summary>
    ///     Sets the callback invoked when files are dropped onto the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The file dropped delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFileDroppedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFileDroppedCallback(IntPtr instance, CppFileDroppedCallback callback);

    /// <summary>
    ///     Enables or disables file drag-and-drop on the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether to enable drag and drop.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetDragDropEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetDragDropEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);
}
