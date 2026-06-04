// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowOpenFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ShowOpenFilePtr(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filtersCount, out int resultCount, out IntPtr values);
    internal static InfiniFrameNativeInteropStatus ShowOpenFile(IntPtr instance, string title, string defaultPath, bool multiSelect, string[] filters, int filtersCount, out string?[] values) {
        InfiniFrameNativeInteropStatus status = ShowOpenFilePtr(instance, title, defaultPath, multiSelect, filters, filtersCount, out int resultCount, out IntPtr ptrValues);
        values = PtrToNativeStringArray(ptrValues, resultCount);
        return status;
    }

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowOpenFolder", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ShowOpenFolderPtr(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, out int resultCount, out IntPtr values);
    internal static InfiniFrameNativeInteropStatus ShowOpenFolder(IntPtr instance, string title, string defaultPath, bool multiSelect, out string?[] values) {
        InfiniFrameNativeInteropStatus status = ShowOpenFolderPtr(instance, title, defaultPath, multiSelect, out int resultCount, out IntPtr ptrValues);
        values = PtrToNativeStringArray(ptrValues, resultCount);
        return status;
    }

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowSaveFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ShowSaveFilePtr(IntPtr inst, string title, string defaultPath, string[] filters, int filtersCount, string? defaultFileName, out IntPtr value);
    internal static InfiniFrameNativeInteropStatus ShowSaveFile(IntPtr instance, string title, string defaultPath, string[] filters, int filtersCount, string? defaultFileName, out string? value) {
        InfiniFrameNativeInteropStatus status = ShowSaveFilePtr(instance, title, defaultPath, filters, filtersCount, defaultFileName, out IntPtr ptrValue);
        try {
            value = PtrToNativeString(ptrValue);
        }
        finally {
            if (ptrValue != IntPtr.Zero) {
                FreeString(ptrValue);
            }
        }

        return status;
    }

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ShowMessage(IntPtr inst, string title, string text, InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon, out InfiniFrameDialogResult value);

    private static string?[] PtrToNativeStringArray(IntPtr valuesPtr, int count) {
        if (valuesPtr == IntPtr.Zero || count <= 0) {
            return Array.Empty<string?>();
        }

        try {
            IntPtr[] ptrArray = new IntPtr[count];
            string?[] values = new string?[count];
            Marshal.Copy(valuesPtr, ptrArray, 0, count);
            for (int i = 0; i < count; i++) {
                values[i] = PtrToNativeString(ptrArray[i]);
            }

            return values;
        }
        finally {
            FreeStringArray(valuesPtr, count);
        }
    }
}
