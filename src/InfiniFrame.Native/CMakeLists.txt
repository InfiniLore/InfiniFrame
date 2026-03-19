cmake_minimum_required(VERSION 4.0)
project(InfiniFrame.Native
    VERSION 1.1.1
    LANGUAGES CXX
    DESCRIPTION "InfiniFrame.Native webview-wrapper library"
)

# ----------------------------------------------------------------------------------------------------------------------
# Global settings
# ----------------------------------------------------------------------------------------------------------------------
set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

if(APPLE)
    set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE STRING "" FORCE)

    # Prevent CMake from finding Homebrew libraries that break universal binaries
    set(CMAKE_IGNORE_PATH "/usr/local/lib;/usr/local/opt")
    set(CMAKE_FIND_FRAMEWORK LAST)

    # Remove Homebrew paths from implicit linker search paths (must be after project())
    list(REMOVE_ITEM CMAKE_CXX_IMPLICIT_LINK_DIRECTORIES "/usr/local/lib")
    list(REMOVE_ITEM CMAKE_C_IMPLICIT_LINK_DIRECTORIES "/usr/local/lib")
endif()

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
    Dependencies/simdjson.h
    Dependencies/simdutf.h
    Dependencies/simdutf_c.h
    Dependencies/wintoastlib.h
)

# ----------------------------------------------------------------------------------------------------------------------
# Object library for third-party dependencies (warnings suppressed)
# ----------------------------------------------------------------------------------------------------------------------
if(WIN32)
    set(DEPENDENCY_SOURCES
        Dependencies/simdjson.cpp
        Dependencies/simdutf.cpp
        Dependencies/wintoastlib.cpp
    )
elseif(APPLE)
    set(DEPENDENCY_SOURCES
        Dependencies/simdjson.cpp
    )
elseif(UNIX)
    set(DEPENDENCY_SOURCES
        Dependencies/simdjson.cpp
    )
endif()

add_library(InfiniFrame.Dependencies OBJECT
    ${DEPENDENCY_SOURCES}
)

if (APPLE)
    set_target_properties(InfiniFrame.Dependencies PROPERTIES
        OSX_ARCHITECTURES "x86_64;arm64"
    )
endif()

if(MSVC)
    target_compile_options(InfiniFrame.Dependencies PRIVATE /W0)
else()
    target_compile_options(InfiniFrame.Dependencies PRIVATE -w)
endif()

target_include_directories(InfiniFrame.Dependencies SYSTEM PRIVATE
    "${CMAKE_SOURCE_DIR}/Dependencies"
)

# ----------------------------------------------------------------------------------------------------------------------
# Windows Build
# ----------------------------------------------------------------------------------------------------------------------
if(WIN32)
    add_library(${PROJECT_NAME} SHARED
        $<TARGET_OBJECTS:InfiniFrame.Dependencies>
        ${COMMON_SOURCES}
        ${TEST_SOURCES}
        ${WINDOWS_SOURCES}
        ${HEADER_FILES}
    )

    # Include all project headers so Core/, Types/, Utils/, Platform/ work
    target_include_directories(${PROJECT_NAME} PRIVATE
        "${CMAKE_SOURCE_DIR}"               # root source folder
    )
    target_include_directories(${PROJECT_NAME} SYSTEM PRIVATE
        "${CMAKE_SOURCE_DIR}/Dependencies"
    )

    # NuGet restore
    execute_process(
        COMMAND nuget restore "${CMAKE_SOURCE_DIR}/packages.config" 
                -PackagesDirectory "${CMAKE_SOURCE_DIR}/packages"
        WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
        RESULT_VARIABLE NUGET_RESULT
    )
    if(NOT NUGET_RESULT EQUAL 0)
        message(FATAL_ERROR "NuGet restore failed")
    endif()

    # Architecture detection
    if(CMAKE_GENERATOR_PLATFORM MATCHES "ARM64|arm64")
        set(ARCH_DIR "arm64")
        set(OUTPUT_ARCH_DIR "arm64")
    else()
        set(ARCH_DIR "x64")
        set(OUTPUT_ARCH_DIR "x64")
    endif()

    set_target_properties(${PROJECT_NAME} PROPERTIES
        OUTPUT_NAME "InfiniFrame.Native"
        RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_ARCH_DIR}/${CMAKE_BUILD_TYPE}"
    )

    # WebView2
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

    # WIL
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

    target_include_directories(${PROJECT_NAME} SYSTEM PRIVATE
        "${WEBVIEW2_BASE_DIR}/include"
        "${WINIMPL_INCLUDE_DIR}"
    )

    # Link libraries
    target_link_libraries(${PROJECT_NAME} PRIVATE
        kernel32 user32 gdi32 winspool comdlg32 advapi32
        shell32 ole32 oleaut32 uuid odbc32 odbccp32 shlwapi shcore
        "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2LoaderStatic.lib"
    )

    target_compile_definitions(${PROJECT_NAME} PRIVATE
        WIN32 _WINDOWS UNICODE _UNICODE NOMINMAX
        $<$<CONFIG:Debug>:_DEBUG>
        $<$<CONFIG:Release>:NDEBUG>
    )

    if(MSVC)
        target_compile_options(${PROJECT_NAME} PRIVATE
            /W4 /permissive- /Zc:__cplusplus /Zc:lambda
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

    target_link_options(${PROJECT_NAME} PRIVATE "/SUBSYSTEM:WINDOWS")

    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2Loader.dll"
        "$<TARGET_FILE_DIR:${PROJECT_NAME}>/WebView2Loader.dll"
    )

