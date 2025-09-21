using System.Runtime.InteropServices;

namespace InfiniLore.Photino;
//These are sent in during the request
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate int CppGetAllMonitorsDelegate(in NativeMonitor monitor);
