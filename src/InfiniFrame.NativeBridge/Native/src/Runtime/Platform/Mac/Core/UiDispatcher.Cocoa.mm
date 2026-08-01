// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "../Window.Cocoa.Internal.h"
#include "Runtime/Shared/Operations/NativeOperation.h"

#include <atomic>
#include <chrono>
#include <memory>

namespace {
    struct InvokeState {
        ACTION callback;
        dispatch_semaphore_t completion = dispatch_semaphore_create(0);
        // 0 = queued, 1 = executing, 2 = abandoned.
        std::atomic<int> state = 0;
    };
}
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::Invoke(ACTION callback) {
    if (callback == nullptr)
        return;

    if ([NSThread isMainThread]) {
        callback();
        return;
    }

    auto state = std::make_shared<InvokeState>();
    state->callback = callback;
    dispatch_async(dispatch_get_main_queue(), ^(void) {
        if (state->state.exchange(1, std::memory_order_acq_rel) != 2)
            state->callback();
        dispatch_semaphore_signal(state->completion);
    });

    const dispatch_time_t deadline = dispatch_time(DISPATCH_TIME_NOW, 15 * NSEC_PER_SEC);
    if (dispatch_semaphore_wait(state->completion, deadline) == 0)
        return;

    int expected = 0;
    if (state->state.compare_exchange_strong(expected, 2, std::memory_order_acq_rel)) {
        NSLog(@"InfiniFrame UI dispatch timed out; late callback suppressed.");
        return;
    }

    // Execution already began. Do not return while native code still owns the managed reverse P/Invoke callback.
    dispatch_semaphore_wait(state->completion, DISPATCH_TIME_FOREVER);
}

bool InfiniFrameWindow::ScheduleOperation(const std::shared_ptr<NativeOperation>& operation) {
    CFRunLoopRef mainRunLoop = CFRunLoopGetMain();
    if (mainRunLoop == nullptr)
        return false;

    CFRunLoopPerformBlock(mainRunLoop, kCFRunLoopCommonModes, ^{
        operation->Execute();
    });
    CFRunLoopWakeUp(mainRunLoop);
    return true;
}
