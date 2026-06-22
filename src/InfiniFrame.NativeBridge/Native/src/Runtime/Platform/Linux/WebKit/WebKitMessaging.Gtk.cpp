// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <JavaScriptCore/JavaScript.h>
#include <webkit2/webkit2.h>

#include "Runtime/Platform/Linux/Core/GtkCallbackGuard.h"
#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Types/Callbacks.h"
#include "Runtime/Platform/Linux/WebKit/WebKit.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace gtk_webkit {
    struct GFreeGuard {
        AutoString value = nullptr;

        explicit GFreeGuard(AutoString initialValue = nullptr) : value(initialValue) {}
        ~GFreeGuard() {
            if (value != nullptr)
                g_free(value);
        }

        GFreeGuard(const GFreeGuard&) = delete;
        GFreeGuard& operator=(const GFreeGuard&) = delete;
    };

    void HandleWebMessage(
        WebKitUserContentManager* contentManager, WebKitJavascriptResult* jsResult, const gpointer userData
    ) {
        infiniframe::linux_gtk::RunGtkCallbackNoThrow("script-message-received", [&] {
            (void)contentManager;
            if (jsResult == nullptr)
                return;

            JSCValue* jsValue = webkit_javascript_result_get_js_value(jsResult);
            if (jsValue == nullptr || !jsc_value_is_string(jsValue))
                return;

            GFreeGuard strValue(jsc_value_to_string(jsValue));
            GFreeGuard originValue;

            JSGlobalContextRef context = webkit_javascript_result_get_global_context(jsResult);
            JSStringRef script = JSStringCreateWithUTF8CString("window.location.href");
            JSValueRef locationValue = JSEvaluateScript(context, script, nullptr, nullptr, 0, nullptr);
            JSStringRelease(script);

            if (locationValue != nullptr) {
                JSStringRef locationString = JSValueToStringCopy(context, locationValue, nullptr);
                if (locationString != nullptr) {
                    size_t maxBytes = JSStringGetMaximumUTF8CStringSize(locationString);
                    originValue.value = static_cast<AutoString>(g_malloc(maxBytes));
                    if (originValue.value != nullptr)
                        JSStringGetUTF8CString(locationString, originValue.value, maxBytes);
                    JSStringRelease(locationString);
                }
            }

            auto callback = reinterpret_cast<WebMessageReceivedCallback>(userData);
            if (callback != nullptr) {
                callback(strValue.value, originValue.value);
            }
        });
    }
} 
