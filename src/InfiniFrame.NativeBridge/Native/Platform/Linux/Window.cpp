#ifdef __linux__
#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameDialog.h"
#include "Utils/Common.h"
#include "Window.Gtk.Internal.h"
#include <climits>
#include <X11/Xlib.h>
#include <sstream>
#include <iomanip>
#include <libnotify/notify.h>
#include <dlfcn.h>
#include <webkit2/webkit2.h>

// Forward declarations for GTK signal handlers
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
void on_webview_load_changed(WebKitWebView* web_view, WebKitLoadEvent load_event, gpointer user_data);
gboolean on_webview_load_failed(
    WebKitWebView* web_view,
    WebKitLoadEvent load_event,
    gchar* failing_uri,
    GError* error,
    gpointer user_data
    );
void on_webview_process_terminated(
    WebKitWebView* web_view,
    WebKitWebProcessTerminationReason reason,
    gpointer user_data
    );
void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data);

// ---------------------------------------------------------------------------------------------------------------------
// Constructor / Destructor
// ---------------------------------------------------------------------------------------------------------------------

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) :
    m_impl(std::make_unique<Impl>()) {
    XInitThreads();
    gtk_init(nullptr, nullptr);
    notify_init(initParams->Title);

    if (initParams->Size != sizeof(InfiniFrameInitParams)) {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "Initial parameters passed are %i bytes, but expected %lu bytes.",
            initParams->Size, sizeof(InfiniFrameInitParams)
            );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        exit(0);
    }

    m_impl->_windowTitle = initParams->Title ? initParams->Title : "";

    if (initParams->StartUrl != nullptr)
        m_impl->_startUrl = initParams->StartUrl;

    if (initParams->StartString != nullptr)
        m_impl->_startString = initParams->StartString;

    if (initParams->TemporaryFilesPath != nullptr)
        m_impl->_temporaryFilesPath = initParams->TemporaryFilesPath;

    if (initParams->UserAgent != nullptr)
        m_impl->_userAgent = initParams->UserAgent;

    if (initParams->BrowserControlInitParameters != nullptr)
        m_impl->_browserControlInitParameters = initParams->BrowserControlInitParameters;

    m_impl->_transparentEnabled = initParams->Transparent;
    m_impl->_contextMenuEnabled = initParams->ContextMenuEnabled;
    m_impl->_zoomEnabled = initParams->ZoomEnabled;
    m_impl->_devToolsEnabled = initParams->DevToolsEnabled;
    m_impl->_grantBrowserPermissions = initParams->GrantBrowserPermissions;
    m_impl->_mediaAutoplayEnabled = initParams->MediaAutoplayEnabled;
    m_impl->_fileSystemAccessEnabled = initParams->FileSystemAccessEnabled;
    m_impl->_webSecurityEnabled = initParams->WebSecurityEnabled;
    m_impl->_javascriptClipboardAccessEnabled = initParams->JavascriptClipboardAccessEnabled;
    m_impl->_mediaStreamEnabled = initParams->MediaStreamEnabled;
    m_impl->_smoothScrollingEnabled = initParams->SmoothScrollingEnabled;
    m_impl->_ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
    m_impl->_isFullScreen = initParams->FullScreen;

    m_impl->_zoom = initParams->Zoom;
    m_impl->_minWidth = initParams->MinWidth;
    m_impl->_minHeight = initParams->MinHeight;
    m_impl->_maxWidth = initParams->MaxWidth;
    m_impl->_maxHeight = initParams->MaxHeight;

    m_impl->_webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
    m_impl->_resizedCallback = initParams->ResizedHandler;
    m_impl->_movedCallback = initParams->MovedHandler;
    m_impl->_closingCallback = initParams->ClosingHandler;
    m_impl->_closedCallback = initParams->ClosedHandler;
    m_impl->_focusInCallback = initParams->FocusInHandler;
    m_impl->_focusOutCallback = initParams->FocusOutHandler;
    m_impl->_maximizedCallback = initParams->MaximizedHandler;
    m_impl->_minimizedCallback = initParams->MinimizedHandler;
    m_impl->_restoredCallback = initParams->RestoredHandler;
    m_impl->_customSchemeCallback = initParams->CustomSchemeHandler;

    for (int i = 0; i < 16; ++i) {
        if (initParams->CustomSchemeNames[i] != nullptr)
            m_impl->_customSchemeNames.emplace_back(initParams->CustomSchemeNames[i]);
    }

    m_impl->_parent = initParams->ParentInstance;

    m_impl->_window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
    m_impl->_dialog = std::make_unique<InfiniFrameDialog>();

    if (initParams->FullScreen)
        SetFullScreen(true);
    else {
        if (initParams->Width > initParams->MaxWidth)
            initParams->Width = initParams->MaxWidth;
        if (initParams->Height > initParams->MaxHeight)
            initParams->Height = initParams->MaxHeight;
        if (initParams->Width < initParams->MinWidth)
            initParams->Width = initParams->MinWidth;
        if (initParams->Height < initParams->MinHeight)
            initParams->Height = initParams->MinHeight;

        if (initParams->UseOsDefaultSize)
            gtk_window_set_default_size(GTK_WINDOW(m_impl->_window), -1, -1);
        else
            gtk_window_set_default_size(GTK_WINDOW(m_impl->_window), initParams->Width, initParams->Height);

        SetMinSize(initParams->MinWidth, initParams->MinHeight);
        SetMaxSize(initParams->MaxWidth, initParams->MaxHeight);

        if (initParams->UseOsDefaultLocation)
            gtk_window_set_position(GTK_WINDOW(m_impl->_window), GTK_WIN_POS_NONE);
        else if (initParams->CenterOnInitialize && !initParams->FullScreen)
            gtk_window_set_position(GTK_WINDOW(m_impl->_window), GTK_WIN_POS_CENTER);
        else
            gtk_window_move(GTK_WINDOW(m_impl->_window), initParams->Left, initParams->Top);
    }

    SetTitle(const_cast<AutoString>(m_impl->_windowTitle.c_str()));

    if (initParams->Chromeless)
        gtk_window_set_decorated(GTK_WINDOW(m_impl->_window), false);

    if (initParams->WindowIconFile != nullptr && strlen(initParams->WindowIconFile) > 0)
        SetIconFile(initParams->WindowIconFile);

    if (initParams->CenterOnInitialize)
        Center();

    if (initParams->Minimized)
        SetMinimized(true);

    if (initParams->Maximized)
        SetMaximized(true);

    if (!initParams->Resizable)
        SetResizable(false);

    if (initParams->Topmost)
        SetTopmost(true);

    g_signal_connect(
        G_OBJECT(m_impl->_window), "configure-event",
        G_CALLBACK(on_configure_event), this
        );

    g_signal_connect(
        G_OBJECT(m_impl->_window), "window-state-event",
        G_CALLBACK(on_window_state_event), this
        );

    g_signal_connect(
        G_OBJECT(m_impl->_window), "delete-event",
        G_CALLBACK(on_widget_deleted), this
        );
    
    g_signal_connect(
        G_OBJECT(m_impl->_window), "destroy",
        G_CALLBACK(on_widget_destroyed), this
    );

    // Register custom schemes before first navigation to avoid first-load races.
    m_impl->AddCustomSchemeHandlers();

    Show(false);

    g_signal_connect(
        G_OBJECT(m_impl->_window), "focus-in-event",
        G_CALLBACK(on_focus_in_event), this
        );

    g_signal_connect(
        G_OBJECT(m_impl->_window), "focus-out-event",
        G_CALLBACK(on_focus_out_event), this
        );

    g_signal_connect(
        G_OBJECT(m_impl->_webview), "context-menu",
        G_CALLBACK(on_webview_context_menu), this
        );

    g_signal_connect(
        G_OBJECT(m_impl->_webview), "permission-request",
        G_CALLBACK(on_permission_request), this
        );

    if (initParams->Transparent)
        SetTransparentEnabled(true);

    if (m_impl->_zoom != 100.0)
        SetZoom(m_impl->_zoom);
}

InfiniFrameWindow::~InfiniFrameWindow() {
    notify_uninit();
    gtk_widget_destroy(m_impl->_window);
}

#endif
