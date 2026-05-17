#include "Public/Exports/Exports.h"

extern "C" {
EXPORTED InteropStatus InfiniFrame_AddCustomSchemeName(InfiniFrameWindow* instance, const AutoString scheme) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(scheme, "scheme"))
            return;
        window->AddCustomSchemeName(scheme);
    });
}

EXPORTED InteropStatus InfiniFrame_GetAllMonitors(InfiniFrameWindow* instance, const GetAllMonitorsCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (callback == nullptr)
            throw std::invalid_argument("Argument 'callback' is null.");
        window->GetAllMonitors(callback);
    });
}

EXPORTED InteropStatus InfiniFrame_SetClosingCallback(InfiniFrameWindow* instance, const ClosingCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetClosingCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_setClosedClosedCallback(InfiniFrameWindow* instance, const ClosedCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetClosedCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetFocusInCallback(InfiniFrameWindow* instance, const FocusInCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFocusInCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetFocusOutCallback(InfiniFrameWindow* instance, const FocusOutCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFocusOutCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetMovedCallback(InfiniFrameWindow* instance, const MovedCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMovedCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetResizedCallback(InfiniFrameWindow* instance, const ResizedCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizedCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_Invoke(InfiniFrameWindow* instance, const ACTION callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (callback == nullptr)
            throw std::invalid_argument("Argument 'callback' is null.");
        window->Invoke(callback);
    });
}
}
