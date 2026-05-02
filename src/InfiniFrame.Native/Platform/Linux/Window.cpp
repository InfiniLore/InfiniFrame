#ifdef __linux__
#include "Core/InfiniFrameDialog.h"
#include "Interop/InitParamsReader.h"
#include "Platform/Linux/WindowImpl.Gtk.h"
#include "Utils/Common.h"

#include <cstring>
#include <stdexcept>
#include <X11/Xlib.h>
#include <webkit2/webkit2.h>

// Forward declarations for GTK signal handlers
static void disconnect_signal(GObject* instance, gulong& handlerId) noexcept;

gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, gpointer self);
gboolean on_window_state_event(GtkWidget* widget, GdkEventWindowState* event, gpointer self);
gboolean on_widget_deleted(GtkWidget* widget, GdkEvent* event, gpointer self);
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

// ---------------------------------------------------------------------------------------------------------------------
// Constructor / Destructor
// ---------------------------------------------------------------------------------------------------------------------

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) :
    m_impl(std::make_unique<Impl>()) {
    const auto initParamsReader = InfiniFrame::Native::Interop::InitParamsReader(initParams);
    initParamsReader.RequireStartContent();

    XInitThreads();
    gtk_init(nullptr, nullptr);
    m_impl->InitializeNotifications(initParams->Title);

    m_impl->_windowTitle = initParams->Title ? initParams->Title : "";

    if (initParams->StartUrl != nullptr)
        m_impl->_startUrl = initParams->StartUrl;

    if (initParams->StartString != nullptr)
        m_impl->_startString = initParams->StartString;

    if (m_impl->_startUrl.empty() && m_impl->_startString.empty())
        throw std::invalid_argument("Either StartUrl or StartString must be specified.");

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

    m_impl->_configureEventHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_window), "configure-event",
        G_CALLBACK(on_configure_event), this
        );

    m_impl->_windowStateEventHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_window), "window-state-event",
        G_CALLBACK(on_window_state_event), this
        );

    m_impl->_deleteEventHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_window), "delete-event",
        G_CALLBACK(on_widget_deleted), this
        );

    Show(false);

    m_impl->_focusInEventHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_window), "focus-in-event",
        G_CALLBACK(on_focus_in_event), this
        );

    m_impl->_focusOutEventHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_window), "focus-out-event",
        G_CALLBACK(on_focus_out_event), this
        );

    m_impl->_contextMenuHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_webview), "context-menu",
        G_CALLBACK(on_webview_context_menu), this
        );

    m_impl->_permissionRequestHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_webview), "permission-request",
        G_CALLBACK(on_permission_request), this
        );

    m_impl->AddCustomSchemeHandlers();

    if (initParams->Transparent)
        SetTransparentEnabled(true);

    if (m_impl->_zoom != 100.0)
        SetZoom(m_impl->_zoom);
}

InfiniFrameWindow::~InfiniFrameWindow() {
    m_impl->DisconnectSignalHandlers();
    m_impl->ShutdownNotifications();
    gtk_widget_destroy(m_impl->_window);
}

// ---------------------------------------------------------------------------------------------------------------------
// Window Operations
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Center() {
    gint windowWidth, windowHeight;
    gtk_window_get_size(GTK_WINDOW(m_impl->_window), &windowWidth, &windowHeight);

    GdkRectangle screen = {0};

    GdkDisplay* d = gdk_display_get_default();
    if (d == nullptr) {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "gdk_display_get_default() returned NULL"
            );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        return;
    }

    GdkMonitor* m = gdk_display_get_primary_monitor(d);
    if (m == nullptr) {
        m = gdk_display_get_monitor(d, 0);
        if (m == nullptr) {
            GtkWidget* dialog = gtk_message_dialog_new(
                nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
                "gdk_display_get_primary_monitor() returned NULL"
                );
            gtk_dialog_run(GTK_DIALOG(dialog));
            gtk_widget_destroy(dialog);
            return;
        }
    }

    gdk_monitor_get_geometry(m, &screen);

    gtk_window_move(
        GTK_WINDOW(m_impl->_window),
        (screen.width - windowWidth) / 2,
        (screen.height - windowHeight) / 2
        );
}

void InfiniFrameWindow::ClearBrowserAutoFill() {
    // TODO
}

void InfiniFrameWindow::Close() {
    gtk_window_close(GTK_WINDOW(m_impl->_window));
}

