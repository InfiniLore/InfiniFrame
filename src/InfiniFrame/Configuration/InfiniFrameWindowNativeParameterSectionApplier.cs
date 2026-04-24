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
    ///     This service exists to be AOT and Trimming compatible, as it uses reflection to apply configuration settings
    ///     otherwise.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="configuration"></param>
    public static void Apply(IConfigurationSection section, IInfiniFrameWindowNativeParameterBuilder configuration) {
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.BrowserControlInitParameters), assign: value => configuration.BrowserControlInitParameters = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Centered), assign: value => configuration.Centered = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Chromeless), assign: value => configuration.Chromeless = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.ContextMenuEnabled), assign: value => configuration.ContextMenuEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.DevToolsEnabled), assign: value => configuration.DevToolsEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.FileSystemAccessEnabled), assign: value => configuration.FileSystemAccessEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.FullScreen), assign: value => configuration.FullScreen = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.GrantBrowserPermissions), assign: value => configuration.GrantBrowserPermissions = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Height), assign: value => configuration.Height = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.IconFilePath), assign: value => configuration.IconFilePath = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.IgnoreCertificateErrorsEnabled), assign: value => configuration.IgnoreCertificateErrorsEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.JavascriptClipboardAccessEnabled), assign: value => configuration.JavascriptClipboardAccessEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Left), assign: value => configuration.Left = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MaxHeight), assign: value => configuration.MaxHeight = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MaxWidth), assign: value => configuration.MaxWidth = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Maximized), assign: value => configuration.Maximized = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.MediaAutoplayEnabled), assign: value => configuration.MediaAutoplayEnabled = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.MediaStreamEnabled), assign: value => configuration.MediaStreamEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MinHeight), assign: value => configuration.MinHeight = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.MinWidth), assign: value => configuration.MinWidth = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Minimized), assign: value => configuration.Minimized = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.NotificationsEnabled), assign: value => configuration.NotificationsEnabled = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.NotificationRegistrationId), assign: value => configuration.NotificationRegistrationId = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Resizable), assign: value => configuration.Resizable = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.SmoothScrollingEnabled), assign: value => configuration.SmoothScrollingEnabled = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.StartString), assign: value => configuration.StartString = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.StartUrl), assign: value => configuration.StartUrl = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.TemporaryFilesPath), assign: value => configuration.TemporaryFilesPath = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.Title), assign: value => configuration.Title = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Top), assign: value => configuration.Top = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.TopMost), assign: value => configuration.TopMost = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.Transparent), assign: value => configuration.Transparent = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.UseOsDefaultLocation), assign: value => configuration.UseOsDefaultLocation = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.UseOsDefaultSize), assign: value => configuration.UseOsDefaultSize = value);
        SetString(section, nameof(InfiniFrameWindowNativeParameterBuilder.UserAgent), assign: value => configuration.UserAgent = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.WebSecurityEnabled), assign: value => configuration.WebSecurityEnabled = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Width), assign: value => configuration.Width = value);
        SetInt(section, nameof(InfiniFrameWindowNativeParameterBuilder.Zoom), assign: value => configuration.Zoom = value);
        SetBool(section, nameof(InfiniFrameWindowNativeParameterBuilder.ZoomEnabled), assign: value => configuration.ZoomEnabled = value);

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
