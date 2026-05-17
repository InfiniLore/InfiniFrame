#ifdef __linux__

#include <format>
#include <simdjson.h>

#include <JavaScriptCore/JavaScript.h>
#include <gio/gio.h>
#include <webkit2/webkit2.h>

#include "Embedded/Embedded.h"
#include "Window.Gtk.Internal.h"

extern void on_webview_load_changed(WebKitWebView* web_view, WebKitLoadEvent load_event, gpointer user_data);
extern gboolean on_webview_load_failed(
    WebKitWebView* web_view,
    WebKitLoadEvent load_event,
    gchar* failing_uri,
    GError* error,
    gpointer user_data
    );
extern void on_webview_process_terminated(
    WebKitWebView* web_view,
    WebKitWebProcessTerminationReason reason,
    gpointer user_data
    );
extern void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data);

namespace {
    void HandleWebMessage(
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

    void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, const gpointer user_data) {
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
}

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
    }
}

void InfiniFrameWindow::Impl::AddCustomSchemeHandlers() {
    if (_customSchemeCallback == nullptr)
        return;

    WebKitWebContext* context = webkit_web_context_get_default();
    WebKitSecurityManager* securityManager = webkit_web_context_get_security_manager(context);
    for (const auto& value : _customSchemeNames) {
        if (securityManager != nullptr && g_ascii_strcasecmp(value.c_str(), "app") == 0) {
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

#endif
