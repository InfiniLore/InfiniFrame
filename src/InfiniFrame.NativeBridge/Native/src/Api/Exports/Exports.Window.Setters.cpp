// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_SetTransparentEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTransparentEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrameNative_SetContextMenuEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetContextMenuEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrameNative_SetZoomEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoomEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrameNative_SetDevToolsEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetDevToolsEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrameNative_SetFullScreen(InfiniFrameWindow* instance, const bool fullScreen) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFullScreen(fullScreen); });
}

EXPORTED InteropStatus InfiniFrameNative_SetIconFile(InfiniFrameWindow* instance, const AutoString filename) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SetIconFile(NullToEmpty(filename));
    });
}

EXPORTED InteropStatus InfiniFrameNative_SetMaximized(InfiniFrameWindow* instance, const bool maximized) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaximized(maximized); });
}

EXPORTED InteropStatus InfiniFrameNative_SetMaxSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaxSize(width, height); });
}

EXPORTED InteropStatus InfiniFrameNative_SetMinimized(InfiniFrameWindow* instance, const bool minimized) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinimized(minimized); });
}

EXPORTED InteropStatus InfiniFrameNative_SetMinSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinSize(width, height); });
}

EXPORTED InteropStatus InfiniFrameNative_SetPosition(InfiniFrameWindow* instance, const int x, const int y) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetPosition(x, y); });
}

EXPORTED InteropStatus InfiniFrameNative_SetResizable(InfiniFrameWindow* instance, const bool resizable) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizable(resizable); });
}

EXPORTED InteropStatus InfiniFrameNative_SetSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetSize(width, height); });
}

EXPORTED InteropStatus InfiniFrameNative_SetTitle(InfiniFrameWindow* instance, const AutoString title) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTitle(NullToEmpty(title)); });
}

EXPORTED InteropStatus InfiniFrameNative_SetTopmost(InfiniFrameWindow* instance, const bool topmost) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTopmost(topmost); });
}

EXPORTED InteropStatus InfiniFrameNative_SetZoom(InfiniFrameWindow* instance, const int zoom) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoom(zoom); });
}
}
