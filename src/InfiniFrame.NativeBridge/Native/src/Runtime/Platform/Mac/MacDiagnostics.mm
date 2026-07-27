// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "MacDiagnostics.h"

#include <Cocoa/Cocoa.h>
#include <dispatch/dispatch.h>
#include <execinfo.h>
#include <pthread.h>
#include <signal.h>
#include <unistd.h>

#include <atomic>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <condition_variable>
#include <mutex>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    std::atomic<bool> diagnosticsEnabled = false;
    thread_local unsigned int nativeCallbackDepth = 0;
    std::atomic<unsigned int> activeNativeCallbacks = 0;
    std::mutex nativeCallbackMutex;
    std::condition_variable nativeCallbackCondition;
    NSMutableArray* pendingWebKitTeardowns = nil;
    bool webKitTeardownDrainScheduled = false;

    void ScheduleNextWebKitTeardown() {
        if (pendingWebKitTeardowns.count == 0) {
            webKitTeardownDrainScheduled = false;
            return;
        }

        // Do not batch view removal. A display refresh callback can be in flight after a view
        // has stopped loading; allowing one full main-run-loop turn between removals avoids the
        // WebKit DisplayLink observer race seen on both Intel and Apple Silicon runners.
        [NSTimer scheduledTimerWithTimeInterval:0.05
                                        repeats:NO
                                          block:^(NSTimer* timer) {
                (void)timer;
                infiniframe::macos::MainRunLoopWork work =
                    [[pendingWebKitTeardowns objectAtIndex:0] retain];
                [pendingWebKitTeardowns removeObjectAtIndex:0];
                work();
                [work release];
                ScheduleNextWebKitTeardown();
            }
        ];
    }

    void WriteSignalMessage(const int signalNumber) noexcept {
        static constexpr char prefix[] = "\n[InfiniFrame macOS fatal signal] native stack follows\n";
        (void)!write(STDERR_FILENO, prefix, sizeof(prefix) - 1);

        void* frames[128];
        const int frameCount = backtrace(frames, 128);
        backtrace_symbols_fd(frames, frameCount, STDERR_FILENO);

        signal(signalNumber, SIG_DFL);
        raise(signalNumber);
    }

    void UncaughtObjectiveCException(NSException* exception) noexcept {
        const char* name = exception.name.UTF8String;
        const char* reason = exception.reason.UTF8String;
        std::fprintf(
            stderr,
            "[InfiniFrame macOS Objective-C exception] name=%s reason=%s\n",
            name == nullptr ? "(null)" : name,
            reason == nullptr ? "(null)" : reason
        );
        for (NSString* frame in exception.callStackSymbols)
            std::fprintf(stderr, "%s\n", frame.UTF8String);
        std::fflush(stderr);
    }
}

infiniframe::macos::NativeCallbackScope::NativeCallbackScope() noexcept {
    ++nativeCallbackDepth;
    activeNativeCallbacks.fetch_add(1, std::memory_order_acq_rel);
}

infiniframe::macos::NativeCallbackScope::~NativeCallbackScope() noexcept {
    --nativeCallbackDepth;
    bool becameIdle = false;
    {
        std::lock_guard lock(nativeCallbackMutex);
        becameIdle = activeNativeCallbacks.fetch_sub(1, std::memory_order_acq_rel) == 1;
    }
    if (becameIdle)
        nativeCallbackCondition.notify_all();
}

void infiniframe::macos::WaitForNativeCallbacksToExit() noexcept {
    std::unique_lock lock(nativeCallbackMutex);
    nativeCallbackCondition.wait(lock, [] {
        return activeNativeCallbacks.load(std::memory_order_acquire) == 0;
    });
}

void infiniframe::macos::EnqueueWebKitTeardown(MainRunLoopWork work) noexcept {
    if (work == nil)
        return;

    void (^enqueue)() = ^{
        if (pendingWebKitTeardowns == nil)
            pendingWebKitTeardowns = [[NSMutableArray alloc] init];

        [pendingWebKitTeardowns addObject:[[work copy] autorelease]];
        if (webKitTeardownDrainScheduled)
            return;

        webKitTeardownDrainScheduled = true;
        ScheduleNextWebKitTeardown();
    };

    if ([NSThread isMainThread])
        enqueue();
    else
        dispatch_async(dispatch_get_main_queue(), enqueue);
}

bool infiniframe::macos::IsInsideNativeCallback() noexcept {
    return nativeCallbackDepth != 0;
}

void infiniframe::macos::InstallDiagnostics() noexcept {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        const char* setting = std::getenv("INFINIFRAME_NATIVE_DIAGNOSTICS");
        if (setting == nullptr || std::strcmp(setting, "0") == 0)
            return;

        diagnosticsEnabled.store(true, std::memory_order_release);
        NSSetUncaughtExceptionHandler(&UncaughtObjectiveCException);
        signal(SIGABRT, &WriteSignalMessage);
        signal(SIGBUS, &WriteSignalMessage);
        signal(SIGSEGV, &WriteSignalMessage);
        signal(SIGILL, &WriteSignalMessage);
        signal(SIGFPE, &WriteSignalMessage);
        LogLifecycle("diagnostics-installed", nullptr);
    });
}

void infiniframe::macos::LogLifecycle(const char* event, const void* instance) noexcept {
    if (!diagnosticsEnabled.load(std::memory_order_acquire))
        return;

    uint64_t threadId = 0;
    pthread_threadid_np(nullptr, &threadId);
    const char* queue = dispatch_queue_get_label(DISPATCH_CURRENT_QUEUE_LABEL);
    std::fprintf(
        stderr,
        "[InfiniFrame macOS] event=%s instance=%p thread=%llu main=%d queue=%s\n",
        event == nullptr ? "(null)" : event,
        instance,
        static_cast<unsigned long long>(threadId),
        [NSThread isMainThread] ? 1 : 0,
        queue == nullptr || queue[0] == '\0' ? "(none)" : queue
    );
    std::fflush(stderr);
}
