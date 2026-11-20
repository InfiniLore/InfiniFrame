# Configure the Linux native target.
# Params:
# - target_name: final CMake target name (usually `${PROJECT_NAME}`)
# - common_sources: list of cross-platform source files
# - test_sources: list of test/helper source files compiled into the native target
# - linux_sources: list of Linux-only source files
# - header_files: list of header files for IDE organization
function(infiniframe_configure_linux_target target_name common_sources test_sources linux_sources header_files)
    add_library(${target_name} SHARED
            ${common_sources}
            ${test_sources}
            ${linux_sources}
            ${header_files}
    )

    if (CMAKE_SYSTEM_PROCESSOR MATCHES "aarch64|arm64")
        set(_output_arch_dir "arm64")
    else ()
        set(_output_arch_dir "x64")
    endif ()

    set_target_properties(${target_name} PROPERTIES
            OUTPUT_NAME "InfiniFrame.Native"
            LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${_output_arch_dir}/${CMAKE_BUILD_TYPE}"
            PREFIX ""
    )

    find_package(PkgConfig REQUIRED)
    pkg_check_modules(GTK4 REQUIRED IMPORTED_TARGET gtk4)
    pkg_check_modules(WEBKIT6 REQUIRED IMPORTED_TARGET webkitgtk-6.0)
    pkg_check_modules(LIBNOTIFY REQUIRED IMPORTED_TARGET libnotify)
    # X11 is an intentional, non-optional build dependency (migration requirement):
    # window positioning, max-size hints, work-area queries and the keep-above hint
    # use Xlib / _NET_WM_STATE hints and are compiled unconditionally. This is a
    # build-time requirement only - Wayland runtime support is unaffected (GTK4 picks
    # its backend at runtime and pure-Wayland hosts never exercise the X11 paths).
    # See docs/docs/guides/getting-started.md ("X11 build dependency").
    pkg_check_modules(X11 REQUIRED IMPORTED_TARGET x11)

    target_include_directories(${target_name} PRIVATE
            "${CMAKE_SOURCE_DIR}"
            "${CMAKE_SOURCE_DIR}/src"
    )

    target_link_libraries(${target_name} PRIVATE
            simdutf::simdutf
            simdjson::simdjson
            PkgConfig::GTK4
            PkgConfig::WEBKIT6
            PkgConfig::LIBNOTIFY
            PkgConfig::X11
    )

    target_compile_options(${target_name} PRIVATE
            -Wall -Wextra
            $<$<CONFIG:Release>:-O2>
            $<$<CONFIG:RelWithDebInfo>:-O2>
            $<$<CONFIG:MinSizeRel>:-Os>
    )
endfunction()
