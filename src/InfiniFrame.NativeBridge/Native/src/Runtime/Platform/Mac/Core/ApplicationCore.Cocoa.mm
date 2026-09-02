// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import <Cocoa/Cocoa.h>
#include <stdexcept>

#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
#include "Runtime/Shared/Application/ApplicationInitParams.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@interface InfiniFrameAppDelegate : NSObject <NSApplicationDelegate>
@end

@implementation InfiniFrameAppDelegate
- (void)applicationDidFinishLaunching:(NSNotification *)notification {
    (void)notification;
}

- (BOOL)applicationShouldTerminateAfterLastWindowClosed:(NSApplication *)sender {
    (void)sender;
    return YES;
}
@end

InfiniFrameApplication* InfiniFrameApplication::s_instance = nullptr;

InfiniFrameApplication* InfiniFrameApplication::GetInstance() {
    return s_instance;
}

InfiniFrameApplication::InfiniFrameApplication(ApplicationInitParams* params) {
    m_impl = std::make_unique<Impl>();
    s_instance = this;

    if (params == nullptr)
        throw std::invalid_argument("Argument 'params' is null.");

    if (params->StructSize != sizeof(ApplicationInitParams))
        throw std::invalid_argument("ApplicationInitParams size mismatch.");
}

InfiniFrameApplication::~InfiniFrameApplication() {
    s_instance = nullptr;
}

void InfiniFrameApplication::Register() {
    @autoreleasepool {
        InfiniFrameAppDelegate* delegate = [[InfiniFrameAppDelegate alloc] init];
        [NSApp setDelegate:delegate];
        [NSApp setActivationPolicy:NSApplicationActivationPolicyRegular];
        [NSApp activateIgnoringOtherApps:YES];
    }
}

void InfiniFrameApplication::TrackWindow(InfiniFrameWindow* window) {
    std::lock_guard lock(m_impl->_windowListMutex);
    m_impl->_windows.push_back(window);
}

void InfiniFrameApplication::UntrackWindow(InfiniFrameWindow* window) {
    std::lock_guard lock(m_impl->_windowListMutex);
    auto it = std::remove(m_impl->_windows.begin(), m_impl->_windows.end(), window);
    m_impl->_windows.erase(it, m_impl->_windows.end());

    if (m_impl->_windows.empty() && !m_impl->_shutdownRequested.load(std::memory_order_acquire)) {
        Shutdown();
    }
}

bool InfiniFrameApplication::HasWindows() const {
    std::lock_guard lock(m_impl->_windowListMutex);
    return !m_impl->_windows.empty();
}

bool InfiniFrameApplication::IsShutdownRequested() const {
    return m_impl->_shutdownRequested.load(std::memory_order_acquire);
}
