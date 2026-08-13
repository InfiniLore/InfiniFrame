// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
        const char* value = nullptr;

        explicit GFreeGuard(const char* initialValue = nullptr) : value(initialValue) {}
        ~GFreeGuard() {
            if (value != nullptr)
                g_free(const_cast<char*>(value));
        }

        GFreeGuard(const GFreeGuard&) = delete;
        GFreeGuard& operator=(const GFreeGuard&) = delete;
    };

    struct GObjectGuard {
        gpointer value = nullptr;

        explicit GObjectGuard(const gpointer initialValue = nullptr) : value(initialValue) {}
        ~GObjectGuard() {
            if (value != nullptr)
                g_object_unref(value);
        }

        GObjectGuard(const GObjectGuard&) = delete;
        GObjectGuard& operator=(const GObjectGuard&) = delete;
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

            JSCContext* context = jsc_value_get_context(jsValue);
            if (context != nullptr) {
                GObjectGuard locationValue(jsc_context_evaluate(context, "window.location.href", -1));
                if (locationValue.value != nullptr && jsc_value_is_string(JSC_VALUE(locationValue.value))) {
                    originValue.value = jsc_value_to_string(JSC_VALUE(locationValue.value));
                }
            }

            auto callback = reinterpret_cast<WebMessageReceivedCallback>(userData);
            if (callback != nullptr) {
                callback(strValue.value, originValue.value);
            }
        });
    }
} 
