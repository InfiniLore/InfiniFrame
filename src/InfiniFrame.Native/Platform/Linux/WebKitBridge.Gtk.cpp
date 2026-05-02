#ifdef __linux__

#include "Platform/Linux/WebKitMessaging.Gtk.h"
#include "Platform/Linux/WindowImpl.Gtk.h"

#include "Embedded/Embedded.h"

#include <signal.h>
#include <stdexcept>

bool InfiniFrameWindow::Impl::EnsureWebView() {
    if (_webview)
        return true;

    if (_startUrl.empty() && _startString.empty())
        throw std::invalid_argument("Either StartUrl or StartString must be specified.");

    struct sigaction old_action;
    sigaction(SIGCHLD, nullptr, &old_action);

    WebKitUserContentManager* contentManager = webkit_user_content_manager_new();
    _webview = webkit_web_view_new_with_user_content_manager(contentManager);

    set_webkit_settings();

    gtk_container_add(GTK_CONTAINER(_window), _webview);

    auto js = Embedded::InfiniFrameHostJsUtf8();

    WebKitUserScript* script = webkit_user_script_new(
        js.c_str(),
        WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES,
        WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START,
        nullptr,
        nullptr
        );

    webkit_user_content_manager_add_script(contentManager, script);
    webkit_user_script_unref(script);

    _webMessageReceivedHandlerId = g_signal_connect(
        contentManager, "script-message-received::infiniFrameInterop",
        G_CALLBACK(HandleWebMessage),
        reinterpret_cast<void*>(_webMessageReceivedCallback)
        );
    webkit_user_content_manager_register_script_message_handler(contentManager, "infiniFrameInterop");

    if (!_startUrl.empty())
        webkit_web_view_load_uri(WEBKIT_WEB_VIEW(_webview), _startUrl.c_str());
    else if (!_startString.empty())
        webkit_web_view_load_html(WEBKIT_WEB_VIEW(_webview), _startString.c_str(), nullptr);

    sigaction(SIGCHLD, &old_action, nullptr);
    return true;
}

void InfiniFrameWindow::NavigateToString(const AutoString content) {
    webkit_web_view_load_html(WEBKIT_WEB_VIEW(m_impl->_webview), content, nullptr);
}

void InfiniFrameWindow::NavigateToUrl(const AutoString url) {
    webkit_web_view_load_uri(WEBKIT_WEB_VIEW(m_impl->_webview), url);
}

void InfiniFrameWindow::CloseWebView() {
    // Not implemented on Linux
}

#endif
