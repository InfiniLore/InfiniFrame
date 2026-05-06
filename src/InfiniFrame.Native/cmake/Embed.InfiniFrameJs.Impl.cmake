file(READ "${INPUT}" JS_CONTENT HEX)

string(LENGTH "${JS_CONTENT}" LEN)
math(EXPR LAST "${LEN} - 2")

set(BYTES "")
set(COUNT 0)

foreach(i RANGE 0 ${LAST} 2)
    string(SUBSTRING "${JS_CONTENT}" ${i} 2 BYTE)

    if(i EQUAL ${LAST})
        string(APPEND BYTES "0x${BYTE}")
    else()
        string(APPEND BYTES "0x${BYTE},")
    endif()

    math(EXPR COUNT "${COUNT} + 1")

    if(COUNT EQUAL 16)
        string(APPEND BYTES "\n")
        set(COUNT 0)
    endif()
endforeach()

# Header file
file(WRITE "${OUTPUT_HEADER}" "#pragma once
// ReSharper disable once CppUnusedIncludeDirective
#include <cstddef>

extern const unsigned char g_infiniframe_js_data[];
extern const size_t g_infiniframe_js_size;
")

# Source file
file(WRITE "${OUTPUT_SOURCE}" "#include \"InfiniFrameJs.h\"

alignas(16) const unsigned char g_infiniframe_js_data[] = {
${BYTES}
};

const size_t g_infiniframe_js_size = sizeof(g_infiniframe_js_data);
")