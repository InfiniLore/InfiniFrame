cmake_minimum_required(VERSION 4.0)
project(InfiniFrame.Native
        VERSION 1.1.1
        LANGUAGES CXX
        DESCRIPTION "InfiniFrame.Native webview-wrapper library"
)

if (APPLE)
    enable_language(OBJCXX)
    if (NOT DEFINED CMAKE_OSX_DEPLOYMENT_TARGET OR CMAKE_OSX_DEPLOYMENT_TARGET STREQUAL "")
        set(CMAKE_OSX_DEPLOYMENT_TARGET "12.0" CACHE STRING "Minimum supported macOS version" FORCE)
    endif ()
endif ()

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_OBJCXX_STANDARD 23)
set(CMAKE_OBJCXX_STANDARD_REQUIRED ON)
set(CMAKE_OBJCXX_EXTENSIONS OFF)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

# ----------------------------------------------------------------------------------------------------------------------
# Dependencies
# ----------------------------------------------------------------------------------------------------------------------
include("${CMAKE_SOURCE_DIR}/.cmake/NativeDependencies.cmake")
include("${CMAKE_SOURCE_DIR}/.cmake/Embed.InfiniFrameJs.cmake")
include("${CMAKE_SOURCE_DIR}/.cmake/BuildOptions.cmake")

include("${CMAKE_SOURCE_DIR}/.cmake/Platform.Windows.cmake")
include("${CMAKE_SOURCE_DIR}/.cmake/Platform.MacOS.cmake")
include("${CMAKE_SOURCE_DIR}/.cmake/Platform.Linux.cmake")

infiniframe_configure_compiler_cache()
infiniframe_setup_dependencies()

# ----------------------------------------------------------------------------------------------------------------------
# Source Files
# ----------------------------------------------------------------------------------------------------------------------
set(COMMON_SOURCES
        src/Embedded/Embedded.cpp
        src/Api/Utilities/ExportErrorState.cpp
        src/Runtime/Internal/Application/InfiniFrameApplication.cpp
        src/Runtime/Core/WindowEvents.cpp
        src/Runtime/Core/WindowState.cpp
        src/Runtime/Internal/Operations/NativeOperation.cpp
        src/Runtime/Internal/Operations/DialogOperation.cpp
        src/Api/Exports/Exports.CustomSchemes.cpp
        src/Api/Exports/Exports.Dispatch.cpp
        src/Api/Exports/Exports.Dialog.cpp
        src/Api/Exports/Exports.Events.cpp
        src/Api/Exports/Exports.Lifecycle.cpp
        src/Api/Exports/Exports.Memory.cpp
        src/Api/Exports/Exports.Monitors.cpp
        src/Api/Exports/Exports.Platform.Linux.cpp
        src/Api/Exports/Exports.Platform.MacOs.cpp
        src/Api/Exports/Exports.Platform.Windows.cpp
        src/Api/Exports/Exports.Window.Actions.cpp
        src/Api/Exports/Exports.Window.Getters.cpp
        src/Api/Exports/Exports.Window.Navigation.cpp
        src/Api/Exports/Exports.Window.Setters.cpp
        src/Api/Exports/Exports.Window.Taskbar.cpp
        src/Api/Exports/Exports.Menu.cpp
)

set(TEST_SOURCES
         src/Api/Testing/Exports.Tests.cpp
         src/Api/Testing/Exports.CustomSchemeResponseTests.cpp
         src/Api/Testing/PublicAbiCompileBoundary.cpp
)

