# Configure the macOS native target.
# Params:
# - target_name: final CMake target name (usually `${PROJECT_NAME}`)
# - mac_sources: list of macOS-only source files
# - header_files: list of header files for IDE organization
function(infiniframe_configure_macos_target target_name mac_sources header_files)
    configure_file(Exports.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm COPYONLY)
    configure_file(Exports.Tests.cpp ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm COPYONLY)

    add_library(${target_name} SHARED
            ${CMAKE_CURRENT_BINARY_DIR}/Exports.mm
            ${CMAKE_CURRENT_BINARY_DIR}/Exports.Tests.mm
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
