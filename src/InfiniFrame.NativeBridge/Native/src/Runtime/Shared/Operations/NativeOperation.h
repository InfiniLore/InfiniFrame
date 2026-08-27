#pragma once

#include <atomic>
#include <cstdint>
#include <memory>

#include "Runtime/Shared/Types/Callbacks.h"

class InfiniFrameWindow;

/// Outcome of a native operation after it has completed.
enum class NativeOperationResult : int32_t {
    /// Operation finished normally.
    Completed = 0,
    /// Operation exceeded its time limit.
    TimedOut = 1,
    /// Operation was cancelled before completion.
    Cancelled = 2,
    /// The owning window was closed while the operation was active.
    WindowClosed = 3,
    /// Operation failed due to an error.
    Failed = 4,
    /// Operation was superseded by a newer equivalent operation.
    Superseded = 5
};

/// Represents a queued native operation that will execute on the UI thread.
struct NativeOperation final {
    /// State constant: operation is queued but not yet running.
    static constexpr int Pending = 0;
    /// State constant: operation is currently executing.
    static constexpr int Running = 1;
    /// State constant: operation has reached a terminal state (completed, failed, cancelled, etc.).
    static constexpr int Terminal = 2;

    /// Unique identifier for this operation.
    uint64_t id;
    /// The action callback to execute on the UI thread.
    ContextAction callback;
    /// Opaque context pointer passed to the action callback.
    void* callbackContext;
    /// Callback invoked when the operation reaches a terminal state.
    OperationCompletedCallback completion;
    /// Opaque context pointer passed to the completion callback.
    void* completionContext;
    /// The window that owns this operation.
    InfiniFrameWindow* owner;
    /// Current state of the operation (Pending, Running, or Terminal).
    std::atomic<int> state = Pending;

    /// Construct a new NativeOperation.
    /// @param operationId Unique identifier.
    /// @param action The action to execute.
    /// @param actionContext Opaque context for the action.
    /// @param completed Completion callback.
    /// @param completedContext Opaque context for the completion callback.
    /// @param window The owning window.
    NativeOperation(
        const uint64_t operationId,
        const ContextAction action,
        void* actionContext,
        const OperationCompletedCallback completed,
        void* completedContext,
        InfiniFrameWindow* window
        ) :
        id(operationId), callback(action), callbackContext(actionContext), completion(completed),
        completionContext(completedContext), owner(window) {}

    /// Execute the operation's action callback on the current thread.
    void Execute() noexcept;
    /// Cancel the operation. Returns true if cancellation succeeded.
    /// @param result The result code to assign (typically Cancelled).
    bool Cancel(NativeOperationResult result) noexcept;
    /// Mark the operation as completed and invoke the completion callback.
    /// @param result The final result code.
    /// @param nativeCode Platform-specific error code (0 for success).
    /// @param failure Optional human-readable failure description.
    void Finish(NativeOperationResult result, int nativeCode = 0, const char* failure = nullptr) noexcept;
};
