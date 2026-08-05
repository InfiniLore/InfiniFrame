#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <memory>
#include <mutex>
#include <string>
#include <unordered_map>
#include <vector>

#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Types/Callbacks.h"
#include "Runtime/Shared/Window/InfiniFrameDialog.h"
#include "Runtime/Shared/Operations/NativeOperation.h"
#include "Runtime/Shared/Operations/NavigationOperation.h"
#include "Runtime/Shared/Operations/DialogOperation.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow;

struct InfiniFrameWindowImpl {
    std::mutex _operationMutex;
    std::unordered_map<uint64_t, std::shared_ptr<NativeOperation>> _operations;
    std::mutex _navigationMutex;
    std::unique_ptr<NavigationOperation> _navigationOperation;
    std::mutex _dialogOperationMutex;
    std::unordered_map<uint64_t, std::shared_ptr<DialogOperation>> _dialogOperations;
    std::mutex _milestoneMutex;
    ContextAction _readyCallback = nullptr;
    void* _readyCallbackContext = nullptr;
    ContextAction _teardownCallback = nullptr;
    void* _teardownCallbackContext = nullptr;
    bool _readySignaled = false;
    bool _teardownSignaled = false;
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
    NavigationStartingCallback _navigationStartingCallback = nullptr;
    FileDroppedCallback _fileDroppedCallback = nullptr;
    bool _dragDropEnabled = false;

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
    NativeString _webView2RuntimePath;
    NativeString _iconFileName;
    uint8_t _backgroundColorR = 0;
    uint8_t _backgroundColorG = 0;
    uint8_t _backgroundColorB = 0;
    uint8_t _backgroundColorA = 0;

    std::vector<NativeString> _customSchemeNames;

    // -----------------------------------------------------------------------------------------------------------------
    // Ownership
    // -----------------------------------------------------------------------------------------------------------------
    InfiniFrameWindow* _parent = nullptr;
    std::unique_ptr<InfiniFrameDialog> _dialog;
};