# ----------------------------------------------------------------------------------------------------------------------
# macOS Build
# ----------------------------------------------------------------------------------------------------------------------

elseif(APPLE)
   
    # Copy Exports to .mm so it compiles as Objective-C++
    configure_file(Exports.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm COPYONLY)
    configure_file(Exports.Tests.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm COPYONLY)

    # Create shared library
    add_library(${PROJECT_NAME} SHARED
        $<TARGET_OBJECTS:InfiniFrame.Dependencies>
        ${MAC_SOURCES}
        ${HEADER_FILES}
        ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm
        ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm
    )

    # Include paths
    target_include_directories(${PROJECT_NAME} PRIVATE
        "${CMAKE_SOURCE_DIR}"
    )
    target_include_directories(${PROJECT_NAME} SYSTEM PRIVATE
        "${CMAKE_SOURCE_DIR}/Dependencies"
    )

    # Output settings
    set_target_properties(${PROJECT_NAME} PROPERTIES
        PREFIX ""
        OUTPUT_NAME "InfiniFrame.Native"
        LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}"
        OSX_ARCHITECTURES "x86_64;arm64"
    )

    # Link system frameworks only
    target_link_libraries(${PROJECT_NAME} PRIVATE
        "-framework Cocoa"
        "-framework WebKit"
        "-framework UserNotifications"
        "-framework Security"
    )

    # Explicitly exclude Homebrew library paths that break universal binary linking
    # Use LINKER: to pass through to the linker without shell expansion
    target_link_options(${PROJECT_NAME} PRIVATE
        "LINKER:-Z"
        "LINKER:-dead_strip_dylibs"
    )

    # Set explicit library search order to avoid Homebrew paths
    set_target_properties(${PROJECT_NAME} PROPERTIES
        LINK_FLAGS "-Wl,-search_paths_first"
    )

    # Compiler options
    target_compile_options(${PROJECT_NAME} PRIVATE
        -Wall -Wextra
        -O2
        -fPIC
    )

# ----------------------------------------------------------------------------------------------------------------------
# Linux Build
# ----------------------------------------------------------------------------------------------------------------------

elseif(UNIX)
    add_library(${PROJECT_NAME} SHARED
        $<TARGET_OBJECTS:InfiniFrame.Dependencies>
        ${COMMON_SOURCES}
        ${TEST_SOURCES}
        ${LINUX_SOURCES}
        ${HEADER_FILES}
    )

    target_include_directories(${PROJECT_NAME} PRIVATE
        "${CMAKE_SOURCE_DIR}"
    )
    target_include_directories(${PROJECT_NAME} SYSTEM PRIVATE
        "${CMAKE_SOURCE_DIR}/Dependencies"
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
        ${GTK3_INCLUDE_DIRS}
        ${WEBKIT2_INCLUDE_DIRS}
        ${LIBNOTIFY_INCLUDE_DIRS}
    )

    target_link_libraries(${PROJECT_NAME} PRIVATE
        ${GTK3_LIBRARIES}
        ${WEBKIT2_LIBRARIES}
        ${LIBNOTIFY_LIBRARIES}
    )

    target_compile_options(${PROJECT_NAME} PRIVATE
        -Wall -Wextra
        -O2
        -fPIC
    )

endif()

# ----------------------------------------------------------------------------------------------------------------------
# Sanitizers (Debug only, non-Apple platforms only due to universal binary issues)
# ----------------------------------------------------------------------------------------------------------------------

if(CMAKE_BUILD_TYPE STREQUAL "Debug" AND NOT APPLE)
    if(CMAKE_CXX_COMPILER_ID MATCHES "Clang|GNU")
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

# ----------------------------------------------------------------------------------------------------------------------
# Precompiled Headers
# ----------------------------------------------------------------------------------------------------------------------

if(NOT APPLE)
    target_precompile_headers(${PROJECT_NAME} PRIVATE
        <string>
        <memory>
        <functional>
        <expected>
    )
endif()