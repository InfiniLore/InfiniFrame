// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class WebInspectorUtility {
    public static bool IsSupportedPlatform() 
        => OperatingSystem.IsMacOS()
            && OperatingSystem.IsMacOSVersionAtLeast(13, 3);

    public static void ThrowIfUnsupported() {
        if (IsSupportedPlatform()) return;

        throw new PlatformNotSupportedException(
            "Web inspector mode is only supported on macOS 13.3+ in InfiniFrame."
        );
    }
}
