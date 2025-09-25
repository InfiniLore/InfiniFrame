// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.Utilities;
using System.Runtime.InteropServices;

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
    public static bool IsWindowsPlatform { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    ///     Indicates whether the current platform is macOS.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is macOS; otherwise, <c>false</c>.
    /// </value>
    public static bool IsMacOsPlatform { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    /// <summary>
    ///     Indicates the version of macOS
    /// </summary>
    public static Version? MacOsVersion { get; } = IsMacOsPlatform && Version.TryParse(RuntimeInformation.OSDescription.Split(' ').ElementAtOrDefault(1), out Version? result) ? result : null;

    /// <summary>
    ///     Indicates whether the current platform is Linux.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the current platform is Linux; otherwise, <c>false</c>.
    /// </value>
    public static bool IsLinuxPlatform { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}
