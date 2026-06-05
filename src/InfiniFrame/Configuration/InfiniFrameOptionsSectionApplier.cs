// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using Microsoft.Extensions.Configuration;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InfiniFrameOptionsSectionApplier {
    /// <summary>
    ///     This service exists to be AOT and Trimming compatible, as it uses reflection to apply configuration settings
    ///     otherwise.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="configuration"></param>
    /// <param name="debug"></param>
    public static void Apply(IConfigurationSection section, IInfiniFrameOptionsBuilder configuration, IInfiniFrameWindowDebugBuilder debug) {
        SetString(section, nameof(InfiniFrameOptionsBuilder.BrowserControlInitParameters), assign: value => configuration.BrowserControlInitParameters = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.Centered), assign: value => configuration.Centered = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.Chromeless), assign: value => configuration.Chromeless = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.ContextMenuEnabled), assign: value => configuration.ContextMenuEnabled = value);
        SetBool(section, nameof(IInfiniFrameWindowDebugBuilder.DevToolsEnabled), assign: value => debug.SetDevToolsEnabled(value));
        SetBool(section, nameof(IInfiniFrameWindowDebugBuilder.WebInspectorEnabled), assign: value => debug.SetWebInspectorEnabled(value));
        SetBool(section, nameof(InfiniFrameOptionsBuilder.FileSystemAccessEnabled), assign: value => configuration.FileSystemAccessEnabled = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.FullScreen), assign: value => configuration.FullScreen = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.GrantBrowserPermissions), assign: value => configuration.GrantBrowserPermissions = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.Height), assign: value => configuration.Height = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.IconFilePath), assign: value => configuration.IconFilePath = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.IgnoreCertificateErrorsEnabled), assign: value => configuration.IgnoreCertificateErrorsEnabled = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.JavascriptClipboardAccessEnabled), assign: value => configuration.JavascriptClipboardAccessEnabled = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.Left), assign: value => configuration.Left = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.MaxHeight), assign: value => configuration.MaxHeight = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.MaxWidth), assign: value => configuration.MaxWidth = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.Maximized), assign: value => configuration.Maximized = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.MediaAutoplayEnabled), assign: value => configuration.MediaAutoplayEnabled = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.MediaStreamEnabled), assign: value => configuration.MediaStreamEnabled = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.MinHeight), assign: value => configuration.MinHeight = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.MinWidth), assign: value => configuration.MinWidth = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.Minimized), assign: value => configuration.Minimized = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.NotificationsEnabled), assign: value => configuration.NotificationsEnabled = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.NotificationRegistrationId), assign: value => configuration.NotificationRegistrationId = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.Resizable), assign: value => configuration.Resizable = value);
        SetInt(section, nameof(IInfiniFrameWindowDebugBuilder.RemoteDebuggingPort), assign: value => debug.SetRemoteDebuggingPort(value));
        SetBool(section, nameof(InfiniFrameOptionsBuilder.SmoothScrollingEnabled), assign: value => configuration.SmoothScrollingEnabled = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.StartString), assign: value => configuration.StartString = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.StartUrl), assign: value => configuration.StartUrl = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.TemporaryFilesPath), assign: value => configuration.TemporaryFilesPath = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.Title), assign: value => configuration.Title = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.Top), assign: value => configuration.Top = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.TopMost), assign: value => configuration.TopMost = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.Transparent), assign: value => configuration.Transparent = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.UseOsDefaultLocation), assign: value => configuration.UseOsDefaultLocation = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.UseOsDefaultSize), assign: value => configuration.UseOsDefaultSize = value);
        SetString(section, nameof(InfiniFrameOptionsBuilder.UserAgent), assign: value => configuration.UserAgent = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.WebSecurityEnabled), assign: value => configuration.WebSecurityEnabled = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.Width), assign: value => configuration.Width = value);
        SetInt(section, nameof(InfiniFrameOptionsBuilder.Zoom), assign: value => configuration.Zoom = value);
        SetBool(section, nameof(InfiniFrameOptionsBuilder.ZoomEnabled), assign: value => configuration.ZoomEnabled = value);

        IConfigurationSection customSchemeNames = section.GetSection(nameof(InfiniFrameOptionsBuilder.CustomSchemeNames));
        if (customSchemeNames.Exists()) {
            configuration.CustomSchemeNames = customSchemeNames
                .GetChildren()
                .Select(static child => child.Value)
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .Select(static value => value!)
                .ToList();
        }
    }

    private static void SetString(IConfigurationSection section, string key, Action<string> assign) {
        string? value = section[key];
        if (!string.IsNullOrEmpty(value)) assign(value);
    }

    private static void SetBool(IConfigurationSection section, string key, Action<bool> assign) {
        string? value = section[key];
        if (bool.TryParse(value, out bool parsed)) assign(parsed);
    }

    private static void SetInt(IConfigurationSection section, string key, Action<int> assign) {
        string? value = section[key];
        if (int.TryParse(value, out int parsed)) assign(parsed);
    }
}
