using System.Runtime.InteropServices;

namespace InfiniLore.InfiniFrame;
//These are sent in during the request
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void InvokeCallback();
