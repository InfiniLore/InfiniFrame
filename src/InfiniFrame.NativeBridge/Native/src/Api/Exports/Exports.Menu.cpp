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
        window->SetMenuBarJson(menuBarJson);
    });
}

EXPORTED InteropStatus InfiniFrameNative_SetMenuItemEnabled(InfiniFrameWindow* instance, const char* menuItemId, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SetMenuItemEnabledById(menuItemId, enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_SetMenuItemVisible(InfiniFrameWindow* instance, const char* menuItemId, const bool visible) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SetMenuItemVisibleById(menuItemId, visible);
    });
}

EXPORTED InteropStatus InfiniFrameNative_ClickMenuItem(InfiniFrameWindow* instance, const char* menuItemId) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->ClickMenuItemById(menuItemId);
    });
}
}
