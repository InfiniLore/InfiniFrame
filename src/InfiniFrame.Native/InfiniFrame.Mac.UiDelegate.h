#ifdef __APPLE__
#pragma once
#include "InfiniFrame.h"

@interface UiDelegate : NSObject <WKUIDelegate, WKScriptMessageHandler> {
    @public
    NSWindow * window;
    InfiniFrame * infiniFrame;
    WebMessageReceivedCallback webMessageReceivedCallback;
}
@end
#endif