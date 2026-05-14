// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class CustomSchemeNameMemory {
    internal const int MaxCustomSchemeNames = 16;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal static IntPtr[] Allocate(IEnumerable<string> names) {
        IntPtr[] pointers = new IntPtr[MaxCustomSchemeNames];
        int index = 0;

        try {
            foreach (string name in names) {
                if (index >= MaxCustomSchemeNames) {
                    throw new InvalidOperationException("Maximum number of custom schemes is 16.");
                }

                pointers[index] = Marshal.StringToHGlobalAnsi(name);
                index++;
            }

            return pointers;
        }
        catch {
            FreeAll(pointers);
            throw;
        }
    }

    internal static void FreeAll(IntPtr[]? pointers) {
        if (pointers is null) return;

        for (int i = 0; i < pointers.Length; i++) {
            if (pointers[i] == IntPtr.Zero) continue;

            Marshal.FreeHGlobal(pointers[i]);
            pointers[i] = IntPtr.Zero;
        }
    }
}
