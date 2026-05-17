#ifdef __APPLE__

#include "../Window.Cocoa.Internal.h"

void InfiniFrameWindow::Invoke(ACTION callback)
{
    if ([NSThread isMainThread])
        callback();
    else
        dispatch_sync(dispatch_get_main_queue(), ^(void){ callback(); });
}

#endif
