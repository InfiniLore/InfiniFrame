#include "Interop/ExportApi.h"

using namespace InfiniFrame::Native::Interop;

extern "C" {
    /**
     * @brief Center window on screen
     * @param instance InfiniFrame instance
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_Center(InfiniFrameWindow* instance) {
        return RunWindowExportStatus(instance, [](InfiniFrameWindow& window) {
            window.Center();
        });
    }

    /**
     * @brief Close window
     * @param instance InfiniFrame instance
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_Close(InfiniFrameWindow* instance) {
        return RunWindowExportStatus(instance, [](InfiniFrameWindow& window) {
            window.Close();
        });
    }

    /**
     * @brief Restore window from minimized/maximized state
     * @param instance InfiniFrame instance
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_Restore(InfiniFrameWindow* instance) {
        return RunWindowExportStatus(instance, [](InfiniFrameWindow& window) {
            window.Restore();
        });
    }

    /**
     * @brief Wait for window exit
     * @param instance InfiniFrame instance
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_WaitForExit(InfiniFrameWindow* instance) {
        return RunWindowExportStatus(instance, [](InfiniFrameWindow& window) {
            window.WaitForExit();
        });
    }

    /**
     * @brief Set window focused
     * @param instance InfiniFrame instance
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SetFocused(InfiniFrameWindow* instance) {
        return RunWindowExportStatus(instance, [](InfiniFrameWindow& window) {
            window.SetFocused();
        });
    }
}
