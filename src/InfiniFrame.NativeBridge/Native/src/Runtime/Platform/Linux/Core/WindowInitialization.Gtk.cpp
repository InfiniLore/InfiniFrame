// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <algorithm>
#include <cstring>

#include "Runtime/Internal/Window/InfiniFrameDialog.h"
#include "Runtime/Internal/Interop/Types/InfiniFrameWindowInitParams.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
// GTK4 signal handlers (defined in WindowSignals.Gtk.cpp)
// ---------------------------------------------------------------------------------------------------------------------
void on_surface_layout(GdkSurface* surface, int width, int height, gpointer self);
void on_window_realized(GtkWidget* widget, gpointer self);
void on_toplevel_state_notify(GObject* object, GParamSpec* pspec, gpointer self);
gboolean on_window_close_request(GtkWindow* window, gpointer self);
void on_widget_destroyed(GtkWidget* widget, gpointer self);
void on_window_active_notify(GObject* object, GParamSpec* pspec, gpointer self);
gboolean on_drop_target_drop(GtkDropTarget* target, const GValue* value, double x, double y, gpointer self);
gboolean on_webview_context_menu(
    WebKitWebView* web_view,
    WebKitContextMenu* context_menu,
    WebKitHitTestResult* hit_test_result,
    gboolean triggered_with_keyboard,
    gpointer user_data
    );
gboolean on_permission_request(WebKitWebView* web_view, WebKitPermissionRequest* request, gpointer user_data);

void InfiniFrameWindow::Impl::InitializeFromParams(const InfiniFrameWindowInitParams* initParams) {
    if (initParams->Title != nullptr) {
        common._windowTitle = initParams->Title;
    } else {
        common._windowTitle = "";
    }

    if (initParams->StartUrl != nullptr) {
        common._startUrl = initParams->StartUrl;
    }
    if (initParams->StartString != nullptr) {
        common._startString = initParams->StartString;
    }
    if (initParams->TemporaryFilesPath != nullptr) {
        _temporaryFilesPath = initParams->TemporaryFilesPath;
    }
    if (initParams->UserAgent != nullptr) {
        common._userAgent = initParams->UserAgent;
    }
    if (initParams->BrowserControlInitParameters != nullptr) {
        common._browserControlInitParameters = initParams->BrowserControlInitParameters;
    }

    common._transparentEnabled = initParams->Transparent;
    common._backgroundColorR = initParams->BackgroundColorR;
    common._backgroundColorG = initParams->BackgroundColorG;
    common._backgroundColorB = initParams->BackgroundColorB;
    common._backgroundColorA = initParams->BackgroundColorA;
    common._contextMenuEnabled = initParams->ContextMenuEnabled;
    common._zoomEnabled = initParams->ZoomEnabled;
    common._devToolsEnabled = initParams->DevToolsEnabled;
    common._grantBrowserPermissions = initParams->GrantBrowserPermissions;
    common._mediaAutoplayEnabled = initParams->MediaAutoplayEnabled;
    common._fileSystemAccessEnabled = initParams->FileSystemAccessEnabled;
    common._webSecurityEnabled = initParams->WebSecurityEnabled;
    common._javascriptClipboardAccessEnabled = initParams->JavascriptClipboardAccessEnabled;
    common._mediaStreamEnabled = initParams->MediaStreamEnabled;
    common._smoothScrollingEnabled = initParams->SmoothScrollingEnabled;
    common._ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
    common._statusBarEnabled = initParams->StatusBarEnabled;
    common._browserShortcutsEnabled = initParams->BrowserShortcutsEnabled;
    common._remoteDebuggingPort = initParams->RemoteDebuggingPort;
    _isFullScreen = initParams->FullScreen;
    if (initParams->DefaultNotificationIcon != nullptr)
        common._defaultNotificationIcon = initParams->DefaultNotificationIcon;

    _zoom = initParams->Zoom;
    _minWidth = initParams->MinWidth;
    _minHeight = initParams->MinHeight;
    _maxWidth = initParams->MaxWidth;
    _maxHeight = initParams->MaxHeight;

    common._webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
    common._resizedCallback = initParams->ResizedHandler;
    common._movedCallback = initParams->MovedHandler;
    common._closingCallback = initParams->ClosingHandler;
    common._closedCallback = initParams->ClosedHandler;
    common._focusInCallback = initParams->FocusInHandler;
    common._focusOutCallback = initParams->FocusOutHandler;
    common._maximizedCallback = initParams->MaximizedHandler;
    common._minimizedCallback = initParams->MinimizedHandler;
    common._restoredCallback = initParams->RestoredHandler;
    common._debugEventCallback = initParams->DebugEventHandler;
    common._customSchemeCallback = initParams->CustomSchemeHandler;
    common._navigationStartingCallback = initParams->NavigationStartingHandler;
    common._fileDroppedCallback = initParams->DragDropHandler;
    common._dragDropEnabled = initParams->DragDropEnabled;

    common._customSchemeNames.clear();
    for (auto* customSchemeName : initParams->CustomSchemeNames) {
        if (customSchemeName == nullptr) {
            continue;
        }
        common._customSchemeNames.emplace_back(customSchemeName);
    }

    common._parent = initParams->ParentInstance;
}

