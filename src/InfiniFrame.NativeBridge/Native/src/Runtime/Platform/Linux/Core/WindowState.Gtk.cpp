// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <format>
#include <string_view>

#include <gdk/x11/gdkx.h>
#include <X11/Xlib.h>
#include <X11/Xutil.h>

#include "Runtime/Platform/Linux/Core/GtkCallbackGuard.h"
#include "Runtime/Internal/Utilities/StringCopy.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    GdkToplevelState GetToplevelState(const GtkWidget* window) {
        GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(const_cast<GtkWidget*>(window)));
        if (surface == nullptr || !GDK_IS_TOPLEVEL(surface))
            return static_cast<GdkToplevelState>(0);
        return gdk_toplevel_get_state(GDK_TOPLEVEL(surface));
    }
}

void InfiniFrameWindow::Impl::ApplySizeConstraints() {
    if (_window == nullptr)
        return;

    // GTK4: minimum size via the widget size request (works pre-realize, all backends).
    gtk_widget_set_size_request(
        GTK_WIDGET(_window),
        _minWidth > 0 ? _minWidth : -1,
        _minHeight > 0 ? _minHeight : -1
        );

    // GTK4 has no max-size API. On X11 the WM honors WM_NORMAL_HINTS (parity with the
    // GTK3 gtk_window_set_geometry_hints path); on Wayland max-size cannot be enforced
    // (xdg_toplevel min/max size is not exposed by GTK4) — documented limitation B1.
    GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(_window));
    if (surface == nullptr || !GDK_IS_X11_DISPLAY(gdk_surface_get_display(surface)))
        return;

G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    const Window xid = gdk_x11_surface_get_xid(surface);
    Display* xdpy = gdk_x11_display_get_xdisplay(GDK_X11_DISPLAY(gdk_surface_get_display(surface)));
G_GNUC_END_IGNORE_DEPRECATIONS
    if (xid == 0)
        return;

    XSizeHints hints = {};
    hints.flags = PMinSize | PMaxSize;
    hints.min_width = _minWidth > 0 ? _minWidth : 1;
    hints.min_height = _minHeight > 0 ? _minHeight : 1;
    hints.max_width = (_maxWidth > 0 && _maxWidth < INT_MAX) ? _maxWidth : 32767;
    hints.max_height = (_maxHeight > 0 && _maxHeight < INT_MAX) ? _maxHeight : 32767;
    XSetWMNormalHints(xdpy, xid, &hints);
    XFlush(xdpy);
}

void InfiniFrameWindow::Impl::MoveTo(const int x, const int y) {
    // GTK4 removed gtk_window_move. Arbitrary positioning exists only on X11; on Wayland
    // the compositor owns placement (documented limitation B2).
    GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(_window));
    if (surface == nullptr || !GDK_IS_X11_DISPLAY(gdk_surface_get_display(surface))) {
        if (surface == nullptr) {
            // Not realized yet: remember the request and apply it from OnSurfaceRealized.
            _hasPendingMove = true;
            _pendingMoveX = x;
            _pendingMoveY = y;
        }
        return;
    }

G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    const Window xid = gdk_x11_surface_get_xid(surface);
    Display* xdpy = gdk_x11_display_get_xdisplay(GDK_X11_DISPLAY(gdk_surface_get_display(surface)));
G_GNUC_END_IGNORE_DEPRECATIONS
    if (xid == 0)
        return;

    XMoveWindow(xdpy, xid, x, y);
    XFlush(xdpy);
}

bool InfiniFrameWindow::Impl::TryGetPosition(int* x, int* y) const {
    GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(const_cast<GtkWidget*>(_window)));
    if (surface == nullptr || !GDK_IS_X11_DISPLAY(gdk_surface_get_display(surface)))
        return false;

G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    const Window xid = gdk_x11_surface_get_xid(surface);
    Display* xdpy = gdk_x11_display_get_xdisplay(GDK_X11_DISPLAY(gdk_surface_get_display(surface)));
