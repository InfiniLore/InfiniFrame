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
        Exports/Exports.Platform.cpp
        Exports/Exports.Lifecycle.cpp
        Exports/Exports.WindowState.cpp
        Exports/Exports.WindowCommands.cpp
        Exports/Exports.Dialog.cpp
        Exports/Exports.Events.cpp
        Exports/Exports.Memory.cpp
)

set(TEST_SOURCES
        Exports.Tests.cpp
)

set(WINDOWS_SOURCES
        Platform/Windows/Window.cpp
        Platform/Windows/WindowState.Win32.cpp
        Platform/Windows/WindowEvents.Win32.cpp
        Platform/Windows/DarkMode.cpp
        Platform/Windows/Dialog.cpp
)

set(LINUX_SOURCES
        Platform/Linux/Window.cpp
        Platform/Linux/WindowLifecycle.Gtk.cpp
        Platform/Linux/WindowState.Gtk.cpp
        Platform/Linux/WindowEvents.Gtk.cpp
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
        Platform/Mac/Window.mm
        Platform/Mac/WindowLifecycle.Cocoa.mm
        Platform/Mac/WindowState.Cocoa.mm
        Platform/Mac/WindowEvents.Cocoa.mm
)

set(HEADER_FILES
        Core/InfiniFrame.h
        Core/InfiniFrameWindow.h
        Core/InfiniFrameDialog.h
        Embedded/Embedded.h
        Embedded/InfiniFrameJs/InfiniFrameJs.h
        Types/Basic.h
        Types/Dialog.h
        Core/InfiniFrameInitParams.h
        Types/Callbacks.h
        Utils/Common.h
        Utils/Event.h
        Exports/Exports.Shared.h
        Platform/Windows/ToastHandler.h
        Platform/Windows/Window.Win32.Internal.h
        Platform/Windows/DarkMode.h
        Platform/Mac/AppDelegate.h
        Platform/Mac/NavigationDelegate.h
        Platform/Mac/NSWindowBorderless.h
        Platform/Mac/UiDelegate.h
        Platform/Mac/WindowDelegate.h
        Platform/Mac/UrlSchemeHandler.h
        Platform/Linux/Window.Gtk.Internal.h
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
