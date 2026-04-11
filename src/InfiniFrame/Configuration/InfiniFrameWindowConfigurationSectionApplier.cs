// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration;

namespace InfiniFrame.Configuration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InfiniFrameWindowConfigurationSectionApplier {
    /// <summary>
    /// This service exists to be AOT and Trimming compatible, as it uses reflection to apply configuration settings otherwise.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="configuration"></param>
    public static void Apply(IConfigurationSection section, IInfiniFrameWindowConfiguration configuration) {
        SetString(section, nameof(InfiniFrameWindowConfiguration.BrowserControlInitParameters), value => configuration.BrowserControlInitParameters = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.Centered), value => configuration.Centered = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.Chromeless), value => configuration.Chromeless = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.ContextMenuEnabled), value => configuration.ContextMenuEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.DevToolsEnabled), value => configuration.DevToolsEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.FileSystemAccessEnabled), value => configuration.FileSystemAccessEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.FullScreen), value => configuration.FullScreen = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.GrantBrowserPermissions), value => configuration.GrantBrowserPermissions = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.Height), value => configuration.Height = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.IconFilePath), value => configuration.IconFilePath = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.IgnoreCertificateErrorsEnabled), value => configuration.IgnoreCertificateErrorsEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.JavascriptClipboardAccessEnabled), value => configuration.JavascriptClipboardAccessEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.Left), value => configuration.Left = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.MaxHeight), value => configuration.MaxHeight = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.MaxWidth), value => configuration.MaxWidth = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.Maximized), value => configuration.Maximized = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.MediaAutoplayEnabled), value => configuration.MediaAutoplayEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.MediaStreamEnabled), value => configuration.MediaStreamEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.MinHeight), value => configuration.MinHeight = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.MinWidth), value => configuration.MinWidth = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.Minimized), value => configuration.Minimized = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.NotificationsEnabled), value => configuration.NotificationsEnabled = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.NotificationRegistrationId), value => configuration.NotificationRegistrationId = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.Resizable), value => configuration.Resizable = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.SmoothScrollingEnabled), value => configuration.SmoothScrollingEnabled = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.StartString), value => configuration.StartString = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.StartUrl), value => configuration.StartUrl = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.TemporaryFilesPath), value => configuration.TemporaryFilesPath = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.Title), value => configuration.Title = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.Top), value => configuration.Top = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.TopMost), value => configuration.TopMost = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.Transparent), value => configuration.Transparent = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.UseOsDefaultLocation), value => configuration.UseOsDefaultLocation = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.UseOsDefaultSize), value => configuration.UseOsDefaultSize = value);
        SetString(section, nameof(InfiniFrameWindowConfiguration.UserAgent), value => configuration.UserAgent = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.WebSecurityEnabled), value => configuration.WebSecurityEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.Width), value => configuration.Width = value);
        SetInt(section, nameof(InfiniFrameWindowConfiguration.Zoom), value => configuration.Zoom = value);
        SetBool(section, nameof(InfiniFrameWindowConfiguration.ZoomEnabled), value => configuration.ZoomEnabled = value);

        IConfigurationSection customSchemeNames = section.GetSection(nameof(InfiniFrameWindowConfiguration.CustomSchemeNames));
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
