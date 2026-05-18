# Configure the macOS native target.
# Params:
# - target_name: final CMake target name (usually `${PROJECT_NAME}`)
# - common_sources: list of cross-platform source files
# - test_sources: list of test/helper source files compiled into the native target
# - mac_sources: list of macOS-only source files
# - header_files: list of header files for IDE organization
function(infiniframe_configure_macos_target target_name common_sources test_sources mac_sources header_files)
    add_library(${target_name} SHARED
            ${common_sources}
            ${test_sources}
            ${mac_sources}
            ${header_files}
    )

    # Export units include platform headers that require Objective-C++ on macOS.
    set_source_files_properties(${common_sources} ${test_sources} PROPERTIES
            LANGUAGE OBJCXX
    )

    target_include_directories(${target_name} PRIVATE "${CMAKE_SOURCE_DIR}")

    set_target_properties(${target_name} PROPERTIES
            PREFIX ""
            OUTPUT_NAME "InfiniFrame.Native"
            LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}"
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
            $<$<CONFIG:Debug>:-O0 -g>
            $<$<CONFIG:Release>:-O2>
            $<$<CONFIG:RelWithDebInfo>:-O2 -g>
            $<$<CONFIG:MinSizeRel>:-Os>
            -fPIC
    )
endfunction()
