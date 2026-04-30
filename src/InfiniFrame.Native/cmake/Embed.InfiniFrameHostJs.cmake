function(infiniframe_setup_embed_js target_name)
    set(js_input "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs/infiniframe.host.js")

    set(header_output "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs/InfiniFrameHostJs.h")
    set(source_output "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs/InfiniFrameHostJs.cpp")

    add_custom_command(
            OUTPUT ${header_output} ${source_output}
            COMMAND ${CMAKE_COMMAND}
            -DINPUT=${js_input}
            -DOUTPUT_HEADER=${header_output}
            -DOUTPUT_SOURCE=${source_output}
            -P ${CMAKE_SOURCE_DIR}/cmake/Embed.InfiniFrameHostJs.Impl.cmake
            DEPENDS ${js_input} ${CMAKE_SOURCE_DIR}/cmake/Embed.InfiniFrameHostJs.Impl.cmake
            COMMENT "Embedding JS: ${js_input}"
            VERBATIM
    )

    set(gen_target "${target_name}_InfiniFrameHostJsGen")

    add_custom_target(${gen_target}
            DEPENDS ${header_output} ${source_output}
    )

    add_dependencies(${target_name} ${gen_target})

    target_sources(${target_name} PRIVATE ${source_output})
    target_include_directories(${target_name} PRIVATE
            "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs"
    )
endfunction()