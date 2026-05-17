#ifdef __linux__

#include <gio/gio.h>
#include <webkit2/webkit2.h>

#include "../Window.Gtk.Internal.h"
#include "WebKit.Gtk.Internal.h"

namespace gtk_webkit {
void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, const gpointer user_data) {
    WebResourceRequestedCallback webResourceRequestedCallback =
        reinterpret_cast<WebResourceRequestedCallback>(user_data);
    if (webResourceRequestedCallback == nullptr) {
        GError* error =
            g_error_new_literal(G_IO_ERROR, G_IO_ERROR_NOT_SUPPORTED, "No custom scheme handler is registered.");
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
} // namespace gtk_webkit

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
            reinterpret_cast<WebKitURISchemeRequestCallback>(gtk_webkit::HandleCustomSchemeRequest),
            reinterpret_cast<void*>(_customSchemeCallback), nullptr
        );
    }
}

#endif
