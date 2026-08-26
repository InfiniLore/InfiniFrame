// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Releases all storage owned by one custom-scheme response.</summary>
/// <remarks>The native consumer must invoke this exactly once when <c>OwnerContext</c> is non-zero.</remarks>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CppReleaseCustomSchemeResponseDelegate(IntPtr ownerContext);
