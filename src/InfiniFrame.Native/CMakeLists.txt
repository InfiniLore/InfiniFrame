cmake_minimum_required(VERSION 4.0)
project(InfiniFrame.Native 
    VERSION 1.0.0
    LANGUAGES CXX
    DESCRIPTION "InfiniFrame.Native webview-wrapper library"
)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# ============================================================================
# Dependencies
# ============================================================================

include(FetchContent)

FetchContent_Declare(
    fmt
    GIT_REPOSITORY https://github.com/fmtlib/fmt.git
    GIT_TAG 11.0.2
    GIT_SHALLOW TRUE
)
FetchContent_MakeAvailable(fmt)

FetchContent_Declare(
    simdjson
    GIT_REPOSITORY https://github.com/simdjson/simdjson.git
    GIT_TAG v4.3.1
    GIT_SHALLOW TRUE
)
FetchContent_MakeAvailable(simdjson)

# ============================================================================
# Source Files
# ============================================================================

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
    Dependencies/simdutf.cpp
    Dependencies/wintoastlib.cpp
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

# ============================================================================
# Windows Build
# ============================================================================

if(WIN32)
    add_library(${PROJECT_NAME} SHARED 
        ${COMMON_SOURCES} 
        ${TEST_SOURCES} 
        ${WINDOWS_SOURCES} 
        ${HEADER_FILES}
    )

    execute_process(
        COMMAND nuget restore "${CMAKE_SOURCE_DIR}/packages.config" 
                -PackagesDirectory "${CMAKE_SOURCE_DIR}/packages"
        WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
        RESULT_VARIABLE NUGET_RESULT
    )
    if(NOT NUGET_RESULT EQUAL 0)
        message(FATAL_ERROR "NuGet restore failed")
    endif()

    if(CMAKE_GENERATOR_PLATFORM MATCHES "ARM64|arm64")
        set(ARCH_DIR "arm64")
        set(OUTPUT_ARCH_DIR "arm64")
    elseif(CMAKE_GENERATOR_PLATFORM MATCHES "x64|Win64")
        set(ARCH_DIR "x64")
        set(OUTPUT_ARCH_DIR "x64")
    else()
        set(ARCH_DIR "x64")
        set(OUTPUT_ARCH_DIR "x64")
    endif()

    set_target_properties(${PROJECT_NAME} PROPERTIES
        OUTPUT_NAME "InfiniFrame.Native"
        RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_ARCH_DIR}/${CMAKE_BUILD_TYPE}"
    )

    set(WEBVIEW2_VERSION "1.0.3800.47" CACHE STRING "WebView2 version")
    
    set(WEBVIEW2_BASE_DIR "")
    set(WEBVIEW2_BASE_DIR_CANDIDATES
        "${CMAKE_SOURCE_DIR}/packages/Microsoft.Web.WebView2.${WEBVIEW2_VERSION}/build/native"
        "$ENV{USERPROFILE}/.nuget/packages/microsoft.web.webview2/${WEBVIEW2_VERSION}/build/native"
    )
    foreach(_cand IN LISTS WEBVIEW2_BASE_DIR_CANDIDATES)
        if(EXISTS "${_cand}/include/WebView2.h")
            set(WEBVIEW2_BASE_DIR "${_cand}")
            break()
        endif()
    endforeach()

    if(NOT WEBVIEW2_BASE_DIR)
        message(FATAL_ERROR "WebView2 headers not found")
    endif()

    set(WINIMPL_VERSION "1.0.260126.7" CACHE STRING "Windows Implementation Library version")
    set(WINIMPL_INCLUDE_DIR "")
    set(WINIMPL_BASE_DIR_CANDIDATES
        "${CMAKE_SOURCE_DIR}/packages/Microsoft.Windows.ImplementationLibrary.${WINIMPL_VERSION}"
        "$ENV{USERPROFILE}/.nuget/packages/microsoft.windows.implementationlibrary/${WINIMPL_VERSION}"
    )
    foreach(_cand IN LISTS WINIMPL_BASE_DIR_CANDIDATES)
        if(EXISTS "${_cand}/include/wil/com.h")
            set(WINIMPL_INCLUDE_DIR "${_cand}/include")
            break()
        elseif(EXISTS "${_cand}/build/native/include/wil/com.h")
            set(WINIMPL_INCLUDE_DIR "${_cand}/build/native/include")
            break()
        endif()
    endforeach()

    if(NOT WINIMPL_INCLUDE_DIR)
        message(FATAL_ERROR "WIL headers not found")
    endif()

    target_include_directories(${PROJECT_NAME} PRIVATE
        "${CMAKE_SOURCE_DIR}"
        "${WEBVIEW2_BASE_DIR}/include"
        "${WINIMPL_INCLUDE_DIR}"
    )

    target_link_libraries(${PROJECT_NAME} PRIVATE
        fmt::fmt
        simdjson::simdjson
        kernel32 user32 gdi32 winspool comdlg32 advapi32
        shell32 ole32 oleaut32 uuid odbc32 odbccp32 shlwapi shcore
        "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2LoaderStatic.lib"
    )

    target_compile_definitions(${PROJECT_NAME} PRIVATE
        WIN32
        _WINDOWS
        UNICODE
        _UNICODE
        FMT_HEADER_ONLY=1
        $<$<CONFIG:Debug>:_DEBUG>
        $<$<CONFIG:Release>:NDEBUG>
    )

    if(MSVC)
        target_compile_options(${PROJECT_NAME} PRIVATE
            /W4
            /permissive-
            /Zc:__cplusplus
            /Zc:lambda
            $<$<CONFIG:Debug>:/Od /MTd /Zi>
            $<$<CONFIG:Release>:/O2 /MT>
        )
    else()
        target_compile_options(${PROJECT_NAME} PRIVATE
            -Wall -Wextra
            $<$<CONFIG:Debug>:-O0 -g>
            $<$<CONFIG:Release>:-O2>
        )
    endif()

    set_target_properties(${PROJECT_NAME} PROPERTIES
        LINK_FLAGS "/SUBSYSTEM:WINDOWS"
    )

    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2Loader.dll"
        "$<TARGET_FILE_DIR:${PROJECT_NAME}>/WebView2Loader.dll"
        COMMENT "Copying WebView2Loader.dll"
    )

