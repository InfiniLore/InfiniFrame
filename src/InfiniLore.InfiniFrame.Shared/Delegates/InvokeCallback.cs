using System.Runtime.InteropServices;

namespace InfiniLore.Photino;
//These are sent in during the request
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void InvokeCallback();
