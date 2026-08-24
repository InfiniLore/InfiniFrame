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
    ///     Centers the native window on the screen.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Center", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Center(IntPtr instance);

    /// <summary>
    ///     Restores the native window from a maximized or minimized state.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Restore", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Restore(IntPtr instance);

    /// <summary>
    ///     Clears the browser auto-fill data for the native instance.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ClearBrowserAutoFill", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ClearBrowserAutoFill(IntPtr instance);

    /// <summary>
    ///     Sets focus to the native window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetFocused", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFocused(IntPtr instance);

    /// <summary>
    ///     Shows a native toast notification.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="body">The notification body text.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowNotification", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ShowNotification(IntPtr instance, string title, string body);

    /// <summary>
    ///     Shows a rich native toast notification with extended options.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="body">The notification body text.</param>
    /// <param name="iconPath">Optional path to an image file, or empty string for none.</param>
    /// <param name="urgency">Urgency level (maps to platform-specific importance).</param>
    /// <param name="tag">Optional tag for grouping/replacing notifications, or empty string for none.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowNotificationWithOptions", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ShowNotificationWithOptions(
        IntPtr instance,
        string title,
        string body,
        string iconPath,
        int urgency,
        string tag
    );

    /// <summary>
    ///     Shows a rich native toast notification with extended options and an activation callback.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="operationId">Unique identifier for this notification operation.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="body">The notification body text.</param>
    /// <param name="iconPath">Optional path to an image file, or empty string for none.</param>
    /// <param name="urgency">Urgency level (maps to platform-specific importance).</param>
    /// <param name="tag">Optional tag for grouping/replacing notifications, or empty string for none.</param>
    /// <param name="completion">Callback invoked when the notification is activated or dismissed.</param>
    /// <param name="completionContext">Opaque context pointer passed to the completion callback.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginShowNotification", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginShowNotification(
        IntPtr instance,
        ulong operationId,
        string title,
        string body,
        string iconPath,
        int urgency,
        string tag,
        OperationCompletedCallback completion,
        IntPtr completionContext
    );

    /// <summary>
    ///     Cancels a pending notification operation.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="operationId">The operation identifier of the notification to cancel.</param>
    /// <param name="canceled">Output: true if the notification was successfully canceled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_CancelNotification", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus CancelNotification(
        IntPtr instance,
        ulong operationId,
        [MarshalAs(UnmanagedType.I1)] out bool canceled
    );
}
