// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#import "UrlSchemeHandler.h"
#include "Runtime/Shared/WebView/CustomSchemeResponse.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

@implementation UrlSchemeHandler : NSObject

- (void)webView:(WKWebView *)webView startURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
    NSURL *url = [[urlSchemeTask request] URL];
    auto *urlUtf8 = const_cast<char *>([url.absoluteString UTF8String]);
    CustomSchemeResponse managedResponse{};
    const int handled = requestHandler == nullptr ? 0 : requestHandler(urlUtf8, &managedResponse);
    infiniframe::CustomSchemeResponseLease responseLease(managedResponse);
    bool valid = handled != 0 && infiniframe::IsValidBufferedCustomSchemeResponse(managedResponse);

    NSInteger statusCode = valid ? static_cast<NSInteger>(managedResponse.StatusCode) : 404;
    NSString* nsContentType = valid
        ? [NSString stringWithUTF8String:managedResponse.ContentTypeUtf8]
        : @"application/octet-stream";
    if (nsContentType == nil) {
        valid = false;
        statusCode = 500;
        nsContentType = @"application/octet-stream";
    }

    NSDictionary* headers = @{ @"Content-Type" : nsContentType, @"Cache-Control": @"no-cache" };
    NSHTTPURLResponse *response = [[NSHTTPURLResponse alloc] initWithURL:url statusCode:statusCode HTTPVersion:nil headerFields:headers];
    [urlSchemeTask didReceiveResponse:response];
    [response release];
    if (valid && managedResponse.ContentLength > 0) {
        // dataWithBytes copies producer-owned memory before the release callback runs.
        [urlSchemeTask didReceiveData:[NSData dataWithBytes:managedResponse.Body
                                                    length:static_cast<NSUInteger>(managedResponse.ContentLength)]];
    }
    [urlSchemeTask didFinish];
}

- (void)webView:(WKWebView *)webView stopURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
}

@end
