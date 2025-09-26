// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class PlatformUtilities {
    
    /// <summary>
    ///     Indicates whether the current platform is Windows.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is Windows; otherwise, <c>false</c>.
    /// </value>
    public static bool IsWindowsPlatform { get; } = OperatingSystem.IsWindows();

    /// <summary>
    ///     Indicates whether the current platform is macOS.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is macOS; otherwise, <c>false</c>.
    /// </value>
    public static bool IsMacOsPlatform { get; } = OperatingSystem.IsMacOS();
    
    /// <summary>
    ///     Indicates whether the current platform is Linux.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is Linux; otherwise, <c>false</c>.
    /// </value>
    public static bool IsLinuxPlatform { get; } = OperatingSystem.IsLinux();
}
