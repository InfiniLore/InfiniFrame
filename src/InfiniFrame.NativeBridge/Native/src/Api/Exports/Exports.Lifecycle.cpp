// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#ifdef __linux__
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Creates a native application instance.
EXPORTED InteropStatus InfiniFrameNativeApplication_ctor(InfiniFrameApplication** value) {
    ResetOut(value, static_cast<InfiniFrameApplication*>(nullptr));
    return RunExportStatus(
        [&] {
            if (!EnsureOutNotNull(value, "value")) return;
            auto instance = std::make_unique<InfiniFrameApplication>();
            *value = instance.release();
        });
}

/// @brief Registers process-wide native application state.
EXPORTED InteropStatus InfiniFrameNativeApplication_Register(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance")) return;
            instance->Register();
        });
}

EXPORTED InteropStatus InfiniFrameNativeApplication_Configure(
    InfiniFrameApplication* instance,
    const char* webView2RuntimePath,
    const char* notificationRegistrationId,
    const char* appUserModelId,
    const char* defaultNotificationIcon
) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance")) return;
            instance->Configure(
                webView2RuntimePath,
                notificationRegistrationId,
                appUserModelId,
                defaultNotificationIcon
            );
        });
}

/// @brief Runs the native application loop.
EXPORTED InteropStatus InfiniFrameNativeApplication_Run(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance")) return;
            instance->Run();
        });
}

/// @brief Requests native application shutdown.
EXPORTED InteropStatus InfiniFrameNativeApplication_Shutdown(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance")) return;
            instance->Shutdown();
        });
}

/// @brief Destroys a native application instance.
EXPORTED InteropStatus InfiniFrameNativeApplication_dtor(InfiniFrameApplication* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance")) return;
            std::unique_ptr<InfiniFrameApplication> guard{instance};
        });
}

/// @brief Returns the number of native windows tracked by the application.
EXPORTED InteropStatus InfiniFrameNativeApplication_GetWindowCount(
    InfiniFrameApplication* instance,
    std::size_t* value
) {
    ResetOut(value, static_cast<std::size_t>(0));
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance") || !EnsureOutNotNull(value, "value")) return;
            *value = instance->GetWindowCount();
        });
}

/// @brief Creates a new native window with the given parameters.
/// @param initParams Initialization parameters for the window.
/// @param[out] value Receives the newly created window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_ctor(InfiniFrameInitParams* initParams, InfiniFrameWindow** value) {
    ResetOut(value, static_cast<InfiniFrameWindow*>(nullptr));
    return RunExportStatus(
        [&] {
            if (!EnsureOutNotNull(value, "value"))
                return;
            if (initParams == nullptr)
                throw std::invalid_argument("Argument 'initParams' is null.");
            if (initParams->StructSize != static_cast<int>(sizeof(InfiniFrameInitParams))) {
                throw std::invalid_argument("InfiniFrameInitParams.Size does not match native struct size.");
            }
            auto instance = std::make_unique<InfiniFrameWindow>(initParams);
            *value = instance.release();
        });
}

/// @brief Destroys the native window and releases resources.
/// @param instance The window handle to destroy.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_dtor(InfiniFrameWindow* instance) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
#ifdef __APPLE__
            // WKWebView close is asynchronous.  SafeHandle may be disposed immediately after
            // Close (and from a reverse P/Invoke callback), so the native instance takes ownership
            // of its own final deletion and performs it only after AppKit's close boundary.
            instance->ScheduleDeferredDestruction();
            return;
#endif
            std::unique_ptr<InfiniFrameWindow> guard{instance};
        });
}

/// @brief Initiates window close.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Close(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->Close();
        });
}

/// @brief Blocks until the window is destroyed.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_WaitForExit(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->WaitForExit();
        });
}

/// @brief Registers a callback for when the window is ready.
/// @param instance The window handle.
/// @param callback Context action invoked when the window is ready.
/// @param context User-defined context passed to the callback.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetReadyCallback(
    InfiniFrameWindow* instance,
    const ContextAction callback,
    void* context
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            window->SetReadyCallback(callback, context);
        });
}

/// @brief Registers a callback for when teardown begins.
/// @param instance The window handle.
/// @param callback Context action invoked when teardown starts.
/// @param context User-defined context passed to the callback.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetTeardownCallback(
    InfiniFrameWindow* instance,
    const ContextAction callback,
    void* context
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            window->SetTeardownCallback(callback, context);
        });
}

#ifdef __linux__
/// @brief Forces immediate shutdown of the native window (Linux only).
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Shutdown() {
    return RunExportStatus(
        [] {
            infiniframe::linux_gtk::ui_thread::Shutdown();
        });
}
#endif
}
