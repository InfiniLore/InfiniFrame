#pragma once

#ifdef __APPLE__

#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameWindowImpl.h"

#include <string>
#include <vector>

@class NavigationDelegate;
@class UiDelegate;
@class WindowDelegate;

struct InfiniFrameInitParams;

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl
{
    NSWindow* _window = nil;
    WKWebView* _webview = nil;
    WKWebViewConfiguration* _webviewConfiguration = nil;
    NavigationDelegate* _navigationDelegate = nil;
    UiDelegate* _uiDelegate = nil;
    WindowDelegate* _windowDelegate = nil;

    std::string _temporaryFilesPath;

    bool _chromeless = false;

    CGFloat _preMaximizedWidth = 0;
    CGFloat _preMaximizedHeight = 0;
    CGFloat _preMaximizedXPosition = 0;
    CGFloat _preMaximizedYPosition = 0;

    std::vector<Monitor> GetMonitors() const;
    void ConfigureWebViewPreferences(InfiniFrameInitParams* initParams);
    void SetUserAgent(AutoString userAgent);
    void SetPreference(NSString* key, NSNumber* value);
    void SetPreference(NSString* key, NSString* value);
    void AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler);
    void AddCustomSchemeHandlers();
};

#endif
