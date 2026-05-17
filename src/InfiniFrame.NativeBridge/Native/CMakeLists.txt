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

if (APPLE)
    set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE STRING "" FORCE)
endif ()

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
        Platform/Windows/WebView/WebView2Host.Win32.cpp
        Platform/Windows/WebView/WebView2Runtime.Win32.cpp
        Platform/Windows/WebView/WebView2Controller.Win32.cpp
        Platform/Windows/WebView/WebView2Attach.Win32.cpp
        Platform/Windows/Core/UiDispatcher.Win32.cpp
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
        Platform/Linux/WebKit/WebKitHost.Gtk.cpp
        Platform/Linux/WebKit/WebKitSettings.Gtk.cpp
        Platform/Linux/WebKit/WebKitMessaging.Gtk.cpp
        Platform/Linux/WebKit/WebKitCustomSchemes.Gtk.cpp
        Platform/Linux/Core/WindowSignals.Gtk.cpp
        Platform/Linux/Dialog.cpp
)

set(MAC_SOURCES
        Platform/Mac/AppDelegate.mm
        Platform/Mac/UiDelegate.mm
        Platform/Mac/WindowDelegate.mm
        Platform/Mac/NavigationDelegate.mm
        Platform/Mac/UrlSchemeHandler.mm
        Platform/Mac/NSWindowBorderless.mm
        Platform/Mac/Dialog.mm
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
        Types/Dialog.h
        Types/DialogResult.h
        Types/DialogButtons.h
        Types/DialogIcon.h
        Types/Monitor.h
        Public/InfiniFrameInitParams.h
        Types/Callbacks.h
        Utils/Common.h
        Utils/Dimensions.h
        Utils/ErrorCode.h
        Utils/Result.h
        Utils/StringCopy.h
        Utils/WindowsHandles.h
        Utils/Event.h
        Public/Exports/Exports.h
        Utils/ExportGuards.h
        Platform/Windows/ToastHandler.h
        Platform/Windows/Window.Win32.Internal.h
        Platform/Windows/Window.Win32.Context.h
        Platform/Windows/DarkMode.h
        Platform/Mac/AppDelegate.h
        Platform/Mac/NavigationDelegate.h
        Platform/Mac/NSWindowBorderless.h
        Platform/Mac/UiDelegate.h
        Platform/Mac/WindowDelegate.h
        Platform/Mac/UrlSchemeHandler.h
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

target_compile_definitions(${PROJECT_NAME} PRIVATE
        $<$<CONFIG:Debug>:INFINIFRAME_BUILD_TEST_EXPORTS=1>
)

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
