// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     Browser settings mapped from the window builder. A provider must apply each value or reject the window with
///     <see cref="NotSupportedException" />; it must never weaken a security-relevant value silently.
/// </summary>
public sealed record InfiniFrameHostBrowserPolicy {
    /// <summary>Same-origin and related web security checks are enforced.</summary>
    public required bool WebSecurity { get; init; }

    /// <summary>Local developer tools may be opened.</summary>
    public required bool DevTools { get; init; }

    /// <summary>The default browser context menu is available.</summary>
    public required bool ContextMenu { get; init; }

    /// <summary>Browser permission requests are granted without prompting.</summary>
    public required bool GrantBrowserPermissions { get; init; }

    /// <summary>Page script may access the clipboard.</summary>
    public required bool JavaScriptClipboardAccess { get; init; }

    /// <summary>Camera and microphone media streams are available.</summary>
    public required bool MediaStream { get; init; }

    /// <summary>Media may autoplay.</summary>
    public required bool MediaAutoplay { get; init; }

    /// <summary>The File System Access API is available.</summary>
    public required bool FileSystemAccess { get; init; }

    /// <summary>TLS certificate errors are ignored.</summary>
    public required bool IgnoreCertificateErrors { get; init; }

    /// <summary>A replacement user agent, or <see langword="null" /> for the engine default.</summary>
    public string? UserAgent { get; init; }
}
