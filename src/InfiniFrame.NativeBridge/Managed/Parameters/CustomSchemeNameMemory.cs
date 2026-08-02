// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Manages the allocation and deallocation of native memory for custom URL scheme names.
/// </summary>
internal static class CustomSchemeNameMemory {
    /// <summary>
    ///     The maximum number of custom scheme names that can be registered.
    /// </summary>
    internal const int MaxCustomSchemeNames = 16;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Allocates a fixed-size array of native pointers (HGlobal-allocated ANSI strings) from a sequence of scheme names.
    /// </summary>
    /// <param name="names">The scheme name strings to allocate.</param>
    /// <returns>An array of native pointers sized <see cref="MaxCustomSchemeNames"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than <see cref="MaxCustomSchemeNames"/> names are provided.</exception>
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

    /// <summary>
    ///     Frees all non-zero native pointers in the array and zeroes them out.
    /// </summary>
    /// <param name="pointers">The array of native pointers to free.</param>
    internal static void FreeAll(IntPtr[]? pointers) {
        if (pointers is null) return;

        for (int i = 0; i < pointers.Length; i++) {
            if (pointers[i] == IntPtr.Zero) continue;

            Marshal.FreeHGlobal(pointers[i]);
            pointers[i] = IntPtr.Zero;
        }
    }
}