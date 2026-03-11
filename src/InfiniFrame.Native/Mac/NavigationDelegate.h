#ifdef __APPLE__
#pragma once
#include "Models/InfiniFrame.h"

@interface NavigationDelegate: NSObject<WKNavigationDelegate>{
    @public
    NSWindow * window;
    InfiniFrame * infiniFrame;
}
@end
#endif
