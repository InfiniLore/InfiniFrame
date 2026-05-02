#include "Interop/ExportApi.h"

using namespace InfiniFrame::Native::Interop;

extern "C" {
    /**
     * @brief Clear browser auto-fill data
     * @param instance InfiniFrame instance
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_ClearBrowserAutoFill(InfiniFrameWindow* instance) {
        return RunWindowExportStatus(instance, [](InfiniFrameWindow& window) {
            window.ClearBrowserAutoFill();
        });
    }

    /**
     * @brief Navigate to HTML string
     * @param instance InfiniFrame instance
     * @param content HTML content string
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_NavigateToString(
        InfiniFrameWindow* instance,
        const AutoString content
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.NavigateToString(content);
        });
    }

    /**
     * @brief Navigate to URL
     * @param instance InfiniFrame instance
     * @param url URL to navigate to
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_NavigateToUrl(
        InfiniFrameWindow* instance,
        const AutoString url
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.NavigateToUrl(url);
        });
    }

    /**
     * @brief Send message to WebView JavaScript
     * @param instance InfiniFrame instance
     * @param message Message string to send
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_SendWebMessage(
        InfiniFrameWindow* instance,
        const AutoString message
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SendWebMessage(message);
        });
    }

    /**
     * @brief Show notification
     * @param instance InfiniFrame instance
     * @param title Notification title
     * @param body Notification body
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_ShowNotification(
        InfiniFrameWindow* instance,
        const AutoString title,
        const AutoString body
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.ShowNotification(title, body);
        });
    }

    /**
     * @brief Add custom scheme name
     * @param instance InfiniFrame instance
     * @param scheme Scheme name to add
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_AddCustomSchemeName(
        InfiniFrameWindow* instance,
        const AutoString scheme
        ) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.AddCustomSchemeName(scheme);
        });
    }
}
