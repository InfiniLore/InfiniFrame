# Centralized dependency setup for `InfiniFrame.Native`.
#
# What this module does:
# - Creates vendored source targets:
#   - `simdjson::simdjson`
#   - `simdutf::simdutf`
#   - `wintoastlib::wintoastlib` (Windows only)
# - Restores NuGet packages needed for Windows SDK-style dependencies.
# - Reads `packages.config` as the single source of truth for package versions.
# - Resolves and creates imported interface targets:
#   - `webview2::sdk` (Windows only)
#   - `wil::headers` (Windows only)
#
# Public entrypoint:
# - `infiniframe_setup_dependencies()`
#
# Variables exported to parent scope (Windows only):
# - `INFINIFRAME_WEBVIEW2_BASE_DIR`
# - `INFINIFRAME_WINDOWS_ARCH_DIR`

# Resolve the first base directory from a candidate list that contains a required file path.
# Params:
# - out_var: parent-scope variable receiving resolved base directory (or empty string if none)
# - required_relative_path: file path expected relative to each candidate base directory
# - ARGN: candidate base directories in priority order
function(_infiniframe_resolve_base_dir out_var required_relative_path)
    set(_resolved "")
    foreach(_candidate IN LISTS ARGN)
        if(EXISTS "${_candidate}/${required_relative_path}")
            set(_resolved "${_candidate}")
            break()
        endif()
    endforeach()
    set(${out_var} "${_resolved}" PARENT_SCOPE)
endfunction()

# Read a NuGet package version from `packages.config`.
# Params:
# - packages_config: absolute path to `packages.config`
# - package_id: package id as defined in `<package id="...">`
# - out_var: parent-scope variable receiving parsed version string
# Fails with `FATAL_ERROR` when the package/version entry is missing.
function(infiniframe_read_package_version packages_config package_id out_var)
    file(READ "${packages_config}" _packages_content)
    string(REPLACE "." "\\." _package_id_regex "${package_id}")
    string(
        REGEX MATCH
        "<package[^>]*id=\"${_package_id_regex}\"[^>]*version=\"([^\"]+)\""
        _match
        "${_packages_content}"
    )
    if(NOT CMAKE_MATCH_1)
        message(FATAL_ERROR "${package_id} package version not found in ${packages_config}")
    endif()
    set(${out_var} "${CMAKE_MATCH_1}" PARENT_SCOPE)
endfunction()

# Create a vendored static library target and namespaced alias from local source/header files.
# Params:
# - vendor_name: logical dependency name (also used for target/alias naming)
# - vendor_dir: directory containing vendored files
# - source_name: source filename expected in `vendor_dir`
# - header_name: header filename expected in `vendor_dir`
# Produces:
# - target `${vendor_name}_vendor`
# - alias `${vendor_name}::${vendor_name}`
# Fails with `FATAL_ERROR` when required files are missing.
function(infiniframe_add_vendor_static_library vendor_name vendor_dir source_name header_name)
    if(NOT EXISTS "${vendor_dir}/${source_name}" OR NOT EXISTS "${vendor_dir}/${header_name}")
        message(FATAL_ERROR "Vendored ${vendor_name} files are missing. Expected ${source_name} and ${header_name} in ${vendor_dir}.")
    endif()

    set(_target_name "${vendor_name}_vendor")
    add_library(${_target_name} STATIC "${vendor_dir}/${source_name}")
    target_include_directories(${_target_name} PUBLIC "${vendor_dir}")
    add_library(${vendor_name}::${vendor_name} ALIAS ${_target_name})
endfunction()

# Restore NuGet packages declared in `packages.config` into `${CMAKE_SOURCE_DIR}/packages`.
# Params:
# - packages_config_path: path to `packages.config`
# Fails with `FATAL_ERROR` when restore returns non-zero.
function(infiniframe_restore_nuget_packages packages_config_path)
    execute_process(
        COMMAND nuget restore "${packages_config_path}" -PackagesDirectory "${CMAKE_SOURCE_DIR}/packages"
        WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
        RESULT_VARIABLE _nuget_result
    )
    if(NOT _nuget_result EQUAL 0)
        message(FATAL_ERROR "NuGet restore failed")
    endif()
