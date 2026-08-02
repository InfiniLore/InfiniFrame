// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <format>
#include <string_view>

#include "Runtime/Platform/Linux/Core/GtkCallbackGuard.h"
#include "Runtime/Shared/Utilities/StringCopy.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const {
    *enabled = m_impl->_transparentEnabled;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const {
    *enabled = m_impl->_contextMenuEnabled;
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const {
    *enabled = m_impl->_zoomEnabled;
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const {
    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    *enabled = webkit_settings_get_enable_developer_extras(settings);
}

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const {
    *fullScreen = m_impl->_isFullScreen;
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const {
    GdkWindow* gdk_window = gtk_widget_get_window(GTK_WIDGET(m_impl->_window));
    GdkWindowState flags = gdk_window_get_state(gdk_window);
    *isMaximized = flags & GDK_WINDOW_STATE_MAXIMIZED;
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const {
    GdkWindow* gdk_window = gtk_widget_get_window(GTK_WIDGET(m_impl->_window));
    GdkWindowState flags = gdk_window_get_state(gdk_window);
    *isMinimized = flags & GDK_WINDOW_STATE_ICONIFIED;
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const {
    gtk_window_get_position(GTK_WINDOW(m_impl->_window), x, y);
}

void InfiniFrameWindow::GetResizable(bool* resizable) const {
    *resizable = gtk_window_get_resizable(GTK_WINDOW(m_impl->_window));
}

unsigned int InfiniFrameWindow::GetScreenDpi() const {
    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    gdouble dpi = gdk_screen_get_resolution(screen);
    if (dpi < 0)
        return 96;
    else
        return static_cast<unsigned int>(dpi);
}

void InfiniFrameWindow::GetSize(int* width, int* height) const {
    gtk_window_get_size(GTK_WINDOW(m_impl->_window), width, height);
}

void InfiniFrameWindow::GetMaxSize(int* width, int* height) const {
    if (width)
        *width = m_impl->_maxWidth;
    if (height)
        *height = m_impl->_maxHeight;
}

void InfiniFrameWindow::GetMinSize(int* width, int* height) const {
    if (width)
        *width = m_impl->_minWidth;
    if (height)
        *height = m_impl->_minHeight;
}

AutoString InfiniFrameWindow::GetTitle() const {
    const char* title = gtk_window_get_title(GTK_WINDOW(m_impl->_window));
    return g_strdup(title ? title : "");
}

AutoString InfiniFrameWindow::GetCurrentUrl() const {
    if (m_impl->_webview == nullptr)
        return g_strdup("");
    const gchar* uri = webkit_web_view_get_uri(WEBKIT_WEB_VIEW(m_impl->_webview));
    return g_strdup(uri ? uri : "");
}

void InfiniFrameWindow::GetTopmost(bool* topmost) const {
    GdkWindow* gdk_window = gtk_widget_get_window(GTK_WIDGET(m_impl->_window));
    GdkWindowState flags = gdk_window_get_state(gdk_window);
    *topmost = flags & GDK_WINDOW_STATE_ABOVE;
}

void InfiniFrameWindow::GetZoom(int* zoom) const {
    double rawValue = webkit_web_view_get_zoom_level(WEBKIT_WEB_VIEW(m_impl->_webview));
    rawValue = (rawValue * 100.0) + 0.5;
    *zoom = static_cast<int>(rawValue);
}

void InfiniFrameWindow::GetFocused(bool* isFocused) const {
    *isFocused = gtk_window_is_active(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::NavigateToString(const AutoString content) {
    if (m_impl->_webviewClosed || m_impl->_webview == nullptr)
        return;

    webkit_web_view_load_html(WEBKIT_WEB_VIEW(m_impl->_webview), content, nullptr);
}

void InfiniFrameWindow::NavigateToUrl(const AutoString url) {
    if (m_impl->_webviewClosed || m_impl->_webview == nullptr)
        return;

    webkit_web_view_load_uri(WEBKIT_WEB_VIEW(m_impl->_webview), url);
}

void InfiniFrameWindow::Restore() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

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
                } else {
                    result += c;
                }
        }
    }

    return result;
}

static void webview_eval_finished(GObject* object, GAsyncResult* result, gpointer) {
    infiniframe::linux_gtk::RunGtkCallbackNoThrow("evaluate-javascript-finished", [&] {
        if (object == nullptr || result == nullptr)
            return;

        GError* error = nullptr;
        webkit_web_view_evaluate_javascript_finish(WEBKIT_WEB_VIEW(object), result, &error);
        if (error) {
            g_warning("JavaScript evaluation failed: %s", error->message);
            g_error_free(error);
        }
    });
}

void InfiniFrameWindow::FlushPendingWebMessages() {
    m_impl->_webviewReady = true;
    if (!m_impl->_pendingWebMessages.empty() && !m_impl->_webviewClosed && m_impl->_webview != nullptr) {
        for (const auto& js : m_impl->_pendingWebMessages) {
            webkit_web_view_evaluate_javascript(
                WEBKIT_WEB_VIEW(m_impl->_webview), js.c_str(), -1, nullptr, nullptr, nullptr, webview_eval_finished, nullptr
            );
        }
        m_impl->_pendingWebMessages.clear();
    }
}

void InfiniFrameWindow::SendWebMessage(const AutoString message) {
    if (m_impl->_webviewClosed || m_impl->_webview == nullptr)
        return;

    std::string escaped = escapeJsonString(message ? message : "");

    std::string js;
    js.append("__dispatchMessageCallback(\"");
    js.append(escaped);
    js.append("\")");

    if (!m_impl->_webviewReady) {
        // WebKit is still loading (e.g. message sent from WindowCreated handler).
        // Queue the message; it will be flushed on the first WEBKIT_LOAD_FINISHED event.
        m_impl->_pendingWebMessages.push_back(std::move(js));
        return;
    }

    webkit_web_view_evaluate_javascript(
        WEBKIT_WEB_VIEW(m_impl->_webview), js.c_str(), -1, nullptr, nullptr, nullptr, webview_eval_finished, nullptr
    );
}

void InfiniFrameWindow::SetContextMenuEnabled(const bool enabled) {
    m_impl->_contextMenuEnabled = enabled;
}

void InfiniFrameWindow::SetMediaAutoplayEnabled(const bool enabled) {
    m_impl->_mediaAutoplayEnabled = enabled;
    if (m_impl->_webview == nullptr)
        return;

    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_media_playback_requires_user_gesture(settings, !enabled);
    webkit_web_view_reload(WEBKIT_WEB_VIEW(m_impl->_webview));
}

void InfiniFrameWindow::SetUserAgent(const AutoString userAgent) {
    m_impl->_userAgent = userAgent != nullptr ? userAgent : "";
    if (m_impl->_webview == nullptr)
        return;

    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_user_agent(
        settings,
        m_impl->_userAgent.empty() ? nullptr : m_impl->_userAgent.c_str()
    );
    webkit_web_view_reload(WEBKIT_WEB_VIEW(m_impl->_webview));
}

void InfiniFrameWindow::SetZoomEnabled(bool enabled) {
    (void)enabled;
}

void InfiniFrameWindow::SetDevToolsEnabled(const bool enabled) {
    m_impl->_devToolsEnabled = enabled;
    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_enable_developer_extras(settings, m_impl->_devToolsEnabled || m_impl->_remoteDebuggingPort > 0);
}

void InfiniFrameWindow::SetFullScreen(const bool fullScreen) {
    if (fullScreen)
        gtk_window_fullscreen(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unfullscreen(GTK_WINDOW(m_impl->_window));

    m_impl->_isFullScreen = fullScreen;
}

void InfiniFrameWindow::SetIconFile(const AutoString filename) {
    gtk_window_set_icon_from_file(GTK_WINDOW(m_impl->_window), filename, nullptr);
    m_impl->_iconFileName = filename ? filename : "";
}

void InfiniFrameWindow::SetMinimized(const bool minimized) {
    if (minimized)
        gtk_window_iconify(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_deiconify(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetMaximized(const bool maximized) {
    if (maximized)
        gtk_window_maximize(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unmaximize(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetPosition(const int x, const int y) {
    GtkWindow* window = GTK_WINDOW(m_impl->_window);
    
    if (gtk_window_is_maximized(window)) {
        gtk_window_unmaximize(window);
    }
    if (m_impl->_isFullScreen) {
        gtk_window_unfullscreen(window);
    }
    
    GdkWindow* gdkWindow = gtk_widget_get_window(GTK_WIDGET(window));
    
    if (gdkWindow) {
        gdk_window_move(gdkWindow, x, y);
    } else {
        gtk_window_move(window, x, y);
    }
}

void InfiniFrameWindow::SetResizable(const bool resizable) {
    gtk_window_set_resizable(GTK_WINDOW(m_impl->_window), resizable);
}

void InfiniFrameWindow::SetMinSize(const int width, const int height) {
    m_impl->_minWidth = width;
    m_impl->_minHeight = height;
    m_impl->_hints.min_width = width;
    m_impl->_hints.min_height = height;

    gtk_window_set_geometry_hints(
        GTK_WINDOW(m_impl->_window), nullptr, &m_impl->_hints,
        static_cast<GdkWindowHints>(GDK_HINT_MIN_SIZE | GDK_HINT_MAX_SIZE)
    );
}

void InfiniFrameWindow::SetMaxSize(const int width, const int height) {
    m_impl->_maxWidth = width;
    m_impl->_maxHeight = height;
    m_impl->_hints.max_width = width;
    m_impl->_hints.max_height = height;

    gtk_window_set_geometry_hints(
        GTK_WINDOW(m_impl->_window), nullptr, &m_impl->_hints,
        static_cast<GdkWindowHints>(GDK_HINT_MIN_SIZE | GDK_HINT_MAX_SIZE)
    );
}

void InfiniFrameWindow::SetSize(const int width, const int height) {
    gtk_window_resize(GTK_WINDOW(m_impl->_window), width, height);
}

void InfiniFrameWindow::SetTitle(const AutoString title) {
    gtk_window_set_title(GTK_WINDOW(m_impl->_window), title);
}

void InfiniFrameWindow::SetTopmost(const bool topmost) {
    gtk_window_set_keep_above(GTK_WINDOW(m_impl->_window), topmost);
}

void InfiniFrameWindow::SetZoom(const int zoom) {
    double newZoom = zoom / 100.0;
    webkit_web_view_set_zoom_level(WEBKIT_WEB_VIEW(m_impl->_webview), newZoom);
}

void InfiniFrameWindow::SetFocused() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetTransparentEnabled(const bool enabled) {
    m_impl->_transparentEnabled = enabled;

    gtk_window_set_decorated(GTK_WINDOW(m_impl->_window), !enabled);

    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    GdkVisual* rgba_visual = gdk_screen_get_rgba_visual(screen);
    if (rgba_visual) {
        gtk_widget_set_visual(GTK_WIDGET(m_impl->_window), rgba_visual);
        gtk_widget_set_app_paintable(GTK_WIDGET(m_impl->_window), true);

        GdkRGBA color;
        webkit_web_view_get_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &color);
        color.alpha = enabled ? 0 : 1;
        webkit_web_view_set_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &color);
    }
}

void InfiniFrameWindow::SetBackgroundColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a) {
    m_impl->_backgroundColorR = r;
    m_impl->_backgroundColorG = g;
    m_impl->_backgroundColorB = b;
    m_impl->_backgroundColorA = a;

    if (m_impl->_webview == nullptr)
        return;

    GdkRGBA rgba = {r / 255.0, g / 255.0, b / 255.0, a / 255.0};
    webkit_web_view_set_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &rgba);
}
