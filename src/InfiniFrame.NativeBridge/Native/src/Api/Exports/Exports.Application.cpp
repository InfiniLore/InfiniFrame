// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/ApplicationInitParams.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {

/// @brief Creates a new application instance with the given parameters.
/// @param params Application initialization parameters.
/// @param[out] value Receives the newly created application handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_ctor(
    ApplicationInitParams* params, InfiniFrameApplication** value) {
    ResetOut(value, static_cast<InfiniFrameApplication*>(nullptr));
    return RunExportStatus(
        [&] {
            if (!EnsureOutNotNull(value, "value"))
                return;
            if (params == nullptr)
                throw std::invalid_argument("Argument 'params' is null.");
            if (params->StructSize != static_cast<int>(sizeof(ApplicationInitParams)))
                throw std::invalid_argument("ApplicationInitParams size mismatch.");
            auto instance = std::make_unique<InfiniFrameApplication>(params);
            *value = instance.release();
        });
}

/// @brief Destroys the application instance and releases resources.
/// @param instance The application handle to destroy.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_dtor(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
            std::unique_ptr<InfiniFrameApplication> guard{instance};
        });
}

/// @brief Runs the application message loop, blocking until all windows close or Shutdown() is called.
/// @param instance The application handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_Run(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
            instance->Run();
        });
}

/// @brief Signals the application message loop to exit.
/// @param instance The application handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_Shutdown(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
            instance->Shutdown();
        });
}

/// @brief Checks if Shutdown() has been called.
/// @param instance The application handle.
/// @param[out] value Receives true if shutdown was requested.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_IsShutdownRequested(
    InfiniFrameApplication* instance, bool* value) {
    ResetOut(value, false);
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = instance->IsShutdownRequested();
        });
}

#ifdef _WIN32
/// @brief Registers the Win32 window class and sets DPI awareness.
/// @param instance The application handle.
/// @param hInstance The application instance handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_register_win32(
    InfiniFrameApplication* instance, HINSTANCE hInstance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
            if (hInstance == nullptr)
                throw std::invalid_argument("Argument 'hInstance' is null.");
            instance->Register(hInstance);
        });
}
#endif

#ifdef __APPLE__
/// @brief Sets up NSApplication delegate and activation policy.
/// @param instance The application handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Application_register_mac(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
            instance->Register();
        });
}
#endif

}
