#ifdef __linux__

#include <JavaScriptCore/JavaScript.h>
#include <webkit2/webkit2.h>

#include "Utils/Common.h"
#include "WebKit.Gtk.Internal.h"

namespace gtk_webkit {
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
}

#endif
