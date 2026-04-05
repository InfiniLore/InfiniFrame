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

# Resolve the first base directory candidate that contains a required relative path.
# Usage:
#   _infiniframe_resolve_base_dir(OUT_VAR "include/foo.h" "${cand1}" "${cand2}" ...)
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

# Read one package version from a `packages.config`-style XML file.
# Usage:
#   _infiniframe_read_package_version(".../packages.config" "Package.Id" OUT_VAR)
function(_infiniframe_read_package_version packages_config package_id out_var)
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

# Configure all native dependencies used by this project.
#
# This function is intended to be called once from the root `CMakeLists.txt`.
# It validates vendored files, creates dependency targets, resolves Windows SDK
# locations, and exports Windows path/arch variables for caller-side post-build steps.
function(infiniframe_setup_dependencies)
    set(_simdjson_vendor_dir "${CMAKE_SOURCE_DIR}/Dependencies/simdjson")
    set(_simdutf_vendor_dir "${CMAKE_SOURCE_DIR}/Dependencies/simdutf")
    set(_wintoast_vendor_dir "${CMAKE_SOURCE_DIR}/Dependencies/wintoastlib")

    if(NOT EXISTS "${_simdjson_vendor_dir}/simdjson.cpp" OR NOT EXISTS "${_simdjson_vendor_dir}/simdjson.h")
        message(FATAL_ERROR "Vendored simdjson files are missing. Expected simdjson.cpp and simdjson.h in ${_simdjson_vendor_dir}.")
    endif()
    if(NOT EXISTS "${_simdutf_vendor_dir}/simdutf.cpp" OR NOT EXISTS "${_simdutf_vendor_dir}/simdutf.h")
        message(FATAL_ERROR "Vendored simdutf files are missing. Expected simdutf.cpp and simdutf.h in ${_simdutf_vendor_dir}.")
    endif()
    if(WIN32 AND (NOT EXISTS "${_wintoast_vendor_dir}/wintoastlib.cpp" OR NOT EXISTS "${_wintoast_vendor_dir}/wintoastlib.h"))
        message(FATAL_ERROR "Vendored wintoastlib files are missing. Expected wintoastlib.cpp and wintoastlib.h in ${_wintoast_vendor_dir}.")
    endif()

    add_library(simdjson_vendor STATIC "${_simdjson_vendor_dir}/simdjson.cpp")
    target_include_directories(simdjson_vendor PUBLIC "${_simdjson_vendor_dir}")
    add_library(simdjson::simdjson ALIAS simdjson_vendor)

    add_library(simdutf_vendor STATIC "${_simdutf_vendor_dir}/simdutf.cpp")
    target_include_directories(simdutf_vendor PUBLIC "${_simdutf_vendor_dir}")
    add_library(simdutf::simdutf ALIAS simdutf_vendor)

    if(NOT WIN32)
        return()
    endif()

    add_library(wintoastlib_vendor STATIC "${_wintoast_vendor_dir}/wintoastlib.cpp")
    target_include_directories(wintoastlib_vendor PUBLIC "${_wintoast_vendor_dir}")
    add_library(wintoastlib::wintoastlib ALIAS wintoastlib_vendor)

    execute_process(
        COMMAND nuget restore "${CMAKE_SOURCE_DIR}/packages.config"
                -PackagesDirectory "${CMAKE_SOURCE_DIR}/packages"
        WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
        RESULT_VARIABLE _nuget_result
    )
    if(NOT _nuget_result EQUAL 0)
        message(FATAL_ERROR "NuGet restore failed")
    endif()

    if(CMAKE_GENERATOR_PLATFORM MATCHES "ARM64|arm64")
        set(_win_arch_dir "arm64")
    elseif(CMAKE_GENERATOR_PLATFORM MATCHES "x64|Win64")
        set(_win_arch_dir "x64")
    else()
        set(_win_arch_dir "x64")
    endif()

    set(_packages_config_path "${CMAKE_SOURCE_DIR}/packages.config")
    if(NOT EXISTS "${_packages_config_path}")
        message(FATAL_ERROR "packages.config not found at ${_packages_config_path}")
    endif()

    _infiniframe_read_package_version("${_packages_config_path}" "Microsoft.Web.WebView2" _webview2_version)
    _infiniframe_read_package_version("${_packages_config_path}" "Microsoft.Windows.ImplementationLibrary" _winimpl_version)

    set(_webview2_base_dir_candidates
        "${CMAKE_SOURCE_DIR}/packages/Microsoft.Web.WebView2.${_webview2_version}/build/native"
        "$ENV{USERPROFILE}/.nuget/packages/microsoft.web.webview2/${_webview2_version}/build/native"
    )
    _infiniframe_resolve_base_dir(_webview2_base_dir "include/WebView2.h" ${_webview2_base_dir_candidates})
    if(NOT _webview2_base_dir)
        message(FATAL_ERROR "WebView2 headers not found")
    endif()

    add_library(webview2::sdk INTERFACE IMPORTED)
    set_target_properties(webview2::sdk PROPERTIES
        INTERFACE_INCLUDE_DIRECTORIES "${_webview2_base_dir}/include"
        INTERFACE_LINK_LIBRARIES "${_webview2_base_dir}/${_win_arch_dir}/WebView2LoaderStatic.lib"
    )

    set(_winimpl_base_dir_candidates
        "${CMAKE_SOURCE_DIR}/packages/Microsoft.Windows.ImplementationLibrary.${_winimpl_version}"
        "$ENV{USERPROFILE}/.nuget/packages/microsoft.windows.implementationlibrary/${_winimpl_version}"
    )
    _infiniframe_resolve_base_dir(_winimpl_include_base "include/wil/com.h" ${_winimpl_base_dir_candidates})
    if(_winimpl_include_base)
        set(_winimpl_include_dir "${_winimpl_include_base}/include")
    else()
        _infiniframe_resolve_base_dir(_winimpl_native_include_base "build/native/include/wil/com.h" ${_winimpl_base_dir_candidates})
        if(_winimpl_native_include_base)
            set(_winimpl_include_dir "${_winimpl_native_include_base}/build/native/include")
        else()
            set(_winimpl_include_dir "")
        endif()
    endif()
    if(NOT _winimpl_include_dir)
        message(FATAL_ERROR "WIL headers not found")
    endif()

    add_library(wil::headers INTERFACE IMPORTED)
    set_target_properties(wil::headers PROPERTIES
        INTERFACE_INCLUDE_DIRECTORIES "${_winimpl_include_dir}"
    )

    set(INFINIFRAME_WEBVIEW2_BASE_DIR "${_webview2_base_dir}" PARENT_SCOPE)
    set(INFINIFRAME_WINDOWS_ARCH_DIR "${_win_arch_dir}" PARENT_SCOPE)
endfunction()
