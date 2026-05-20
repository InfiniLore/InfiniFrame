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
        Public/Exports/Exports.Platform.cpp
        Public/Exports/Exports.Lifecycle.cpp
        Public/Exports/Exports.WindowState.cpp
        Public/Exports/Exports.WindowCommands.cpp
        Public/Exports/Exports.Dialog.cpp
        Public/Exports/Exports.Events.cpp
        Public/Exports/Exports.Memory.cpp
        Platform/Shared/WindowEvents.cpp
        Platform/Shared/WindowState.cpp
)

set(TEST_SOURCES
        Public/Exports/Exports.Tests.cpp
)

set(WINDOWS_SOURCES
        Platform/Windows/Core/WindowCore.Win32.cpp
        Platform/Windows/Core/WindowTracing.Win32.cpp
        Platform/Windows/Core/WindowEncoding.Win32.cpp
        Platform/Windows/Core/WindowStorage.Win32.cpp
        Platform/Windows/Core/WindowOwnership.Win32.cpp
        Platform/Windows/Core/WindowLifecycle.Win32.cpp
        Platform/Windows/Core/WindowProc.Win32.cpp
        Platform/Windows/Core/WindowState.Win32.cpp
        Platform/Windows/Core/WindowEvents.Win32.cpp
        Platform/Windows/Core/UiDispatcher.Win32.cpp
        Platform/Windows/WebView/WebView2Host.Win32.cpp
        Platform/Windows/WebView/WebView2Runtime.Win32.cpp
        Platform/Windows/WebView/WebView2Controller.Win32.cpp
        Platform/Windows/WebView/WebView2Attach.Win32.cpp
        Platform/Windows/WebView/WebView2CustomSchemes.Win32.cpp
        Platform/Windows/WebView/WebView2Settings.Win32.cpp
        Platform/Windows/Monitors.Win32.cpp
        Platform/Windows/Notifications.WinToast.cpp
        Platform/Windows/Dpi.Win32.cpp
        Platform/Windows/DarkMode.cpp
        Platform/Windows/Dialog.cpp
)

set(LINUX_SOURCES
        Platform/Linux/Core/WindowCore.Gtk.cpp
        Platform/Linux/Core/WindowInitialization.Gtk.cpp
        Platform/Linux/Core/WindowLifecycle.Gtk.cpp
        Platform/Linux/Core/WindowState.Gtk.cpp
        Platform/Linux/Core/WindowEvents.Gtk.cpp
        Platform/Linux/Core/UiDispatcher.Gtk.cpp
        Platform/Linux/Core/WindowSignals.Gtk.cpp
        Platform/Linux/WebKit/WebKitHost.Gtk.cpp
        Platform/Linux/WebKit/WebKitSettings.Gtk.cpp
        Platform/Linux/WebKit/WebKitMessaging.Gtk.cpp
        Platform/Linux/WebKit/WebKitCustomSchemes.Gtk.cpp
        Platform/Linux/Notifications.LibNotify.cpp
        Platform/Linux/Monitors.Gtk.cpp
        Platform/Linux/Dialog.cpp
)

set(MAC_SOURCES
        Platform/Mac/Delegates/AppDelegate.mm
        Platform/Mac/Delegates/UiDelegate.mm
        Platform/Mac/Delegates/WindowDelegate.mm
        Platform/Mac/Delegates/NavigationDelegate.mm
        Platform/Mac/Delegates/UrlSchemeHandler.mm
        Platform/Mac/NSWindowBorderless.mm
        Platform/Mac/Dialog.mm
        Platform/Mac/Notifications.UserNotifications.mm
        Platform/Mac/Monitors.Cocoa.mm
        Platform/Mac/Core/WindowCore.Cocoa.mm
        Platform/Mac/Core/WindowLifecycle.Cocoa.mm
        Platform/Mac/Core/WindowState.Cocoa.mm
        Platform/Mac/Core/WindowEvents.Cocoa.mm
        Platform/Mac/Core/UiDispatcher.Cocoa.mm
        Platform/Mac/WebKit/WebKitBridge.Cocoa.mm
        Platform/Mac/WebKit/WebKitCustomSchemes.Cocoa.mm
        Platform/Mac/WebKit/WebKitHost.Cocoa.mm
)

set(HEADER_FILES
        Public/InfiniFrame.h
        Public/InfiniFrameWindow.h
        Public/InfiniFrameDialog.h
        Embedded/Embedded.h
        Embedded/InfiniFrameJs/InfiniFrameJs.h
        Types/Basic.h
        Types/DialogResult.h
        Types/DialogButtons.h
        Types/DialogIcon.h
        Types/Monitor.h
        Public/InfiniFrameInitParams.h
        Types/Callbacks.h
        Utils/Dimensions.h
        Utils/ErrorCode.h
        Utils/Result.h
        Utils/StringCopy.h
        Utils/StringArrayCopy.h
        Utils/WindowsHandles.h
        Utils/Event.h
        Public/Exports/Exports.h
        Utils/ExportGuards.h
        Platform/Shared/CustomSchemeResponse.h
        Platform/Windows/ToastHandler.h
        Platform/Windows/Window.Win32.Internal.h
        Platform/Windows/Window.Win32.Context.h
        Platform/Windows/DarkMode.h
        Platform/Mac/Delegates/AppDelegate.h
        Platform/Mac/Delegates/NavigationDelegate.h
        Platform/Mac/NSWindowBorderless.h
        Platform/Mac/Delegates/UiDelegate.h
        Platform/Mac/Delegates/WindowDelegate.h
        Platform/Mac/Delegates/UrlSchemeHandler.h
        Platform/Linux/Window.Gtk.Internal.h
        Platform/Linux/WebKit/WebKit.Gtk.Internal.h
        Platform/Mac/Window.Cocoa.Internal.h
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
