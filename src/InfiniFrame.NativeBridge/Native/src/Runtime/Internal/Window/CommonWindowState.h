#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <memory>
#include <mutex>
#include <string>
#include <unordered_map>
#include <vector>

#include "Api/Abi/Basic.h"
#include "Api/Abi/Callbacks.h"
#include "Runtime/Internal/Window/InfiniFrameDialog.h"
#include "Runtime/Internal/Operations/NativeOperation.h"
#include "Runtime/Internal/Operations/NavigationOperation.h"
#include "Runtime/Internal/Operations/DialogOperation.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow;
struct CommonWindowState;

CommonWindowState* GetCommonWindowState(InfiniFrameWindow* window) noexcept;
const CommonWindowState* GetCommonWindowState(const InfiniFrameWindow* window) noexcept;


// Shared logical state. Platform backends own this state through their private
// implementation object; it is never part of the public window declaration.
struct CommonWindowState {
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
    bool _statusBarEnabled = true;
    bool _browserShortcutsEnabled = true;
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
    NativeString _defaultNotificationIcon;
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
