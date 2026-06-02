// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_NavigateToString(InfiniFrameWindow* instance, const AutoString content) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(content, "content"))
            return;
        window->NavigateToString(content);
    });
}

EXPORTED InteropStatus InfiniFrame_NavigateToUrl(InfiniFrameWindow* instance, const AutoString url) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(url, "url"))
            return;
        window->NavigateToUrl(url);
    });
}

EXPORTED InteropStatus InfiniFrame_SendWebMessage(InfiniFrameWindow* instance, const AutoString message) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SendWebMessage(NullToEmpty(message));
    });
}
}
