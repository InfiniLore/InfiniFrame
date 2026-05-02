#ifdef __linux__

#include "Platform/Linux/WindowImpl.Gtk.h"
#include "Shared/CustomSchemeResponse.h"

#include <gio/gio.h>

namespace {
    void FreeNativeBufferForStream(gpointer data) {
        InfiniFrame::Native::Interop::FreeNativeBuffer(data);
    }

    void FinishCustomSchemeRequestWithError(WebKitURISchemeRequest* request, const char* message) {
        GError* error = g_error_new_literal(
            G_IO_ERROR,
            G_IO_ERROR_NOT_FOUND,
            message);
        webkit_uri_scheme_request_finish_error(request, error);
        g_error_free(error);
    }
}

static void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, const gpointer user_data) {
    WebResourceRequestedCallback webResourceRequestedCallback = reinterpret_cast<WebResourceRequestedCallback>(
        user_data);
    if (webResourceRequestedCallback == nullptr) {
        FinishCustomSchemeRequestWithError(request, "No custom scheme handler is registered.");
        return;
    }

    const gchar* uri = webkit_uri_scheme_request_get_uri(request);
    auto dotNetResponse = InfiniFrame::Native::Shared::InvokeCustomSchemeCallback(
        webResourceRequestedCallback,
        const_cast<AutoString>(uri)
        );

    if (!dotNetResponse.HasBody()) {
        FinishCustomSchemeRequestWithError(request, "Custom scheme handler returned no response.");
        return;
    }

    GInputStream* stream = g_memory_input_stream_new_from_data(
        dotNetResponse.body.get(),
        dotNetResponse.length,
        FreeNativeBufferForStream
        );
    dotNetResponse.body.release();

    webkit_uri_scheme_request_finish(
        request,
        reinterpret_cast<GInputStream*>(stream),
        -1,
        dotNetResponse.ContentTypeOrDefault()
        );
    g_object_unref(stream);
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

#endif
