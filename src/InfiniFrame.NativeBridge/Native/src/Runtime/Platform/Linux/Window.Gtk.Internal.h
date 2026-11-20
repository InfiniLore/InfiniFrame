#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <climits>
#include <condition_variable>
#include <memory>
#include <mutex>
#include <string>
#include <unordered_map>
#include <vector>
#include <gtk/gtk.h>
#include <webkit/webkit.h>

#include "Runtime/Internal/Interop/Types/InfiniFrameWindow.h"
#include "Runtime/Internal/Window/CommonWindowState.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct InfiniFrameWindow::Impl {
    CommonWindowState common;
    GtkWidget* _window = nullptr;
    GtkWidget* _webview = nullptr;
    WebKitWebContext* _webContext = nullptr;
    gulong _webMessageSignalHandlerId = 0;
    std::string _temporaryFilesPath;

    GtkWidget* _menuBar = nullptr; // horizontal menu-bar row (flat submenu/command buttons)
    GtkWidget* _menuBox = nullptr; // vertical layout container: menu bar + webview
    GSimpleActionGroup* _menuActions = nullptr;
    std::string _menuBarJson;
    std::unordered_map<std::string, guint> _menuItemIdToCommandId;
    std::unordered_map<guint, std::string> _menuCommandIdToItemId;
    std::unordered_map<std::string, bool> _menuEnabledOverrides;
    std::unordered_map<std::string, bool> _menuVisibleOverrides;
    std::unordered_map<std::string, GtkWidget*> _menuWidgetById;
    guint _nextMenuCommandId = 1;
    std::vector<void*> _menuActivateDataList;

    bool _isFullScreen = false;
    bool _webviewReady = false;
    bool _webviewClosed = false;
    bool _webviewFinalized = false;
    bool _windowDestroyed = false;
    bool _teardownCompletionScheduled = false;
    bool _teardownCompleted = false;
    bool _applicationNotified = false;
    bool _maximized = false;
    bool _minimized = false;
    // Focus-event gating: -1 = unknown (before the first activation notification),
    // 0 = inactive, 1 = active. Suppresses spurious initial focus events (migration requirement).
    int _activeEventState = -1;
    double _zoom = 100.0;
    int _minWidth = 0;
    int _minHeight = 0;
    int _maxWidth = INT_MAX;
    int _maxHeight = INT_MAX;

    // GTK4 has no window-positioning API; a requested move before the surface is
    // realized is stored here and applied from OnSurfaceRealized (X11 only).
    bool _hasPendingMove = false;
    int _pendingMoveX = 0;
    int _pendingMoveY = 0;

    // gtk_window_set_default_size is silently clobbered when it lands while the first
    // map/size negotiation is in flight (widget unallocated). The request is kept here and
    // re-asserted once from the first layout callback, when the toplevel is fully allocated
    // (teardown work: SetSize flake fix).
    int _pendingResizeWidth = 0;
    int _pendingResizeHeight = 0;

    int _lastLeft = 0;
    int _lastTop = 0;
    int _lastWidth = 0;
    int _lastHeight = 0;

    std::mutex _lifecycleMutex;
    std::condition_variable _lifecycleClosed;
    bool _destroyed = false;

    // Serializes the deferred teardown completion (ScheduleTeardownCompletion's idle)
    // against the window object's destruction, while preserving exactly-once notification:
    // whichever runs first performs SignalTeardown/NotifyWindowClosed and marks `completed`;
    // the loser observes `completed` and returns without touching the object.
    struct TeardownGuard {
        std::mutex mutex;
        bool completed = false;
    };
    std::shared_ptr<TeardownGuard> _teardownGuard = std::make_shared<TeardownGuard>();

    // Messages queued while WebKit is still loading (e.g. sent from WindowCreated handler). 
    // Flushed on the first WEBKIT_LOAD_FINISHED event.
    std::vector<std::string> _pendingWebMessages;

    void set_webkit_settings();
    void configure_webkit_remote_debugging() const;
    void set_webkit_customsettings(WebKitSettings* settings);
    void AddCustomSchemeHandlers();
    void InitializeFromParams(const InfiniFrameWindowInitParams* initParams);
    void ConfigureInitialWindow(InfiniFrameWindow* window, InfiniFrameWindowInitParams* initParams);
    void ApplyInitialWindowState(InfiniFrameWindow* window, const InfiniFrameWindowInitParams* initParams);
    void ConnectWindowSignals(InfiniFrameWindow* window);
    void ConnectWebViewSignals(InfiniFrameWindow* window);

    // GTK4 window primitives.
    void ApplySizeConstraints();
    void MoveTo(int x, int y);
    bool TryGetPosition(int* x, int* y) const;

    // (Re)builds the window content layout: vertical box [menu bar, webview] when a
    // menu bar exists, otherwise the webview becomes the window child directly.
    void RelayoutWindow();

    // Menu resource ownership (defined in Menu.Gtk.cpp).
    void ReleaseMenuResources();
};