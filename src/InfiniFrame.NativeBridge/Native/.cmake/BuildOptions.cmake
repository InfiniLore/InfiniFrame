# Native build-speed options. All optimizations are target-scoped so vendored
# dependencies and consumers do not inherit first-party build policy.

option(INFINIFRAME_ENABLE_UNITY_BUILD
        "Enable CMake unity builds for faster clean builds (not recommended for day-to-day incremental work)"
        OFF)

set(INFINIFRAME_COMPILER_CACHE "AUTO" CACHE STRING
        "Compiler cache launcher: AUTO, OFF, or an executable path")
set_property(CACHE INFINIFRAME_COMPILER_CACHE PROPERTY STRINGS AUTO OFF)

set(INFINIFRAME_SANITIZER "None" CACHE STRING
        "Native sanitizer instrumentation: None, AddressUndefined, or Thread")
set_property(CACHE INFINIFRAME_SANITIZER PROPERTY STRINGS None AddressUndefined Thread)

function(infiniframe_enable_sanitizers target_name)
    if (INFINIFRAME_SANITIZER STREQUAL "None")
        return()
    endif ()

    if (MSVC)
        message(FATAL_ERROR "INFINIFRAME_SANITIZER is currently supported only with Clang/GCC")
    elseif (INFINIFRAME_SANITIZER STREQUAL "AddressUndefined")
        target_compile_options(${target_name} PRIVATE
                -fsanitize=address,undefined -fno-omit-frame-pointer -fno-optimize-sibling-calls)
        target_link_options(${target_name} PRIVATE -fsanitize=address,undefined)
    elseif (INFINIFRAME_SANITIZER STREQUAL "Thread")
        target_compile_options(${target_name} PRIVATE -fsanitize=thread -fno-omit-frame-pointer)
        target_link_options(${target_name} PRIVATE -fsanitize=thread)
    else ()
        message(FATAL_ERROR "Unknown INFINIFRAME_SANITIZER value '${INFINIFRAME_SANITIZER}'")
    endif ()
endfunction()

function(infiniframe_configure_compiler_cache)
    if (DEFINED CMAKE_CXX_COMPILER_LAUNCHER AND NOT "${CMAKE_CXX_COMPILER_LAUNCHER}" STREQUAL "")
        message(STATUS "Using preconfigured C++ compiler launcher: ${CMAKE_CXX_COMPILER_LAUNCHER}")
        return()
    endif ()

    if (INFINIFRAME_COMPILER_CACHE STREQUAL "OFF")
        return()
    endif ()

    if (INFINIFRAME_COMPILER_CACHE STREQUAL "AUTO")
        # sccache supports MSVC, clang, and GCC; ccache is the usual Unix fallback.
        find_program(_infiniframe_compiler_cache NAMES sccache ccache)
    else ()
        find_program(_infiniframe_compiler_cache NAMES "${INFINIFRAME_COMPILER_CACHE}" NO_CACHE)
    endif ()

    if (_infiniframe_compiler_cache)
        set(CMAKE_CXX_COMPILER_LAUNCHER "${_infiniframe_compiler_cache}" CACHE STRING
                "C++ compiler launcher" FORCE)
        if (APPLE)
            set(CMAKE_OBJCXX_COMPILER_LAUNCHER "${_infiniframe_compiler_cache}" CACHE STRING
                    "Objective-C++ compiler launcher" FORCE)
        endif ()
        message(STATUS "Using compiler cache: ${_infiniframe_compiler_cache}")
    elseif (NOT INFINIFRAME_COMPILER_CACHE STREQUAL "AUTO")
        message(FATAL_ERROR "INFINIFRAME_COMPILER_CACHE='${INFINIFRAME_COMPILER_CACHE}' was not found")
    endif ()
endfunction()

function(infiniframe_enable_fast_build_settings target_name)
    if (INFINIFRAME_ENABLE_UNITY_BUILD)
        set_target_properties(${target_name} PROPERTIES
                UNITY_BUILD ON
                UNITY_BUILD_BATCH_SIZE 8)
    endif ()

    # PCHs pay off for the small, repeatedly included STL base while avoiding
    # platform SDK headers that would make the cache broad and fragile.
    if (NOT APPLE)
        target_precompile_headers(${target_name} PRIVATE
                <string>
                <memory>
                <functional>
                <expected>)
    endif ()
endfunction()
