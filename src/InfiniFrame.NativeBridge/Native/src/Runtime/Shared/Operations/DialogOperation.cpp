#include "Runtime/Shared/Operations/DialogOperation.h"

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"

#include <vector>

void DialogOperation::SetCancelAction(std::function<void()> action) {
    std::function<void()> invokeAction;
    {
        std::lock_guard lock(_cancelMutex);
        _cancelAction = std::move(action);
        const bool invoke = !terminal.load(std::memory_order_acquire)
            && finalResult.load(std::memory_order_acquire)
                != static_cast<int32_t>(NativeOperationResult::Completed);
        if (invoke)
            invokeAction = _cancelAction;
    }
    if (invokeAction)
        invokeAction();
}

bool DialogOperation::CompleteFile(const int32_t result, const int32_t valueCount, const char** values) noexcept {
    bool expected = false;
    if (!terminal.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
        return false;
    const int32_t requested = finalResult.load(std::memory_order_acquire);
    const int32_t effective = requested == static_cast<int32_t>(NativeOperationResult::Completed)
        ? result : requested;
    finalResult.store(effective, std::memory_order_release);
    fileCompletion(
        completionContext, id, effective,
        effective == 0 ? valueCount : 0, effective == 0 ? values : nullptr
    );
    return true;
}

bool DialogOperation::CompleteMessage(const DialogResult value) noexcept {
    bool expected = false;
    if (!terminal.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
        return false;
    const int32_t requested = finalResult.load(std::memory_order_acquire);
    const int32_t effective = requested == static_cast<int32_t>(NativeOperationResult::Completed)
        ? static_cast<int32_t>(NativeOperationResult::Completed) : requested;
    finalResult.store(effective, std::memory_order_release);
    messageCompletion(
        completionContext, id, effective,
        effective == static_cast<int32_t>(NativeOperationResult::Completed)
            ? static_cast<int32_t>(value) : static_cast<int32_t>(DialogResult::Cancel), nullptr
    );
    return true;
}

bool DialogOperation::Cancel(const NativeOperationResult result) noexcept {
    if (terminal.load(std::memory_order_acquire))
        return false;
    int32_t expected = static_cast<int32_t>(NativeOperationResult::Completed);
    if (!finalResult.compare_exchange_strong(
            expected, static_cast<int32_t>(result), std::memory_order_acq_rel))
        return false;

    std::function<void()> cancel;
    {
        std::lock_guard lock(_cancelMutex);
        cancel = _cancelAction;
    }
    if (cancel)
        cancel();

    return true;
}

std::shared_ptr<DialogOperation> InfiniFrameWindow::RegisterFileDialogOperation(
    const uint64_t id, const char* name, const FileDialogCompletedCallback completion, void* context
) {
    auto operation = std::make_shared<DialogOperation>(id, name, completion, context);
    std::lock_guard lock(ImplBase()->_dialogOperationMutex);
    for (auto it = ImplBase()->_dialogOperations.begin(); it != ImplBase()->_dialogOperations.end();) {
        if (it->second->terminal.load(std::memory_order_acquire))
            it = ImplBase()->_dialogOperations.erase(it);
        else
            ++it;
    }
    if (!ImplBase()->_dialogOperations.emplace(id, operation).second)
        throw std::invalid_argument("A dialog operation with this ID already exists.");
    return operation;
}

std::shared_ptr<DialogOperation> InfiniFrameWindow::RegisterMessageDialogOperation(
    const uint64_t id, const OperationCompletedCallback completion, void* context
) {
    auto operation = std::make_shared<DialogOperation>(id, "ShowMessage", completion, context);
    std::lock_guard lock(ImplBase()->_dialogOperationMutex);
    if (!ImplBase()->_dialogOperations.emplace(id, operation).second)
        throw std::invalid_argument("A dialog operation with this ID already exists.");
    return operation;
}

bool InfiniFrameWindow::CancelDialog(const uint64_t id) {
    std::shared_ptr<DialogOperation> operation;
    {
        std::lock_guard lock(ImplBase()->_dialogOperationMutex);
        const auto found = ImplBase()->_dialogOperations.find(id);
        if (found == ImplBase()->_dialogOperations.end())
            return false;
        operation = found->second;
    }
    return operation->Cancel(NativeOperationResult::Cancelled);
}

void InfiniFrameWindow::CompleteDialogsForClose() {
    std::vector<std::shared_ptr<DialogOperation>> operations;
    {
        std::lock_guard lock(ImplBase()->_dialogOperationMutex);
        for (const auto& [id, operation] : ImplBase()->_dialogOperations)
            operations.push_back(operation);
        ImplBase()->_dialogOperations.clear();
    }
    for (const auto& operation : operations)
        operation->Cancel(NativeOperationResult::WindowClosed);
}
