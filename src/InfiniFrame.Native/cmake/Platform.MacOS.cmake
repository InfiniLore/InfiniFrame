# Configure the macOS native target.
# Params:
# - target_name: final CMake target name (usually `${PROJECT_NAME}`)
# - common_sources: list of cross-platform source files
# - test_sources: list of test/helper source files compiled into the native target
# - mac_sources: list of macOS-only source files
# - header_files: list of header files for IDE organization
function(infiniframe_configure_macos_target target_name common_sources test_sources mac_sources header_files)
    set(_mac_common_sources)

    foreach (_source IN LISTS common_sources)
        get_filename_component(_source_extension "${_source}" EXT)

        if (_source_extension STREQUAL ".cpp")
            get_filename_component(_source_name "${_source}" NAME_WE)
            set(_copied_source "${CMAKE_CURRENT_BINARY_DIR}/${_source_name}.mm")
            configure_file("${_source}" "${_copied_source}" COPYONLY)
            list(APPEND _mac_common_sources "${_copied_source}")
        else ()
            list(APPEND _mac_common_sources "${_source}")
        endif ()
    endforeach ()

    set(_mac_test_sources)

    if (test_sources)
        configure_file(Exports.Tests.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm COPYONLY)
        list(APPEND _mac_test_sources ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm)
    endif ()

    add_library(${target_name} SHARED
            ${_mac_common_sources}
            ${_mac_test_sources}
            ${mac_sources}
            ${header_files}
    )

    target_include_directories(${target_name} PRIVATE "${CMAKE_SOURCE_DIR}")

    set_target_properties(${target_name} PROPERTIES
            PREFIX ""
            OUTPUT_NAME "InfiniFrame.Native"
            LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}"
            OSX_ARCHITECTURES "x86_64;arm64"
    )

    target_link_libraries(${target_name} PRIVATE
            simdutf::simdutf
            simdjson::simdjson
            "-framework Cocoa"
            "-framework WebKit"
            "-framework UserNotifications"
            "-framework Security"
    )

    target_compile_options(${target_name} PRIVATE
            -Wall -Wextra
            -O2
            -fPIC
    )
endfunction()
