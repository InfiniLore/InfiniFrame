// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <exception>

#include <gio/gio.h>
#include <webkit2/webkit2.h>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
#include "Runtime/Platform/Linux/WebKit/WebKit.Gtk.Internal.h"
#include "Runtime/Shared/WebView/CustomSchemeResponse.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace gtk_webkit {
    void FinishCustomSchemeError(WebKitURISchemeRequest* request, const GIOErrorEnum code, const char* message) {
        if (request == nullptr)
            return;

        GError* error = g_error_new_literal(G_IO_ERROR, code, message);
        webkit_uri_scheme_request_finish_error(request, error);
        g_error_free(error);
    }

    void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, const gpointer user_data) {
        try {
            WebResourceRequestedCallback webResourceRequestedCallback =
                reinterpret_cast<WebResourceRequestedCallback>(user_data);
            if (webResourceRequestedCallback == nullptr) {
                FinishCustomSchemeError(request, G_IO_ERROR_NOT_SUPPORTED, "No custom scheme handler is registered.");
                return;
            }

            const gchar* uri = webkit_uri_scheme_request_get_uri(request);
            CustomSchemeResponse managedResponse{};
            const int handled = webResourceRequestedCallback(const_cast<const char*>(uri), &managedResponse);
            infiniframe::CustomSchemeResponseLease responseLease(managedResponse);
            if (handled == 0) {
                FinishCustomSchemeError(request, G_IO_ERROR_NOT_FOUND, "Custom scheme resource was not found.");
                return;
            }
            if (!infiniframe::IsValidBufferedCustomSchemeResponse(managedResponse)) {
                FinishCustomSchemeError(
                    request, G_IO_ERROR_FAILED, "Custom scheme handler returned an invalid response."
                    );
                return;
            }

            // GBytes copies producer-owned memory. The producer can therefore be released immediately after finish.
            GBytes* bytes = g_bytes_new(managedResponse.Body, static_cast<gsize>(managedResponse.ContentLength));
            if (bytes == nullptr) {
                FinishCustomSchemeError(request, G_IO_ERROR_FAILED, "Could not copy custom scheme response data.");
                return;
            }
            GInputStream* stream = g_memory_input_stream_new_from_bytes(bytes);
            g_bytes_unref(bytes);
            if (stream == nullptr) {
                FinishCustomSchemeError(request, G_IO_ERROR_FAILED, "Could not create custom scheme response stream.");
                return;
            }

            WebKitURISchemeResponse* response = webkit_uri_scheme_response_new(
                stream, static_cast<gint64>(managedResponse.ContentLength));
            webkit_uri_scheme_response_set_content_type(response, managedResponse.ContentTypeUtf8);
            webkit_uri_scheme_response_set_status(response, static_cast<guint>(managedResponse.StatusCode), nullptr);
            webkit_uri_scheme_request_finish_with_response(request, response);
            g_object_unref(response);
            g_object_unref(stream);
        } catch (const std::exception& ex) {
            g_warning("[InfiniFrame/Linux] custom-scheme-request failed: %s", ex.what());
            FinishCustomSchemeError(request, G_IO_ERROR_FAILED, "Custom scheme handler failed.");
        } catch (...) {
            g_warning("[InfiniFrame/Linux] custom-scheme-request failed with an unknown native exception.");
            FinishCustomSchemeError(request, G_IO_ERROR_FAILED, "Custom scheme handler failed.");
        }
    }
}

void InfiniFrameWindow::Impl::AddCustomSchemeHandlers() {
    if (_customSchemeCallback == nullptr || _webContext == nullptr)
        return;

    WebKitSecurityManager* securityManager = webkit_web_context_get_security_manager(_webContext);
    for (const auto& value : _customSchemeNames) {
        if (securityManager != nullptr && g_ascii_strcasecmp(value.c_str(), "app") == 0) {
            webkit_security_manager_register_uri_scheme_as_secure(securityManager, value.c_str());
            webkit_security_manager_register_uri_scheme_as_cors_enabled(securityManager, value.c_str());
        }

        webkit_web_context_register_uri_scheme(
            _webContext, value.c_str(),
            reinterpret_cast<WebKitURISchemeRequestCallback>(gtk_webkit::HandleCustomSchemeRequest),
            reinterpret_cast<void*>(_customSchemeCallback), nullptr
            );
    }
}