void InfiniFrameWindow::Impl::ConfigureInitialWindow(
    InfiniFrameWindow* window,
    InfiniFrameWindowInitParams* initParams) {
    _window = gtk_window_new();
    common._dialog = std::make_unique<InfiniFrameDialog>();

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
        // GTK4 has no positioning API; the compositor/WM picks the initial location.
    } else if (initParams->CenterOnInitialize) {
        // Handled by ApplyInitialWindowState -> Center().
    } else {
        MoveTo(initParams->Left, initParams->Top);
    }
}

void InfiniFrameWindow::Impl::ApplyInitialWindowState(
    InfiniFrameWindow* window,
    const InfiniFrameWindowInitParams* initParams
    ) {
    window->SetTitle(const_cast<const char*>(common._windowTitle.c_str()));

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
    // GTK4: "close-request" replaces "delete-event" (return TRUE cancels the close).
    g_signal_connect(G_OBJECT(_window), "close-request", G_CALLBACK(on_window_close_request), window);

    // GtkWidget::destroy survives in GTK4 and fires when the toplevel is destroyed.
    g_signal_connect(G_OBJECT(_window), "destroy", G_CALLBACK(on_widget_destroyed), window);

    // GTK4: window activation replaces the GTK3 focus-in/focus-out GdkEvents.
    g_signal_connect(G_OBJECT(_window), "notify::is-active", G_CALLBACK(on_window_active_notify), window);

    // GTK4: GdkSurface signals ("layout", "notify::state") only exist after
    // realization, so they are connected from the realize handler.
    g_signal_connect(G_OBJECT(_window), "realize", G_CALLBACK(on_window_realized), window);

    if (common._dragDropEnabled) {
        // GTK4: GtkDropTarget replaces gtk_drag_dest_set/GtkSelectionData. File drops
        // arrive as a GdkFileList value on the "drop" signal.
        GtkDropTarget* dropTarget = gtk_drop_target_new(GDK_TYPE_FILE_LIST, GDK_ACTION_COPY);
        g_signal_connect(G_OBJECT(dropTarget), "drop", G_CALLBACK(on_drop_target_drop), window);
        gtk_widget_add_controller(GTK_WIDGET(_window), GTK_EVENT_CONTROLLER(dropTarget));
    }
}

void InfiniFrameWindow::Impl::ConnectWebViewSignals(InfiniFrameWindow* window) {
    g_signal_connect(G_OBJECT(_webview), "context-menu", G_CALLBACK(on_webview_context_menu), window);

    g_signal_connect(G_OBJECT(_webview), "permission-request", G_CALLBACK(on_permission_request), window);
}