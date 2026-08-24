#pragma once

#include <cstdint>

#include "Runtime/Shared/Types/Callbacks.h"

struct NavigationOperation final {
    uint64_t id;
    uint64_t backendId = 0;
    OperationCompletedCallback completion;
    void* completionContext;
};