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
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_AddCustomSchemeName", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus AddCustomSchemeName(IntPtr instance, string scheme);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetAllMonitors", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetAllMonitors(IntPtr instance, CppGetAllMonitorsDelegate callback);

    // InfiniFrame_SetClosingCallback
    // InfiniFrame_setClosedCallback
    // InfiniFrame_SetFocusInCallback
    // InfiniFrame_SetFocusOutCallback
    // InfiniFrame_SetMovedCallback
    // InfiniFrame_SetResizedCallback
    
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_Invoke", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Invoke(IntPtr instance, Action callback);
}
