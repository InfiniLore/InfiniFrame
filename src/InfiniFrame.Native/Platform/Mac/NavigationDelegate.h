#ifdef __APPLE__
#pragma once
#include "Core/InfiniFrame.h"

@interface NavigationDelegate: NSObject<WKNavigationDelegate>{
    @public
    NSWindow * window;
    InfiniFrame * infiniFrame;
}
@end
#endif
