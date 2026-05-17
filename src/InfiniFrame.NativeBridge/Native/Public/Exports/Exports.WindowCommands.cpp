#include "Public/Exports/Exports.h"

extern "C" {
EXPORTED InteropStatus InfiniFrame_Center(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Center(); });
}

EXPORTED InteropStatus InfiniFrame_ClearBrowserAutoFill(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->ClearBrowserAutoFill(); });
}

EXPORTED InteropStatus InfiniFrame_NavigateToString(InfiniFrameWindow* instance, const AutoString content) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(content, "content")) throw std::invalid_argument("Argument 'content' is null.");
        window->NavigateToString(content);
    });
}

EXPORTED InteropStatus InfiniFrame_NavigateToUrl(InfiniFrameWindow* instance, const AutoString url) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(url, "url")) throw std::invalid_argument("Argument 'url' is null.");
        window->NavigateToUrl(url);
    });
}

EXPORTED InteropStatus InfiniFrame_Restore(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Restore(); });
}

EXPORTED InteropStatus InfiniFrame_SendWebMessage(InfiniFrameWindow* instance, const AutoString message) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(message, "message")) throw std::invalid_argument("Argument 'message' is null.");
        window->SendWebMessage(message);
    });
}

EXPORTED InteropStatus InfiniFrame_SetTransparentEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTransparentEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetContextMenuEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetContextMenuEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetZoomEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoomEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetDevToolsEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetDevToolsEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetFullScreen(InfiniFrameWindow* instance, const bool fullScreen) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFullScreen(fullScreen); });
}

EXPORTED InteropStatus InfiniFrame_SetIconFile(InfiniFrameWindow* instance, const AutoString filename) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(filename, "filename")) throw std::invalid_argument("Argument 'filename' is null.");
        window->SetIconFile(filename);
    });
}

EXPORTED InteropStatus InfiniFrame_SetMaximized(InfiniFrameWindow* instance, const bool maximized) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaximized(maximized); });
}

EXPORTED InteropStatus InfiniFrame_SetMaxSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaxSize(width, height); });
}

EXPORTED InteropStatus InfiniFrame_SetMinimized(InfiniFrameWindow* instance, const bool minimized) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinimized(minimized); });
}

EXPORTED InteropStatus InfiniFrame_SetMinSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinSize(width, height); });
}

EXPORTED InteropStatus InfiniFrame_SetPosition(InfiniFrameWindow* instance, const int x, const int y) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetPosition(x, y); });
}

EXPORTED InteropStatus InfiniFrame_SetResizable(InfiniFrameWindow* instance, const bool resizable) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizable(resizable); });
}

EXPORTED InteropStatus InfiniFrame_SetSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetSize(width, height); });
}

EXPORTED InteropStatus InfiniFrame_SetTitle(InfiniFrameWindow* instance, const AutoString title) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(title, "title")) throw std::invalid_argument("Argument 'title' is null.");
        window->SetTitle(title);
    });
}

EXPORTED InteropStatus InfiniFrame_SetTopmost(InfiniFrameWindow* instance, const bool topmost) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTopmost(topmost); });
}

EXPORTED InteropStatus InfiniFrame_SetZoom(InfiniFrameWindow* instance, const int zoom) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoom(zoom); });
}

EXPORTED InteropStatus InfiniFrame_ShowNotification(InfiniFrameWindow* instance, const AutoString title, const AutoString body) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(title, "title") || !EnsureNotNull(body, "body")) throw std::invalid_argument("ShowNotification argument is null.");
        window->ShowNotification(title, body);
    });
}

EXPORTED InteropStatus InfiniFrame_SetFocused(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->SetFocused(); });
}
}
