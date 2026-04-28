#ifdef __linux__
#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameDialog.h"
#include "Core/InfiniFrameWindowImpl.h"
#include "Utils/Common.h"
#include <climits>
#include <mutex>
#include <condition_variable>
#include <X11/Xlib.h>
#include <gio/gio.h>
#include <webkit2/webkit2.h>
#include <JavaScriptCore/JavaScript.h>
#include <sstream>
#include <iomanip>
#include <libnotify/notify.h>
#include <dlfcn.h>
#include <format>
#include <simdjson.h>

std::mutex invokeLockMutex;

struct InvokeWaitInfo {
    ACTION callback;
    std::condition_variable completionNotifier;
    bool isCompleted;
};

// Forward declarations for GTK signal handlers
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
// Platform Impl
// ---------------------------------------------------------------------------------------------------------------------

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
};

// ---------------------------------------------------------------------------------------------------------------------
// Static signal handlers and helpers
// ---------------------------------------------------------------------------------------------------------------------

static gboolean invokeCallback(const gpointer data) {
    auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(data);
    waitInfo->callback();
    {
        std::lock_guard<std::mutex> guard(invokeLockMutex);
        waitInfo->isCompleted = true;
    }
    waitInfo->completionNotifier.notify_one();
    return false;
}

static void HandleWebMessage(
    WebKitUserContentManager* contentManager,
    WebKitJavascriptResult* jsResult,
    const gpointer userData
    ) {
    JSCValue* jsValue = webkit_javascript_result_get_js_value(jsResult);
    if (jsc_value_is_string(jsValue)) {
        AutoString str_value = jsc_value_to_string(jsValue);
        WebMessageReceivedCallback callback = reinterpret_cast<WebMessageReceivedCallback>(userData);
        AutoString originValue = nullptr;

        JSGlobalContextRef context = webkit_javascript_result_get_global_context(jsResult);
        JSStringRef script = JSStringCreateWithUTF8CString("window.location.href");
        JSValueRef locationValue = JSEvaluateScript(context, script, nullptr, nullptr, 0, nullptr);
        JSStringRelease(script);

        if (locationValue != nullptr) {
            JSStringRef locationString = JSValueToStringCopy(context, locationValue, nullptr);
            if (locationString != nullptr) {
                size_t maxBytes = JSStringGetMaximumUTF8CStringSize(locationString);
                originValue = static_cast<AutoString>(g_malloc(maxBytes));
                JSStringGetUTF8CString(locationString, originValue, maxBytes);
                JSStringRelease(locationString);
            }
        }

        if (callback != nullptr) {
            callback(str_value, originValue);
        }

        if (originValue != nullptr)
            g_free(originValue);

        g_free(str_value);
    }
    webkit_javascript_result_unref(jsResult);
}

static void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, const gpointer user_data) {
    WebResourceRequestedCallback webResourceRequestedCallback = reinterpret_cast<WebResourceRequestedCallback>(
        user_data);
    if (webResourceRequestedCallback == nullptr) {
        GError* error = g_error_new_literal(
            G_IO_ERROR,
            G_IO_ERROR_NOT_SUPPORTED,
            "No custom scheme handler is registered.");
        webkit_uri_scheme_request_finish_error(request, error);
        g_error_free(error);
        return;
    }

    const gchar* uri = webkit_uri_scheme_request_get_uri(request);
    int numBytes = 0;
    AutoString contentType = nullptr;
    void* dotNetResponse = webResourceRequestedCallback(const_cast<AutoString>(uri), &numBytes, &contentType);
    GInputStream* stream = g_memory_input_stream_new_from_data(dotNetResponse, numBytes, nullptr);
    webkit_uri_scheme_request_finish(request, reinterpret_cast<GInputStream*>(stream), -1, contentType);
    g_object_unref(stream);
    free(contentType);
}

static std::string escapeJsonString(std::string_view input) {
    std::string result;
    result.reserve(input.size() + 2);

    for (char c : input) {
        switch (c) {
            case '"':
                result += "\\\"";
                break;
            case '\\':
                result += "\\\\";
                break;
            case '\b':
                result += "\\b";
                break;
            case '\f':
                result += "\\f";
                break;
            case '\n':
                result += "\\n";
                break;
            case '\r':
                result += "\\r";
                break;
            case '\t':
                result += "\\t";
                break;
            default:
                if (static_cast<unsigned char>(c) < 0x20) {
                    std::format_to(std::back_inserter(result), "\\u{:04x}", static_cast<unsigned char>(c));
                }
                else {
                    result += c;
                }
        }
    }

    return result;
}

