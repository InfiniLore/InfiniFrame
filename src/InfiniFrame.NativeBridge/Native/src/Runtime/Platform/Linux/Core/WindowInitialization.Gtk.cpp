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

void InfiniFrameWindow::Impl::ConfigureInitialWindow(InfiniFrameWindow* window, InfiniFrameWindowInitParams* initParams) {
    _window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
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
        gtk_window_set_position(GTK_WINDOW(_window), GTK_WIN_POS_NONE);
    } else if (initParams->CenterOnInitialize) {
        gtk_window_set_position(GTK_WINDOW(_window), GTK_WIN_POS_CENTER);
    } else {
        gtk_window_move(GTK_WINDOW(_window), initParams->Left, initParams->Top);
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
    g_signal_connect(G_OBJECT(_window), "configure-event", G_CALLBACK(on_configure_event), window);

    g_signal_connect(G_OBJECT(_window), "window-state-event", G_CALLBACK(on_window_state_event), window);

    g_signal_connect(G_OBJECT(_window), "delete-event", G_CALLBACK(on_widget_deleted), window);

    g_signal_connect(G_OBJECT(_window), "destroy", G_CALLBACK(on_widget_destroyed), window);

    g_signal_connect(G_OBJECT(_window), "focus-in-event", G_CALLBACK(on_focus_in_event), window);

    g_signal_connect(G_OBJECT(_window), "focus-out-event", G_CALLBACK(on_focus_out_event), window);

    if (common._dragDropEnabled) {
        constexpr GtkTargetEntry targets[] = {};
        gtk_drag_dest_set(GTK_WIDGET(_window), GTK_DEST_DEFAULT_ALL, targets, 0, GDK_ACTION_COPY);

        g_signal_connect(
            G_OBJECT(_window), "drag-data-received",
            G_CALLBACK(
                +[](
                GtkWidget* /*widget*/,
                GdkDragContext* context,
                const gint x,
                const gint y,
                GtkSelectionData* data,
                guint /*info*/,
                const guint time,
                const gpointer userData) {
                    auto* instance = static_cast<InfiniFrameWindow*>(userData);

                    gchar** uris = gtk_selection_data_get_uris(data);
                    if (uris) {
                        int count = 0;
                        while (uris[count])
                            count++;

                        std::vector<std::string> paths;
                        for (int i = 0; i < count; i++) {
                            gchar* filename = g_filename_from_uri(uris[i], nullptr, nullptr);
                            if (filename) {
                                paths.push_back(filename);
                                g_free(filename);
                            }
                        }

                        std::vector<const char*> autoStrings;
                        autoStrings.reserve(paths.size());
                        for (const auto& p : paths) {
                            autoStrings.push_back(p.c_str());
                        }

                        instance->InvokeFileDropped(autoStrings.data(), static_cast<int>(autoStrings.size()), x, y);
                    }
                    g_free(uris);
                    gtk_drag_finish(context, TRUE, FALSE, time);
                }), window
            );
    }
}

void InfiniFrameWindow::Impl::ConnectWebViewSignals(InfiniFrameWindow* window) {
    g_signal_connect(G_OBJECT(_webview), "context-menu", G_CALLBACK(on_webview_context_menu), window);

    g_signal_connect(G_OBJECT(_webview), "permission-request", G_CALLBACK(on_permission_request), window);
}
