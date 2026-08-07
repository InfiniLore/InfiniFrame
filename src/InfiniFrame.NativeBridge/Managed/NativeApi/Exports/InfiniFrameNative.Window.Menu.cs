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
    ///     Sets the menu bar for the window, replacing any existing menu.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="menuBarJson">JSON-serialized menu bar, or <c>null</c> to clear the menu.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMenuBar", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMenuBar(IntPtr instance, string? menuBarJson);

    /// <summary>
    ///     Enables or disables a menu item by its identifier.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="menuItemId">The identifier of the menu item to update.</param>
    /// <param name="enabled">Whether the item is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMenuItemEnabled", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMenuItemEnabled(IntPtr instance, string? menuItemId, [MarshalAs(UnmanagedType.I1)] bool enabled);

    /// <summary>
    ///     Shows or hides a menu item by its identifier.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="menuItemId">The identifier of the menu item to update.</param>
    /// <param name="visible">Whether the item is visible.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetMenuItemVisible", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMenuItemVisible(IntPtr instance, string? menuItemId, [MarshalAs(UnmanagedType.I1)] bool visible);

    /// <summary>
    ///     Simulates a click on a menu item by its identifier.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="menuItemId">The identifier of the menu item to click.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ClickMenuItem", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ClickMenuItem(IntPtr instance, string? menuItemId);
}
