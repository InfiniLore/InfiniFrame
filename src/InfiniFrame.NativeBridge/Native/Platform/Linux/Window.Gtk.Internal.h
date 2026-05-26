#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <climits>
#include <condition_variable>
#include <mutex>
#include <string>
#include <thread>
#include <vector>
#include <gtk/gtk.h>
#include <webkit2/webkit2.h>

#include "Public/InfiniFrameWindow.h"
#include "Public/InfiniFrameWindowImpl.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    GtkWidget* _window = nullptr;
    GtkWidget* _webview = nullptr;
    WebKitWebContext* _webContext = nullptr;

    std::string _temporaryFilesPath;

    bool _isFullScreen = false;
    bool _webviewReady = false;
    std::thread::id _gtkThreadId = std::thread::id();

    std::mutex _destroyedMutex;
    std::condition_variable _destroyedCv;
    // Nested GMainLoop started by WaitForExit() when called on the GTK worker thread (via C# Invoke()).
    // OnWidgetDestroyed() calls g_main_loop_quit() on it so events keep processing until window close.
    GMainLoop* _exitLoop = nullptr;
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

    // Messages queued while WebKit is still loading (e.g. sent from WindowCreated handler). 
    // Flushed on the first WEBKIT_LOAD_FINISHED event.
    std::vector<std::string> _pendingWebMessages;

    void set_webkit_settings();
    void set_webkit_customsettings(WebKitSettings* settings);
    void AddCustomSchemeHandlers();
    void InitializeFromParams(const InfiniFrameInitParams* initParams);
    void ConfigureInitialWindow(InfiniFrameWindow* window, InfiniFrameInitParams* initParams);
    void ApplyInitialWindowState(InfiniFrameWindow* window, const InfiniFrameInitParams* initParams);
    void ConnectWindowSignals(InfiniFrameWindow* window);
    void ConnectWebViewSignals(InfiniFrameWindow* window);

    bool IsGtkThread() const {
        return _gtkThreadId == std::this_thread::get_id();
    }
};
