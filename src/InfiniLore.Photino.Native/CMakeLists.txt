cmake_minimum_required(VERSION 4.0)
project(InfiniLore.Photino.Native VERSION 1.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Restore NuGet packages from packages.config
execute_process(
    COMMAND nuget restore "${CMAKE_SOURCE_DIR}/packages.config" -PackagesDirectory "${CMAKE_SOURCE_DIR}/packages" 
    WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}" 
    RESULT_VARIABLE NUGET_RESULT
)
if(NUGET_RESULT)
    message(FATAL_ERROR "NuGet restore failed" )
endif()

# Source files common to all platforms
set(COMMON_SOURCES
        Exports.cpp
)

# Windows-specific sources
set(WINDOWS_SOURCES
        Photino.Windows.cpp
        Photino.Windows.DarkMode.cpp
        Photino.Windows.Dialog.cpp
        Dependencies/wintoastlib.cpp
)

# Linux-specific sources
set(LINUX_SOURCES
        Photino.Linux.cpp
        Photino.Linux.Dialog.cpp
)

# macOS-specific sources (Objective-C++)
set(MAC_SOURCES
        Photino.Mac.AppDelegate.mm
        Photino.Mac.UiDelegate.mm
        Photino.Mac.WindowDelegate.mm
        Photino.Mac.NavigationDelegate.mm
        Photino.Mac.UrlSchemeHandler.mm
        Photino.Mac.NSWindowBorderless.mm
        Photino.Mac.Dialog.mm
        Photino.Mac.mm
)

# Header files
set(HEADER_FILES
        Photino.h
        Photino.Dialog.h
        Photino.Windows.ToastHandler.h
        Photino.Windows.DarkMode.h
        Photino.Mac.AppDelegate.h
        Photino.Mac.NavigationDelegate.h
        Photino.Mac.NSWindowBorderless.h
        Photino.Mac.UiDelegate.h
        Photino.Mac.WindowDelegate.h
        Photino.Mac.UrlSchemeHandler.h
        Dependencies/wintoastlib.h
        json.hpp
        resource.h
)

# Platform-specific configuration
if(WIN32)
    # define architecture
    if(CMAKE_SYSTEM_PROCESSOR MATCHES "ARM64|arm64|aarch64")
        set(ARCH_DIR "arm64")
    else()
        set(ARCH_DIR "x64")
    endif()
    
    # Windows build
    add_library(${PROJECT_NAME} SHARED ${COMMON_SOURCES} ${WINDOWS_SOURCES} ${HEADER_FILES})

    # Set output directory
    set_target_properties(${PROJECT_NAME} PROPERTIES
            OUTPUT_NAME "InfiniLore.Photino.Native"
            RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${CMAKE_SYSTEM_PROCESSOR}/${CMAKE_BUILD_TYPE}"
    )

    # WebView2 package configuration
    set(WEBVIEW2_VERSION "1.0.3537.50" CACHE STRING "WebView2 version")

    set(WEBVIEW2_BASE_DIR "${CMAKE_SOURCE_DIR}/packages/Microsoft.Web.WebView2.${WEBVIEW2_VERSION}/build/native")
    set(WEBVIEW2_DLL_SOURCE "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2Loader.dll")
    set(WEBVIEW2_LIB "${WEBVIEW2_BASE_DIR}/${ARCH_DIR}/WebView2LoaderStatic.lib")
    
    # WIL package configuration
    set(WINIMPL_VERSION "1.0.250325.1" CACHE STRING "Windows Implementation Library version")
    
    set(WINIMPL_BASE_DIR "${CMAKE_SOURCE_DIR}/packages/Microsoft.Windows.ImplementationLibrary.${WINIMPL_VERSION}")

    # Include directories
    target_include_directories(${PROJECT_NAME} PRIVATE
        "${WEBVIEW2_BASE_DIR}/include"
        "${WINIMPL_BASE_DIR}/include"
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
    target_compile_options(${PROJECT_NAME} PRIVATE
            /W3
            $<$<CONFIG:Debug>:/Od /MTd>
            $<$<CONFIG:Release>:/O2 /MT>
    )

    # Linker flags
    set_target_properties(${PROJECT_NAME} PROPERTIES
            LINK_FLAGS "/SUBSYSTEM:WINDOWS"
    )

    # Resource file
    target_sources(${PROJECT_NAME} PRIVATE Photino.Native.rc)

    # Copy webview2loader to output
    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            "${WEBVIEW2_DLL_SOURCE}"
            "$<TARGET_FILE_DIR:${PROJECT_NAME}>/WebView2Loader.dll"
            COMMENT "Copying WebView2Loader.dll for ${ARCH}"
    )
    
elseif(APPLE)
    # macOS build - copy Exports.cpp to Exports.mm for Objective-C++ compilation
    configure_file(Exports.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm COPYONLY)

    add_library(${PROJECT_NAME} SHARED
            ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm
            ${MAC_SOURCES}
            ${HEADER_FILES}
    )

    set_target_properties(${PROJECT_NAME} PROPERTIES
            OUTPUT_NAME "InfiniLore.Photino.Native"
            LIBRARY_OUTPUT_DIRECTORY "${CMAKE_SOURCE_DIR}/lib/x64"
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
    add_library(${PROJECT_NAME} SHARED ${COMMON_SOURCES} ${LINUX_SOURCES} ${HEADER_FILES})

    # Determine architecture
    if(CMAKE_SYSTEM_PROCESSOR MATCHES "aarch64|arm64")
        set(ARCH_DIR "arm64")
    else()
        set(ARCH_DIR "x64")
    endif()

    set_target_properties(${PROJECT_NAME} PROPERTIES
            OUTPUT_NAME "InfiniLore.Photino.Native"
            LIBRARY_OUTPUT_DIRECTORY "${CMAKE_SOURCE_DIR}/lib/${ARCH_DIR}"
    )

    # Find required packages
    find_package(PkgConfig REQUIRED)
    pkg_check_modules(GTK3 REQUIRED gtk+-3.0)
    pkg_check_modules(WEBKIT2 REQUIRED webkit2gtk-4.1)
    pkg_check_modules(LIBNOTIFY REQUIRED libnotify)

    # Include directories
    target_include_directories(${PROJECT_NAME} PRIVATE
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
target_compile_features(${PROJECT_NAME} PRIVATE cxx_std_20)