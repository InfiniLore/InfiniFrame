// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_SetMenuBar(InfiniFrameWindow* instance, const char* menuBarJson) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        // Menu bar is applied at window creation via InfiniFrameInitParams.MenuBarJson.
        // Runtime updates will be supported when native menu rendering is implemented.
        static_cast<void>(window);
        static_cast<void>(menuBarJson);
    });
}

EXPORTED InteropStatus InfiniFrameNative_SetMenuItemEnabled(InfiniFrameWindow* instance, const char* menuItemId, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        // Runtime menu item updates will be supported when native menu rendering is implemented.
        static_cast<void>(window);
        static_cast<void>(menuItemId);
        static_cast<void>(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_SetMenuItemVisible(InfiniFrameWindow* instance, const char* menuItemId, const bool visible) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        // Runtime menu item updates will be supported when native menu rendering is implemented.
        static_cast<void>(window);
        static_cast<void>(menuItemId);
        static_cast<void>(visible);
    });
}

EXPORTED InteropStatus InfiniFrameNative_ClickMenuItem(InfiniFrameWindow* instance, const char* menuItemId) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        // Runtime menu item click dispatch will be supported when native menu rendering is implemented.
        static_cast<void>(window);
        static_cast<void>(menuItemId);
    });
}
}
