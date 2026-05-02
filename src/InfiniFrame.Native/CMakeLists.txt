cmake_minimum_required(VERSION 4.0)
project(InfiniFrame.Native
        VERSION 1.1.1
        LANGUAGES CXX
        DESCRIPTION "InfiniFrame.Native webview-wrapper library"
)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

option(INFINIFRAME_BUILD_TEST_EXPORTS "Build test-only native exports into InfiniFrame.Native" OFF)

if (APPLE)
    set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE STRING "" FORCE)
endif ()

# ----------------------------------------------------------------------------------------------------------------------
# Dependencies
# ----------------------------------------------------------------------------------------------------------------------
include("${CMAKE_SOURCE_DIR}/cmake/NativeDependencies.cmake")
include("${CMAKE_SOURCE_DIR}/cmake/Embed.InfiniFrameHostJs.cmake")

include("${CMAKE_SOURCE_DIR}/cmake/Platform.Windows.cmake")
include("${CMAKE_SOURCE_DIR}/cmake/Platform.MacOS.cmake")
include("${CMAKE_SOURCE_DIR}/cmake/Platform.Linux.cmake")

infiniframe_setup_dependencies()

# ----------------------------------------------------------------------------------------------------------------------
# Source Files
# ----------------------------------------------------------------------------------------------------------------------
set(COMMON_SOURCES
        Interop/Exports.cpp
        Interop/Exports.Browser.cpp
        Interop/Exports.Dialog.cpp
        Interop/Exports.Events.cpp
        Interop/Exports.Lifecycle.cpp
        Interop/Exports.Memory.cpp
        Interop/Exports.Platform.cpp
        Interop/Exports.WindowCommands.cpp
)

set(TEST_SOURCES
        Exports.Tests.cpp
)

if (NOT INFINIFRAME_BUILD_TEST_EXPORTS)
    set(TEST_SOURCES)
endif ()

set(WINDOWS_SOURCES
        Platform/Windows/Window.cpp
        Platform/Windows/Monitors.Win32.cpp
        Platform/Windows/Notifications.WinToast.cpp
        Platform/Windows/UiDispatcher.Win32.cpp
        Platform/Windows/WindowProc.Win32.cpp
        Platform/Windows/WebView2Bridge.Win32.cpp
        Platform/Windows/WebView2CustomSchemes.Win32.cpp
        Platform/Windows/WebView2Host.Win32.cpp
        Platform/Windows/WebView2Messaging.Win32.cpp
        Platform/Windows/WebView2ResourceRequests.Win32.cpp
        Platform/Windows/WebView2Settings.Win32.cpp
        Platform/Windows/DarkMode.cpp
        Platform/Windows/Dialog.cpp
)

set(LINUX_SOURCES
        Platform/Linux/Window.cpp
        Platform/Linux/Monitors.Gtk.cpp
        Platform/Linux/Notifications.LibNotify.cpp
        Platform/Linux/UiDispatcher.Gtk.cpp
        Platform/Linux/WindowState.Gtk.cpp
        Platform/Linux/WebKitBridge.Gtk.cpp
        Platform/Linux/WebKitCustomSchemes.Gtk.cpp
        Platform/Linux/WebKitMessaging.Gtk.cpp
        Platform/Linux/WebKitSettings.Gtk.cpp
        Platform/Linux/Dialog.cpp
)

set(MAC_SOURCES
        Platform/Mac/AppDelegate.mm
        Platform/Mac/UiDelegate.mm
        Platform/Mac/WindowDelegate.mm
        Platform/Mac/NavigationDelegate.mm
        Platform/Mac/UrlSchemeHandler.mm
        Platform/Mac/NSWindowBorderless.mm
        Platform/Mac/Monitors.Cocoa.mm
        Platform/Mac/Notifications.UserNotifications.Cocoa.mm
        Platform/Mac/UiDispatcher.Cocoa.mm
        Platform/Mac/WKCustomSchemes.Cocoa.mm
        Platform/Mac/WKJsInterop.Cocoa.mm
        Platform/Mac/WKWebViewBridge.Cocoa.mm
        Platform/Mac/WKWebViewSettings.Cocoa.mm
        Platform/Mac/WindowState.Cocoa.mm
        Platform/Mac/Dialog.mm
        Platform/Mac/Window.mm
)

set(HEADER_FILES
        Core/InfiniFrame.h
        Interop/InitParamsReader.h
        Interop/ExportApi.h
        Interop/NativeBuffer.h
        Interop/NativeResult.h
        Interop/NativeString.h
        Shared/CustomSchemeResponse.h
        Core/InfiniFrameWindow.h
        Core/InfiniFrameDialog.h
        Embedded/Embedded.h
        Embedded/InfiniFrameHostJs/InfiniFrameHostJs.h
        Types/Basic.h
        Types/Dialog.h
        Core/InfiniFrameInitParams.h
        Types/Callbacks.h
        Utils/Common.h
        Utils/Event.h
        Platform/Windows/ToastHandler.h
        Platform/Windows/WindowImpl.Win32.h
        Platform/Windows/WindowProc.Win32.h
        Platform/Windows/WebView2CustomSchemes.Win32.h
        Platform/Linux/WindowImpl.Gtk.h
        Platform/Linux/WebKitMessaging.Gtk.h
        Platform/Windows/DarkMode.h
        Platform/Mac/AppDelegate.h
        Platform/Mac/NavigationDelegate.h
        Platform/Mac/NSWindowBorderless.h
        Platform/Mac/UiDelegate.h
        Platform/Mac/WindowDelegate.h
        Platform/Mac/WindowImpl.Cocoa.h
        Platform/Mac/UrlSchemeHandler.h
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

# ----------------------------------------------------------------------------------------------------------------------
# Sanitizers (Debug only)
# ----------------------------------------------------------------------------------------------------------------------
if (CMAKE_BUILD_TYPE STREQUAL "Debug")
    if (CMAKE_CXX_COMPILER_ID MATCHES "Clang|GNU")
        message(STATUS "Enabling sanitizers for Debug build")
        target_compile_options(${PROJECT_NAME} PRIVATE
                -fsanitize=address,undefined,leak
                -fno-omit-frame-pointer
                -fno-optimize-sibling-calls
        )
        target_link_options(${PROJECT_NAME} PRIVATE
                -fsanitize=address,undefined,leak
        )
    endif ()
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