# ============================================================================
# macOS Build
# ============================================================================

elseif(APPLE)
    configure_file(Exports.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm COPYONLY)
    configure_file(Exports.Tests.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm COPYONLY)

    add_library(${PROJECT_NAME} SHARED
        ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm
        ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm
        ${MAC_SOURCES}
        ${HEADER_FILES}
    )

    target_include_directories(${PROJECT_NAME} PRIVATE ${CMAKE_SOURCE_DIR})

    set_target_properties(${PROJECT_NAME} PROPERTIES
        PREFIX ""
        OUTPUT_NAME "InfiniFrame.Native"
        LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}"
        OSX_ARCHITECTURES "x86_64;arm64"
    )

    target_link_libraries(${PROJECT_NAME} PRIVATE
        fmt::fmt
        simdjson::simdjson
        "-framework Cocoa"
        "-framework WebKit"
        "-framework UserNotifications"
        "-framework Security"
    )

    target_compile_options(${PROJECT_NAME} PRIVATE
        -Wall -Wextra
        -O2
        -fPIC
    )

    target_compile_definitions(${PROJECT_NAME} PRIVATE
        FMT_HEADER_ONLY=1
    )

# ============================================================================
# Linux Build
# ============================================================================

elseif(UNIX)
    add_library(${PROJECT_NAME} SHARED 
        ${COMMON_SOURCES} 
        ${TEST_SOURCES} 
        ${LINUX_SOURCES} 
        ${HEADER_FILES}
    )

    if(CMAKE_SYSTEM_PROCESSOR MATCHES "aarch64|arm64")
        set(ARCH_DIR "arm64")
        set(OUTPUT_ARCH_DIR "arm64")
    else()
        set(ARCH_DIR "x64")
        set(OUTPUT_ARCH_DIR "x64")
    endif()

    set_target_properties(${PROJECT_NAME} PROPERTIES
        OUTPUT_NAME "InfiniFrame.Native"
        LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_ARCH_DIR}/${CMAKE_BUILD_TYPE}"
        PREFIX ""
    )

    find_package(PkgConfig REQUIRED)
    pkg_check_modules(GTK3 REQUIRED gtk+-3.0)
    pkg_check_modules(WEBKIT2 REQUIRED webkit2gtk-4.1)
    pkg_check_modules(LIBNOTIFY REQUIRED libnotify)

    target_include_directories(${PROJECT_NAME} PRIVATE
        "${CMAKE_SOURCE_DIR}"
        ${GTK3_INCLUDE_DIRS}
        ${WEBKIT2_INCLUDE_DIRS}
        ${LIBNOTIFY_INCLUDE_DIRS}
    )

    target_link_libraries(${PROJECT_NAME} PRIVATE
        fmt::fmt
        simdjson::simdjson
        ${GTK3_LIBRARIES}
        ${WEBKIT2_LIBRARIES}
        ${LIBNOTIFY_LIBRARIES}
    )

    target_compile_options(${PROJECT_NAME} PRIVATE
        -Wall -Wextra
        -O2
        -fPIC
    )

    target_compile_definitions(${PROJECT_NAME} PRIVATE
        FMT_HEADER_ONLY=1
    )
endif()

# ============================================================================
# Sanitizers (Debug only)
# ============================================================================

if(CMAKE_BUILD_TYPE STREQUAL "Debug")
    if(CMAKE_CXX_COMPILER_ID MATCHES "Clang|GNU")
        message(STATUS "Enabling sanitizers for Debug build")
        target_compile_options(${PROJECT_NAME} PRIVATE
            -fsanitize=address,undefined,leak
            -fno-omit-frame-pointer
            -fno-optimize-sibling-calls
        )
        target_link_options(${PROJECT_NAME} PRIVATE
            -fsanitize=address,undefined,leak
        )
    endif()
endif()

# ============================================================================
# Precompiled Headers
# ============================================================================

if(NOT APPLE)
    target_precompile_headers(${PROJECT_NAME} PRIVATE
        <string>
        <memory>
        <functional>
        <expected>
    )
endif()
