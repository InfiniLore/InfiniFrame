// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
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
        window->SetIconFile(NullToEmpty(filename));
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
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTitle(NullToEmpty(title)); });
}

EXPORTED InteropStatus InfiniFrame_SetTopmost(InfiniFrameWindow* instance, const bool topmost) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTopmost(topmost); });
}

EXPORTED InteropStatus InfiniFrame_SetZoom(InfiniFrameWindow* instance, const int zoom) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoom(zoom); });
}
}
