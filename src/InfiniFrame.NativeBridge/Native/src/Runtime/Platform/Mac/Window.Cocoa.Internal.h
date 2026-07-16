#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include <vector>

#include <Cocoa/Cocoa.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"

@class UiDelegate;
@class NavigationDelegate;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    NSWindow* _window = nil;
    WKWebView* _webview = nil;
    WKWebViewConfiguration* _webviewConfiguration = nil;
    UiDelegate* _uiDelegate = nil;
    NavigationDelegate* _navigationDelegate = nil;
    NSWindow* _nativeParentWindow = nil;
    id _parentWillCloseObserver = nil;

    std::string _temporaryFilesPath;

    bool _chromeless = false;
    bool _webviewReady = false;
    bool _isClosingOrClosed = false;

    // Messages queued while WKWebView is still loading (e.g. sent from WindowCreated handler).
    // Flushed on the first didFinishNavigation callback.
    std::vector<std::string> _pendingWebMessages;

    CGFloat _preMaximizedWidth = 0;
    CGFloat _preMaximizedHeight = 0;
    CGFloat _preMaximizedXPosition = 0;
    CGFloat _preMaximizedYPosition = 0;

    std::vector<Monitor> GetMonitors() const;
    void SetUserAgent(AutoString userAgent);
    void SetPreference(NSString* key, NSNumber* value);
    void SetPreference(NSString* key, NSString* value);
    void AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler);
};
