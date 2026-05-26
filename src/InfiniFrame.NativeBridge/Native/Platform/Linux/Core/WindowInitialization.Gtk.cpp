// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <algorithm>
#include <cstring>
#include <mutex>
#include <thread>

#include "Public/InfiniFrameDialog.h"
#include "Platform/Linux/Window.Gtk.Internal.h"

// Defined in WindowCore.Gtk.cpp — identifies the permanent GTK worker thread.
extern std::thread::id g_gtk_worker_thread_id;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, gpointer self);
gboolean on_window_state_event(GtkWidget* widget, GdkEventWindowState* event, gpointer self);
gboolean on_widget_deleted(GtkWidget* widget, GdkEvent* event, gpointer self);
void on_widget_destroyed(GtkWidget* widget, gpointer self);
gboolean on_focus_in_event(GtkWidget* widget, GdkEvent* event, gpointer self);
gboolean on_focus_out_event(GtkWidget* widget, GdkEvent* event, gpointer self);
gboolean on_webview_context_menu(
    WebKitWebView* web_view,
    GtkWidget* default_menu,
    WebKitHitTestResult* hit_test_result,
    gboolean triggered_with_keyboard,
    gpointer user_data
);
gboolean on_permission_request(WebKitWebView* web_view, WebKitPermissionRequest* request, gpointer user_data);

void InfiniFrameWindow::Impl::InitializeFromParams(const InfiniFrameInitParams* initParams) {
    if (initParams->Title != nullptr) {
        _windowTitle = initParams->Title;
    } else {
        _windowTitle = "";
    }

    if (initParams->StartUrl != nullptr) {
        _startUrl = initParams->StartUrl;
    }
    if (initParams->StartString != nullptr) {
        _startString = initParams->StartString;
    }
    if (initParams->TemporaryFilesPath != nullptr) {
        _temporaryFilesPath = initParams->TemporaryFilesPath;
    }
    if (initParams->UserAgent != nullptr) {
        _userAgent = initParams->UserAgent;
    }
    if (initParams->BrowserControlInitParameters != nullptr) {
        _browserControlInitParameters = initParams->BrowserControlInitParameters;
    }

    _transparentEnabled = initParams->Transparent;
    _contextMenuEnabled = initParams->ContextMenuEnabled;
    _zoomEnabled = initParams->ZoomEnabled;
    _devToolsEnabled = initParams->DevToolsEnabled;
    _grantBrowserPermissions = initParams->GrantBrowserPermissions;
    _mediaAutoplayEnabled = initParams->MediaAutoplayEnabled;
    _fileSystemAccessEnabled = initParams->FileSystemAccessEnabled;
    _webSecurityEnabled = initParams->WebSecurityEnabled;
    _javascriptClipboardAccessEnabled = initParams->JavascriptClipboardAccessEnabled;
    _mediaStreamEnabled = initParams->MediaStreamEnabled;
    _smoothScrollingEnabled = initParams->SmoothScrollingEnabled;
    _ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
    _isFullScreen = initParams->FullScreen;

    _zoom = initParams->Zoom;
    _minWidth = initParams->MinWidth;
    _minHeight = initParams->MinHeight;
    _maxWidth = initParams->MaxWidth;
    _maxHeight = initParams->MaxHeight;

    _webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
    _resizedCallback = initParams->ResizedHandler;
    _movedCallback = initParams->MovedHandler;
    _closingCallback = initParams->ClosingHandler;
    _closedCallback = initParams->ClosedHandler;
    _focusInCallback = initParams->FocusInHandler;
    _focusOutCallback = initParams->FocusOutHandler;
    _maximizedCallback = initParams->MaximizedHandler;
    _minimizedCallback = initParams->MinimizedHandler;
    _restoredCallback = initParams->RestoredHandler;
    _customSchemeCallback = initParams->CustomSchemeHandler;

    _customSchemeNames.clear();
    for (auto* customSchemeName : initParams->CustomSchemeNames) {
        if (customSchemeName == nullptr) {
            continue;
        }
        _customSchemeNames.emplace_back(customSchemeName);
    }

    _parent = initParams->ParentInstance;
}

