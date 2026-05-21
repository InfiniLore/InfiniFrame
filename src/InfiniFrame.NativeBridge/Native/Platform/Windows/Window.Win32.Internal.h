#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <string>

#include <windows.h>
#include <wil/com.h>
#include <WebView2.h>

#include "Public/InfiniFrameWindow.h"
#include "Public/InfiniFrameWindowImpl.h"
#include "Platform/Windows/ToastHandler.h"
#include "Utils/Dimensions.h"
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
    bool _hasWebMessageReceivedToken = false;
    bool _hasWebResourceRequestedToken = false;
    bool _hasPermissionRequestedToken = false;

    std::unique_ptr<WinToastHandler> _toastHandler;
};
