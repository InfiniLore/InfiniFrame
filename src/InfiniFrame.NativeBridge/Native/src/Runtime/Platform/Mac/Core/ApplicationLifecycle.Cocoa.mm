// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import <Cocoa/Cocoa.h>
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameApplication::Run() {
    [NSApp run];
}

void InfiniFrameApplication::Shutdown() {
    bool expected = false;
    if (!m_impl->_shutdownRequested.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
        return;

    dispatch_async(dispatch_get_main_queue(), ^{
        [NSApp terminate:nil];
    });
}
