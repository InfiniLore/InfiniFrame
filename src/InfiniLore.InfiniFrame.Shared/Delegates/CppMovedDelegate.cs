using System.Runtime.InteropServices;

namespace InfiniLore.InfiniFrame;
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate void CppMovedDelegate(int x, int y);
