#pragma once
/**
 * @file WindowImpl.Win32.h
 * @brief Private Win32/WebView2 implementation state for InfiniFrameWindow.
 */

#ifndef INFINIFRAME_PLATFORM_WINDOWS_WINDOWIMPL_WIN32_H
#define INFINIFRAME_PLATFORM_WINDOWS_WINDOWIMPL_WIN32_H

#include <memory>
#include <string>

#include <windows.h>
#include <WebView2.h>
#include <wil/com.h>

#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameWindowImpl.h"
#include "ToastHandler.h"
#include "Utils/Common.h"

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    std::wstring _temporaryFilesPath;
    std::wstring _notificationRegistrationId;

    bool _notificationsEnabled = false;
    bool _isInitialized = false;
    bool _isWebView2Initializing = false;
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

    int _zoom = 100;
    int _minWidth = MinWindowDimension;
    int _minHeight = MinWindowDimension;
    int _maxWidth = MaxWindowDimension;
    int _maxHeight = MaxWindowDimension;

    HWND _hWnd = nullptr;
    wil::com_ptr<ICoreWebView2Controller> _webviewController;
    wil::com_ptr<ICoreWebView2> _webviewWindow;
    wil::com_ptr<ICoreWebView2Environment> _webviewEnvironment;

    EventRegistrationToken _webMessageReceivedToken = {};
    EventRegistrationToken _webResourceRequestedTokenForCustomScheme = {};
    EventRegistrationToken _permissionRequestedToken = {};

    bool _webMessageReceivedRegistered = false;
    bool _webResourceRequestedRegistered = false;
    bool _permissionRequestedRegistered = false;
    bool _webviewInitializationFailed = false;

    HRESULT _webviewInitializationResult = S_OK;
    std::wstring _webviewInitializationError;

    std::unique_ptr<WinToastHandler> _toastHandler;

    void FailWebViewInitialization(HRESULT result, const wchar_t* stage) noexcept;
    void MarkWebViewInitialized() noexcept;
    void ThrowIfWebViewInitializationFailed() const;
    void WaitForWebViewInitialization();
    void UnregisterWebViewEventHandlers() noexcept;
    void ConfigureNotificationIdentityForTitle(const std::wstring& title);
    void InitializeNotifications(InfiniFrameWindow* window);

    HRESULT ConfigureWebViewSettings() const;
    void RegisterPermissionRequestedHandler();

    void RegisterBridgeScriptAndNavigate();
    void NavigateToInitialContent();

    void RegisterWebMessageReceivedHandler();
    HRESULT HandleWebMessageReceived(ICoreWebView2WebMessageReceivedEventArgs* args);

    void RegisterWebResourceRequestedHandler();
    HRESULT HandleWebResourceRequested(ICoreWebView2WebResourceRequestedEventArgs* args);
};

#endif // INFINIFRAME_PLATFORM_WINDOWS_WINDOWIMPL_WIN32_H
