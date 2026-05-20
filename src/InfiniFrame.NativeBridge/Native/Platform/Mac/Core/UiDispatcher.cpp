// Pure-C++ implementation of Invoke() using the C GCD API (dispatch_sync_f) and
// pthread_main_np() instead of Obj-C [NSThread isMainThread] and Obj-C Block syntax.
// No Obj-C runtime required.

#include <dispatch/dispatch.h>
#include <pthread.h>

#include "Public/InfiniFrameWindow.h"

void InfiniFrameWindow::Invoke(ACTION callback)
{
    if (pthread_main_np())
    {
        callback();
    }
    else
    {
        // dispatch_sync_f is the C API counterpart to dispatch_sync(queue, ^{ … }).
        // We store the function pointer on the stack; dispatch_sync_f blocks until the
        // submitted function returns, so the pointer remains valid for the whole call.
        ACTION fn = callback;
        dispatch_sync_f(dispatch_get_main_queue(), &fn, [](void* ctx)
        {
            (*static_cast<ACTION*>(ctx))();
        });
    }
}
