file(READ "${INPUT}" JS_CONTENT HEX)

file(WRITE "${OUTPUT}" "
#include <cstddef>

extern \"C\" {

static const unsigned char g_infiniframe_host_js_data[] = {
")

string(LENGTH "${JS_CONTENT}" LEN)

foreach(i RANGE 0 ${LEN} 2)
    string(SUBSTRING "${JS_CONTENT}" ${i} 2 BYTE)
    if(NOT BYTE STREQUAL "")
        file(APPEND "${OUTPUT}" "0x${BYTE},")
    endif()
endforeach()

file(APPEND "${OUTPUT}" "
};

constexpr size_t g_infiniframe_host_js_size = sizeof(g_infiniframe_host_js_data);

}
")