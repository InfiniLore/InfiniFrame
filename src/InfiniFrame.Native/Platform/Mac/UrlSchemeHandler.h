#ifdef __APPLE__
#pragma once
#include "Core/InfiniFrame.h"

@interface UrlSchemeHandler : NSObject <WKURLSchemeHandler> {
    @public
    WebResourceRequestedCallback requestHandler;
}
@end
#endif
