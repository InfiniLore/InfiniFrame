foreach(required_var NPM_EXECUTABLE JS_PROJECT_DIR JS_OUTPUT)
    if(NOT DEFINED ${required_var} OR "${${required_var}}" STREQUAL "")
        message(FATAL_ERROR "${required_var} is required")
    endif()
endforeach()

function(run_npm result_var)
    if(WIN32)
        execute_process(
                COMMAND cmd.exe /D /C call "${NPM_EXECUTABLE}" ${ARGN}
                WORKING_DIRECTORY "${JS_PROJECT_DIR}"
                RESULT_VARIABLE local_result
        )
    else()
        execute_process(
                COMMAND "${NPM_EXECUTABLE}" ${ARGN}
                WORKING_DIRECTORY "${JS_PROJECT_DIR}"
                RESULT_VARIABLE local_result
        )
    endif()

    set(${result_var} "${local_result}" PARENT_SCOPE)
endfunction()

run_npm(npm_ci_result ci)

if(NOT npm_ci_result EQUAL 0)
    message(FATAL_ERROR "npm ci failed with exit code ${npm_ci_result}")
endif()

run_npm(npm_build_result run build)

if(NOT npm_build_result EQUAL 0)
    message(FATAL_ERROR "npm run build failed with exit code ${npm_build_result}")
endif()

if(NOT EXISTS "${JS_OUTPUT}")
    message(FATAL_ERROR "JS build completed but did not create expected output: ${JS_OUTPUT}")
endif()
