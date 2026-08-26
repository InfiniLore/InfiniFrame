#pragma once

#include <cstdint>

#include "Runtime/Shared/Types/Callbacks.h"

/// Represents an in-flight navigation operation (load URL or load string).
struct NavigationOperation final {
    /// Unique identifier for this operation.
    uint64_t id;
    /// Backend identifier used to resolve and complete the navigation.
    uint64_t backendId = 0;
    /// Callback invoked when the navigation completes.
    OperationCompletedCallback completion;
    /// Opaque context pointer passed to the completion callback.
    void* completionContext;
};
