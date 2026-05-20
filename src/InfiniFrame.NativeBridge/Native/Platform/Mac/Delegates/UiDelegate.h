#ifdef __APPLE__
#pragma once
/**
 * @file UiDelegate.h
 * @brief WKUIDelegate and WKScriptMessageHandler that routes JavaScript messages to the .NET layer
 */
#include "Public/InfiniFrame.h"

/**
 * @brief UI delegate conforming to WKUIDelegate and WKScriptMessageHandler.
 *
 * Receives messages posted by JavaScript via window.chrome.webview.postMessage
 * and forwards them to the registered WebMessageReceivedCallback
 */
@ interface UiDelegate :
    NSObject<WKUIDelegate, WKScriptMessageHandler>{
        @public
         NSWindow * window;                                     /// The host NSWindow
         InfiniFrameWindow * infiniFrame
;                       /// The InfiniFrameWindow instance this delegate belongs to
    WebMessageReceivedCallback webMessageReceivedCallback; /// Callback invoked with each incoming web message

    }
@ end
#endif