G_GNUC_END_IGNORE_DEPRECATIONS
    if (xid == 0)
        return false;

    Window child = 0;
    int rootX = 0;
    int rootY = 0;
    if (!XTranslateCoordinates(xdpy, xid, DefaultRootWindow(xdpy), 0, 0, &rootX, &rootY, &child))
        return false;
    if (x != nullptr)
        *x = rootX;
    if (y != nullptr)
        *y = rootY;
    return true;
}
void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const {
    *enabled = m_impl->common._transparentEnabled;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const {
    *enabled = m_impl->common._contextMenuEnabled;
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const {
    *enabled = m_impl->common._zoomEnabled;
}

void InfiniFrameWindow::GetStatusBarEnabled(bool* enabled) const {
    *enabled = m_impl->common._statusBarEnabled;
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const {
    if (m_impl->_webview == nullptr) {
        *enabled = m_impl->common._devToolsEnabled;
        return;
    }
    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    *enabled = webkit_settings_get_enable_developer_extras(settings);
}

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const {
    *fullScreen = m_impl->_isFullScreen;
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const {
    *isMaximized = (GetToplevelState(m_impl->_window) & GDK_TOPLEVEL_STATE_MAXIMIZED) != 0;
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const {
    *isMinimized = (GetToplevelState(m_impl->_window) & GDK_TOPLEVEL_STATE_MINIMIZED) != 0;
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const {
    // GTK4 removed gtk_window_get_position; query the X11 root position where possible
    // and fall back to the last observed position (constant on Wayland, matching GTK3).
    if (!m_impl->TryGetPosition(x, y)) {
        if (x != nullptr)
            *x = m_impl->_lastLeft;
        if (y != nullptr)
            *y = m_impl->_lastTop;
    }
}

void InfiniFrameWindow::GetResizable(bool* resizable) const {
    *resizable = gtk_window_get_resizable(GTK_WINDOW(m_impl->_window));
}

unsigned int InfiniFrameWindow::GetScreenDpi() const {
    // GTK4 removed gdk_screen_get_resolution. Use the widget scale factor against the
    // 96 dpi baseline (scale 1 reproduces the GTK3 default resolution of 96).
    const int scale = gtk_widget_get_scale_factor(GTK_WIDGET(m_impl->_window));
    return static_cast<unsigned int>(96 * (scale > 0 ? scale : 1));
}

void InfiniFrameWindow::GetSize(int* width, int* height) const {
    // GTK4 removed gtk_window_get_size; use the allocated widget size and fall back to
    // the default size before first allocation.
    const int allocatedWidth = gtk_widget_get_width(GTK_WIDGET(m_impl->_window));
    const int allocatedHeight = gtk_widget_get_height(GTK_WIDGET(m_impl->_window));
    if (allocatedWidth > 0 && allocatedHeight > 0) {
        if (width != nullptr)
            *width = allocatedWidth;
        if (height != nullptr)
            *height = allocatedHeight;
        return;
    }

    int defaultWidth = 0;
    int defaultHeight = 0;
    gtk_window_get_default_size(GTK_WINDOW(m_impl->_window), &defaultWidth, &defaultHeight);
    if (width != nullptr)
        *width = defaultWidth;
    if (height != nullptr)
        *height = defaultHeight;
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

const char* InfiniFrameWindow::GetTitle() const {
    const char* title = gtk_window_get_title(GTK_WINDOW(m_impl->_window));
    return AllocateStringCopy(std::string(title ? title : ""));
}

const char* InfiniFrameWindow::GetCurrentUrl() const {
    if (m_impl->_webview == nullptr)
        return AllocateStringCopy(std::string(""));
    const gchar* uri = webkit_web_view_get_uri(WEBKIT_WEB_VIEW(m_impl->_webview));
    return AllocateStringCopy(std::string(uri ? uri : ""));
}

void InfiniFrameWindow::GetTopmost(bool* topmost) const {
    *topmost = (GetToplevelState(m_impl->_window) & GDK_TOPLEVEL_STATE_ABOVE) != 0;
}

void InfiniFrameWindow::GetZoom(int* zoom) const {
    if (m_impl->_webview == nullptr) {
        *zoom = m_impl->_zoom;
        return;
    }
    double rawValue = webkit_web_view_get_zoom_level(WEBKIT_WEB_VIEW(m_impl->_webview));
    rawValue = (rawValue * 100.0) + 0.5;
    *zoom = static_cast<int>(rawValue);
}

void InfiniFrameWindow::GetFocused(bool* isFocused) const {
    *isFocused = gtk_window_is_active(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::NavigateToString(const char* content) {
    if (m_impl->_webviewClosed || m_impl->_webview == nullptr)
        return;

    webkit_web_view_load_html(WEBKIT_WEB_VIEW(m_impl->_webview), content, nullptr);
}

void InfiniFrameWindow::NavigateToUrl(const char* url) {
    if (m_impl->_webviewClosed || m_impl->_webview == nullptr)
        return;

    webkit_web_view_load_uri(WEBKIT_WEB_VIEW(m_impl->_webview), url);
}

void InfiniFrameWindow::Restore() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

static std::string escapeJsonString(const std::string_view input) {
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
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "evaluate-javascript-finished", [&] {
            if (object == nullptr || result == nullptr)
                return;

            GError* error = nullptr;
            JSCValue* value = webkit_web_view_evaluate_javascript_finish(WEBKIT_WEB_VIEW(object), result, &error);
            if (value != nullptr)
                g_object_unref(value);
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
                WEBKIT_WEB_VIEW(m_impl->_webview), js.c_str(), -1, nullptr, nullptr, nullptr, webview_eval_finished,
                nullptr
                );
        }
        m_impl->_pendingWebMessages.clear();
    }
}

void InfiniFrameWindow::SendWebMessage(const char* message) {
    if (m_impl->_webviewClosed || m_impl->_webview == nullptr)
        return;

    std::string escaped = escapeJsonString(message ? message : "");

    std::string js;
    js.append("__infiniframe_dispatch(\"");
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
    m_impl->common._contextMenuEnabled = enabled;
    if (m_impl->_webview == nullptr)
        return;
    std::string payload = "{\"enabled\":" + std::string(enabled ? "true" : "false") + "}";
    std::string envelope = "{\"version\":1,\"messageId\":\"__infiniframe:browser:setContextMenuEnabled\",\"payload\":\""
        + escapeJsonString(payload) + "\"}";
    SendWebMessage(envelope.c_str());
}

void InfiniFrameWindow::SetMediaAutoplayEnabled(const bool enabled) {
    m_impl->common._mediaAutoplayEnabled = enabled;
    if (m_impl->_webview == nullptr)
        return;

    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_media_playback_requires_user_gesture(settings, !enabled);
    webkit_web_view_reload(WEBKIT_WEB_VIEW(m_impl->_webview));
}

void InfiniFrameWindow::SetUserAgent(const char* userAgent) {
    m_impl->common._userAgent = userAgent != nullptr ? userAgent : "";
    if (m_impl->_webview == nullptr)
        return;

    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_user_agent(
        settings,
        m_impl->common._userAgent.empty() ? nullptr : m_impl->common._userAgent.c_str()
        );
    webkit_web_view_reload(WEBKIT_WEB_VIEW(m_impl->_webview));
}

void InfiniFrameWindow::SetZoomEnabled(const bool enabled) {
    m_impl->common._zoomEnabled = enabled;
    if (m_impl->_webview == nullptr)
        return;
    std::string payload = "{\"enabled\":" + std::string(enabled ? "true" : "false") + "}";
    std::string envelope = "{\"version\":1,\"messageId\":\"__infiniframe:browser:setZoomEnabled\",\"payload\":\"" +
        escapeJsonString(payload) + "\"}";
    SendWebMessage(envelope.c_str());
}

void InfiniFrameWindow::SetStatusBarEnabled(const bool enabled) {
    // WebKitGTK has no native status bar concept — this is a WebView2-only feature.
    // The flag is stored for API consistency but has no visible effect on Linux.
    m_impl->common._statusBarEnabled = enabled;
}

void InfiniFrameWindow::SetBrowserShortcutsEnabled(const bool enabled) {
    m_impl->common._browserShortcutsEnabled = enabled;
    if (m_impl->_webview == nullptr)
        return;
    std::string payload = "{\"enabled\":" + std::string(enabled ? "true" : "false") + "}";
    std::string envelope =
        "{\"version\":1,\"messageId\":\"__infiniframe:browser:setBrowserShortcutsEnabled\",\"payload\":\"" +
        escapeJsonString(payload) + "\"}";
    SendWebMessage(envelope.c_str());
}

void InfiniFrameWindow::SetDevToolsEnabled(const bool enabled) {
    m_impl->common._devToolsEnabled = enabled;
    if (m_impl->_webview == nullptr)
        return;
    WebKitSettings* settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(m_impl->_webview));
    webkit_settings_set_enable_developer_extras(
        settings, m_impl->common._devToolsEnabled || m_impl->common._remoteDebuggingPort > 0);
}

void InfiniFrameWindow::SetFullScreen(const bool fullScreen) {
    if (fullScreen)
        gtk_window_fullscreen(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unfullscreen(GTK_WINDOW(m_impl->_window));

    m_impl->_isFullScreen = fullScreen;
}

void InfiniFrameWindow::SetIconFile(const char* filename) {
    m_impl->common._iconFileName = filename ? filename : "";
    if (filename == nullptr || filename[0] == '\0')
        return;

    // GTK4 removed gtk_window_set_icon_from_file; load the file into a GdkTexture and
    // publish it through the toplevel icon list (requires a realized surface; when called
    // before realize, OnSurfaceRealized re-applies the stored icon file).
    GdkTexture* texture = gdk_texture_new_from_filename(filename, nullptr);
    if (texture == nullptr)
        return;

    GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(m_impl->_window));
    if (surface != nullptr && GDK_IS_TOPLEVEL(surface)) {
        GList* icons = g_list_append(nullptr, texture);
        gdk_toplevel_set_icon_list(GDK_TOPLEVEL(surface), icons);
        g_list_free(icons);
    }
    g_object_unref(texture);
}

void InfiniFrameWindow::SetMinimized(const bool minimized) {
    if (minimized)
        gtk_window_minimize(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unminimize(GTK_WINDOW(m_impl->_window));
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

    m_impl->MoveTo(x, y);
}

void InfiniFrameWindow::SetResizable(const bool resizable) {
    gtk_window_set_resizable(GTK_WINDOW(m_impl->_window), resizable);
}

void InfiniFrameWindow::SetMinSize(const int width, const int height) {
    m_impl->_minWidth = width;
    m_impl->_minHeight = height;
    m_impl->ApplySizeConstraints();
}

void InfiniFrameWindow::SetMaxSize(const int width, const int height) {
    m_impl->_maxWidth = width;
    m_impl->_maxHeight = height;
    m_impl->ApplySizeConstraints();
}

void InfiniFrameWindow::SetSize(const int width, const int height) {
    // GTK4 removed gtk_window_resize. gtk_window_set_default_size resizes a mapped
    // toplevel on both X11 and Wayland (runtime-verified; see blockers-resolution.md),
    // but the request is clobbered if it lands while the first map/size negotiation is
    // still in flight. Remember the target and let OnSurfaceLayout re-assert it once the
    // toplevel is fully allocated (Phase 07 flake fix).
    if (g_getenv("INFINIFRAME_LINUX_WEBVIEW_DIAGNOSTICS") != nullptr) {
        g_message("[InfiniFrame/Linux] SetSize requested: %dx%d (widget now %dx%d mapped=%d)",
                  width, height,
                  gtk_widget_get_width(GTK_WIDGET(m_impl->_window)),
                  gtk_widget_get_height(GTK_WIDGET(m_impl->_window)),
                  gtk_widget_get_mapped(GTK_WIDGET(m_impl->_window)));
    }
    m_impl->_pendingResizeWidth = width;
    m_impl->_pendingResizeHeight = height;
    gtk_window_set_default_size(GTK_WINDOW(m_impl->_window), width, height);
}

void InfiniFrameWindow::SetTitle(const char* title) {
    gtk_window_set_title(GTK_WINDOW(m_impl->_window), title);
}

void InfiniFrameWindow::SetTopmost(const bool topmost) {
    // GTK4 removed gtk_window_set_keep_above. On X11 the EWMH _NET_WM_STATE_ABOVE hint
    // is equivalent (read back through GDK_TOPLEVEL_STATE_ABOVE); Wayland has no
    // keep-above concept (documented limitation, same as GTK3-on-Wayland).
    GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(m_impl->_window));
    if (surface == nullptr || !GDK_IS_X11_DISPLAY(gdk_surface_get_display(surface)))
        return;

G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    GdkX11Display* x11Display = GDK_X11_DISPLAY(gdk_surface_get_display(surface));
    const Window xid = gdk_x11_surface_get_xid(surface);
    Display* xdpy = gdk_x11_display_get_xdisplay(x11Display);
    const Atom stateAtom = gdk_x11_get_xatom_by_name_for_display(x11Display, "_NET_WM_STATE");
    const Atom aboveAtom = gdk_x11_get_xatom_by_name_for_display(x11Display, "_NET_WM_STATE_ABOVE");
G_GNUC_END_IGNORE_DEPRECATIONS
    if (xid == 0)
        return;

    XEvent event = {};
    event.xclient.type = ClientMessage;
    event.xclient.window = xid;
    event.xclient.message_type = stateAtom;
    event.xclient.format = 32;
    event.xclient.data.l[0] = topmost ? 1 : 0; // _NET_WM_STATE_ADD : _NET_WM_STATE_REMOVE
    event.xclient.data.l[1] = static_cast<long>(aboveAtom);
    event.xclient.data.l[2] = 0;
    event.xclient.data.l[3] = 1; // source indication: application
    XSendEvent(
        xdpy, DefaultRootWindow(xdpy), False,
        SubstructureRedirectMask | SubstructureNotifyMask, &event
        );
    XFlush(xdpy);
}

void InfiniFrameWindow::SetZoom(const int zoom) {
    // Software guard: respect EnableZoom(false) to match cross-platform API semantics.
    if (!m_impl->common._zoomEnabled)
        return;

    // Clamp to valid range (25-500%) to match Windows/macOS behavior.
    if (zoom < 25 || zoom > 500)
        return;

    m_impl->_zoom = zoom;
    if (m_impl->_webview == nullptr)
        return;
    double newZoom = zoom / 100.0;
    webkit_web_view_set_zoom_level(WEBKIT_WEB_VIEW(m_impl->_webview), newZoom);
}

void InfiniFrameWindow::SetFocused() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetTransparentEnabled(const bool enabled) {
    m_impl->common._transparentEnabled = enabled;

    gtk_window_set_decorated(GTK_WINDOW(m_impl->_window), !enabled);

    // GTK4 always uses RGBA visuals for windows (GTK 3→4 migration guide), so the old
    // gtk_widget_set_visual / app-paintable setup is obsolete. True desktop transparency
    // additionally requires the toplevel's own CSS background to be transparent — otherwise
    // the theme background shows through page alpha. A compositor blends the result:
    // inherent on Wayland; required on X11 (audit finding 2).
    static const gchar* kTransparentCss = "window.infiniframe-transparent { background: transparent; }";
    static std::once_flag cssOnce;
    GdkDisplay* display = gtk_widget_get_display(GTK_WIDGET(m_impl->_window));
    std::call_once(
        cssOnce, [display] {
            GtkCssProvider* provider = gtk_css_provider_new();
            gtk_css_provider_load_from_string(provider, kTransparentCss);
            gtk_style_context_add_provider_for_display(
                display, GTK_STYLE_PROVIDER(provider), GTK_STYLE_PROVIDER_PRIORITY_APPLICATION);
            g_object_unref(provider);
        });
    if (enabled)
        gtk_widget_add_css_class(GTK_WIDGET(m_impl->_window), "infiniframe-transparent");
    else
        gtk_widget_remove_css_class(GTK_WIDGET(m_impl->_window), "infiniframe-transparent");

    // WebKit background alpha drives the page's own transparency.
    if (m_impl->_webview != nullptr) {
        GdkRGBA color;
        webkit_web_view_get_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &color);
        color.alpha = enabled ? 0 : 1;
        webkit_web_view_set_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &color);
    }
}

void InfiniFrameWindow::SetBackgroundColor(const uint8_t r, const uint8_t g, const uint8_t b, const uint8_t a) {
    m_impl->common._backgroundColorR = r;
    m_impl->common._backgroundColorG = g;
    m_impl->common._backgroundColorB = b;
    m_impl->common._backgroundColorA = a;

    if (m_impl->_webview == nullptr)
        return;

    GdkRGBA rgba = {r / 255.0, g / 255.0, b / 255.0, a / 255.0};
    webkit_web_view_set_background_color(WEBKIT_WEB_VIEW(m_impl->_webview), &rgba);
}