#pragma once

#include <vector>

#include <Cocoa/Cocoa.h>
#include <UserNotifications/UserNotifications.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

#include "Public/InfiniFrameWindow.h"
#include "Public/InfiniFrameWindowImpl.h"

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    NSWindow* _window = nil;
    WKWebView* _webview = nil;
    WKWebViewConfiguration* _webviewConfiguration = nil;
    NSWindow* _nativeParentWindow = nil;
    id _parentWillCloseObserver = nil;

    std::string _temporaryFilesPath;

    bool _chromeless = false;

    CGFloat _preMaximizedWidth = 0;
    CGFloat _preMaximizedHeight = 0;
    CGFloat _preMaximizedXPosition = 0;
    CGFloat _preMaximizedYPosition = 0;

    void SetUserAgent(AutoString userAgent);
    void SetPreference(const char* key, bool value);
    void SetPreference(const char* key, int64_t value);
    void SetPreference(const char* key, double value);
    void SetPreference(const char* key, const char* value);
    void AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler);
};
