#pragma once

#ifdef __linux__

#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameWindowImpl.h"

#include <climits>
#include <gtk/gtk.h>
#include <string>
#include <webkit2/webkit2.h>

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl {
    GtkWidget* _window = nullptr;
    GtkWidget* _webview = nullptr;

    std::string _temporaryFilesPath;

    bool _isFullScreen = false;
    double _zoom = 100.0;
    int _minWidth = 0;
    int _minHeight = 0;
    int _maxWidth = INT_MAX;
    int _maxHeight = INT_MAX;

    GdkGeometry _hints = {};

    gulong _configureEventHandlerId = 0;
    gulong _windowStateEventHandlerId = 0;
    gulong _deleteEventHandlerId = 0;
    gulong _focusInEventHandlerId = 0;
    gulong _focusOutEventHandlerId = 0;
    gulong _contextMenuHandlerId = 0;
    gulong _permissionRequestHandlerId = 0;
    gulong _destroyHandlerId = 0;
    gulong _webMessageReceivedHandlerId = 0;

    int _lastLeft = 0;
    int _lastTop = 0;
    int _lastWidth = 0;
    int _lastHeight = 0;

    void set_webkit_settings();
    void set_webkit_customsettings(WebKitSettings* settings);
    void AddCustomSchemeHandlers();
    [[nodiscard]] bool EnsureWebView();
    void DisconnectSignalHandlers() noexcept;
    void InitializeNotifications(AutoStringConst appName) const;
    void ShutdownNotifications() const noexcept;
};

#endif
