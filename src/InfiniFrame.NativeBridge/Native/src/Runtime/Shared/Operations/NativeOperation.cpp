#include "Runtime/Shared/Operations/NativeOperation.h"

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"

#include <vector>

void NativeOperation::Execute() noexcept {
    int expected = Pending;
    if (!state.compare_exchange_strong(expected, Running, std::memory_order_acq_rel))
        return;

    try {
        if (callback != nullptr)
            callback(callbackContext);
        Finish(NativeOperationResult::Completed);
    } catch (...) {
        Finish(NativeOperationResult::Failed, 0, "The dispatched native callback failed.");
    }
}

bool NativeOperation::Cancel(const NativeOperationResult result) noexcept {
    int expected = Pending;
    if (!state.compare_exchange_strong(expected, Terminal, std::memory_order_acq_rel))
        return false;

    owner->FinalizeOperation(id, completion, completionContext, result, 0, nullptr);
    return true;
}

void NativeOperation::Finish(const NativeOperationResult result, const int nativeCode, const char* failure) noexcept {
    int expected = Running;
    if (!state.compare_exchange_strong(expected, Terminal, std::memory_order_acq_rel))
        return;

    owner->FinalizeOperation(id, completion, completionContext, result, nativeCode, failure);
}

bool InfiniFrameWindow::BeginInvoke(
    const uint64_t operationId,
    const ContextAction callback,
    void* callbackContext,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    if (operationId == 0 || callback == nullptr || completion == nullptr)
        return false;

    auto operation = std::make_shared<NativeOperation>(
        operationId, callback, callbackContext, completion, completionContext, this
    );
    {
        std::lock_guard lock(ImplBase()->_operationMutex);
        if (!ImplBase()->_operations.emplace(operationId, operation).second)
            return false;
    }

    if (ScheduleOperation(operation))
        return true;

    operation->Cancel(NativeOperationResult::Failed);
    return false;
}

bool InfiniFrameWindow::CancelOperation(const uint64_t operationId, const NativeOperationResult result) {
    std::shared_ptr<NativeOperation> operation;
    {
        std::lock_guard lock(ImplBase()->_operationMutex);
        const auto found = ImplBase()->_operations.find(operationId);
        if (found == ImplBase()->_operations.end())
            return false;
        operation = found->second;
    }
    return operation->Cancel(result);
}

void InfiniFrameWindow::CompleteOperationsForClose() {
    struct DetachedCompletion {
        uint64_t id;
        OperationCompletedCallback callback;
        void* context;
    };
    std::vector<DetachedCompletion> completions;
    {
        std::lock_guard lock(ImplBase()->_operationMutex);
        completions.reserve(ImplBase()->_operations.size());
        for (const auto& [id, operation] : ImplBase()->_operations) {
            int expected = NativeOperation::Pending;
            if (operation && operation->state.compare_exchange_strong(
                    expected, NativeOperation::Terminal, std::memory_order_acq_rel)) {
                completions.push_back({id, operation->completion, operation->completionContext});
            }
        }
        ImplBase()->_operations.clear();
    }

    // All window-owned state is detached before the first reverse callback. A callback may
    // dispose the managed owner, so this method must not access `this` in this loop.
    for (const auto& completion : completions) {
        completion.callback(
            completion.context, completion.id,
            static_cast<int32_t>(NativeOperationResult::WindowClosed), 0, nullptr
        );
    }
}

void InfiniFrameWindow::FinalizeOperation(
    const uint64_t operationId,
    const OperationCompletedCallback completion,
    void* completionContext,
    const NativeOperationResult result,
    const int nativeCode,
    const char* failure
) noexcept {
    {
        std::lock_guard lock(ImplBase()->_operationMutex);
        ImplBase()->_operations.erase(operationId);
    }

    // This reverse callback can dispose the managed window. It must be the final access
    // to this operation/window on this stack.
    completion(completionContext, operationId, static_cast<int32_t>(result), nativeCode, failure);
}

void InfiniFrameWindow::SetReadyCallback(const ContextAction callback, void* context) {
    bool invoke = false;
    {
        std::lock_guard lock(ImplBase()->_milestoneMutex);
        ImplBase()->_readyCallback = callback;
        ImplBase()->_readyCallbackContext = context;
        invoke = ImplBase()->_readySignaled && callback != nullptr;
    }
    if (invoke)
        callback(context);
}

void InfiniFrameWindow::SetTeardownCallback(const ContextAction callback, void* context) {
    bool invoke = false;
    {
        std::lock_guard lock(ImplBase()->_milestoneMutex);
        ImplBase()->_teardownCallback = callback;
        ImplBase()->_teardownCallbackContext = context;
        invoke = ImplBase()->_teardownSignaled && callback != nullptr;
    }
    if (invoke)
        callback(context);
}

void InfiniFrameWindow::SignalReady() {
    ContextAction callback = nullptr;
    void* context = nullptr;
    {
        std::lock_guard lock(ImplBase()->_milestoneMutex);
        if (ImplBase()->_readySignaled)
            return;
        ImplBase()->_readySignaled = true;
        callback = ImplBase()->_readyCallback;
        context = ImplBase()->_readyCallbackContext;
    }
    if (callback != nullptr)
        callback(context);
}

