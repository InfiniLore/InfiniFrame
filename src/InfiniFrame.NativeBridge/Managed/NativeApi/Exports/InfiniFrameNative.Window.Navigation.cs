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

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginNavigateToString", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginNavigateToString(
        IntPtr instance,
        ulong operationId,
        string content,
        OperationCompletedCallback completion,
        IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginNavigateToUrl", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginNavigateToUrl(
        IntPtr instance,
        ulong operationId,
        string url,
        OperationCompletedCallback completion,
        IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_CancelNavigation", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus CancelNavigation(IntPtr instance, ulong operationId);

    /// <summary>
    ///     Retrieves the current page URL string pointer from the native layer.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetCurrentUrl", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetCurrentUrlPtr(IntPtr instance, out IntPtr value);
    /// <summary>
    ///     Gets the current page URL, or null if no URL is available.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="url">The current page URL.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus GetCurrentUrl(IntPtr instance, out string? url) {
        InfiniFrameNativeInteropStatus status = GetCurrentUrlPtr(instance, out IntPtr ptr);
        try {
            url = PtrToNativeString(ptr);
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }
}