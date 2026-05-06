function(infiniframe_setup_embed_js target_name)
    set(js_input "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameJs/infiniframe.host.js")

    set(header_output "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameJs/InfiniFrameJs.h")
    set(source_output "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameJs/InfiniFrameJs.cpp")

    add_custom_command(
            OUTPUT ${header_output} ${source_output}
            COMMAND ${CMAKE_COMMAND}
            -DINPUT=${js_input}
            -DOUTPUT_HEADER=${header_output}
            -DOUTPUT_SOURCE=${source_output}
            -P ${CMAKE_SOURCE_DIR}/cmake/Embed.InfiniFrameJs.Impl.cmake
            DEPENDS ${js_input} ${CMAKE_SOURCE_DIR}/cmake/Embed.InfiniFrameJs.Impl.cmake
            COMMENT "Embedding JS: ${js_input}"
            VERBATIM
    )

    set(gen_target "${target_name}_InfiniFrameJsGen")

    add_custom_target(${gen_target}
            DEPENDS ${header_output} ${source_output}
    )

    add_dependencies(${target_name} ${gen_target})

    target_sources(${target_name} PRIVATE ${source_output})
    target_include_directories(${target_name} PRIVATE
            "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameJs"
    )
endfunction()