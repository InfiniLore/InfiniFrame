// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for working with the macOS web inspector.
/// </summary>
internal static class MacOsWebInspectorUtility {
    /// <summary>
    ///     Determines whether the web inspector is supported on the current platform.
    /// </summary>
    /// <returns><c>true</c> if the platform is macOS 13.3 or later; otherwise, <c>false</c>.</returns>
    public static bool IsSupportedPlatform()
        => OperatingSystem.IsMacOSVersionAtLeast(13, 3);

    /// <summary>
    ///     Throws a <see cref="PlatformNotSupportedException" /> if the web inspector is not supported on the current
    ///     platform.
    /// </summary>
    public static void ThrowIfUnsupported() {
        if (IsSupportedPlatform()) return;

        throw new PlatformNotSupportedException(
            "Web inspector mode is only supported on macOS 13.3+ in InfiniFrame."
        );
    }
}
