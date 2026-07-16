cmake_minimum_required(VERSION 4.0)
project(InfiniFrame.Native
        VERSION 1.1.1
        LANGUAGES CXX
        DESCRIPTION "InfiniFrame.Native webview-wrapper library"
)

if (APPLE)
    enable_language(OBJCXX)
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

include("${CMAKE_SOURCE_DIR}/.cmake/Platform.Windows.cmake")
include("${CMAKE_SOURCE_DIR}/.cmake/Platform.MacOS.cmake")
include("${CMAKE_SOURCE_DIR}/.cmake/Platform.Linux.cmake")

infiniframe_setup_dependencies()

# ----------------------------------------------------------------------------------------------------------------------
# Source Files
# ----------------------------------------------------------------------------------------------------------------------
set(COMMON_SOURCES
        src/Runtime/Shared/Window/WindowEvents.cpp
        src/Runtime/Shared/Window/WindowState.cpp
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
)

set(TEST_SOURCES
        src/Api/Testing/Exports.Tests.cpp
)

set(WINDOWS_SOURCES
        src/Runtime/Platform/Windows/DarkMode.h
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
        src/Runtime/Platform/Windows/Monitors.Win32.cpp
        src/Runtime/Platform/Windows/Notifications.WinToast.cpp
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
        
        src/Runtime/Platform/Linux/Core/UiDispatcher.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowCore.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowEvents.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowInitialization.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowLifecycle.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowSignals.Gtk.cpp
        src/Runtime/Platform/Linux/Core/WindowState.Gtk.cpp
        src/Runtime/Platform/Linux/Dialog.cpp
        src/Runtime/Platform/Linux/Monitors.Gtk.cpp
        src/Runtime/Platform/Linux/Notifications.LibNotify.cpp
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
        src/Runtime/Platform/Mac/Monitors.Cocoa.mm
        src/Runtime/Platform/Mac/NSWindowBorderless.mm
        src/Runtime/Platform/Mac/Notifications.UserNotifications.mm
        src/Runtime/Platform/Mac/WebKit/WebKitBridge.Cocoa.mm
        src/Runtime/Platform/Mac/WebKit/WebKitCustomSchemes.Cocoa.mm
        src/Runtime/Platform/Mac/WebKit/WebKitHost.Cocoa.mm
)

set(HEADER_FILES
        src/Embedded/Embedded.h
        src/Embedded/InfiniFrameJs/InfiniFrameJs.h
        src/Runtime/Shared/WebView/CustomSchemeResponse.h
        src/Api/Exports/Exports.h
        src/Api/Utilities/ExportErrorState.h
        src/Api/Utilities/ExportExecution.h
        src/Api/Utilities/ExportStringHelpers.h
        src/Api/Utilities/ExportValidation.h
        src/Api/Utilities/Utilities.h
        src/Runtime/Shared/Window/InfiniFrame.h
        src/Runtime/Shared/Window/InfiniFrameDialog.h
        src/Runtime/Shared/Window/InfiniFrameInitParams.h
        src/Runtime/Shared/Window/InfiniFrameWindow.h
        src/Runtime/Shared/Types/Basic.h
        src/Runtime/Shared/Types/Callbacks.h
        src/Runtime/Shared/Types/DialogButtons.h
        src/Runtime/Shared/Types/DialogIcon.h
        src/Runtime/Shared/Types/DialogResult.h
        src/Runtime/Shared/Types/Monitor.h
        src/Runtime/Shared/Utilities/Dimensions.h
        src/Runtime/Shared/Utilities/ErrorCode.h
        src/Runtime/Shared/Utilities/InteropStatus.h
        src/Runtime/Shared/Utilities/StringArrayCopy.h
        src/Runtime/Shared/Utilities/StringCopy.h
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

# ----------------------------------------------------------------------------------------------------------------------
# Sanitizers (Debug only)
# ----------------------------------------------------------------------------------------------------------------------
if (CMAKE_CXX_COMPILER_ID MATCHES "Clang|GNU")
    message(STATUS "Enabling sanitizers for Debug configuration")
    target_compile_options(${PROJECT_NAME} PRIVATE
            $<$<CONFIG:Debug>:-fsanitize=address,undefined,leak>
            $<$<CONFIG:Debug>:-fno-omit-frame-pointer>
            $<$<CONFIG:Debug>:-fno-optimize-sibling-calls>
    )
    target_link_options(${PROJECT_NAME} PRIVATE
            $<$<CONFIG:Debug>:-fsanitize=address,undefined,leak>
    )
endif ()

# ----------------------------------------------------------------------------------------------------------------------
# Precompiled Headers
# ----------------------------------------------------------------------------------------------------------------------
if (NOT APPLE)
    target_precompile_headers(${PROJECT_NAME} PRIVATE
            <string>
            <memory>
            <functional>
            <expected>
    )
endif ()
