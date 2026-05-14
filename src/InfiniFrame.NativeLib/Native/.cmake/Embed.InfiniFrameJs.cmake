function(infiniframe_setup_embed_js target_name)
    set(INFINIFRAME_JS_PROJECT_DIR "${CMAKE_SOURCE_DIR}/../../../InfiniFrame.Js" CACHE PATH "Path to InfiniFrame JS project")
    set(js_project_dir "${INFINIFRAME_JS_PROJECT_DIR}")

    set(js_input "${js_project_dir}/wwwroot/InfiniFrame.js")

    set(embed_dir "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameJs")
    set(header_output "${embed_dir}/InfiniFrameJs.h")
    set(source_output "${embed_dir}/InfiniFrameJs.cpp")

    # Ensure output directory exists
    file(MAKE_DIRECTORY "${embed_dir}")

    # Locate npm (works on Windows/Linux/macOS)
    find_program(NPM_EXECUTABLE NAMES npm npm.cmd REQUIRED)

    # Track TS sources
    file(GLOB_RECURSE js_sources CONFIGURE_DEPENDS
            "${js_project_dir}/TypeScript/*.ts"
    )

    # Build JS bundle
    add_custom_command(
            OUTPUT "${js_input}"
            COMMAND "${CMAKE_COMMAND}"
            "-DNPM_EXECUTABLE=${NPM_EXECUTABLE}"
            "-DJS_PROJECT_DIR=${js_project_dir}"
            "-DJS_OUTPUT=${js_input}"
            -P "${CMAKE_SOURCE_DIR}/.cmake/Build.InfiniFrameJs.Impl.cmake"
            DEPENDS
            ${js_sources}
            "${js_project_dir}/package.json"
            "${js_project_dir}/package-lock.json"
            "${js_project_dir}/tsconfig.json"
            "${js_project_dir}/vite.config.dev.ts"
            "${js_project_dir}/vite.config.prod.ts"
            "${CMAKE_SOURCE_DIR}/.cmake/Build.InfiniFrameJs.Impl.cmake"
            COMMENT "Building JS: ${js_input}"
            VERBATIM
    )

    set(js_build_target "${target_name}_InfiniFrameJsBuild")

    add_custom_target(${js_build_target}
            DEPENDS ${js_input}
    )

    # Embed JS into C++
    add_custom_command(
            OUTPUT ${header_output} ${source_output}
            COMMAND ${CMAKE_COMMAND}
            -DINPUT=${js_input}
            -DOUTPUT_HEADER=${header_output}
            -DOUTPUT_SOURCE=${source_output}
            -P ${CMAKE_SOURCE_DIR}/.cmake/Embed.InfiniFrameJs.Impl.cmake
            DEPENDS
            ${js_input}
            ${CMAKE_SOURCE_DIR}/.cmake/Embed.InfiniFrameJs.Impl.cmake
            COMMENT "Embedding JS: ${js_input}"
            VERBATIM
    )

    set(gen_target "${target_name}_InfiniFrameJsGen")

    add_custom_target(${gen_target}
            DEPENDS ${header_output} ${source_output}
    )

    # Ensure correct build order
    add_dependencies(${gen_target} ${js_build_target})
    add_dependencies(${target_name} ${gen_target})

    # Attach generated source to target
    target_sources(${target_name} PRIVATE ${source_output})

    target_include_directories(${target_name} PRIVATE
            "${embed_dir}"
    )
endfunction()
