// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.Native;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeDll {
    internal const string DllName = "InfiniLore.InfiniFrame.Native";

    #region Photino
    // ReSharper disable InconsistentNaming
    internal const string Photino_register_win32 = nameof(Photino_register_win32);
    internal const string Photino_register_mac = nameof(Photino_register_mac);
    internal const string Photino_ctor = nameof(Photino_ctor);
    internal const string Photino_dtor = nameof(Photino_dtor);
    internal const string Photino_AddCustomSchemeName = nameof(Photino_AddCustomSchemeName);
    internal const string Photino_Close = nameof(Photino_Close);
    internal const string Photino_getHwnd_win32 = nameof(Photino_getHwnd_win32);
    internal const string Photino_GetAllMonitors = nameof(Photino_GetAllMonitors);
    internal const string Photino_GetTransparentEnabled = nameof(Photino_GetTransparentEnabled);
    internal const string Photino_GetContextMenuEnabled = nameof(Photino_GetContextMenuEnabled);
    internal const string Photino_GetDevToolsEnabled = nameof(Photino_GetDevToolsEnabled);
    internal const string Photino_GetFullScreen = nameof(Photino_GetFullScreen);
    internal const string Photino_GetGrantBrowserPermissions = nameof(Photino_GetGrantBrowserPermissions);
    internal const string Photino_GetUserAgent = nameof(Photino_GetUserAgent);
    internal const string Photino_GetMediaAutoplayEnabled = nameof(Photino_GetMediaAutoplayEnabled);
    internal const string Photino_GetFileSystemAccessEnabled = nameof(Photino_GetFileSystemAccessEnabled);
    internal const string Photino_GetWebSecurityEnabled = nameof(Photino_GetWebSecurityEnabled);
    internal const string Photino_GetJavascriptClipboardAccessEnabled = nameof(Photino_GetJavascriptClipboardAccessEnabled);
    internal const string Photino_GetMediaStreamEnabled = nameof(Photino_GetMediaStreamEnabled);
    internal const string Photino_GetSmoothScrollingEnabled = nameof(Photino_GetSmoothScrollingEnabled);
    internal const string Photino_GetIgnoreCertificateErrorsEnabled = nameof(Photino_GetIgnoreCertificateErrorsEnabled);
    internal const string Photino_GetNotificationsEnabled = nameof(Photino_GetNotificationsEnabled);
    internal const string Photino_GetPosition = nameof(Photino_GetPosition);
    internal const string Photino_GetResizable = nameof(Photino_GetResizable);
    internal const string Photino_GetScreenDpi = nameof(Photino_GetScreenDpi);
    internal const string Photino_GetSize = nameof(Photino_GetSize);
    internal const string Photino_GetTitle = nameof(Photino_GetTitle);
    internal const string Photino_GetTopmost = nameof(Photino_GetTopmost);
    internal const string Photino_GetZoom = nameof(Photino_GetZoom);
    internal const string Photino_GetMaximized = nameof(Photino_GetMaximized);
    internal const string Photino_GetMinimized = nameof(Photino_GetMinimized);
    internal const string Photino_Invoke = nameof(Photino_Invoke);
    internal const string Photino_NavigateToString = nameof(Photino_NavigateToString);
    internal const string Photino_NavigateToUrl = nameof(Photino_NavigateToUrl);
    internal const string Photino_setWebView2RuntimePath_win32 = nameof(Photino_setWebView2RuntimePath_win32);
    internal const string Photino_SetTransparentEnabled = nameof(Photino_SetTransparentEnabled);
    internal const string Photino_SetContextMenuEnabled = nameof(Photino_SetContextMenuEnabled);
    internal const string Photino_SetDevToolsEnabled = nameof(Photino_SetDevToolsEnabled);
    internal const string Photino_SetFullScreen = nameof(Photino_SetFullScreen);
    internal const string Photino_SetGrantBrowserPermissions = nameof(Photino_SetGrantBrowserPermissions);
    internal const string Photino_SetMaximized = nameof(Photino_SetMaximized);
    internal const string Photino_SetMaxSize = nameof(Photino_SetMaxSize);
    internal const string Photino_SetMinimized = nameof(Photino_SetMinimized);
    internal const string Photino_SetMinSize = nameof(Photino_SetMinSize);
    internal const string Photino_SetResizable = nameof(Photino_SetResizable);
    internal const string Photino_SetPosition = nameof(Photino_SetPosition);
    internal const string Photino_SetSize = nameof(Photino_SetSize);
    internal const string Photino_SetTitle = nameof(Photino_SetTitle);
    internal const string Photino_SetTopmost = nameof(Photino_SetTopmost);
    internal const string Photino_SetIconFile = nameof(Photino_SetIconFile);
    internal const string Photino_SetZoom = nameof(Photino_SetZoom);
    internal const string Photino_Center = nameof(Photino_Center);
    internal const string Photino_ClearBrowserAutoFill = nameof(Photino_ClearBrowserAutoFill);
    internal const string Photino_SendWebMessage = nameof(Photino_SendWebMessage);
    internal const string Photino_ShowNotification = nameof(Photino_ShowNotification);
    internal const string Photino_WaitForExit = nameof(Photino_WaitForExit);
    internal const string Photino_ShowOpenFile = nameof(Photino_ShowOpenFile);
    internal const string Photino_ShowOpenFolder = nameof(Photino_ShowOpenFolder);
    internal const string Photino_ShowSaveFile = nameof(Photino_ShowSaveFile);
    internal const string Photino_ShowMessage = nameof(Photino_ShowMessage);
    internal const string Photino_GetZoomEnabled = nameof(Photino_GetZoomEnabled);
    internal const string Photino_SetZoomEnabled = nameof(Photino_SetZoomEnabled);
    // ReSharper restore InconsistentNaming
    #endregion
    
    #region InfiniWindowTests
    // ReSharper disable InconsistentNaming
    internal const string InfiniWindowTests_NativeParametersReturnAsIs = nameof(InfiniWindowTests_NativeParametersReturnAsIs);
    // ReSharper restore InconsistentNaming
    #endregion
}
