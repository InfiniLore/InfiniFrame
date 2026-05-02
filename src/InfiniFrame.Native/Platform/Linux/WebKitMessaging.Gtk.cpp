#ifdef __linux__

#include "Platform/Linux/WebKitMessaging.Gtk.h"
#include "Platform/Linux/WindowImpl.Gtk.h"
#include "Utils/Common.h"

#include <JavaScriptCore/JavaScript.h>
#include <format>
#include <iterator>
#include <string>
#include <string_view>

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

void HandleWebMessage(
    WebKitUserContentManager* contentManager,
    WebKitJavascriptResult* jsResult,
    const gpointer userData
    ) {
    (void)contentManager;

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

#endif