set(WINDOWS_SOURCES
        src/Runtime/Platform/Windows/DarkMode.h
        src/Runtime/Platform/Windows/Menu.Win32.h
        src/Runtime/Platform/Windows/ToastHandler.h
        src/Runtime/Platform/Windows/Window.Win32.Context.h
        src/Runtime/Platform/Windows/Window.Win32.Internal.h

        src/Runtime/Platform/Windows/Core/UiDispatcher.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowCore.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowEncoding.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowLifecycle.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowOwnership.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowProc.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowState.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowStorage.Win32.cpp
        src/Runtime/Platform/Windows/Core/WindowTracing.Win32.cpp
        src/Runtime/Platform/Windows/DarkMode.cpp
        src/Runtime/Platform/Windows/Dialog.cpp
        src/Runtime/Platform/Windows/Dpi.Win32.cpp
        src/Runtime/Platform/Windows/Menu.Win32.cpp
        src/Runtime/Platform/Windows/Monitors.Win32.cpp
        src/Runtime/Platform/Windows/Notifications.WinToast.cpp
        src/Runtime/Platform/Windows/Taskbar.Win32.cpp
        src/Runtime/Platform/Windows/WebView/WebView2Attach.Win32.cpp
        src/Runtime/Platform/Windows/WebView/WebView2Controller.Win32.cpp
        src/Runtime/Platform/Windows/WebView/WebView2CustomSchemes.Win32.cpp
        src/Runtime/Platform/Windows/WebView/WebView2Host.Win32.cpp
        src/Runtime/Platform/Windows/WebView/WebView2Runtime.Win32.cpp
        src/Runtime/Platform/Windows/WebView/WebView2Settings.Win32.cpp
)

set(LINUX_SOURCES
        src/Runtime/Platform/Linux/WebKit/WebKit.Gtk.Internal.h
        src/Runtime/Platform/Linux/Window.Gtk.Internal.h
        src/Runtime/Platform/Linux/Core/GtkCallbackGuard.h
        src/Runtime/Platform/Linux/Core/LinuxGraphicsEnvironment.Gtk.h

        src/Runtime/Platform/Linux/Core/LinuxGraphicsEnvironment.Gtk.cpp
        src/Runtime/Platform/Linux/Core/UiDispatcher.Gtk.cpp
        src/Runtime/Platform/Linux/Core/UiThread.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowCore.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowEvents.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowInitialization.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowLifecycle.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowSignals.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowState.Gtk.cpp
        src/Runtime/Platform/Linux/Dialog.cpp
        src/Runtime/Platform/Linux/Menu.Gtk.cpp
        src/Runtime/Platform/Linux/Monitors.Gtk.cpp
        src/Runtime/Platform/Linux/Notifications.LibNotify.cpp
        src/Runtime/Platform/Linux/Taskbar.Gtk.cpp
        src/Runtime/Platform/Linux/WebKit/WebKitCustomSchemes.Gtk.cpp
        src/Runtime/Platform/Linux/WebKit/WebKitHost.Gtk.cpp
        src/Runtime/Platform/Linux/WebKit/WebKitInspector.Gtk.cpp
        src/Runtime/Platform/Linux/WebKit/WebKitMessaging.Gtk.cpp
        src/Runtime/Platform/Linux/WebKit/WebKitSettings.Gtk.cpp
)

set(MAC_SOURCES
        src/Runtime/Platform/Mac/Delegates/AppDelegate.h
        src/Runtime/Platform/Mac/Delegates/NavigationDelegate.h
        src/Runtime/Platform/Mac/Delegates/UiDelegate.h
        src/Runtime/Platform/Mac/Delegates/UrlSchemeHandler.h
        src/Runtime/Platform/Mac/Delegates/WindowDelegate.h
        src/Runtime/Platform/Mac/NSWindowBorderless.h
        src/Runtime/Platform/Mac/Window.Cocoa.Internal.h
        src/Runtime/Platform/Mac/CocoaCoordinates.h
        src/Runtime/Platform/Mac/MacDiagnostics.h
        src/Runtime/Platform/Mac/WebKit/InfiniFrameWebView.h

        src/Runtime/Platform/Mac/Core/UiDispatcher.Cocoa.mm
        src/Runtime/Platform/Mac/Core/WindowCore.Cocoa.mm
        src/Runtime/Platform/Mac/Core/WindowEvents.Cocoa.mm
        src/Runtime/Platform/Mac/Core/WindowLifecycle.Cocoa.mm
        src/Runtime/Platform/Mac/Core/WindowState.Cocoa.mm
        src/Runtime/Platform/Mac/Delegates/AppDelegate.mm
        src/Runtime/Platform/Mac/Delegates/NavigationDelegate.mm
        src/Runtime/Platform/Mac/Delegates/UiDelegate.mm
        src/Runtime/Platform/Mac/Delegates/UrlSchemeHandler.mm
        src/Runtime/Platform/Mac/Delegates/WindowDelegate.mm
        src/Runtime/Platform/Mac/Dialog.mm
        src/Runtime/Platform/Mac/Menu.Cocoa.mm
        src/Runtime/Platform/Mac/Monitors.Cocoa.mm
        src/Runtime/Platform/Mac/MacDiagnostics.mm
        src/Runtime/Platform/Mac/NSWindowBorderless.mm
        src/Runtime/Platform/Mac/Notifications.UserNotifications.mm
        src/Runtime/Platform/Mac/Taskbar.Cocoa.mm
        src/Runtime/Platform/Mac/WebKit/WebKitBridge.Cocoa.mm
        src/Runtime/Platform/Mac/WebKit/WebKitCustomSchemes.Cocoa.mm
        src/Runtime/Platform/Mac/WebKit/WebKitHost.Cocoa.mm
        src/Runtime/Platform/Mac/WebKit/InfiniFrameWebView.mm
)

