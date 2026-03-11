
cmake_minimum_required(VERSION 4.0)
project(InfiniFrame.Native VERSION 1.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Source files common to all platforms
set(COMMON_SOURCES
        Exports.cpp
)

# Test exports, compiled into the same DLL for integration test access
set(TEST_SOURCES
        Exports.Tests.cpp
)

# Windows-specific sources
set(WINDOWS_SOURCES
        Windows/Window.cpp
        Windows/DarkMode.cpp
        Windows/Dialog.cpp
        Dependencies/simdutf.cpp
        Dependencies/wintoastlib.cpp
)

# Linux-specific sources
set(LINUX_SOURCES
        Linux/Window.cpp
        Linux/Dialog.cpp
)

# macOS-specific sources (Objective-C++)
set(MAC_SOURCES
        Mac/AppDelegate.mm
        Mac/UiDelegate.mm
        Mac/WindowDelegate.mm
        Mac/NavigationDelegate.mm
        Mac/UrlSchemeHandler.mm
        Mac/NSWindowBorderless.mm
        Mac/Dialog.mm
        Mac/Window.mm
)

# Header files
set(HEADER_FILES
        Models/InfiniFrame.h
        Models/InfiniFrameDialog.h
        Models/Types.h
        Models/Monitor.h
        Models/Callbacks.h
        Models/InitParams.h
        Models/DialogEnums.h
        Windows/ToastHandler.h
        Windows/DarkMode.h
        Mac/AppDelegate.h
        Mac/NavigationDelegate.h
        Mac/NSWindowBorderless.h
        Mac/UiDelegate.h
        Mac/WindowDelegate.h
        Mac/UrlSchemeHandler.h
        Dependencies/wintoastlib.h
        Dependencies/simdutf.h
        Dependencies/json.hpp
        resource.h
)

# Platform-specific configuration
if(WIN32)
    # Restore NuGet packages from packages.config
    execute_process(
            COMMAND nuget restore "${CMAKE_SOURCE_DIR}/packages.config" -PackagesDirectory "${CMAKE_SOURCE_DIR}/packages"
            WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
            RESULT_VARIABLE NUGET_RESULT
    )
    if(NUGET_RESULT)
        message(FATAL_ERROR "NuGet restore failed" )
    endif()

    # define architecture - map CMAKE values to MSBuild expected values
    if(CMAKE_GENERATOR_PLATFORM MATCHES "ARM64|arm64")
        set(ARCH_DIR "arm64")
        set(OUTPUT_ARCH_DIR "arm64")
    elseif(CMAKE_GENERATOR_PLATFORM MATCHES "x64|Win64")
        set(ARCH_DIR "x64")
        set(OUTPUT_ARCH_DIR "x64")
    else()
        # Default fallback
        set(ARCH_DIR "x64")
        set(OUTPUT_ARCH_DIR "x64")
    endif()

    # Windows build
    add_library(${PROJECT_NAME} SHARED ${COMMON_SOURCES} ${TEST_SOURCES} ${WINDOWS_SOURCES} ${HEADER_FILES})

    # Set output directory to match MSBuild expectations
    set_target_properties(${PROJECT_NAME} PROPERTIES
            OUTPUT_NAME "InfiniFrame.Native"
            RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_ARCH_DIR}/${CMAKE_BUILD_TYPE}"
    )

    # WebView2 package configuration
    set(WEBVIEW2_VERSION "1.0.3800.47" CACHE STRING "WebView2 version")

    # Prefer local packages folder, fall back to user NuGet cache (useful on CI/agents)
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
        message(FATAL_ERROR "WebView2 headers not found. Looked in: ${WEBVIEW2_BASE_DIR_CANDIDATES}")
    endif()

    set(WEBVIEW2_DLL_SOURCE "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2Loader.dll")
    set(WEBVIEW2_LIB "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2LoaderStatic.lib")

    if(NOT EXISTS "${WEBVIEW2_LIB}")
        message(FATAL_ERROR "WebView2 loader static lib not found at ${WEBVIEW2_LIB}")
    endif()

    # WIL package configuration
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
        message(FATAL_ERROR "WIL headers not found. Looked in: ${WINIMPL_BASE_DIR_CANDIDATES}")
    endif()

    # Include directories
    target_include_directories(${PROJECT_NAME} PRIVATE
            "${CMAKE_SOURCE_DIR}"
            "${WEBVIEW2_BASE_DIR}/include"
            "${WINIMPL_INCLUDE_DIR}"
    )

    # Link libraries
    target_link_libraries(${PROJECT_NAME} PRIVATE
            kernel32 user32 gdi32 winspool comdlg32 advapi32
            shell32 ole32 oleaut32 uuid odbc32 odbccp32 shlwapi shcore
            "${WEBVIEW2_LIB}"
    )

    # Preprocessor definitions
    target_compile_definitions(${PROJECT_NAME} PRIVATE
            WIN32
            _WINDOWS
            UNICODE
            _UNICODE
            $<$<CONFIG:Debug>:_DEBUG>
            $<$<CONFIG:Release>:NDEBUG>
    )

    # Compiler flags
    if(MSVC)
        target_compile_options(${PROJECT_NAME} PRIVATE
                /W3
                $<$<CONFIG:Debug>:/Od /MTd>
                $<$<CONFIG:Release>:/O2 /MT>
        )
    else()
        # GCC/Clang flags
        target_compile_options(${PROJECT_NAME} PRIVATE
                -Wall
                $<$<CONFIG:Debug>:-O0 -g>
                $<$<CONFIG:Release>:-O2>
        )
    endif()

    # Linker flags
    set_target_properties(${PROJECT_NAME} PROPERTIES
            LINK_FLAGS "/SUBSYSTEM:WINDOWS"
    )

    # Resource file
    target_sources(${PROJECT_NAME} PRIVATE InfiniFrame.Native.rc)

    # Copy webview2loader to output
    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            "${WEBVIEW2_DLL_SOURCE}"
            "$<TARGET_FILE_DIR:${PROJECT_NAME}>/WebView2Loader.dll"
            COMMENT "Copying WebView2Loader.dll for ${OUTPUT_ARCH_DIR}"
    )