endfunction()

# Determine Windows architecture folder token from current CMake generator platform.
# Params:
# - out_var: parent-scope variable receiving one of `x64` or `arm64`
# Defaults to `x64` when platform cannot be inferred.
function(infiniframe_detect_windows_arch_dir out_var)
    if(CMAKE_GENERATOR_PLATFORM MATCHES "ARM64|arm64")
        set(_arch "arm64")
    elseif(CMAKE_GENERATOR_PLATFORM MATCHES "x64|Win64")
        set(_arch "x64")
    else()
        set(_arch "x64")
    endif()
    set(${out_var} "${_arch}" PARENT_SCOPE)
endfunction()

# Resolve WebView2 native SDK base directory from known package locations.
# Params:
# - webview2_version: package version string
# - out_var: parent-scope variable receiving resolved base dir
# Requires `${base}/include/WebView2.h` to exist.
# Fails with `FATAL_ERROR` if no candidate is valid.
function(infiniframe_resolve_webview2_base_dir webview2_version out_var)
    set(_candidates
        "${CMAKE_SOURCE_DIR}/packages/Microsoft.Web.WebView2.${webview2_version}/build/native"
        "$ENV{USERPROFILE}/.nuget/packages/microsoft.web.webview2/${webview2_version}/build/native"
    )
    infiniframe_resolve_base_dir(_base_dir "include/WebView2.h" ${_candidates})
    if(NOT _base_dir)
        message(FATAL_ERROR "WebView2 headers not found")
    endif()
    set(${out_var} "${_base_dir}" PARENT_SCOPE)
endfunction()

# Resolve WIL include directory from known package locations.
# Params:
# - winimpl_version: package version string
# - out_var: parent-scope variable receiving include directory to use
# Supports both:
# - `${base}/include/wil/com.h`
# - `${base}/build/native/include/wil/com.h`
# Fails with `FATAL_ERROR` if neither layout exists.
function(infiniframe_resolve_wil_include_dir winimpl_version out_var)
    set(_candidates
        "${CMAKE_SOURCE_DIR}/packages/Microsoft.Windows.ImplementationLibrary.${winimpl_version}"
        "$ENV{USERPROFILE}/.nuget/packages/microsoft.windows.implementationlibrary/${winimpl_version}"
    )

    infiniframe_resolve_base_dir(_base_include "include/wil/com.h" ${_candidates})
    if(_base_include)
        set(_include_dir "${_base_include}/include")
    else()
        infiniframe_resolve_base_dir(_native_include "build/native/include/wil/com.h" ${_candidates})
        if(_native_include)
            set(_include_dir "${_native_include}/build/native/include")
        else()
            set(_include_dir "")
        endif()
    endif()

    if(NOT _include_dir)
        message(FATAL_ERROR "WIL headers not found")
    endif()
    set(${out_var} "${_include_dir}" PARENT_SCOPE)
endfunction()

# Create imported interface targets for Windows SDK-style dependencies.
# Params:
# - webview2_base_dir: base directory containing WebView2 include/libs
# - win_arch_dir: architecture token (`x64` or `arm64`)
# - wil_include_dir: resolved WIL include directory
# Produces:
# - `webview2::sdk`
# - `wil::headers`
function(infiniframe_add_imported_windows_sdk_targets webview2_base_dir win_arch_dir wil_include_dir)
    add_library(webview2::sdk INTERFACE IMPORTED)
    set_target_properties(webview2::sdk PROPERTIES
        INTERFACE_INCLUDE_DIRECTORIES "${webview2_base_dir}/include"
        INTERFACE_LINK_LIBRARIES "${webview2_base_dir}/${win_arch_dir}/WebView2LoaderStatic.lib"
    )

    add_library(wil::headers INTERFACE IMPORTED)
    set_target_properties(wil::headers PROPERTIES
        INTERFACE_INCLUDE_DIRECTORIES "${wil_include_dir}"
    )
endfunction()