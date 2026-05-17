#pragma once

#ifndef INFINIFRAME_PLATFORM_LINUX_WINDOW_GTK_INTERNAL_H
#define INFINIFRAME_PLATFORM_LINUX_WINDOW_GTK_INTERNAL_H

#include <climits>
#include <string>

#include <gtk/gtk.h>
#include <webkit2/webkit2.h>

#include "Public/InfiniFrameWindow.h"
#include "Public/InfiniFrameWindowImpl.h"

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    GtkWidget* _window = nullptr;
    GtkWidget* _webview = nullptr;

    std::string _temporaryFilesPath;

    bool _isFullScreen = false;
    double _zoom = 100.0;
    int _minWidth = 0;
    int _minHeight = 0;
    int _maxWidth = INT_MAX;
    int _maxHeight = INT_MAX;

    GdkGeometry _hints = {};

    int _lastLeft = 0;
    int _lastTop = 0;
    int _lastWidth = 0;
    int _lastHeight = 0;

    void set_webkit_settings();
    void set_webkit_customsettings(WebKitSettings* settings);
    void AddCustomSchemeHandlers();
    void InitializeFromParams(const InfiniFrameInitParams* initParams);
    void ConfigureInitialWindow(InfiniFrameWindow* window, InfiniFrameInitParams* initParams);
    void ApplyInitialWindowState(InfiniFrameWindow* window, const InfiniFrameInitParams* initParams);
    void ConnectWindowSignals(InfiniFrameWindow* window);
    void ConnectWebViewSignals(InfiniFrameWindow* window);
};

#endif // INFINIFRAME_PLATFORM_LINUX_WINDOW_GTK_INTERNAL_H