elseif(APPLE)
    # macOS build - copy .cpp files to .mm for Objective-C++ compilation
    configure_file(Exports.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm COPYONLY)
    configure_file(Exports.Tests.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm COPYONLY)

    add_library(${PROJECT_NAME} SHARED
            ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm
            ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm
            ${MAC_SOURCES}
            ${HEADER_FILES}
    )

    # Add include directories so headers can be found
    target_include_directories(${PROJECT_NAME} PRIVATE ${CMAKE_SOURCE_DIR})

    set_target_properties(${PROJECT_NAME} PROPERTIES
            PREFIX ""
            OUTPUT_NAME "InfiniFrame.Native"
            LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}"
            OSX_ARCHITECTURES "x86_64;arm64"
    )

    # Link macOS frameworks
    target_link_libraries(${PROJECT_NAME} PRIVATE
            "-framework Cocoa"
            "-framework WebKit"
            "-framework UserNotifications"
            "-framework Security"
    )

    # Compiler flags
    target_compile_options(${PROJECT_NAME} PRIVATE
            -Wall
            -O2
            -fPIC
    )

elseif(UNIX)
    # Linux build
    add_library(${PROJECT_NAME} SHARED ${COMMON_SOURCES} ${TEST_SOURCES} ${LINUX_SOURCES} ${HEADER_FILES})
    
    # Determine architecture
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
            PREFIX ""  # removes the 'lib' prefix
    )

    # Find required packages
    find_package(PkgConfig REQUIRED)
    pkg_check_modules(GTK3 REQUIRED gtk+-3.0)
    pkg_check_modules(WEBKIT2 REQUIRED webkit2gtk-4.1)
    pkg_check_modules(LIBNOTIFY REQUIRED libnotify)

    # Include directories
    target_include_directories(${PROJECT_NAME} PRIVATE
            "${CMAKE_SOURCE_DIR}"
            ${GTK3_INCLUDE_DIRS}
            ${WEBKIT2_INCLUDE_DIRS}
            ${LIBNOTIFY_INCLUDE_DIRS}
    )

    # Link libraries
    target_link_libraries(${PROJECT_NAME} PRIVATE
            ${GTK3_LIBRARIES}
            ${WEBKIT2_LIBRARIES}
            ${LIBNOTIFY_LIBRARIES}
    )

    # Compiler flags
    target_compile_options(${PROJECT_NAME} PRIVATE
            -Wall
            -O2
            -fPIC
    )

endif()

# Common compiler features
target_compile_features(${PROJECT_NAME} PRIVATE cxx_std_23)
