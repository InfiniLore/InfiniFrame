file(READ "${INPUT}" JS_CONTENT HEX)

string(LENGTH "${JS_CONTENT}" LEN)
math(EXPR LAST "${LEN} - 2")

set(BYTES "")

foreach(i RANGE 0 ${LAST} 2)
    string(SUBSTRING "${JS_CONTENT}" ${i} 2 BYTE)
    string(APPEND BYTES "0x${BYTE},")
endforeach()

file(WRITE "${OUTPUT}" "
#pragma once
#include <cstddef>

extern \"C\" {

static const unsigned char g_infiniframe_host_js_data[] = {
${BYTES}
};

constexpr size_t g_infiniframe_host_js_size = sizeof(g_infiniframe_host_js_data);

}
")