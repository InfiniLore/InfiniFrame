# Centralized dependency setup for `InfiniFrame.Native`.
#
# What this module does:
# - Creates vendored source targets:
#   - `simdjson::simdjson`
#   - `simdutf::simdutf`
#   - `wintoastlib::wintoastlib` (Windows only)
# - Resolves NuGet packages needed for Windows SDK-style dependencies.
# - Uses versions passed from CMake cache vars.
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
    message(STATUS "Searching dependency base dir for '${required_relative_path}'")
    set(_resolved "")

    foreach (_candidate IN LISTS ARGN)
        message(STATUS "  candidate: ${_candidate}")
        if (EXISTS "${_candidate}/${required_relative_path}")
            set(_resolved "${_candidate}")
            break()
        endif ()
    endforeach ()

    if (_resolved)
        message(STATUS "  resolved: ${_resolved}")
    else ()
        message(STATUS "  resolved: <not found>")
    endif ()
    set(${out_var} "${_resolved}" PARENT_SCOPE)
endfunction()

# Resolve package version from a required CMake cache variable.
# Params:
# - package_id: package id as defined in `<package id="...">`
# - cmake_var_name: cache variable expected to contain the package version
# - out_var: parent-scope variable receiving parsed version string
function(infiniframe_get_package_version package_id cmake_var_name out_var)
    if (DEFINED ${cmake_var_name} AND NOT "${${cmake_var_name}}" STREQUAL "")
        message(STATUS "Using ${package_id} version from -D${cmake_var_name}=${${cmake_var_name}}")
        set(${out_var} "${${cmake_var_name}}" PARENT_SCOPE)
        return()
    endif ()

    message(FATAL_ERROR "Missing ${package_id} version. Pass -D${cmake_var_name}=<version>.")
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
    message(STATUS "Configuring vendored library '${vendor_name}' from ${vendor_dir}")
    if (NOT EXISTS "${vendor_dir}/${source_name}" OR NOT EXISTS "${vendor_dir}/${header_name}")
        message(FATAL_ERROR "Vendored ${vendor_name} files are missing. Expected ${source_name} and ${header_name} in ${vendor_dir}.")
    endif ()

    set(_target_name "${vendor_name}_vendor")
    add_library(${_target_name} STATIC "${vendor_dir}/${source_name}")
    target_include_directories(${_target_name} PUBLIC "${vendor_dir}")

    # Third-party vendored code should not inherit first-party warning-as-error policy.
    if (MSVC)
        target_compile_options(${_target_name} PRIVATE /WX-)
    elseif (CMAKE_CXX_COMPILER_ID MATCHES "Clang|GNU")
        target_compile_options(${_target_name} PRIVATE -Wno-error)
    endif ()

    add_library(${vendor_name}::${vendor_name} ALIAS ${_target_name})
    message(STATUS "  created target '${_target_name}' and alias '${vendor_name}::${vendor_name}'")
endfunction()

# Determine Windows architecture folder token from current CMake generator platform.
# Params:
# - out_var: parent-scope variable receiving one of `x64` or `arm64`
# Defaults to `x64` when platform cannot be inferred.
function(infiniframe_detect_windows_arch_dir out_var)
    if (CMAKE_GENERATOR_PLATFORM MATCHES "ARM64|arm64")
        set(_arch "arm64")
    elseif (CMAKE_GENERATOR_PLATFORM MATCHES "x64|Win64")
        set(_arch "x64")
    else ()
        set(_arch "x64")
    endif ()
    message(STATUS "Resolved Windows architecture: ${_arch}")
    set(${out_var} "${_arch}" PARENT_SCOPE)
endfunction()

