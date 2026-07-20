// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import "UiDelegate.h"

#include "../MacDiagnostics.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@implementation UiDelegate : NSObject
- (void)userContentController:(WKUserContentController *)userContentController
        didReceiveScriptMessage:(WKScriptMessage *)message
{
    if (infiniFrame == nullptr || webMessageReceivedCallback == nullptr) return;

    NSString* messageText = nil;
    if ([message.body isKindOfClass:[NSString class]]) {
        messageText = (NSString *)message.body;
    } else if ([NSJSONSerialization isValidJSONObject:message.body]) {
        NSData* json = [NSJSONSerialization dataWithJSONObject:message.body options:0 error:nil];
        if (json != nil)
            messageText = [[[NSString alloc] initWithData:json encoding:NSUTF8StringEncoding] autorelease];
    }
    if (messageText == nil)
        messageText = [message.body description];
    if (messageText == nil)
        messageText = @"";

    char *messageUtf8 = const_cast<char *>([messageText UTF8String]);
    NSString* source = message.frameInfo.request.URL.absoluteString;

    if (source == nil) {
        WKSecurityOrigin* securityOrigin = message.frameInfo.securityOrigin;
        if (securityOrigin != nil && securityOrigin.protocol != nil && securityOrigin.host != nil) {
            NSInteger port = securityOrigin.port;
            if (port > 0)
                source = [NSString stringWithFormat:@"%@://%@:%ld/", securityOrigin.protocol, securityOrigin.host, (long)port];
            else
                source = [NSString stringWithFormat:@"%@://%@/", securityOrigin.protocol, securityOrigin.host];
        }
    }

    char* sourceUtf8 = source == nil ? nullptr : const_cast<char*>([source UTF8String]);
    infiniframe::macos::NativeCallbackScope callbackScope;
    webMessageReceivedCallback(messageUtf8, sourceUtf8);
}

- (void)webView:(WKWebView *)webView
        runJavaScriptAlertPanelWithMessage:(NSString *)message
        initiatedByFrame:(WKFrameInfo *)frame
        completionHandler:(void (^)(void))completionHandler
{
    NSAlert* alert = [[NSAlert alloc] init];

    [alert setMessageText: @"Alert"];
    [alert setInformativeText:message];
    [alert addButtonWithTitle:@"OK"];

    [alert beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        (void)response;
        completionHandler();
        [alert release];
    }];
}

- (void)webView:(WKWebView *)webView
        runJavaScriptConfirmPanelWithMessage:(NSString *)message
        initiatedByFrame:(WKFrameInfo *)frame
        completionHandler:(void (^)(BOOL result))completionHandler
{
    NSAlert* alert = [[NSAlert alloc] init];

    [alert setMessageText: @"Confirm"];
    [alert setInformativeText:message];

    [alert addButtonWithTitle:@"OK"];
    [alert addButtonWithTitle:@"Cancel"];

    [alert beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        completionHandler(response == NSAlertFirstButtonReturn);
        [alert release];
    }];
}

- (void)webView:(WKWebView *)webView
        runJavaScriptTextInputPanelWithPrompt:(NSString *)prompt
        defaultText:(NSString *)defaultText
        initiatedByFrame:(WKFrameInfo *)frame
        completionHandler:(void (^)(NSString *result))completionHandler
{
    NSAlert* alert = [[NSAlert alloc] init];

    [alert setMessageText: @"Prompt"];
    [alert setInformativeText:prompt];

    [alert addButtonWithTitle:@"OK"];
    [alert addButtonWithTitle:@"Cancel"];

    NSTextField* input = [[NSTextField alloc] initWithFrame:NSMakeRect(0, 0, 200, 24)];
    [input setStringValue:defaultText];
    [alert setAccessoryView:input];
    [input release];

    [alert beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        [input validateEditing];
        completionHandler(response == NSAlertFirstButtonReturn ? [input stringValue] : nil);
        [alert release];
    }];
}

- (void)webView:(WKWebView *)webView
        runOpenPanelWithParameters:(WKOpenPanelParameters *)parameters
        initiatedByFrame:(WKFrameInfo *)frame
        completionHandler:(void (^)(NSArray<NSURL *> *URLs))completionHandler
{
    NSOpenPanel* openDlg = [NSOpenPanel openPanel];
    [openDlg setCanChooseFiles:![parameters allowsDirectories]];
    [openDlg setCanChooseDirectories:[parameters allowsDirectories]];
    openDlg.allowsMultipleSelection = [parameters allowsMultipleSelection];
    [openDlg setPrompt:NSLocalizedString(@"OK", nil)];

    [openDlg beginSheetModalForWindow:window completionHandler:^void (NSModalResponse response) {
        completionHandler(response == NSModalResponseOK ? [openDlg URLs] : nil);
    }];
}

- (void)webView:(WKWebView *)webView
        requestMediaCapturePermissionForOrigin:(WKSecurityOrigin *)origin
        initiatedByFrame:(WKFrameInfo *)frame
        type:(WKMediaCaptureType)type
        decisionHandler:(void (^)(WKPermissionDecision decision))decisionHandler
{
    bool grantPermissions = false;
    if (infiniFrame != nullptr)
        infiniFrame->GetGrantBrowserPermissions(&grantPermissions);
    decisionHandler(grantPermissions ? WKPermissionDecisionGrant : WKPermissionDecisionPrompt);
}
@end
