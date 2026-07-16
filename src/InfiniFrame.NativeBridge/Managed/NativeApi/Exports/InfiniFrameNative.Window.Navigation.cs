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
    ///     Navigates the browser control to an HTML string.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="content">The HTML content to render.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_NavigateToString", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus NavigateToString(IntPtr instance, string content);

    /// <summary>
    ///     Navigates the browser control to a URL.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="url">The URL to navigate to.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_NavigateToUrl", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus NavigateToUrl(IntPtr instance, string url);

    /// <summary>
    ///     Sends a web message to the browser control's JavaScript context.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="message">The message string to send.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SendWebMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SendWebMessage(IntPtr instance, string message);
}
