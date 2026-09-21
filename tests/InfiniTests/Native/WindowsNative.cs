// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniTests.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class WindowsNative {
    [LibraryImport("shell32.dll", EntryPoint = "GetCurrentProcessExplicitAppUserModelID")]
    public static partial int GetCurrentProcessAppUserModelId(out IntPtr appUserModelId);

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    public static partial IntPtr SendWindowMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll", EntryPoint = "GetClassLongPtrW", SetLastError = true)]
    public static partial IntPtr GetWindowClassLongPointer(IntPtr hWnd, int nIndex);

    [LibraryImport("user32.dll", EntryPoint = "GetWindow", SetLastError = true)]
    public static partial IntPtr GetRelatedWindow(IntPtr hWnd, uint uCmd);
}
