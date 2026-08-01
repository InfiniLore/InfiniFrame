#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <climits>
#include <condition_variable>
#include <mutex>
#include <string>
#include <vector>
#include <gtk/gtk.h>
#include <webkit2/webkit2.h>

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    GtkWidget* _window = nullptr;
    GtkWidget* _webview = nullptr;
    WebKitWebContext* _webContext = nullptr;
    gulong _webMessageSignalHandlerId = 0;
    int _remoteDebuggingPort = 0;

    std::string _temporaryFilesPath;

    bool _isFullScreen = false;
    bool _webviewReady = false;
    bool _webviewClosed = false;
    bool _webviewFinalized = false;
    bool _windowDestroyed = false;
    bool _teardownCompletionScheduled = false;
    bool _maximized = false;
    bool _minimized = false;
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

    std::mutex _lifecycleMutex;
    std::condition_variable _lifecycleClosed;
    bool _destroyed = false;

    // Messages queued while WebKit is still loading (e.g. sent from WindowCreated handler). 
    // Flushed on the first WEBKIT_LOAD_FINISHED event.
    std::vector<std::string> _pendingWebMessages;

    void set_webkit_settings();
    void configure_webkit_remote_debugging() const;
    void set_webkit_customsettings(WebKitSettings* settings);
    void AddCustomSchemeHandlers();
    void InitializeFromParams(const InfiniFrameInitParams* initParams);
    void ConfigureInitialWindow(InfiniFrameWindow* window, InfiniFrameInitParams* initParams);
    void ApplyInitialWindowState(InfiniFrameWindow* window, const InfiniFrameInitParams* initParams);
    void ConnectWindowSignals(InfiniFrameWindow* window);
    void ConnectWebViewSignals(InfiniFrameWindow* window);
};
