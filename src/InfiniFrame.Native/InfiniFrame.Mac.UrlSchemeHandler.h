#ifdef __APPLE__
#pragma once
#include "InfiniFrame.h"

@interface UrlSchemeHandler : NSObject <WKURLSchemeHandler> {
    @public
    WebResourceRequestedCallback requestHandler;
}
@end
#endif