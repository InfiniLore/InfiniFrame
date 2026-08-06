#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <condition_variable>
#include <mutex>
#include <vector>
#include <string>

#include <Cocoa/Cocoa.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"

@class UiDelegate;
@class NavigationDelegate;
@class WindowDelegate;
@class UrlSchemeHandler;

// A pooled host owns every AppKit/WebKit object whose destruction can race WebKit's display
// link.  It is deliberately separate from an InfiniFrameWindow logical session.
struct PooledMacHost {
    std::string compatibilityKey;
    NSWindow* window = nil;
    WKWebView* webview = nil;
    WKWebViewConfiguration* webviewConfiguration = nil;
    UiDelegate* uiDelegate = nil;
    NavigationDelegate* navigationDelegate = nil;
    WindowDelegate* windowDelegate = nil;
    std::vector<UrlSchemeHandler*> urlSchemeHandlers;
};

// Process-shutdown hook; must run on AppKit's main thread.
void DrainPooledMacHosts();
size_t PooledMacHostCountForTesting();
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    NSWindow* _window = nil;
    WKWebView* _webview = nil;
    WKWebViewConfiguration* _webviewConfiguration = nil;
    UiDelegate* _uiDelegate = nil;
    NavigationDelegate* _navigationDelegate = nil;
    WindowDelegate* _windowDelegate = nil;
    NSWindow* _nativeParentWindow = nil;
    id _parentWillCloseObserver = nil;
    std::vector<UrlSchemeHandler*> _urlSchemeHandlers;

    std::string _temporaryFilesPath;

    bool _chromeless = false;
    bool _webviewReady = false;
    bool _isClosingOrClosed = false;
    bool _nativeDestructionScheduled = false;
    std::string _hostCompatibilityKey;
    std::atomic<bool> _windowClosed = false;
    std::mutex _windowClosedMutex;
    std::condition_variable _windowClosedCondition;

    // Messages queued while WKWebView is still loading (e.g. sent from WindowCreated handler).
    // Flushed on the first didFinishNavigation callback.
    std::vector<std::string> _pendingWebMessages;

    int _zoom = 100;
    CGFloat _preMaximizedWidth = 0;
    CGFloat _preMaximizedHeight = 0;
    CGFloat _preMaximizedXPosition = 0;
    CGFloat _preMaximizedYPosition = 0;

    std::vector<Monitor> GetMonitors() const;
    void SetUserAgent(const char* userAgent);
    void SetPreference(NSString* key, NSNumber* value);
    void SetPreference(NSString* key, NSString* value);
    void AddCustomScheme(const char* scheme, WebResourceRequestedCallback requestHandler);
    bool LeasePooledMacHost(const std::string& compatibilityKey);
    void ReturnPooledMacHost();
};
