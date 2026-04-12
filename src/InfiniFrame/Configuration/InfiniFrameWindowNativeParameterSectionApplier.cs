// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration;

namespace InfiniFrame.Configuration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InfiniFrameWindowNativeParameterSectionApplier {
    /// <summary>
    /// This service exists to be AOT and Trimming compatible, as it uses reflection to apply configuration settings otherwise.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="configuration"></param>
    public static void Apply(IConfigurationSection section, IInfiniFrameWindowNativeParameterBuilder configuration) {
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.BrowserControlInitParameters), value => configuration.BrowserControlInitParameters = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Centered), value => configuration.Centered = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Chromeless), value => configuration.Chromeless = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.ContextMenuEnabled), value => configuration.ContextMenuEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.DevToolsEnabled), value => configuration.DevToolsEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.FileSystemAccessEnabled), value => configuration.FileSystemAccessEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.FullScreen), value => configuration.FullScreen = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.GrantBrowserPermissions), value => configuration.GrantBrowserPermissions = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Height), value => configuration.Height = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.IconFilePath), value => configuration.IconFilePath = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.IgnoreCertificateErrorsEnabled), value => configuration.IgnoreCertificateErrorsEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.JavascriptClipboardAccessEnabled), value => configuration.JavascriptClipboardAccessEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Left), value => configuration.Left = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MaxHeight), value => configuration.MaxHeight = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MaxWidth), value => configuration.MaxWidth = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Maximized), value => configuration.Maximized = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.MediaAutoplayEnabled), value => configuration.MediaAutoplayEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.MediaStreamEnabled), value => configuration.MediaStreamEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MinHeight), value => configuration.MinHeight = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MinWidth), value => configuration.MinWidth = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Minimized), value => configuration.Minimized = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.NotificationsEnabled), value => configuration.NotificationsEnabled = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.NotificationRegistrationId), value => configuration.NotificationRegistrationId = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Resizable), value => configuration.Resizable = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.SmoothScrollingEnabled), value => configuration.SmoothScrollingEnabled = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.StartString), value => configuration.StartString = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.StartUrl), value => configuration.StartUrl = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.TemporaryFilesPath), value => configuration.TemporaryFilesPath = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.Title), value => configuration.Title = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Top), value => configuration.Top = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.TopMost), value => configuration.TopMost = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Transparent), value => configuration.Transparent = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.UseOsDefaultLocation), value => configuration.UseOsDefaultLocation = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.UseOsDefaultSize), value => configuration.UseOsDefaultSize = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.UserAgent), value => configuration.UserAgent = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.WebSecurityEnabled), value => configuration.WebSecurityEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Width), value => configuration.Width = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Zoom), value => configuration.Zoom = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.ZoomEnabled), value => configuration.ZoomEnabled = value);

        IConfigurationSection customSchemeNames = section.GetSection(nameof(InfiniFrameWindowNativeParameterBuilder.CustomSchemeNames));
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
