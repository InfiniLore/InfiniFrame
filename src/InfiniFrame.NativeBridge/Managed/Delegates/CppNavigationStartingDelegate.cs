// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Delegate for native navigation starting callback.
/// </summary>
/// <param name="url">The target URL of the navigation.</param>
/// <param name="isUserInitiated">Non-zero if the user initiated the navigation.</param>
/// <param name="isRedirect">Non-zero if the navigation is the result of a redirect.</param>
/// <param name="isMainFrame">Non-zero if the navigation is in the main frame.</param>
/// <returns>0 to allow, 1 to cancel.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate byte CppNavigationStartingDelegate(
    [MarshalAs(UnmanagedType.LPUTF8Str)] string url,
    int isUserInitiated,
    int isRedirect,
    int isMainFrame
);
