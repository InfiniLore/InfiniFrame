#ifdef __linux__
#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameDialog.h"
#include "Utils/Common.h"
#include "Window.Gtk.Internal.h"
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
#include "Embedded/Embedded.h"

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

static bool linux_webview_diagnostics_enabled() {
    const char* value = g_getenv("INFINIFRAME_LINUX_WEBVIEW_DIAGNOSTICS");
    return value != nullptr && value[0] != '\0' && g_strcmp0(value, "0") != 0;
}

static const char* webkit_load_event_to_string(WebKitLoadEvent event) {
    switch (event) {
        case WEBKIT_LOAD_STARTED:
            return "started";
        case WEBKIT_LOAD_REDIRECTED:
            return "redirected";
        case WEBKIT_LOAD_COMMITTED:
            return "committed";
        case WEBKIT_LOAD_FINISHED:
            return "finished";
        default:
            return "unknown";
    }
}

static const char* webkit_termination_reason_to_string(WebKitWebProcessTerminationReason reason) {
    switch (reason) {
        case WEBKIT_WEB_PROCESS_CRASHED:
            return "crashed";
        case WEBKIT_WEB_PROCESS_EXCEEDED_MEMORY_LIMIT:
            return "exceeded-memory-limit";
        case WEBKIT_WEB_PROCESS_TERMINATED_BY_API:
            return "terminated-by-api";
        default:
            return "unknown";
    }
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

// ---------------------------------------------------------------------------------------------------------------------
// Window Operations
// ---------------------------------------------------------------------------------------------------------------------

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
        gtk_widget_set_hexpand(m_impl->_webview, TRUE);
        gtk_widget_set_vexpand(m_impl->_webview, TRUE);

        auto js = Embedded::InfiniFrameJsUtf8();

        WebKitUserScript* script = webkit_user_script_new(
            js.c_str(),
            WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES,
            WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START,
            nullptr,
            nullptr
        );
        
        webkit_user_content_manager_add_script(contentManager, script);
        webkit_user_script_unref(script);

        g_signal_connect(
            contentManager, "script-message-received::infiniFrameInterop",
            G_CALLBACK(HandleWebMessage),
            reinterpret_cast<void*>(m_impl->_webMessageReceivedCallback)
            );
        webkit_user_content_manager_register_script_message_handler(contentManager, "infiniFrameInterop");

        g_signal_connect(
            G_OBJECT(m_impl->_webview), "load-changed",
            G_CALLBACK(on_webview_load_changed), this
            );
        g_signal_connect(
            G_OBJECT(m_impl->_webview), "load-failed",
            G_CALLBACK(on_webview_load_failed), this
            );
        g_signal_connect(
            G_OBJECT(m_impl->_webview), "web-process-terminated",
            G_CALLBACK(on_webview_process_terminated), this
            );
        g_signal_connect(
            G_OBJECT(m_impl->_webview), "size-allocate",
            G_CALLBACK(on_webview_size_allocate), this
            );

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

void on_widget_destroyed(GtkWidget* widget, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->InvokeClosed();
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

void on_webview_load_changed(WebKitWebView* web_view, WebKitLoadEvent load_event, gpointer user_data) {
    if (!linux_webview_diagnostics_enabled())
        return;

    const char* uri = webkit_web_view_get_uri(web_view);
    g_message(
        "[InfiniFrame/Linux] WebKit load-changed: event=%s uri=%s",
        webkit_load_event_to_string(load_event),
        uri ? uri : "<null>"
        );
}

gboolean on_webview_load_failed(
    WebKitWebView* web_view,
    WebKitLoadEvent load_event,
    gchar* failing_uri,
    GError* error,
    gpointer user_data
    ) {
    if (!linux_webview_diagnostics_enabled())
        return FALSE;

    g_warning(
        "[InfiniFrame/Linux] WebKit load-failed: event=%s uri=%s error=%s",
        webkit_load_event_to_string(load_event),
        failing_uri ? failing_uri : "<null>",
        error ? error->message : "<null>"
        );
    return FALSE;
}

void on_webview_process_terminated(
    WebKitWebView* web_view,
    WebKitWebProcessTerminationReason reason,
    gpointer user_data
    ) {
    g_warning(
        "[InfiniFrame/Linux] WebKit web process terminated: reason=%s",
        webkit_termination_reason_to_string(reason)
        );
}

void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data) {
    if (!linux_webview_diagnostics_enabled())
        return;

    g_message(
        "[InfiniFrame/Linux] WebView size-allocate: %dx%d",
        allocation ? allocation->width : -1,
        allocation ? allocation->height : -1
        );
}

#endif