# Resolve WebView2 native SDK base directory from known package locations.
# Params:
# - webview2_version: package version string
# - out_var: parent-scope variable receiving resolved base dir
# Requires `${base}/include/WebView2.h` to exist.
# Fails with `FATAL_ERROR` if no candidate is valid.
function(infiniframe_resolve_webview2_base_dir webview2_version out_var)
    message(STATUS "Resolving WebView2 SDK path for version ${webview2_version}")
    set(_nuget_roots "")
    if (DEFINED INFINIFRAME_NUGET_PACKAGES_ROOT AND NOT "${INFINIFRAME_NUGET_PACKAGES_ROOT}" STREQUAL "")
        list(APPEND _nuget_roots "${INFINIFRAME_NUGET_PACKAGES_ROOT}")
    endif ()
    if (DEFINED ENV{NUGET_PACKAGES} AND NOT "$ENV{NUGET_PACKAGES}" STREQUAL "")
        list(APPEND _nuget_roots "$ENV{NUGET_PACKAGES}")
    endif ()
    if (DEFINED ENV{USERPROFILE} AND NOT "$ENV{USERPROFILE}" STREQUAL "")
        list(APPEND _nuget_roots "$ENV{USERPROFILE}/.nuget/packages")
    endif ()

    set(_candidates
            "${CMAKE_SOURCE_DIR}/packages/Microsoft.Web.WebView2.${webview2_version}/build/native"
            "${CMAKE_SOURCE_DIR}/packages/microsoft.web.webview2/${webview2_version}/build/native"
    )
    foreach (_root IN LISTS _nuget_roots)
        list(APPEND _candidates "${_root}/microsoft.web.webview2/${webview2_version}/build/native")
    endforeach ()
    _infiniframe_resolve_base_dir(_base_dir "include/WebView2.h" ${_candidates})
    if (NOT _base_dir)
        message(FATAL_ERROR "WebView2 headers not found")
    endif ()
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
    message(STATUS "Resolving WIL include path for version ${winimpl_version}")
    set(_nuget_roots "")
    if (DEFINED INFINIFRAME_NUGET_PACKAGES_ROOT AND NOT "${INFINIFRAME_NUGET_PACKAGES_ROOT}" STREQUAL "")
        list(APPEND _nuget_roots "${INFINIFRAME_NUGET_PACKAGES_ROOT}")
    endif ()
    if (DEFINED ENV{NUGET_PACKAGES} AND NOT "$ENV{NUGET_PACKAGES}" STREQUAL "")
        list(APPEND _nuget_roots "$ENV{NUGET_PACKAGES}")
    endif ()
    if (DEFINED ENV{USERPROFILE} AND NOT "$ENV{USERPROFILE}" STREQUAL "")
        list(APPEND _nuget_roots "$ENV{USERPROFILE}/.nuget/packages")
    endif ()

    set(_candidates
            "${CMAKE_SOURCE_DIR}/packages/Microsoft.Windows.ImplementationLibrary.${winimpl_version}"
            "${CMAKE_SOURCE_DIR}/packages/microsoft.windows.implementationlibrary/${winimpl_version}"
    )
    foreach (_root IN LISTS _nuget_roots)
        list(APPEND _candidates "${_root}/microsoft.windows.implementationlibrary/${winimpl_version}")
    endforeach ()

    _infiniframe_resolve_base_dir(_base_include "include/wil/com.h" ${_candidates})
    if (_base_include)
        set(_include_dir "${_base_include}/include")
    else ()
        _infiniframe_resolve_base_dir(_native_include "build/native/include/wil/com.h" ${_candidates})
        if (_native_include)
            set(_include_dir "${_native_include}/build/native/include")
        else ()
            set(_include_dir "")
        endif ()
    endif ()

    if (NOT _include_dir)
        message(FATAL_ERROR "WIL headers not found")
    endif ()
    message(STATUS "Resolved WIL include dir: ${_include_dir}")
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
    message(STATUS "Creating imported target webview2::sdk")
    add_library(webview2::sdk INTERFACE IMPORTED)
    set_target_properties(webview2::sdk PROPERTIES
            INTERFACE_INCLUDE_DIRECTORIES "${webview2_base_dir}/include"
            INTERFACE_LINK_LIBRARIES "${webview2_base_dir}/${win_arch_dir}/WebView2LoaderStatic.lib"
    )

    message(STATUS "Creating imported target wil::headers")
    add_library(wil::headers INTERFACE IMPORTED)
    set_target_properties(wil::headers PROPERTIES
            INTERFACE_INCLUDE_DIRECTORIES "${wil_include_dir}"
    )
endfunction()

# Main module entrypoint. Sets up all dependency targets used by `InfiniFrame.Native`.
# Behavior:
# - Always configures vendored `simdjson` and `simdutf`.
# - On Windows also configures vendored `wintoastlib`, resolves
#   WebView2/WIL from package versions, and creates imported SDK targets.
# Exports to parent scope on Windows:
# - `INFINIFRAME_WEBVIEW2_BASE_DIR`
# - `INFINIFRAME_WINDOWS_ARCH_DIR`
function(infiniframe_setup_dependencies)
    message(STATUS "Setting up InfiniFrame native dependencies")
    infiniframe_add_vendor_static_library("simdjson" "${CMAKE_SOURCE_DIR}/src/Dependencies/simdjson" "simdjson.cpp" "simdjson.h")
    infiniframe_add_vendor_static_library("simdutf" "${CMAKE_SOURCE_DIR}/src/Dependencies/simdutf" "simdutf.cpp" "simdutf.h")

    if (NOT WIN32)
        message(STATUS "Windows-specific dependencies skipped on non-Windows host")
        return()
    endif ()

    infiniframe_add_vendor_static_library("wintoastlib" "${CMAKE_SOURCE_DIR}/src/Dependencies/wintoastlib" "wintoastlib.cpp" "wintoastlib.h")

    infiniframe_detect_windows_arch_dir(_win_arch_dir)
    infiniframe_get_package_version("Microsoft.Web.WebView2" "INFINIFRAME_WEBVIEW2_VERSION" _webview2_version)
    infiniframe_get_package_version("Microsoft.Windows.ImplementationLibrary" "INFINIFRAME_WINDOWS_IMPLEMENTATION_LIBRARY_VERSION" _winimpl_version)
    infiniframe_resolve_webview2_base_dir("${_webview2_version}" _webview2_base_dir)
    infiniframe_resolve_wil_include_dir("${_winimpl_version}" _wil_include_dir)
    infiniframe_add_imported_windows_sdk_targets("${_webview2_base_dir}" "${_win_arch_dir}" "${_wil_include_dir}")

    set(INFINIFRAME_WEBVIEW2_BASE_DIR "${_webview2_base_dir}" PARENT_SCOPE)
    set(INFINIFRAME_WINDOWS_ARCH_DIR "${_win_arch_dir}" PARENT_SCOPE)
    message(STATUS "Dependency setup complete")
endfunction()
