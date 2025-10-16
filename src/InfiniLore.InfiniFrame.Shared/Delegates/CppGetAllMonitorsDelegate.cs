using System.Runtime.InteropServices;

namespace InfiniLore.InfiniFrame;
//These are sent in during the request
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate int CppGetAllMonitorsDelegate(in NativeMonitor monitor);