void InfiniFrameWindow::SignalTeardown() {
    ContextAction callback = nullptr;
    void* context = nullptr;
    {
        std::lock_guard lock(ImplBase()->_milestoneMutex);
        if (ImplBase()->_teardownSignaled)
            return;
        ImplBase()->_teardownSignaled = true;
        callback = ImplBase()->_teardownCallback;
        context = ImplBase()->_teardownCallbackContext;
    }
    if (callback != nullptr)
        callback(context);
}

namespace {
    void CompleteDetachedNavigation(
        std::unique_ptr<NavigationOperation> operation,
        const NativeOperationResult result,
        const int nativeCode = 0,
        const char* failure = nullptr
    ) {
        if (operation && operation->completion)
            operation->completion(
                operation->completionContext, operation->id,
                static_cast<int32_t>(result), nativeCode, failure
            );
    }
}

bool InfiniFrameWindow::BeginNavigateToString(
    const uint64_t operationId,
    AutoString content,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    std::unique_ptr<NavigationOperation> superseded;
    {
        std::lock_guard lock(ImplBase()->_navigationMutex);
        superseded = std::move(ImplBase()->_navigationOperation);
        ImplBase()->_navigationOperation = std::make_unique<NavigationOperation>(NavigationOperation{
            operationId, 0, completion, completionContext
        });
    }
    NavigateToString(content);
    CompleteDetachedNavigation(std::move(superseded), NativeOperationResult::Superseded);
    return true;
}

bool InfiniFrameWindow::BeginNavigateToUrl(
    const uint64_t operationId,
    AutoString url,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    std::unique_ptr<NavigationOperation> superseded;
    {
        std::lock_guard lock(ImplBase()->_navigationMutex);
        superseded = std::move(ImplBase()->_navigationOperation);
        ImplBase()->_navigationOperation = std::make_unique<NavigationOperation>(NavigationOperation{
            operationId, 0, completion, completionContext
        });
    }
    NavigateToUrl(url);
    CompleteDetachedNavigation(std::move(superseded), NativeOperationResult::Superseded);
    return true;
}

bool InfiniFrameWindow::CancelNavigation(const uint64_t operationId) {
    std::unique_ptr<NavigationOperation> cancelled;
    {
        std::lock_guard lock(ImplBase()->_navigationMutex);
        if (!ImplBase()->_navigationOperation || ImplBase()->_navigationOperation->id != operationId)
            return false;
        cancelled = std::move(ImplBase()->_navigationOperation);
    }
    CompleteDetachedNavigation(std::move(cancelled), NativeOperationResult::Cancelled);
    return true;
}

void InfiniFrameWindow::BindNavigationBackendId(const uint64_t backendId) {
    std::lock_guard lock(ImplBase()->_navigationMutex);
    if (ImplBase()->_navigationOperation && ImplBase()->_navigationOperation->backendId == 0)
        ImplBase()->_navigationOperation->backendId = backendId;
}

void InfiniFrameWindow::CompleteNavigation(
    const uint64_t backendId,
    const bool succeeded,
    const int nativeCode,
    const char* failureUtf8
) {
    std::unique_ptr<NavigationOperation> completed;
    {
        std::lock_guard lock(ImplBase()->_navigationMutex);
        if (!ImplBase()->_navigationOperation)
            return;
        if (backendId != 0 && ImplBase()->_navigationOperation->backendId != 0
            && backendId != ImplBase()->_navigationOperation->backendId)
            return;
        completed = std::move(ImplBase()->_navigationOperation);
    }
    CompleteDetachedNavigation(
        std::move(completed),
        succeeded ? NativeOperationResult::Completed : NativeOperationResult::Failed,
        nativeCode, failureUtf8
    );
}

void InfiniFrameWindow::CompleteNavigationAndSignalReady(
    const uint64_t backendId,
    const bool succeeded,
    const int nativeCode,
    const char* failureUtf8
) {
    std::unique_ptr<NavigationOperation> completed;
    ContextAction readyCallback = nullptr;
    void* readyContext = nullptr;
    {
        std::lock_guard lock(ImplBase()->_navigationMutex);
        if (ImplBase()->_navigationOperation
            && (backendId == 0 || ImplBase()->_navigationOperation->backendId == 0
                || backendId == ImplBase()->_navigationOperation->backendId)) {
            completed = std::move(ImplBase()->_navigationOperation);
        }
    }
    {
        std::lock_guard lock(ImplBase()->_milestoneMutex);
        if (!ImplBase()->_readySignaled) {
            ImplBase()->_readySignaled = true;
            readyCallback = ImplBase()->_readyCallback;
            readyContext = ImplBase()->_readyCallbackContext;
        }
    }

    // Both callbacks may synchronously initiate managed disposal. All window-owned state
    // has therefore been detached above, and this method must not access `this` below.
    CompleteDetachedNavigation(
        std::move(completed),
        succeeded ? NativeOperationResult::Completed : NativeOperationResult::Failed,
        nativeCode, failureUtf8
    );
    if (readyCallback != nullptr)
        readyCallback(readyContext);
}

void InfiniFrameWindow::CompleteNavigationForClose() {
    std::unique_ptr<NavigationOperation> completed;
    {
        std::lock_guard lock(ImplBase()->_navigationMutex);
        completed = std::move(ImplBase()->_navigationOperation);
    }
    CompleteDetachedNavigation(std::move(completed), NativeOperationResult::WindowClosed);
}
