function(infiniframe_setup_embed_js target_name output_var)
    set(js_input "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs/infiniframe.host.js")
    set(cpp_output "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs/InfiniFrameHostJs.h")

    add_custom_command(
            OUTPUT ${cpp_output}
            COMMAND ${CMAKE_COMMAND}
            -DINPUT=${js_input}
            -DOUTPUT=${cpp_output}
            -P ${CMAKE_SOURCE_DIR}/cmake/EmbedJs_impl.cmake
            DEPENDS ${js_input}
            COMMENT "Embedding JS: ${js_input}"
            VERBATIM
    )

    target_sources(${target_name} PRIVATE ${cpp_output})
endfunction()