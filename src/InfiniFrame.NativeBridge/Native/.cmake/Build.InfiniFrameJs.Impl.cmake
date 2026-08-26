foreach (required_var NODE_EXECUTABLE FRONTEND_BUILD_SCRIPT JS_PROJECT_DIR JS_STAMP_FILE JS_OUTPUT)
    if (NOT DEFINED ${required_var} OR "${${required_var}}" STREQUAL "")
        message(FATAL_ERROR "${required_var} is required")
    endif ()
endforeach ()

execute_process(
        COMMAND "${NODE_EXECUTABLE}" "${FRONTEND_BUILD_SCRIPT}" "${JS_PROJECT_DIR}" "${JS_STAMP_FILE}" "${JS_OUTPUT}"
        WORKING_DIRECTORY "${JS_PROJECT_DIR}"
        RESULT_VARIABLE frontend_build_result
)

if (NOT frontend_build_result EQUAL 0)
    message(FATAL_ERROR "Frontend build failed with exit code ${frontend_build_result}")
endif ()

if (NOT EXISTS "${JS_OUTPUT}")
    message(FATAL_ERROR "JS build completed but did not create expected output: ${JS_OUTPUT}")
endif ()
