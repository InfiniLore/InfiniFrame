#ifdef __APPLE__
#pragma once
#include "Models/InfiniFrame.h"

@interface WindowDelegate : NSObject <NSWindowDelegate>
{
    @public
        InfiniFrame * infiniFrame;
}
@end
#endif