void InfiniFrameWindow::Impl::ConfigureInitialWindow(InfiniFrameWindow* window, InfiniFrameInitParams* initParams) {
    // This function is always called on the permanent GTK worker thread (dispatched from the constructor).
    _gtkThreadId = g_gtk_worker_thread_id;
    _window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    g_object_ref_sink(_window);
    // Use a process-global context created once and intentionally never freed (leaked).
    //
    // Two problems to avoid:
    //   1. Per-window contexts (webkit_web_context_new per InfiniFrameWindow) trigger async WebKit
    //      cleanup when g_object_unref'd. If a new window is created while that cleanup is still
    //      running on WebKit's background threads, webkit_web_view_new_with_context() hits an
    //      internal assertion and calls abort() (exit 134).
    //   2. webkit_web_context_get_default() causes GLib to register the singleton for automatic
    //      finalization at process exit. That finalization fires WebKit's own abort() call.
    //
    // Holding a permanent reference (refcount always ≥ 1) prevents GLib from ever finalizing the
    // context: GLib only finalizes objects whose refcount reaches 0, so the abort() never fires.
    // The small one-time memory leak is harmless — the OS reclaims it on process exit anyway.
    static WebKitWebContext* s_processContext = nullptr;
    static std::once_flag s_contextOnce;
    std::call_once(s_contextOnce, [] {
        s_processContext = webkit_web_context_new();
        // Intentionally not calling g_object_unref(). The single floating reference is held
        // permanently so GLib never finalizes the context and WebKit's abort() never fires.
    });
    _webContext = s_processContext;
    _dialog = std::make_unique<InfiniFrameDialog>();

    if (initParams->FullScreen) {
        window->SetFullScreen(true);
        return;
    }

    initParams->Width = std::min(initParams->Width, initParams->MaxWidth);
    initParams->Height = std::min(initParams->Height, initParams->MaxHeight);
    initParams->Width = std::max(initParams->Width, initParams->MinWidth);
    initParams->Height = std::max(initParams->Height, initParams->MinHeight);

    if (initParams->UseOsDefaultSize) {
        gtk_window_set_default_size(GTK_WINDOW(_window), -1, -1);
    } else {
        gtk_window_set_default_size(GTK_WINDOW(_window), initParams->Width, initParams->Height);
    }

    window->SetMinSize(initParams->MinWidth, initParams->MinHeight);
    window->SetMaxSize(initParams->MaxWidth, initParams->MaxHeight);

    if (initParams->UseOsDefaultLocation) {
        gtk_window_set_position(GTK_WINDOW(_window), GTK_WIN_POS_NONE);
    } else if (initParams->CenterOnInitialize) {
        gtk_window_set_position(GTK_WINDOW(_window), GTK_WIN_POS_CENTER);
    } else {
        gtk_window_move(GTK_WINDOW(_window), initParams->Left, initParams->Top);
    }
}

void InfiniFrameWindow::Impl::ApplyInitialWindowState(
    InfiniFrameWindow* window, const InfiniFrameInitParams* initParams
) {
    window->SetTitle(const_cast<AutoString>(_windowTitle.c_str()));

    if (initParams->Chromeless) {
        gtk_window_set_decorated(GTK_WINDOW(_window), false);
    }

    if (initParams->WindowIconFile != nullptr && std::strlen(initParams->WindowIconFile) > 0) {
        window->SetIconFile(initParams->WindowIconFile);
    }

    if (initParams->CenterOnInitialize) {
        window->Center();
    }
    if (initParams->Minimized) {
        window->SetMinimized(true);
    }
    if (initParams->Maximized) {
        window->SetMaximized(true);
    }
    if (!initParams->Resizable) {
        window->SetResizable(false);
    }
    if (initParams->Topmost) {
        window->SetTopmost(true);
    }
}

void InfiniFrameWindow::Impl::ConnectWindowSignals(InfiniFrameWindow* window) {
    g_signal_connect(G_OBJECT(_window), "configure-event", G_CALLBACK(on_configure_event), window);

    g_signal_connect(G_OBJECT(_window), "window-state-event", G_CALLBACK(on_window_state_event), window);

    g_signal_connect(G_OBJECT(_window), "delete-event", G_CALLBACK(on_widget_deleted), window);

    g_signal_connect(G_OBJECT(_window), "destroy", G_CALLBACK(on_widget_destroyed), window);

    g_signal_connect(G_OBJECT(_window), "focus-in-event", G_CALLBACK(on_focus_in_event), window);

    g_signal_connect(G_OBJECT(_window), "focus-out-event", G_CALLBACK(on_focus_out_event), window);
}

void InfiniFrameWindow::Impl::ConnectWebViewSignals(InfiniFrameWindow* window) {
    g_signal_connect(G_OBJECT(_webview), "context-menu", G_CALLBACK(on_webview_context_menu), window);

    g_signal_connect(G_OBJECT(_webview), "permission-request", G_CALLBACK(on_permission_request), window);
}
