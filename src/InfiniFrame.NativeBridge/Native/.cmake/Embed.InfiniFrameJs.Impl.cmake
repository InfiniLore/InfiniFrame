file(READ "${INPUT}" JS_CONTENT HEX)

string(LENGTH "${JS_CONTENT}" LEN)
math(EXPR LAST "${LEN} - 2")

set(BYTES "")

foreach(i RANGE 0 ${LAST} 2)
    string(SUBSTRING "${JS_CONTENT}" ${i} 2 BYTE)

    if(i EQUAL ${LAST})
        string(APPEND BYTES "0x${BYTE}")
    else()
        string(APPEND BYTES "0x${BYTE},")
    endif()
endforeach()

# Generate timestamp
string(TIMESTAMP GENERATED_AT "%Y-%m-%d %H:%M:%S UTC" UTC)

# Header file
file(WRITE "${OUTPUT_HEADER}" "#pragma once
// ReSharper disable once CppUnusedIncludeDirective
#include <cstddef>

extern const unsigned char GInfiniframeJsData[];
extern const size_t GInfiniframeJsSize;
")

# Source file
file(WRITE "${OUTPUT_SOURCE}" "#include \"InfiniFrameJs.h\"

// -----------------------------------------------------------------------------
// Auto-generated file. Do not edit manually.
// Generated at: ${GENERATED_AT}
// -----------------------------------------------------------------------------

alignas(16) const unsigned char GInfiniframeJsData[] = {${BYTES}};

const size_t GInfiniframeJsSize = sizeof(GInfiniframeJsData);
")