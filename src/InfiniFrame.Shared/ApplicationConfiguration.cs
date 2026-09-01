// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configuration for an InfiniFrame application.
/// </summary>
public class ApplicationConfiguration {
    /// <summary>
    ///     WINDOWS ONLY: The Win32 HINSTANCE handle. Required on Windows.
    /// </summary>
    public IntPtr HInstance { get; set; }

    /// <summary>
    ///     WINDOWS ONLY: Explicit application identity used by the taskbar for grouping and pinning.
    /// </summary>
    public string? WindowsAppUserModelId { get; set; }

    /// <summary>
    ///     WINDOWS ONLY: Registers the application for toast notifications.
    /// </summary>
    public string? NotificationRegistrationId { get; set; }

    /// <summary>
    ///     WINDOWS ONLY: Path to an extracted fixed-version WebView2 runtime.
    /// </summary>
    public string? WebView2RuntimePath { get; set; }
}
