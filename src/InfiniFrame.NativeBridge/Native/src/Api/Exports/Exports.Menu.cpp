// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Sets the native menu bar from a JSON description.
/// @param instance The window handle.
/// @param menuBarJson Null-terminated JSON string describing the menu bar.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMenuBar(InfiniFrameWindow* instance, const char* menuBarJson) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMenuBarJson(menuBarJson);
        });
}

/// @brief Enables or disables a menu item by ID.
/// @param instance The window handle.
/// @param menuItemId Null-terminated ID of the menu item.
/// @param enabled True to enable, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMenuItemEnabled(
    InfiniFrameWindow* instance,
    const char* menuItemId,
    const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMenuItemEnabledById(menuItemId, enabled);
        });
}

/// @brief Shows or hides a menu item by ID.
/// @param instance The window handle.
/// @param menuItemId Null-terminated ID of the menu item.
/// @param visible True to show, false to hide.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMenuItemVisible(
    InfiniFrameWindow* instance,
    const char* menuItemId,
    const bool visible) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMenuItemVisibleById(menuItemId, visible);
        });
}

/// @brief Programmatically clicks a menu item by ID.
/// @param instance The window handle.
/// @param menuItemId Null-terminated ID of the menu item to click.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_ClickMenuItem(InfiniFrameWindow* instance, const char* menuItemId) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->ClickMenuItemById(menuItemId);
        });
}
}