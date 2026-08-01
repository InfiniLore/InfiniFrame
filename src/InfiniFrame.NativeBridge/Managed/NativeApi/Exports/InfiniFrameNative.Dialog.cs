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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FileDialogCompletedCallback(
        IntPtr context,
        ulong operationId,
        int result,
        int valueCount,
        IntPtr values
    );
    /// <summary>
    ///     Shows an open-file dialog via native code.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowOpenFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ShowOpenFilePtr(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filtersCount, out int resultCount, out IntPtr values);
    /// <summary>
    ///     Shows an open-file dialog and returns the selected file paths.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
    /// <param name="filters">File filter strings.</param>
    /// <param name="filtersCount">The number of filters.</param>
    /// <param name="values">The selected file paths.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus ShowOpenFile(IntPtr instance, string title, string defaultPath, bool multiSelect, string[] filters, int filtersCount, out string?[] values) {
        InfiniFrameNativeInteropStatus status = ShowOpenFilePtr(instance, title, defaultPath, multiSelect, filters, filtersCount, out int resultCount, out IntPtr ptrValues);
        values = PtrToNativeStringArray(ptrValues, resultCount);
        return status;
    }

    /// <summary>
    ///     Shows an open-folder dialog via native code.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowOpenFolder", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ShowOpenFolderPtr(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, out int resultCount, out IntPtr values);
    /// <summary>
    ///     Shows an open-folder dialog and returns the selected folder paths.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
    /// <param name="values">The selected folder paths.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus ShowOpenFolder(IntPtr instance, string title, string defaultPath, bool multiSelect, out string?[] values) {
        InfiniFrameNativeInteropStatus status = ShowOpenFolderPtr(instance, title, defaultPath, multiSelect, out int resultCount, out IntPtr ptrValues);
        values = PtrToNativeStringArray(ptrValues, resultCount);
        return status;
    }

    /// <summary>
    ///     Shows a save-file dialog via native code.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowSaveFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ShowSaveFilePtr(IntPtr inst, string title, string defaultPath, string[] filters, int filtersCount, string? defaultFileName, out IntPtr value);
    /// <summary>
    ///     Shows a save-file dialog and returns the selected file path.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="filters">File filter strings.</param>
    /// <param name="filtersCount">The number of filters.</param>
    /// <param name="defaultFileName">The default file name.</param>
    /// <param name="value">The selected file path.</param>
    /// <returns>A status code indicating success or failure.</returns>
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

    /// <summary>
    ///     Shows a message dialog via native code.
    /// </summary>
    /// <param name="inst">The native window instance handle.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="text">The dialog message text.</param>
    /// <param name="buttons">The buttons to display.</param>
    /// <param name="icon">The icon to display.</param>
    /// <param name="value">The dialog result.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ShowMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ShowMessage(IntPtr inst, string title, string text, InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon, out InfiniFrameDialogResult value);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginShowOpenFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginShowOpenFile(
        IntPtr instance, ulong operationId, string title, string defaultPath,
        [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filterCount,
        FileDialogCompletedCallback completion, IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginShowOpenFolder", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginShowOpenFolder(
        IntPtr instance, ulong operationId, string title, string defaultPath,
        [MarshalAs(UnmanagedType.I1)] bool multiSelect,
        FileDialogCompletedCallback completion, IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginShowSaveFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginShowSaveFile(
        IntPtr instance, ulong operationId, string title, string defaultPath,
        string[] filters, int filterCount, string defaultFileName,
        FileDialogCompletedCallback completion, IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginShowMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginShowMessage(
        IntPtr instance, ulong operationId, string title, string text,
        InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon,
        OperationCompletedCallback completion, IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_CancelDialog", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus CancelDialog(
        IntPtr instance, ulong operationId, [MarshalAs(UnmanagedType.I1)] out bool cancelled
    );

    /// <summary>
    ///     Converts a native pointer to an array of native string pointers into a managed string array.
    /// </summary>
    /// <param name="valuesPtr">The pointer to the native string array.</param>
    /// <param name="count">The number of strings in the array.</param>
    /// <returns>A managed array of strings.</returns>
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
