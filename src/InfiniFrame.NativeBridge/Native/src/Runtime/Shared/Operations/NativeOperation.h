#pragma once

#include <atomic>
#include <cstdint>
#include <memory>

#include "Runtime/Shared/Types/Callbacks.h"

class InfiniFrameWindow;

enum class NativeOperationResult : int32_t {
    Completed = 0,
    TimedOut = 1,
    Cancelled = 2,
    WindowClosed = 3,
    Failed = 4,
    Superseded = 5
};

struct NativeOperation final {
    static constexpr int Pending = 0;
    static constexpr int Running = 1;
    static constexpr int Terminal = 2;

    uint64_t id;
    ContextAction callback;
    void* callbackContext;
    OperationCompletedCallback completion;
    void* completionContext;
    InfiniFrameWindow* owner;
    std::atomic<int> state = Pending;

    NativeOperation(
        uint64_t operationId,
        ContextAction action,
        void* actionContext,
        OperationCompletedCallback completed,
        void* completedContext,
        InfiniFrameWindow* window
    ) : id(operationId), callback(action), callbackContext(actionContext), completion(completed),
        completionContext(completedContext), owner(window) {}

    void Execute() noexcept;
    bool Cancel(NativeOperationResult result) noexcept;
    void Finish(NativeOperationResult result, int nativeCode = 0, const char* failure = nullptr) noexcept;
};
