#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstddef>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/**
 * @brief Process-wide initialization parameters for InfiniFrameApplication.
 *
 * Field order defines the ABI layout shared with the managed
 * InfiniFrameNativeApplicationParameters type. New fields must be appended
 * before StructSize and the managed/native layouts must be updated together.
 */
struct InfiniFrameApplicationInitParams {
    const char* WebView2RuntimePath;
    const char* NotificationRegistrationId;
    const char* AppUserModelId;
    const char* DefaultNotificationIcon;

    // ABI version/size marker. This field must remain last.
    int StructSize;
};
