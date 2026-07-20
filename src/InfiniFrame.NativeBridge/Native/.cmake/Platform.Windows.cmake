# Configure the Windows native target.
# Params:
# - target_name: final CMake target name (usually `${PROJECT_NAME}`)
# - common_sources: list of cross-platform source files
# - test_sources: list of test/helper source files compiled into the native target
# - windows_sources: list of Windows-only source files
# - header_files: list of header files for IDE organization
function(infiniframe_configure_windows_target target_name common_sources test_sources windows_sources header_files)
    add_library(${target_name} SHARED)

    target_sources(${target_name} PRIVATE
            ${common_sources}
            ${test_sources}
            ${windows_sources}
            ${header_files}
    )

    set_target_properties(${target_name} PROPERTIES
            OUTPUT_NAME "InfiniFrame.Native"
            RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${INFINIFRAME_WINDOWS_ARCH_DIR}/${CMAKE_BUILD_TYPE}"
    )

    target_link_options(${target_name} PRIVATE /SUBSYSTEM:WINDOWS)

    target_include_directories(${target_name} PRIVATE
            "${CMAKE_SOURCE_DIR}"
            "${CMAKE_SOURCE_DIR}/src"
    )

    target_link_libraries(${target_name} PRIVATE
            simdutf::simdutf
            wintoastlib::wintoastlib
            webview2::sdk
            wil::headers
            kernel32 user32 gdi32 winspool comdlg32 advapi32
            shell32 ole32 oleaut32 uuid odbc32 odbccp32 shlwapi shcore
    )

    target_compile_definitions(${target_name} PRIVATE
            WIN32
            _WINDOWS
            UNICODE
            _UNICODE
            NOMINMAX
            $<$<CONFIG:Debug>:_DEBUG>
            $<$<CONFIG:Release>:NDEBUG>
    )

    if (MSVC)
        target_compile_options(${target_name} PRIVATE
                /W4
                /permissive-
                /Zc:__cplusplus
                /Zc:lambda
                $<$<CONFIG:Debug>:/Od>
                $<$<CONFIG:Release>:/O2>
        )
        target_link_options(${target_name} PRIVATE
                $<$<CONFIG:Debug>:/INCREMENTAL>
        )
    else ()
        target_compile_options(${target_name} PRIVATE
                -Wall -Wextra
                $<$<CONFIG:Debug>:-O0 -g>
                $<$<CONFIG:Release>:-O2>
        )
    endif ()

    add_custom_command(TARGET ${target_name} POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            "${INFINIFRAME_WEBVIEW2_BASE_DIR}/${INFINIFRAME_WINDOWS_ARCH_DIR}/WebView2Loader.dll"
            "$<TARGET_FILE_DIR:${target_name}>/WebView2Loader.dll"
            COMMENT "Copying WebView2Loader.dll"
    )
endfunction()
