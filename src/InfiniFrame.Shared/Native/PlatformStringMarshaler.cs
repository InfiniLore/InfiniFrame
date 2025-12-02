// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using System.Runtime.InteropServices;

namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Custom marshaler that handles platform-specific string encoding.
/// Windows: wchar_t* (UTF-16)
/// Linux/macOS: char* (UTF-8)
/// </summary>
public class PlatformStringMarshaler : ICustomMarshaler {
    private static PlatformStringMarshaler? _instance;

    [UsedImplicitly] 
    public static ICustomMarshaler GetInstance(string _)
        => _instance ??= new PlatformStringMarshaler();

    public object MarshalNativeToManaged(IntPtr pNativeData) {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (pNativeData == IntPtr.Zero) return null!;
        
        #if WINDOWS
        return Marshal.PtrToStringUni(pNativeData)!;
        #else
        return Marshal.PtrToStringUTF8(pNativeData)!;
        #endif
    }

    public IntPtr MarshalManagedToNative(object? managedObj) {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (managedObj is not string str) return IntPtr.Zero;

        #if WINDOWS
        return Marshal.StringToHGlobalUni(str);
        #else
        return Marshal.StringToHGlobalAnsi(str);
        #endif
    }

    public void CleanUpNativeData(IntPtr pNativeData) {
        if (pNativeData != IntPtr.Zero) {
            Marshal.FreeHGlobal(pNativeData);
        }
    }

    public void CleanUpManagedData(object managedObj) {
        // Nothing to clean up
    }

    public int GetNativeDataSize() => IntPtr.Size;
}
