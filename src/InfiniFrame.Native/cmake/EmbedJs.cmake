function(infiniframe_setup_embed_js target_name js_input output_var)
    set(output_cpp "${CMAKE_SOURCE_DIR}/Embedded/InfiniFrameHostJs.h")

    add_custom_command(
            OUTPUT ${output_cpp}
            COMMAND ${CMAKE_COMMAND}
            -DINPUT=${js_input}
            -DOUTPUT=${output_cpp}
            -P ${CMAKE_SOURCE_DIR}/cmake/EmbedJs_impl.cmake
            DEPENDS ${js_input}
            COMMENT "Embedding JS: ${js_input}"
            VERBATIM
    )

    # ALWAYS attach here (safe because target exists now in your flow)
    target_sources(${target_name} PRIVATE ${output_cpp})

    set(${output_var} ${output_cpp} PARENT_SCOPE)
endfunction()