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

    if(CMAKE_SYSTEM_PROCESSOR MATCHES "aarch64|arm64")
        set(_output_arch_dir "arm64")
    else()
        set(_output_arch_dir "x64")
    endif()

    set_target_properties(${target_name} PROPERTIES
        OUTPUT_NAME "InfiniFrame.Native"
        LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${_output_arch_dir}/${CMAKE_BUILD_TYPE}"
        PREFIX ""
    )

    find_package(PkgConfig REQUIRED)
    pkg_check_modules(GTK3 REQUIRED gtk+-3.0)
    pkg_check_modules(WEBKIT2 REQUIRED webkit2gtk-4.1)
    pkg_check_modules(LIBNOTIFY REQUIRED libnotify)

    target_include_directories(${target_name} PRIVATE
        "${CMAKE_SOURCE_DIR}"
        ${GTK3_INCLUDE_DIRS}
        ${WEBKIT2_INCLUDE_DIRS}
        ${LIBNOTIFY_INCLUDE_DIRS}
    )

    target_link_libraries(${target_name} PRIVATE
        simdjson::simdjson
        ${GTK3_LIBRARIES}
        ${WEBKIT2_LIBRARIES}
        ${LIBNOTIFY_LIBRARIES}
    )

    target_compile_options(${target_name} PRIVATE
        -Wall -Wextra
        -O2
        -fPIC
    )
endfunction()