set(HEADER_FILES
        src/Embedded/Embedded.h
        src/Runtime/Internal/WebView/CustomSchemeResponse.h
        src/Runtime/Internal/Operations/NativeOperation.h
        src/Runtime/Internal/Operations/NavigationOperation.h
        src/Api/Exports/Exports.h
        src/Api/Utilities/ExportErrorState.h
        src/Api/Utilities/ExportExecution.h
        src/Api/Utilities/ExportStringHelpers.h
        src/Api/Utilities/ExportValidation.h
        src/Api/Utilities/Utilities.h
        src/Api/Abi/InfiniFrame.h
        src/Api/Abi/InfiniFrameApplicationInitParams.h
        src/Runtime/Internal/Window/InfiniFrameDialog.h
        src/Api/Abi/InfiniFrameWindowInitParams.h
        src/Api/Abi/InfiniFrameWindow.h
        src/Runtime/Internal/Application/InfiniFrameApplication.h
        src/Api/Abi/Basic.h
        src/Api/Abi/Callbacks.h
        src/Api/Abi/CustomSchemeResponse.h
        src/Api/Abi/DialogButtons.h
        src/Api/Abi/DialogIcon.h
        src/Api/Abi/DialogResult.h
        src/Api/Abi/Monitor.h
        src/Api/Utilities/Dimensions.h
        src/Api/Utilities/ErrorCode.h
        src/Api/Utilities/InteropStatus.h
        src/Api/Utilities/StringArrayCopy.h
        src/Api/Utilities/StringCopy.h
)

if (WIN32)
    infiniframe_configure_windows_target(
            ${PROJECT_NAME}
            "${COMMON_SOURCES}"
            "${TEST_SOURCES}"
            "${WINDOWS_SOURCES}"
            "${HEADER_FILES}"
    )

    infiniframe_setup_embed_js(${PROJECT_NAME})

elseif (APPLE)
    infiniframe_configure_macos_target(
            ${PROJECT_NAME}
            "${COMMON_SOURCES}"
            "${TEST_SOURCES}"
            "${MAC_SOURCES}"
            "${HEADER_FILES}"
    )

    infiniframe_setup_embed_js(${PROJECT_NAME})
elseif (UNIX)
    infiniframe_configure_linux_target(
            ${PROJECT_NAME}
            "${COMMON_SOURCES}"
            "${TEST_SOURCES}"
            "${LINUX_SOURCES}"
            "${HEADER_FILES}"
    )

    infiniframe_setup_embed_js(${PROJECT_NAME})
endif ()

if (DEFINED INFINIFRAME_BUILD_TEST_EXPORTS)
    target_compile_definitions(${PROJECT_NAME} PRIVATE
            $<$<BOOL:${INFINIFRAME_BUILD_TEST_EXPORTS}>:INFINIFRAME_BUILD_TEST_EXPORTS=1>
    )
else ()
    target_compile_definitions(${PROJECT_NAME} PRIVATE
            $<$<CONFIG:Debug>:INFINIFRAME_BUILD_TEST_EXPORTS=1>
    )
endif ()

infiniframe_enable_sanitizers(${PROJECT_NAME})

# ----------------------------------------------------------------------------------------------------------------------
infiniframe_enable_fast_build_settings(${PROJECT_NAME})
