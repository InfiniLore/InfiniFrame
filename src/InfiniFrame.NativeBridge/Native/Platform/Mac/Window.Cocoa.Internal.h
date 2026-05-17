#pragma once

#ifndef INFINIFRAME_PLATFORM_MAC_WINDOW_COCOA_INTERNAL_H
#define INFINIFRAME_PLATFORM_MAC_WINDOW_COCOA_INTERNAL_H

#include <vector>

#include <Cocoa/Cocoa.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>

#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameWindowImpl.h"

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

    std::vector<Monitor> GetMonitors() const;
    void SetUserAgent(AutoString userAgent);
    void SetPreference(NSString* key, NSNumber* value);
    void SetPreference(NSString* key, NSString* value);
    void AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler);
};

#endif // INFINIFRAME_PLATFORM_MAC_WINDOW_COCOA_INTERNAL_H
