#pragma once
/**
 * @file InfiniFrameInitParams.h
 * @brief Window initialization parameters
 */

#ifndef INFINIFRAME_CORE_INITPARAMS_H
#define INFINIFRAME_CORE_INITPARAMS_H

#include "../Types/Basic.h"
#include "../Types/Callbacks.h"

class InfiniFrameWindow;  // Forward declaration

/**
 * @brief Initialization parameters for InfiniFrame window
 */
struct InfiniFrameInitParams
{
    // Content
    AutoString StartString;
    AutoString StartUrl;

    // Window appearance
    AutoString Title;
    AutoString WindowIconFile;
    AutoString TemporaryFilesPath;
    AutoString UserAgent;
    AutoString BrowserControlInitParameters;
    AutoString NotificationRegistrationId;

    // Parent window
    InfiniFrameWindow* ParentInstance;

    // Event handlers
    ClosingCallback ClosingHandler;
    FocusInCallback FocusInHandler;
    FocusOutCallback FocusOutHandler;
    ResizedCallback ResizedHandler;
    MaximizedCallback MaximizedHandler;
    RestoredCallback RestoredHandler;
    MinimizedCallback MinimizedHandler;
    MovedCallback MovedHandler;
    WebMessageReceivedCallback WebMessageReceivedHandler;
    AutoString CustomSchemeNames[16];
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
    bool ContextMenuEnabled;
    bool ZoomEnabled;
    bool DevToolsEnabled;
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
    int Size;
};

#endif // INFINIFRAME_CORE_INITPARAMS_H
