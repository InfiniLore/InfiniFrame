#ifdef __APPLE__
#pragma once
#include "Models/InfiniFrame.h"

@interface UrlSchemeHandler : NSObject <WKURLSchemeHandler> {
    @public
    WebResourceRequestedCallback requestHandler;
}
@end
#endif