// ---------------------------------------------------------------------------------------------------------------------
// Get Properties
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const {
    *enabled = m_impl->_transparentEnabled;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const {
    *enabled = m_impl->_contextMenuEnabled;
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const {
    *enabled = m_impl->_zoomEnabled;
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const {
    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    *enabled = webkit_settings_get_enable_developer_extras(settings);
}

void InfiniFrameWindow::GetGrantBrowserPermissions(bool* grant) const {
    *grant = m_impl->_grantBrowserPermissions;
}

AutoString InfiniFrameWindow::GetUserAgent() const {
    return AllocateStringCopy(m_impl->_userAgent);
}

void InfiniFrameWindow::GetMediaAutoplayEnabled(bool* enabled) const {
    *enabled = m_impl->_mediaAutoplayEnabled;
}

void InfiniFrameWindow::GetFileSystemAccessEnabled(bool* enabled) const {
    *enabled = m_impl->_fileSystemAccessEnabled;
}

void InfiniFrameWindow::GetWebSecurityEnabled(bool* enabled) const {
    *enabled = m_impl->_webSecurityEnabled;
}

void InfiniFrameWindow::GetJavascriptClipboardAccessEnabled(bool* enabled) const {
    *enabled = m_impl->_javascriptClipboardAccessEnabled;
}

void InfiniFrameWindow::GetMediaStreamEnabled(bool* enabled) const {
    *enabled = m_impl->_mediaStreamEnabled;
}

void InfiniFrameWindow::GetSmoothScrollingEnabled(bool* enabled) const {
    *enabled = m_impl->_smoothScrollingEnabled;
}

void InfiniFrameWindow::GetIgnoreCertificateErrorsEnabled(bool* enabled) const {
    *enabled = m_impl->_ignoreCertificateErrorsEnabled;
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const {
    GdkWindow* gdk_window = gtk_widget_get_window(GTK_WIDGET(m_impl->_window));
    GdkWindowState flags = gdk_window_get_state(gdk_window);
    *isMaximized = flags & GDK_WINDOW_STATE_MAXIMIZED;
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const {
    GdkWindow* gdk_window = gtk_widget_get_window(GTK_WIDGET(m_impl->_window));
    GdkWindowState flags = gdk_window_get_state(gdk_window);
    *isMinimized = flags & GDK_WINDOW_STATE_ICONIFIED;
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const {
    gtk_window_get_position(GTK_WINDOW(m_impl->_window), x, y);
}

void InfiniFrameWindow::GetResizable(bool* resizable) const {
    *resizable = gtk_window_get_resizable(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::GetSize(int* width, int* height) const {
    gtk_window_get_size(GTK_WINDOW(m_impl->_window), width, height);
}

void InfiniFrameWindow::GetMaxSize(int* width, int* height) const {
    if (width)
        *width = m_impl->_maxWidth;
    if (height)
        *height = m_impl->_maxHeight;
}

void InfiniFrameWindow::GetMinSize(int* width, int* height) const {
    if (width)
        *width = m_impl->_minWidth;
    if (height)
        *height = m_impl->_minHeight;
}

AutoString InfiniFrameWindow::GetTitle() const {
    const char* title = gtk_window_get_title(GTK_WINDOW(m_impl->_window));
    return InfiniFrame::Native::Interop::AllocateNativeStringCopy(title);
}

void InfiniFrameWindow::GetTopmost(bool* topmost) const {
    GdkWindow* gdk_window = gtk_widget_get_window(GTK_WIDGET(m_impl->_window));
    GdkWindowState flags = gdk_window_get_state(gdk_window);
    *topmost = flags & GDK_WINDOW_STATE_ABOVE;
}

void InfiniFrameWindow::GetZoom(int* zoom) const {
    double rawValue = webkit_web_view_get_zoom_level(WEBKIT_WEB_VIEW(m_impl->_webview));
    rawValue = (rawValue * 100.0) + 0.5;
    *zoom = static_cast<int>(rawValue);
}

void InfiniFrameWindow::GetFocused(bool* isFocused) const {
    *isFocused = gtk_window_is_active(GTK_WINDOW(m_impl->_window));
}

AutoString InfiniFrameWindow::GetIconFileName() const {
    return AllocateStringCopy(m_impl->_iconFileName);
}

// ---------------------------------------------------------------------------------------------------------------------
// Set Properties
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::SetContextMenuEnabled(const bool enabled) {
    m_impl->_contextMenuEnabled = enabled;
}

void InfiniFrameWindow::SetZoomEnabled(bool enabled) {
    // Not implemented on Linux
}

void InfiniFrameWindow::SetDevToolsEnabled(const bool enabled) {
    m_impl->_devToolsEnabled = enabled;
    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_enable_developer_extras(settings, m_impl->_devToolsEnabled);
}

void InfiniFrameWindow::SetIconFile(const AutoString filename) {
    gtk_window_set_icon_from_file(GTK_WINDOW(m_impl->_window), filename, nullptr);
    m_impl->_iconFileName = filename ? filename : "";
}

void InfiniFrameWindow::SetPosition(const int x, const int y) {
    gtk_window_move(GTK_WINDOW(m_impl->_window), x, y);
}

void InfiniFrameWindow::SetResizable(const bool resizable) {
    gtk_window_set_resizable(GTK_WINDOW(m_impl->_window), resizable);
}

void InfiniFrameWindow::SetMinSize(const int width, const int height) {
    m_impl->_minWidth = width;
    m_impl->_minHeight = height;
    m_impl->_hints.min_width = width;
    m_impl->_hints.min_height = height;

    gtk_window_set_geometry_hints(
        GTK_WINDOW(m_impl->_window),
        nullptr,
        &m_impl->_hints,
        (GdkWindowHints)(GDK_HINT_MIN_SIZE | GDK_HINT_MAX_SIZE)
        );
}

void InfiniFrameWindow::SetMaxSize(const int width, const int height) {
    m_impl->_maxWidth = width;
    m_impl->_maxHeight = height;
    m_impl->_hints.max_width = width;
    m_impl->_hints.max_height = height;

    gtk_window_set_geometry_hints(
        GTK_WINDOW(m_impl->_window),
        nullptr,
        &m_impl->_hints,
        (GdkWindowHints)(GDK_HINT_MIN_SIZE | GDK_HINT_MAX_SIZE)
        );
}

void InfiniFrameWindow::SetSize(const int width, const int height) {
    gtk_window_resize(GTK_WINDOW(m_impl->_window), width, height);
}

void InfiniFrameWindow::SetTitle(const AutoString title) {
    gtk_window_set_title(GTK_WINDOW(m_impl->_window), title);
}

void InfiniFrameWindow::SetTopmost(const bool topmost) {
    gtk_window_set_keep_above(GTK_WINDOW(m_impl->_window), topmost);
}

void InfiniFrameWindow::SetZoom(const int zoom) {
    double newZoom = zoom / 100.0;
    webkit_web_view_set_zoom_level(WEBKIT_WEB_VIEW(m_impl->_webview), newZoom);
}

void InfiniFrameWindow::SetFocused() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetTransparentEnabled(const bool enabled) {
    m_impl->_transparentEnabled = enabled;

    gtk_window_set_decorated(GTK_WINDOW(m_impl->_window), !enabled);

    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    GdkVisual* rgba_visual = gdk_screen_get_rgba_visual(screen);
    if (rgba_visual) {
        gtk_widget_set_visual(GTK_WIDGET(m_impl->_window), rgba_visual);
        gtk_widget_set_app_paintable(GTK_WIDGET(m_impl->_window), true);

        GdkRGBA color;
        webkit_web_view_get_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &color);
        color.alpha = enabled ? 0 : 1;
        webkit_web_view_set_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &color);
    }
}

void InfiniFrameWindow::WaitForExit() {
    m_impl->_destroyHandlerId = g_signal_connect(
        G_OBJECT(m_impl->_window), "destroy",
        G_CALLBACK(
            +[](GtkWidget*, gpointer) {
                gtk_main_quit();
            }
            ),
        nullptr
        );
    gtk_main();
}

// ---------------------------------------------------------------------------------------------------------------------
// Callbacks
// ---------------------------------------------------------------------------------------------------------------------

InfiniFrameDialog* InfiniFrameWindow::GetDialog() const {
    return m_impl->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme) {
    if (scheme)
        m_impl->_customSchemeNames.emplace_back(scheme);
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback) {
    m_impl->_closingCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback) {
    m_impl->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback) {
    m_impl->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback) {
    m_impl->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback) {
    m_impl->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback) {
    m_impl->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback) {
    m_impl->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback) {
    m_impl->_minimizedCallback = callback;
}

[[nodiscard]] bool InfiniFrameWindow::InvokeClose() const noexcept {
    if (m_impl->_closingCallback)
        return m_impl->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept {
    if (m_impl->_focusInCallback)
        m_impl->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept {
    if (m_impl->_focusOutCallback)
        m_impl->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept {
    if (m_impl->_movedCallback)
        m_impl->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept {
    if (m_impl->_resizedCallback)
        m_impl->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept {
    if (m_impl->_maximizedCallback)
        m_impl->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept {
    if (m_impl->_restoredCallback)
        m_impl->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept {
    if (m_impl->_minimizedCallback)
        m_impl->_minimizedCallback();
}

// ---------------------------------------------------------------------------------------------------------------------
// Private methods
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Show(bool isAlreadyShown) {
    (void)isAlreadyShown;

    if (!m_impl->EnsureWebView())
        return;

    gtk_widget_show_all(m_impl->_window);
}

void InfiniFrameWindow::AttachWebView() {
    // On Linux, WebView is attached in Show()
}

void InfiniFrameWindow::OnConfigureEvent(int x, int y, int width, int height) {
    if (m_impl->_lastLeft != x || m_impl->_lastTop != y) {
        InvokeMove(x, y);
        m_impl->_lastLeft = x;
        m_impl->_lastTop = y;
    }

    if (m_impl->_lastHeight != height || m_impl->_lastWidth != width) {
        InvokeResize(width, height);
        m_impl->_lastWidth = width;
        m_impl->_lastHeight = height;
    }
}

void InfiniFrameWindow::OnWindowStateEvent(GdkWindowState newState) {
    if (newState & GDK_WINDOW_STATE_MAXIMIZED) {
        InvokeMaximized();
    }
    else if ((newState & GDK_WINDOW_STATE_ICONIFIED) || !gtk_widget_get_mapped(m_impl->_window)) {
        InvokeMinimized();
    }
    else if (!(newState & GDK_WINDOW_STATE_MAXIMIZED) && !(newState & GDK_WINDOW_STATE_ICONIFIED)) {
        InvokeRestored();
    }
}

// ---------------------------------------------------------------------------------------------------------------------
// GTK Signal Handlers
// ---------------------------------------------------------------------------------------------------------------------

static void disconnect_signal(GObject* instance, gulong& handlerId) noexcept {
    if (instance == nullptr || handlerId == 0)
        return;

    g_signal_handler_disconnect(instance, handlerId);
    handlerId = 0;
}

void InfiniFrameWindow::Impl::DisconnectSignalHandlers() noexcept {
    disconnect_signal(G_OBJECT(_window), _configureEventHandlerId);
    disconnect_signal(G_OBJECT(_window), _windowStateEventHandlerId);
    disconnect_signal(G_OBJECT(_window), _deleteEventHandlerId);
    disconnect_signal(G_OBJECT(_window), _focusInEventHandlerId);
    disconnect_signal(G_OBJECT(_window), _focusOutEventHandlerId);
    disconnect_signal(G_OBJECT(_window), _destroyHandlerId);

    if (_webview == nullptr)
        return;

    disconnect_signal(G_OBJECT(_webview), _contextMenuHandlerId);
    disconnect_signal(G_OBJECT(_webview), _permissionRequestHandlerId);

    WebKitUserContentManager* contentManager = webkit_web_view_get_user_content_manager(WEBKIT_WEB_VIEW(_webview));
    if (contentManager == nullptr)
        return;

    disconnect_signal(G_OBJECT(contentManager), _webMessageReceivedHandlerId);
    webkit_user_content_manager_unregister_script_message_handler(contentManager, "infiniFrameInterop");
}

gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    if (event->type == GDK_CONFIGURE) {
        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        instance->OnConfigureEvent(
            event->configure.x, event->configure.y,
            event->configure.width, event->configure.height
            );
    }
    return FALSE;
}

gboolean on_window_state_event(GtkWidget* widget, GdkEventWindowState* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->OnWindowStateEvent(event->new_window_state);
    return TRUE;
}

gboolean on_widget_deleted(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    return instance->InvokeClose();
}

gboolean on_focus_in_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->InvokeFocusIn();
    return FALSE;
}

gboolean on_focus_out_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->InvokeFocusOut();
    return FALSE;
}

gboolean on_webview_context_menu(
    WebKitWebView* web_view,
    GtkWidget* default_menu,
    WebKitHitTestResult* hit_test_result,
    gboolean triggered_with_keyboard,
    const gpointer self
    ) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    bool contextMenuEnabled = false;
    instance->GetContextMenuEnabled(&contextMenuEnabled);
    return !contextMenuEnabled;
}

gboolean on_permission_request(WebKitWebView* web_view, WebKitPermissionRequest* request, gpointer user_data) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
    bool grant = false;
    instance->GetGrantBrowserPermissions(&grant);
    if (grant)
        webkit_permission_request_allow(request);
    else
        webkit_permission_request_deny(request);
    return TRUE;
}

#endif
