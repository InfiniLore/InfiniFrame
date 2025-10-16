using System.Runtime.InteropServices;

namespace InfiniLore.Photino;
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate void CppWebMessageReceivedDelegate(string message);