// ---------------------------------------------------------------------------------------------------------------------
// Impl method definitions
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Impl::set_webkit_settings() {
    WebKitSettings* settings = webkit_settings_new_with_settings(
        "allow_modal_dialogs", TRUE,
        "allow_top_navigation_to_data_urls", TRUE,
        "allow_universal_access_from_file_urls", TRUE,
        "enable_back_forward_navigation_gestures", TRUE,
        "enable_media_capabilities", TRUE,
        "enable_mock_capture_devices", TRUE,
        "enable_page_cache", TRUE,
        "enable_webrtc", TRUE,
        "javascript_can_open_windows_automatically", TRUE,

        "allow_file_access_from_file_urls", _fileSystemAccessEnabled,
        "disable_web_security", !_webSecurityEnabled,
        "enable_developer_extras", _devToolsEnabled,
        "enable_media_stream", _mediaStreamEnabled,
        "enable_smooth_scrolling", _smoothScrollingEnabled,
        "javascript_can_access_clipboard", _javascriptClipboardAccessEnabled,
        "media_playback_requires_user_gesture", !_mediaAutoplayEnabled,
        "user_agent", _userAgent.c_str(),

        NULL
        );

    if (!_browserControlInitParameters.empty())
        set_webkit_customsettings(settings);

    WebKitWebsiteDataManager* manager = webkit_web_view_get_website_data_manager(WEBKIT_WEB_VIEW(_webview));
    if (_ignoreCertificateErrorsEnabled)
        webkit_website_data_manager_set_tls_errors_policy(manager, WEBKIT_TLS_ERRORS_POLICY_IGNORE);
    else
        webkit_website_data_manager_set_tls_errors_policy(manager, WEBKIT_TLS_ERRORS_POLICY_FAIL);

    webkit_web_view_set_settings(WEBKIT_WEB_VIEW(_webview), settings);
}

void InfiniFrameWindow::Impl::set_webkit_customsettings(WebKitSettings* settings) {
    try {
        simdjson::ondemand::parser parser;
        auto padded = simdjson::padded_string(_browserControlInitParameters);
        auto doc = parser.iterate(padded);

        for (auto field : doc.get_object()) {
            std::string_view keyView = field.unescaped_key();
            auto value = field.value();

            gchar* propertyName = g_strdup(std::string(keyView).c_str());
            GValue propertyValue = G_VALUE_INIT;
            bool hasValidValue = false;

            switch (value.type()) {
                case simdjson::ondemand::json_type::string: {
                    std::string_view strVal;
                    if (value.get(strVal) == simdjson::SUCCESS) {
                        g_value_init(&propertyValue, G_TYPE_STRING);
                        g_value_set_string(&propertyValue, std::string(strVal).c_str());
                        hasValidValue = true;
                    }
                    break;
                }
                case simdjson::ondemand::json_type::boolean: {
                    bool boolVal;
                    if (value.get(boolVal) == simdjson::SUCCESS) {
                        g_value_init(&propertyValue, G_TYPE_BOOLEAN);
                        g_value_set_boolean(&propertyValue, boolVal);
                        hasValidValue = true;
                    }
                    break;
                }
                case simdjson::ondemand::json_type::number: {
                    int64_t intVal;
                    if (value.get(intVal) == simdjson::SUCCESS) {
                        g_value_init(&propertyValue, G_TYPE_INT);
                        g_value_set_int(&propertyValue, static_cast<int>(intVal));
                        hasValidValue = true;
                    }
                    else {
                        double doubleVal;
                        if (value.get(doubleVal) == simdjson::SUCCESS) {
                            g_value_init(&propertyValue, G_TYPE_DOUBLE);
                            g_value_set_double(&propertyValue, doubleVal);
                            hasValidValue = true;
                        }
                    }
                    break;
                }
                default:
                    // Ignore unsupported JSON value types instead of crashing.
                    break;
            }

            if (hasValidValue) {
                g_object_set_property(G_OBJECT(settings), propertyName, &propertyValue);
                g_value_unset(&propertyValue);
            }

            g_free(propertyName);
        }
    }
    catch (const simdjson::simdjson_error&) {
        // Some callers pass CLI-like strings (e.g. --remote-debugging-port=9222).
        // Ignore non-JSON payloads instead of aborting the process.
    }
}

