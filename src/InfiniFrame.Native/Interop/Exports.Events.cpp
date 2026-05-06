#include "Interop/ExportApi.h"

using namespace InfiniFrame::Native::Interop;

extern "C" {
    /**
     * @brief Get all monitors
     * @param instance InfiniFrame instance
     * @param callback Callback function to receive monitor info
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_GetAllMonitors(
        InfiniFrameWindow* instance,
        const GetAllMonitorsCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetAllMonitors(callback);
        });
    }

    /**
     * @brief Set closing callback
     * @param instance InfiniFrame instance
     * @param callback Closing callback
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetClosingCallback(
        InfiniFrameWindow* instance,
        const ClosingCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetClosingCallback(callback);
        });
    }

    /**
     * @brief Set closed callback
     * @param instance InfiniFrame instance
     * @param callback Closed callback
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetClosedCallback(
        InfiniFrameWindow* instance,
        const ClosedCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetClosedCallback(callback);
        });
    }

    // Backward-compatible export name retained for existing consumers.
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_setClosedClosedCallback(
        InfiniFrameWindow* instance,
        const ClosedCallback callback
        ) {
        return InfiniFrame_SetClosedCallback(instance, callback);
    }

    /**
     * @brief Set focus-in callback
     * @param instance InfiniFrame instance
     * @param callback Focus-in callback
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetFocusInCallback(
        InfiniFrameWindow* instance,
        const FocusInCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetFocusInCallback(callback);
        });
    }

    /**
     * @brief Set focus-out callback
     * @param instance InfiniFrame instance
     * @param callback Focus-out callback
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetFocusOutCallback(
        InfiniFrameWindow* instance,
        const FocusOutCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetFocusOutCallback(callback);
        });
    }

    /**
     * @brief Set moved callback
     * @param instance InfiniFrame instance
     * @param callback Moved callback
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetMovedCallback(
        InfiniFrameWindow* instance,
        const MovedCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetMovedCallback(callback);
        });
    }

    /**
     * @brief Set resized callback
     * @param instance InfiniFrame instance
     * @param callback Resized callback
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetResizedCallback(
        InfiniFrameWindow* instance,
        const ResizedCallback callback
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetResizedCallback(callback);
        });
    }

    /**
     * @brief Invoke callback on UI thread
     * @param instance InfiniFrame instance
     * @param callback Callback to invoke
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_Invoke(InfiniFrameWindow* instance, const ACTION callback) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.Invoke(callback);
        });
    }
}
