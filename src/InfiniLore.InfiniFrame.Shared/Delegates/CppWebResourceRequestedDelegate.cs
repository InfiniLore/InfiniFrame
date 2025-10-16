using System.Runtime.InteropServices;

namespace InfiniLore.InfiniFrame;
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate IntPtr CppWebResourceRequestedDelegate(string url, out int outNumBytes, out string? outContentType);