void InfiniFrameWindow::Impl::AddCustomSchemeHandlers() {
    if (_customSchemeCallback == nullptr)
        return;

    WebKitWebContext* context = webkit_web_context_get_default();
    WebKitSecurityManager* securityManager = webkit_web_context_get_security_manager(context);
    for (const auto& value : _customSchemeNames) {
        if (securityManager != nullptr && g_ascii_strcasecmp(value.c_str(), "app") == 0) {
            // Mirror Windows behavior for embedded static assets:
            // only app:// is explicitly treated as a secure custom scheme.
            webkit_security_manager_register_uri_scheme_as_secure(securityManager, value.c_str());
        }

        webkit_web_context_register_uri_scheme(
            context, value.c_str(),
            reinterpret_cast<WebKitURISchemeRequestCallback>(HandleCustomSchemeRequest),
            reinterpret_cast<void*>(_customSchemeCallback),
            nullptr
            );
    }
}

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

    m_impl->AddCustomSchemeHandlers();

    if (initParams->Transparent)
        SetTransparentEnabled(true);

    if (m_impl->_zoom != 100.0)
        SetZoom(m_impl->_zoom);
}

InfiniFrameWindow::~InfiniFrameWindow() {
    notify_uninit();
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

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const {
    *fullScreen = m_impl->_isFullScreen;
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

unsigned int InfiniFrameWindow::GetScreenDpi() const {
    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    gdouble dpi = gdk_screen_get_resolution(screen);
    if (dpi < 0)
        return 96;
    else
        return static_cast<unsigned int>(dpi);
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
    return g_strdup(title ? title : "");
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
// Navigation
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::NavigateToString(const AutoString content) {
    webkit_web_view_load_html(WEBKIT_WEB_VIEW(m_impl->_webview), content, nullptr);
}

void InfiniFrameWindow::NavigateToUrl(const AutoString url) {
    webkit_web_view_load_uri(WEBKIT_WEB_VIEW(m_impl->_webview), url);
}

void InfiniFrameWindow::Restore() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

static void webview_eval_finished(GObject* object, GAsyncResult* result, gpointer) {
    GError* error = nullptr;
    webkit_web_view_evaluate_javascript_finish(WEBKIT_WEB_VIEW(object), result, &error);
    if (error) {
        g_warning("JavaScript evaluation failed: %s", error->message);
        g_error_free(error);
    }
}

void InfiniFrameWindow::SendWebMessage(const AutoString message) {
    std::string escaped = escapeJsonString(message ? message : "");

    std::string js;
    js.append("__dispatchMessageCallback(\"");
    js.append(escaped);
    js.append("\")");

    webkit_web_view_evaluate_javascript(
        WEBKIT_WEB_VIEW(m_impl->_webview),
        js.c_str(),
        -1,
        nullptr,
        nullptr,
        nullptr,
        webview_eval_finished,
        nullptr
        );
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

void InfiniFrameWindow::SetFullScreen(const bool fullScreen) {
    if (fullScreen)
        gtk_window_fullscreen(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unfullscreen(GTK_WINDOW(m_impl->_window));

    m_impl->_isFullScreen = fullScreen;
}

void InfiniFrameWindow::SetIconFile(const AutoString filename) {
    gtk_window_set_icon_from_file(GTK_WINDOW(m_impl->_window), filename, nullptr);
    m_impl->_iconFileName = filename ? filename : "";
}

void InfiniFrameWindow::SetMinimized(const bool minimized) {
    if (minimized)
        gtk_window_iconify(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_deiconify(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetMaximized(const bool maximized) {
    if (maximized)
        gtk_window_maximize(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unmaximize(GTK_WINDOW(m_impl->_window));
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

// ---------------------------------------------------------------------------------------------------------------------
// Notifications / Event loop
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::ShowNotification(const AutoString title, const AutoString message) {
    NotifyNotification* notification = notify_notification_new(title, message, nullptr);
    notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(m_impl->_window)));
    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

void InfiniFrameWindow::WaitForExit() {
    g_signal_connect(
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

void InfiniFrameWindow::CloseWebView() {
    // Not implemented on Linux
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

void InfiniFrameWindow::GetAllMonitors(const GetAllMonitorsCallback callback) const {
    if (callback) {
        GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
        GdkDisplay* display = gdk_screen_get_display(screen);
        int n = gdk_display_get_n_monitors(display);
        for (int i = 0; i < n; i++) {
            GdkMonitor* monitor = gdk_display_get_monitor(display, i);
            Monitor props = {};
            gdk_monitor_get_geometry(monitor, (GdkRectangle*)&props.monitor);
            gdk_monitor_get_workarea(monitor, (GdkRectangle*)&props.work);
            props.scale = gdk_monitor_get_scale_factor(monitor);
            if (!callback(&props))
                break;
        }
    }
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

void InfiniFrameWindow::Invoke(const ACTION callback) {
    InvokeWaitInfo waitInfo = {};
    waitInfo.callback = callback;
    gdk_threads_add_idle(invokeCallback, &waitInfo);

    std::unique_lock<std::mutex> uLock(invokeLockMutex);
    waitInfo.completionNotifier.wait(
        uLock, [&] {
            return waitInfo.isCompleted;
        }
        );
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
    if (!m_impl->_webview) {
        struct sigaction old_action;
        sigaction(SIGCHLD, nullptr, &old_action);
        WebKitUserContentManager* contentManager = webkit_user_content_manager_new();
        m_impl->_webview = webkit_web_view_new_with_user_content_manager(contentManager);

        m_impl->set_webkit_settings();

        gtk_container_add(GTK_CONTAINER(m_impl->_window), m_impl->_webview);

        WebKitUserScript* script = webkit_user_script_new(
            "window.__receiveCallbackCallbacks = [];"
            "window.__dispatchMessageCallback = function(message) {"
            "	window.__receiveCallbackCallbacks.forEach(function(callback) { callback(message); });"
            "};"
            "window.__infiniframe = window.__infiniframe || {};"
            "window.__infiniframe.host = window.__infiniframe.host || {};"
            "window.__infiniframe.host.postData = window.__infiniframe.host.postData || function(envelope) {"
            "	var message = (typeof envelope === 'string') ? envelope : JSON.stringify(envelope);"
            "	window.webkit.messageHandlers.InfiniFrameInterop.postMessage(message);"
            "};"
            "window.__infiniframe.host.receiveCallback = window.__infiniframe.host.receiveCallback || function(callback) {"
            "	window.__receiveCallbackCallbacks.push(callback);"
            "};"
            "window.__infiniframe.host.getData = window.__infiniframe.host.getData || function(message) {"
            "	var requestId='if_req_'+Date.now().toString(36)+'_'+Math.random().toString(36).slice(2);"
            "	var serializedMessage=(typeof message==='string')?message:JSON.stringify(message);"
            "	return new Promise(function(resolve,reject){"
            "		var callback=function(rawMessage){"
            "			try{"
            "				var envelope=JSON.parse(rawMessage);"
            "				if(!envelope||envelope.id!=='__infiniframe:get:response'||typeof envelope.data!=='string'){return;}"
            "				var payload=JSON.parse(envelope.data);"
            "				if(!payload||payload.requestId!==requestId){return;}"
            "				var callbackIndex=window.__receiveCallbackCallbacks.indexOf(callback);"
            "				if(callbackIndex>=0){window.__receiveCallbackCallbacks.splice(callbackIndex,1);}"
            "				if(payload.success===true){resolve(payload.data||'');}"
            "				else{reject(new Error(payload.error||'Host getData failed.'));}"
            "			}catch(_){ }"
            "		};"
            "		window.__infiniframe.host.receiveCallback(callback);"
            "		window.__infiniframe.host.postData({id:'__infiniframe:get:request',data:{requestId:requestId,message:serializedMessage},version:1});"
            "	});"
            "};"
            "(function(){"
            "	if(window.__infiniframeRegisterBlazorCustomElement){return;}"
            "	window.__infiniframeRegisterBlazorCustomElement=true;"
            "	function toKebabCase(name){"
            "		return String(name).replace(/([a-z0-9])([A-Z])/g,'$1-$2').replace(/_/g,'-').toLowerCase();"
            "	}"
            "	function toParameterValue(rawValue,typeName){"
            "		if(typeName==='bool'||typeName==='boolean'){"
            "			if(rawValue===null){return false;}"
            "			if(rawValue===''){return true;}"
            "			return String(rawValue).toLowerCase()!=='false';"
            "		}"
            "		if(typeName==='number'||typeName==='int'||typeName==='float'||typeName==='double'||typeName==='decimal'){"
            "			var numericValue=Number(rawValue);"
            "			return Number.isNaN(numericValue)?rawValue:numericValue;"
            "		}"
            "		return rawValue;"
            "	}"
            "	window.registerBlazorCustomElement=window.registerBlazorCustomElement||function(identifier,parameterDefinitions){"
            "		if(!window.Blazor||!window.Blazor.rootComponents||!window.Blazor.rootComponents.add){"
            "			console.warn('registerBlazorCustomElement skipped: Blazor.rootComponents is unavailable.');"
            "			return;"
            "		}"
            "		if(!window.customElements||typeof window.customElements.define!=='function'){"
            "			console.warn('registerBlazorCustomElement skipped: customElements API is unavailable.');"
            "			return;"
            "		}"
            "		if(window.customElements.get(identifier)){return;}"
            "		var definitions=Array.isArray(parameterDefinitions)?parameterDefinitions:[];"
            "		var parametersByAttribute={};"
            "		for(var index=0;index<definitions.length;index++){"
            "			var definition=definitions[index];"
            "			if(!definition||!definition.name){continue;}"
            "			var parameterType=String(definition.type||'').toLowerCase();"
            "			if(parameterType==='eventcallback'){continue;}"
            "			var attributeName=toKebabCase(definition.name);"
            "			parametersByAttribute[attributeName]={name:definition.name,type:parameterType};"
            "		}"
            "		var observedAttributes=Object.keys(parametersByAttribute);"
            "		class BlazorCustomElementHost extends HTMLElement{"
            "			static get observedAttributes(){return observedAttributes;}"
            "			constructor(){"
            "				super();"
            "				this._component=null;"
            "				this._isDisconnected=false;"
            "			}"
            "			connectedCallback(){"
            "				this._isDisconnected=false;"
            "				var parameters=this._getCurrentParameters();"
            "				window.Blazor.rootComponents.add(this,identifier,parameters).then((component)=>{"
            "					this._component=component;"
            "					if(this._isDisconnected&&this._component){"
            "						var detachedComponent=this._component;"
            "						this._component=null;"
            "						return detachedComponent.dispose();"
            "					}"
            "					return null;"
            "				}).catch((error)=>{"
            "					console.error('Failed to attach custom element component.',error);"
            "				});"
            "			}"
            "			disconnectedCallback(){"
            "				this._isDisconnected=true;"
            "				var component=this._component;"
            "				this._component=null;"
            "				if(component&&typeof component.dispose==='function'){"
            "					Promise.resolve(component.dispose()).catch(function(){});"
            "				}"
            "			}"
            "			attributeChangedCallback(attributeName,oldValue,newValue){"
            "				if(oldValue===newValue){return;}"
            "				if(!this._component||typeof this._component.setParameters!=='function'){return;}"
            "				var parameterInfo=parametersByAttribute[String(attributeName).toLowerCase()];"
            "				if(!parameterInfo){return;}"
            "				var nextParameters={};"
            "				nextParameters[parameterInfo.name]=toParameterValue(newValue,parameterInfo.type);"
            "				var updateResult=this._component.setParameters(nextParameters);"
            "				if(updateResult&&typeof updateResult.catch==='function'){"
            "					updateResult.catch(function(error){"
            "						console.error('Failed to update custom element parameters.',error);"
            "					});"
            "				}"
            "			}"
            "			_getCurrentParameters(){"
            "				var parameters={};"
            "				for(var index=0;index<observedAttributes.length;index++){"
            "					var attributeName=observedAttributes[index];"
            "					if(!this.hasAttribute(attributeName)){continue;}"
            "					var parameterInfo=parametersByAttribute[attributeName];"
            "					parameters[parameterInfo.name]=toParameterValue(this.getAttribute(attributeName),parameterInfo.type);"
            "				}"
            "				return parameters;"
            "			}"
            "		}"
            "		window.customElements.define(identifier,BlazorCustomElementHost);"
            "	};"
            "	function shouldAutoRegisterMissingInitializerCustomElements(){"
            "		return true;"
            "	}"
            "	function autoRegisterMissingInitializerCustomElements(componentDefinitionsByIdentifier,identifiersByInitializer){"
            "		if(!shouldAutoRegisterMissingInitializerCustomElements()){return;}"
            "		if(typeof window.registerBlazorCustomElement!=='function'){return;}"
            "		var initializedIdentifiers={};"
            "		for(var initializerIdentifiers of Object.values(identifiersByInitializer||{})){"
            "			if(!Array.isArray(initializerIdentifiers)){continue;}"
            "			for(var identifier of initializerIdentifiers){"
            "				initializedIdentifiers[identifier]=true;"
            "			}"
            "		}"
            "		for(var entry of Object.entries(componentDefinitionsByIdentifier||{})){"
            "			var identifier=entry[0];"
            "			if(initializedIdentifiers[identifier]){continue;}"
            "			window.registerBlazorCustomElement(identifier,entry[1]);"
            "		}"
            "	}"
            "	function patchAttachWebRendererInteropIfAvailable(){"
            "		var blazor=window.Blazor;"
            "		if(!blazor||!blazor._internal||typeof blazor._internal.attachWebRendererInterop!=='function'){return false;}"
            "		if(blazor._internal.__infiniframeAttachWebRendererInteropPatched){return true;}"
            "		var originalAttach=blazor._internal.attachWebRendererInterop;"
            "		blazor._internal.attachWebRendererInterop=function(rendererId,interopMethods,componentDefinitionsByIdentifier,identifiersByInitializer){"
            "			var attachResult=originalAttach.apply(this,arguments);"
            "			autoRegisterMissingInitializerCustomElements(componentDefinitionsByIdentifier,identifiersByInitializer);"
            "			return attachResult;"
            "		};"
            "		blazor._internal.__infiniframeAttachWebRendererInteropPatched=true;"
            "		return true;"
            "	}"
            "	if(!patchAttachWebRendererInteropIfAvailable()){"
            "		var blazorDescriptor=Object.getOwnPropertyDescriptor(window,'Blazor');"
            "		if(!blazorDescriptor||blazorDescriptor.configurable){"
            "			var blazorValue=window.Blazor;"
            "			Object.defineProperty(window,'Blazor',{"
            "				configurable:true,"
            "				enumerable:true,"
            "				get:function(){return blazorValue;},"
            "				set:function(value){"
            "					blazorValue=value;"
            "					patchAttachWebRendererInteropIfAvailable();"
            "				}"
            "			});"
            "			if(blazorValue){"
            "				patchAttachWebRendererInteropIfAvailable();"
            "			}"
            "		}"
            "	}"
            "})();",
            WEBKIT_USER_CONTENT_INJECT_TOP_FRAME, WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START, nullptr, nullptr
            );
        webkit_user_content_manager_add_script(contentManager, script);
        webkit_user_script_unref(script);

        g_signal_connect(
            contentManager, "script-message-received::InfiniFrameInterop",
            G_CALLBACK(HandleWebMessage),
            reinterpret_cast<void*>(m_impl->_webMessageReceivedCallback)
            );
        webkit_user_content_manager_register_script_message_handler(contentManager, "InfiniFrameInterop");

        if (!m_impl->_startUrl.empty())
            NavigateToUrl(const_cast<AutoString>(m_impl->_startUrl.c_str()));
        else if (!m_impl->_startString.empty())
            NavigateToString(const_cast<AutoString>(m_impl->_startString.c_str()));
        else {
            GtkWidget* dialog = gtk_message_dialog_new(
                nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
                "Neither StartUrl nor StartString was specified"
                );
            gtk_dialog_run(GTK_DIALOG(dialog));
            gtk_widget_destroy(dialog);
            sigaction(SIGCHLD, &old_action, nullptr);
            return;
        }
        sigaction(SIGCHLD, &old_action, nullptr);
    }

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
