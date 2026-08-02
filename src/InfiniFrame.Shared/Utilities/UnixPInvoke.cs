// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides P/Invoke declarations for Unix system library (libc) functions.
/// </summary>
internal static partial class UnixPInvoke {
    /// <summary>
    ///     Gets the real user ID of the calling process.
    /// </summary>
    /// <returns>The real user ID of the calling process.</returns>
    [LibraryImport("libc", EntryPoint = "getuid", SetLastError = true)]
    internal static partial uint GetUid();
}
