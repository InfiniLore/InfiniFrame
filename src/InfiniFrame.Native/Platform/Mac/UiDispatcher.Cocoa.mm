#ifdef __APPLE__

#include "Platform/Mac/WindowImpl.Cocoa.h"

#include <dispatch/dispatch.h>

void InfiniFrameWindow::Invoke(ACTION callback)
{
    if (callback == nullptr)
        return;

    if ([NSThread isMainThread])
        callback();
    else
        dispatch_sync(dispatch_get_main_queue(), ^{
            callback();
        });
}

#endif
