// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import "UrlSchemeHandler.h"

#include "../MacDiagnostics.h"
#include "Runtime/Shared/WebView/CustomSchemeResponse.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@implementation UrlSchemeHandler : NSObject

- (id)init
{
    self = [super init];
    if (self != nil)
        activeTasks = [[NSMutableSet alloc] init];
    return self;
}

- (void)dealloc
{
    [self invalidate];
    [activeTasks release];
    [super dealloc];
}

- (void)invalidate
{
    @synchronized (self) {
        requestHandler = nullptr;
        [activeTasks removeAllObjects];
    }
}

- (void)webView:(WKWebView *)webView startURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
    NSURL *url = [[urlSchemeTask request] URL];
    if (url == nil) {
        NSError* error = [NSError errorWithDomain:NSURLErrorDomain code:NSURLErrorBadURL userInfo:nil];
        [urlSchemeTask didFailWithError:error];
        return;
    }

    @synchronized (self) {
        [activeTasks addObject:urlSchemeTask];
    }

    auto *urlUtf8 = const_cast<char *>([url.absoluteString UTF8String]);
    CustomSchemeResponse managedResponse{};
    int handled = 0;
    @synchronized (self) {
        if (requestHandler != nullptr)
        {
            infiniframe::macos::NativeCallbackScope callbackScope;
            handled = requestHandler(urlUtf8, &managedResponse);
        }
    }
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
    @synchronized (self) {
        if (![activeTasks containsObject:urlSchemeTask]) {
            [response release];
            return;
        }
        [urlSchemeTask didReceiveResponse:response];
    }
    [response release];
    if (valid && managedResponse.ContentLength > 0) {
        // dataWithBytes copies producer-owned memory before the release callback runs.
        @synchronized (self) {
            if ([activeTasks containsObject:urlSchemeTask])
            [urlSchemeTask didReceiveData:[NSData dataWithBytes:managedResponse.Body
                                                        length:static_cast<NSUInteger>(managedResponse.ContentLength)]];
        }
    }
    @synchronized (self) {
        if ([activeTasks containsObject:urlSchemeTask])
            [urlSchemeTask didFinish];
        [activeTasks removeObject:urlSchemeTask];
    }
}

- (void)webView:(WKWebView *)webView stopURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
    @synchronized (self) {
        [activeTasks removeObject:urlSchemeTask];
    }
}

@end
