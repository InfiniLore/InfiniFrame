#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <memory>
#include <string>
#include <vector>

#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Types/Callbacks.h"
#include "Runtime/Shared/Window/InfiniFrameDialog.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow;

struct InfiniFrameWindowImpl {
    // -----------------------------------------------------------------------------------------------------------------
    // Callbacks
    // -----------------------------------------------------------------------------------------------------------------
    WebMessageReceivedCallback _webMessageReceivedCallback = nullptr;
    WebResourceRequestedCallback _customSchemeCallback = nullptr;
    ResizedCallback _resizedCallback = nullptr;
    MaximizedCallback _maximizedCallback = nullptr;
    RestoredCallback _restoredCallback = nullptr;
    MinimizedCallback _minimizedCallback = nullptr;
    MovedCallback _movedCallback = nullptr;
    ClosingCallback _closingCallback = nullptr;
    ClosedCallback _closedCallback = nullptr;
    FocusInCallback _focusInCallback = nullptr;
    FocusOutCallback _focusOutCallback = nullptr;
    DebugEventCallback _debugEventCallback = nullptr;

    // -----------------------------------------------------------------------------------------------------------------
    // Feature flags
    // -----------------------------------------------------------------------------------------------------------------
    bool _transparentEnabled = false;
    bool _contextMenuEnabled = true;
    bool _zoomEnabled = true;
    bool _devToolsEnabled = false;
    bool _webInspectorEnabled = false;
    bool _grantBrowserPermissions = false;
    bool _mediaAutoplayEnabled = false;
    bool _fileSystemAccessEnabled = false;
    bool _webSecurityEnabled = true;
    bool _javascriptClipboardAccessEnabled = false;
    bool _mediaStreamEnabled = false;
    bool _smoothScrollingEnabled = true;
    bool _ignoreCertificateErrorsEnabled = false;
    int _remoteDebuggingPort = 0;

    // -----------------------------------------------------------------------------------------------------------------
    // String state
    // -----------------------------------------------------------------------------------------------------------------
    NativeString _windowTitle;
    NativeString _startUrl;
    NativeString _startString;
    NativeString _userAgent;
    NativeString _browserControlInitParameters;
    NativeString _iconFileName;

    std::vector<NativeString> _customSchemeNames;

    // -----------------------------------------------------------------------------------------------------------------
    // Ownership
    // -----------------------------------------------------------------------------------------------------------------
    InfiniFrameWindow* _parent = nullptr;
    std::unique_ptr<InfiniFrameDialog> _dialog;
};
