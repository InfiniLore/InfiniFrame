// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Delegates;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    internal static partial InfiniFrameNativeInteropStatus SetClosingCallback(IntPtr instance, CppClosingDelegate callback);

    /// <summary>
    ///     Sets the callback invoked when the native window has been closed.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The closed delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_setClosedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetClosedCallback(IntPtr instance, CppClosedDelegate callback);

    /// <summary>
    ///     Sets the callback invoked when the native window receives focus.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The focus-in delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFocusInCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFocusInCallback(IntPtr instance, CppFocusInDelegate callback);

    /// <summary>
    ///     Sets the callback invoked when the native window loses focus.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The focus-out delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFocusOutCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFocusOutCallback(IntPtr instance, CppFocusOutDelegate callback);

    /// <summary>
    ///     Sets the callback invoked when the native window is moved.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The moved delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMovedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMovedCallback(IntPtr instance, CppMovedDelegate callback);

    /// <summary>
    ///     Sets the callback invoked when the native window is resized.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The resized delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetResizedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetResizedCallback(IntPtr instance, CppResizedDelegate callback);

    /// <summary>
    ///     Sets the callback invoked when files are dropped onto the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The file dropped delegate.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFileDroppedCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFileDroppedCallback(IntPtr instance, CppFileDroppedDelegate callback);

    /// <summary>
    ///     Enables or disables file drag-and-drop on the window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether to enable drag and drop.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetDragDropEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetDragDropEnabled(IntPtr instance, [MarshalAs(UnmanagedType.U1)] bool enabled);
}