// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeDll {
    internal const string DllName = "InfiniFrame.Native";

    #region InfiniFrame
    // ReSharper disable InconsistentNaming
    internal const string InfiniFrame_register_win32 = nameof(InfiniFrame_register_win32);
    internal const string InfiniFrame_register_mac = nameof(InfiniFrame_register_mac);
    internal const string InfiniFrame_ctor = nameof(InfiniFrame_ctor);
    internal const string InfiniFrame_dtor = nameof(InfiniFrame_dtor);
    internal const string InfiniFrame_GetLastErrorMessage = nameof(InfiniFrame_GetLastErrorMessage);
    internal const string InfiniFrame_AddCustomSchemeName = nameof(InfiniFrame_AddCustomSchemeName);
    internal const string InfiniFrame_Close = nameof(InfiniFrame_Close);
    internal const string InfiniFrame_getHwnd_win32 = nameof(InfiniFrame_getHwnd_win32);
    internal const string InfiniFrame_GetAllMonitors = nameof(InfiniFrame_GetAllMonitors);
    internal const string InfiniFrame_GetTransparentEnabled = nameof(InfiniFrame_GetTransparentEnabled);
    internal const string InfiniFrame_GetContextMenuEnabled = nameof(InfiniFrame_GetContextMenuEnabled);
    internal const string InfiniFrame_GetDevToolsEnabled = nameof(InfiniFrame_GetDevToolsEnabled);
    internal const string InfiniFrame_GetFullScreen = nameof(InfiniFrame_GetFullScreen);
    internal const string InfiniFrame_GetGrantBrowserPermissions = nameof(InfiniFrame_GetGrantBrowserPermissions);
    internal const string InfiniFrame_GetUserAgent = nameof(InfiniFrame_GetUserAgent);
    internal const string InfiniFrame_GetMediaAutoplayEnabled = nameof(InfiniFrame_GetMediaAutoplayEnabled);
    internal const string InfiniFrame_GetFileSystemAccessEnabled = nameof(InfiniFrame_GetFileSystemAccessEnabled);
    internal const string InfiniFrame_GetWebSecurityEnabled = nameof(InfiniFrame_GetWebSecurityEnabled);
    internal const string InfiniFrame_GetJavascriptClipboardAccessEnabled = nameof(InfiniFrame_GetJavascriptClipboardAccessEnabled);
    internal const string InfiniFrame_GetMediaStreamEnabled = nameof(InfiniFrame_GetMediaStreamEnabled);
    internal const string InfiniFrame_GetSmoothScrollingEnabled = nameof(InfiniFrame_GetSmoothScrollingEnabled);
    internal const string InfiniFrame_GetIgnoreCertificateErrorsEnabled = nameof(InfiniFrame_GetIgnoreCertificateErrorsEnabled);
    internal const string InfiniFrame_GetNotificationsEnabled = nameof(InfiniFrame_GetNotificationsEnabled);
    internal const string InfiniFrame_GetPosition = nameof(InfiniFrame_GetPosition);
    internal const string InfiniFrame_GetResizable = nameof(InfiniFrame_GetResizable);
    internal const string InfiniFrame_GetScreenDpi = nameof(InfiniFrame_GetScreenDpi);
    internal const string InfiniFrame_GetSize = nameof(InfiniFrame_GetSize);
    internal const string InfiniFrame_GetMaxSize = nameof(InfiniFrame_GetMaxSize);
    internal const string InfiniFrame_GetMinSize = nameof(InfiniFrame_GetMinSize);
    internal const string InfiniFrame_GetTitle = nameof(InfiniFrame_GetTitle);
    internal const string InfiniFrame_GetTopmost = nameof(InfiniFrame_GetTopmost);
    internal const string InfiniFrame_GetZoom = nameof(InfiniFrame_GetZoom);
    internal const string InfiniFrame_GetMaximized = nameof(InfiniFrame_GetMaximized);
    internal const string InfiniFrame_GetMinimized = nameof(InfiniFrame_GetMinimized);
    internal const string InfiniFrame_Invoke = nameof(InfiniFrame_Invoke);
    internal const string InfiniFrame_NavigateToString = nameof(InfiniFrame_NavigateToString);
    internal const string InfiniFrame_NavigateToUrl = nameof(InfiniFrame_NavigateToUrl);
    internal const string InfiniFrame_setWebView2RuntimePath_win32 = nameof(InfiniFrame_setWebView2RuntimePath_win32);
    internal const string InfiniFrame_SetTransparentEnabled = nameof(InfiniFrame_SetTransparentEnabled);
    internal const string InfiniFrame_SetContextMenuEnabled = nameof(InfiniFrame_SetContextMenuEnabled);
    internal const string InfiniFrame_SetDevToolsEnabled = nameof(InfiniFrame_SetDevToolsEnabled);
    internal const string InfiniFrame_SetFullScreen = nameof(InfiniFrame_SetFullScreen);
    internal const string InfiniFrame_SetMaximized = nameof(InfiniFrame_SetMaximized);
    internal const string InfiniFrame_SetMaxSize = nameof(InfiniFrame_SetMaxSize);
    internal const string InfiniFrame_SetMinimized = nameof(InfiniFrame_SetMinimized);
    internal const string InfiniFrame_SetMinSize = nameof(InfiniFrame_SetMinSize);
    internal const string InfiniFrame_SetResizable = nameof(InfiniFrame_SetResizable);
    internal const string InfiniFrame_SetPosition = nameof(InfiniFrame_SetPosition);
    internal const string InfiniFrame_SetSize = nameof(InfiniFrame_SetSize);
    internal const string InfiniFrame_SetTitle = nameof(InfiniFrame_SetTitle);
    internal const string InfiniFrame_SetTopmost = nameof(InfiniFrame_SetTopmost);
    internal const string InfiniFrame_SetIconFile = nameof(InfiniFrame_SetIconFile);
    internal const string InfiniFrame_GetIconFileName = nameof(InfiniFrame_GetIconFileName);
    internal const string InfiniFrame_SetZoom = nameof(InfiniFrame_SetZoom);
    internal const string InfiniFrame_Center = nameof(InfiniFrame_Center);
    internal const string InfiniFrame_ClearBrowserAutoFill = nameof(InfiniFrame_ClearBrowserAutoFill);
    internal const string InfiniFrame_SendWebMessage = nameof(InfiniFrame_SendWebMessage);
    internal const string InfiniFrame_ShowNotification = nameof(InfiniFrame_ShowNotification);
    internal const string InfiniFrame_WaitForExit = nameof(InfiniFrame_WaitForExit);
    internal const string InfiniFrame_ShowOpenFile = nameof(InfiniFrame_ShowOpenFile);
    internal const string InfiniFrame_ShowOpenFolder = nameof(InfiniFrame_ShowOpenFolder);
    internal const string InfiniFrame_ShowSaveFile = nameof(InfiniFrame_ShowSaveFile);
    internal const string InfiniFrame_ShowMessage = nameof(InfiniFrame_ShowMessage);
    internal const string InfiniFrame_GetZoomEnabled = nameof(InfiniFrame_GetZoomEnabled);
    internal const string InfiniFrame_SetZoomEnabled = nameof(InfiniFrame_SetZoomEnabled);
    internal const string InfiniFrame_FreeString = nameof(InfiniFrame_FreeString);
    internal const string InfiniFrame_FreeStringArray = nameof(InfiniFrame_FreeStringArray);
    internal const string InfiniFrame_SetFocused = nameof(InfiniFrame_SetFocused);
    internal const string InfiniFrame_GetFocused = nameof(InfiniFrame_GetFocused);
    internal const string InfiniFrame_Restore = nameof(InfiniFrame_Restore);
    // ReSharper restore InconsistentNaming
    #endregion

    #region InfiniWindowTests
    // ReSharper disable InconsistentNaming
    internal const string InfiniWindowTests_NativeParametersReturnAsIs = nameof(InfiniWindowTests_NativeParametersReturnAsIs);
    internal const string InfiniWindowTests_FreeInitParams = nameof(InfiniWindowTests_FreeInitParams);
    // ReSharper restore InconsistentNaming
    #endregion
}
