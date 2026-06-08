#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <string>
#include <vector>

#include <windows.h>
#include <wil/com.h>
#include <WebView2.h>

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"
#include "Runtime/Platform/Windows/ToastHandler.h"
#include "Runtime/Shared/Utilities/Dimensions.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    std::wstring _temporaryFilesPath;
    std::wstring _notificationRegistrationId;

    bool _notificationsEnabled = false;
    bool _isInitialized = false;
    bool _isWebView2Initializing = false;
    std::atomic<bool> _isClosingOrClosed = false;
    bool _centerOnInitialize = false;
    bool _chromeless = false;
    bool _fullScreen = false;
    bool _maximized = false;
    bool _minimized = false;
    bool _resizable = true;
    bool _topmost = false;
    bool _useOsDefaultLocation = false;
    bool _useOsDefaultSize = false;
    bool _hasSavedRect = false;

    RECT _savedRect = {};

    int _lastLeft = INT_MIN;
    int _lastTop = INT_MIN;
    int _lastWidth = INT_MIN;
    int _lastHeight = INT_MIN;

    int _zoom = 100;
    int _minWidth = MinWindowDimension;
    int _minHeight = MinWindowDimension;
    int _maxWidth = MaxWindowDimension;
    int _maxHeight = MaxWindowDimension;

    HWND _hWnd = nullptr;
    HWND _pendingOwnerHwnd = nullptr;
    bool _ownerAssigned = false;
    wil::com_ptr<ICoreWebView2Controller> _webviewController;
    wil::com_ptr<ICoreWebView2> _webviewWindow;
    wil::com_ptr<ICoreWebView2Environment> _webviewEnvironment;

    EventRegistrationToken _webMessageReceivedToken = {};
    EventRegistrationToken _webResourceRequestedTokenForCustomScheme = {};
    EventRegistrationToken _permissionRequestedToken = {};
    EventRegistrationToken _navigationCompletedToken = {};
    EventRegistrationToken _processFailedToken = {};
    bool _hasWebMessageReceivedToken = false;
    bool _hasWebResourceRequestedToken = false;
    bool _hasPermissionRequestedToken = false;
    bool _hasNavigationCompletedToken = false;
    bool _hasProcessFailedToken = false;

    // Messages queued while WebView2 is still initializing (e.g. sent from WindowCreated).
    // Flushed to the WebView on the first NavigationCompleted event.
    std::vector<std::wstring> _pendingWebMessages;

    std::unique_ptr<WinToastHandler> _toastHandler;
};
