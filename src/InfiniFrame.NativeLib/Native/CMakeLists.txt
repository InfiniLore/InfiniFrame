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
include("${CMAKE_SOURCE_DIR}/Cmake/NativeDependencies.cmake")
include("${CMAKE_SOURCE_DIR}/Cmake/Embed.InfiniFrameJs.cmake")

include("${CMAKE_SOURCE_DIR}/Cmake/Platform.Windows.cmake")
include("${CMAKE_SOURCE_DIR}/Cmake/Platform.MacOS.cmake")
include("${CMAKE_SOURCE_DIR}/Cmake/Platform.Linux.cmake")

infiniframe_setup_dependencies()

# ----------------------------------------------------------------------------------------------------------------------
# Source Files
# ----------------------------------------------------------------------------------------------------------------------
set(COMMON_SOURCES
        Exports.cpp
)

set(TEST_SOURCES
        Exports.Tests.cpp
)

set(WINDOWS_SOURCES
        Platform/Windows/Window.cpp
        Platform/Windows/DarkMode.cpp
        Platform/Windows/Dialog.cpp
)

set(LINUX_SOURCES
        Platform/Linux/Window.cpp
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
        Platform/Windows/ToastHandler.h
        Platform/Windows/DarkMode.h
        Platform/Mac/AppDelegate.h
        Platform/Mac/NavigationDelegate.h
        Platform/Mac/NSWindowBorderless.h
        Platform/Mac/UiDelegate.h
        Platform/Mac/WindowDelegate.h
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
