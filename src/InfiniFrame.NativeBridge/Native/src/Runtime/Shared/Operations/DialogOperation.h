#pragma once

#include <atomic>
#include <chrono>
#include <cstdint>
#include <functional>
#include <mutex>
#include <string>

#include "Runtime/Shared/Operations/NativeOperation.h"
#include "Runtime/Shared/Types/Callbacks.h"
#include "Runtime/Shared/Types/DialogResult.h"

/** Window-owned state shared with an asynchronous native dialog callback. */
struct DialogOperation final {
    enum class Kind { File, Message };

    uint64_t id;
    Kind kind;
    std::string name;
    std::chrono::steady_clock::time_point started = std::chrono::steady_clock::now();
    FileDialogCompletedCallback fileCompletion = nullptr;
    OperationCompletedCallback messageCompletion = nullptr;
    void* completionContext = nullptr;
    std::atomic<bool> terminal = false;
    std::atomic<int32_t> finalResult = static_cast<int32_t>(NativeOperationResult::Completed);

    DialogOperation(
        uint64_t operationId, std::string operationName,
        FileDialogCompletedCallback completion, void* context
    ) : id(operationId), kind(Kind::File), name(std::move(operationName)),
        fileCompletion(completion), completionContext(context) {}

    DialogOperation(
        uint64_t operationId, std::string operationName,
        OperationCompletedCallback completion, void* context
    ) : id(operationId), kind(Kind::Message), name(std::move(operationName)),
        messageCompletion(completion), completionContext(context) {}

    void SetCancelAction(std::function<void()> action);
    bool CompleteFile(int32_t result, int32_t valueCount, AutoString* values) noexcept;
    bool CompleteMessage(DialogResult value) noexcept;
    bool Cancel(NativeOperationResult result) noexcept;

    private:
    std::mutex _cancelMutex;
    std::function<void()> _cancelAction;
};
