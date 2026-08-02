#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Types/Callbacks.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow; // Forward declaration

/**
 * @brief Initialization parameters for InfiniFrame window
 */
struct InfiniFrameInitParams {
    static constexpr std::size_t MaxCustomSchemeNames = 16;
    
    // Content
    AutoString StartString;
    AutoString StartUrl;

    // Window appearance
    AutoString Title;
    AutoString WindowIconFile;
    AutoString TemporaryFilesPath;
    AutoString UserAgent;
    AutoString BrowserControlInitParameters;
    AutoString WebView2RuntimePath;
    AutoString NotificationRegistrationId;
    AutoString WindowsAppUserModelId;
    int RemoteDebuggingPort;

    // Parent window
    InfiniFrameWindow* ParentInstance;

    // Event handlers
    ClosingCallback ClosingHandler;
    ClosedCallback ClosedHandler;
    FocusInCallback FocusInHandler;
    FocusOutCallback FocusOutHandler;
    ResizedCallback ResizedHandler;
    MaximizedCallback MaximizedHandler;
    RestoredCallback RestoredHandler;
    MinimizedCallback MinimizedHandler;
    MovedCallback MovedHandler;
    WebMessageReceivedCallback WebMessageReceivedHandler;
    DebugEventCallback DebugEventHandler;
    AutoString CustomSchemeNames[MaxCustomSchemeNames]; // NOLINT(*-avoid-c-arrays)
    WebResourceRequestedCallback CustomSchemeHandler;

    // Position and size
    int Left;
    int Top;
    int Width;
    int Height;
    int Zoom;
    int MinWidth;
    int MinHeight;
    int MaxWidth;
    int MaxHeight;

    // Behavior flags
    bool CenterOnInitialize;
    bool Chromeless;
    bool Transparent;
    uint8_t BackgroundColorR;
    uint8_t BackgroundColorG;
    uint8_t BackgroundColorB;
    uint8_t BackgroundColorA;
    bool ContextMenuEnabled;
    bool ZoomEnabled;
    bool DevToolsEnabled;
    bool WebInspectorEnabled;
    bool FullScreen;
    bool Maximized;
    bool Minimized;
    bool Resizable;
    bool Topmost;
    bool UseOsDefaultLocation;
    bool UseOsDefaultSize;
    bool GrantBrowserPermissions;
    bool MediaAutoplayEnabled;
    bool FileSystemAccessEnabled;
    bool WebSecurityEnabled;
    bool JavascriptClipboardAccessEnabled;
    bool MediaStreamEnabled;
    bool SmoothScrollingEnabled;
    bool IgnoreCertificateErrorsEnabled;
    bool NotificationsEnabled;

    // Struct size (for version checking)
    int StructSize;
};
